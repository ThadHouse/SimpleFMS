using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using SimpleFMS.DriverStation.Base.Enums;
using SimpleFMS.DriverStation.Base.Interfaces;
using SimpleFMS.DriverStation.TcpData;

namespace SimpleFMS.DriverStation.TcpControllers
{
    public class DriverStationConnectionListener : IRestartable
    {
        private readonly int m_port;

        public delegate void DriverStationNewConnectionInfo(
            int teamNumber, IPAddress ipAddress, out AllianceStationSide allianceSide,
            out AllianceStationNumber stationNumber, out bool isRequested);

        public event DriverStationNewConnectionInfo OnNewDriverStationConnected;

        private readonly ConcurrentDictionary<int, Task> m_clientTasks = new ConcurrentDictionary<int, Task>();

        private CancellationTokenSource m_cancellationTokenSource;

        private Task m_listenerTask;

        private readonly object m_lockObject = new object();

        public void Dispose()
        {
            OnNewDriverStationConnected = null;

            // Cancel our tasks
            if (m_cancellationTokenSource != null)
            {
                m_cancellationTokenSource.Cancel();

                if (m_listenerTask != null && m_listenerTask.Status == TaskStatus.Running)
                    m_listenerTask.Wait();

                // Wait for our task list to finish and 
                // empty itself out
                int taskLoopCount = 0;
                while (!m_clientTasks.IsEmpty)
                {
                    if (taskLoopCount > 10)
                    {
                        // 1 second is long enough to wait
                        // for tasks to end
                        break;
                    }
                    Thread.Sleep(100);
                    taskLoopCount++;
                }
            }

            m_clientTasks.Clear();
        }

        public void Restart()
        {
            // Restart our tasks
            lock (m_lockObject)
            {
                // Cancel our tasks
                if (m_cancellationTokenSource != null)
                {
                    m_cancellationTokenSource.Cancel();

                    if (m_listenerTask != null && m_listenerTask.Status == TaskStatus.Running)
                        m_listenerTask.Wait();

                    // Wait for our task list to finish and 
                    // empty itself out
                    int taskLoopCount = 0;
                    while (!m_clientTasks.IsEmpty)
                    {
                        if (taskLoopCount > 20)
                        {
                            // 2 seconds is long enough to wait
                            // for tasks to end
                            break;
                        }
                        Thread.Sleep(100);
                        taskLoopCount++;
                    }
                }

                m_clientTasks.Clear();

                m_cancellationTokenSource = new CancellationTokenSource();
                m_listenerTask = TcpClientAcceptor(m_cancellationTokenSource.Token);
            }
        }

        public DriverStationConnectionListener(int port)
        {
            m_port = port;
        }

        private Task TcpClientAcceptor(CancellationToken token)
        {
            return Task.Factory.StartNew(() =>
            {
                TcpListener listener = new TcpListener(IPAddress.Any, m_port);
                listener.Start();
                try
                {
                    while (!token.IsCancellationRequested)
                    {
                        Task<TcpClient> clientTask = listener.AcceptTcpClientAsync();
                        clientTask.Wait(token);
                        if (clientTask.IsCompleted)
                        {
                            StartTcpClientLoop(clientTask.Result, token);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Do nothing on a canceled operation
                }
                finally
                {
                    listener.Stop();
                }
            }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private class TcpClientTaskParameters
        {
            internal TcpClient Client { get; set; }
            internal CancellationToken Token { get; set; }
            internal int TaskId { get; set; }
        }

        private void TcpClientLoop(object parameter)
        {
            TcpClientTaskParameters parameters = parameter as TcpClientTaskParameters;
            // If parameter is passed wrong, we can't do anything
            if (parameters == null)
            {
                throw new ArgumentOutOfRangeException(nameof(parameter), "Parameter must be TcpClientLoopParameters");
            }

            var client = parameters.Client;
            var token = parameters.Token;
            var taskId = parameters.TaskId;

            int watchDog = 0;
            DriverStationConnectionStatusData receiveData = new DriverStationConnectionStatusData();
            DriverStationConnectionControlData sendData = new DriverStationConnectionControlData();
            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (client.Client.Poll(1000, SelectMode.SelectRead))
                    {
                        int available = client.Client.Available;
                        if (available <= 0)
                        {
                            if (++watchDog <= 100)
                                Thread.Sleep(100);
                            else
                                break;
                        }

                        byte[] buffer = new byte[available];
                        client.Client.Receive(buffer);

                        ReceiveParseStatus status = receiveData.ParseData(buffer);

                        if (status == ReceiveParseStatus.TrackedPacketValid)
                        {
                            if (receiveData.Status == DriverStationConnectionPacketType.SetTeam)
                            {
                                // Fire the new Driver Station Event
                                AllianceStationSide alliance = 0;
                                AllianceStationNumber station = 0;
                                bool requested = false;

                                OnNewDriverStationConnected?.Invoke(receiveData.TeamNumber, ((IPEndPoint)client.Client.RemoteEndPoint).Address, out alliance, out station,
                                    out requested);

                                sendData.AllianceSide = alliance;
                                sendData.StationNumber = station;
                                sendData.StationStatus = requested ? AllianceStationStatus.CorrectStation : AllianceStationStatus.InvalidTeam;
                                byte[] toSend = sendData.PackData();

                                client.GetStream().Write(toSend, 0, toSend.Length);
                                continue;
                            }
                            else if (receiveData.Status == DriverStationConnectionPacketType.Ping)
                            {
                                watchDog = 0;
                                continue;
                            }
                        }
                    }
                    else
                    {
                        if (++watchDog <= 100)
                            Thread.Sleep(100);
                        else
                            break;
                    }
                }
                catch (Exception)
                {
                    // TODO: Log
                    ;
                }
            }
            client.GetStream().Close();
            client.Close();
            // Remove ourselves from the client list
            Task task = null;
            m_clientTasks.TryRemove(taskId, out task);
            // Since we never wait on this task, we do not need
            // to dispose of it
        }

        private void StartTcpClientLoop(TcpClient client, CancellationToken token)
        {
            TcpClientTaskParameters parameters = new TcpClientTaskParameters();

            Task clientTask = new Task(TcpClientLoop, parameters, token, TaskCreationOptions.LongRunning);
            parameters.TaskId = clientTask.Id;
            parameters.Client = client;
            parameters.Token = token;
            // Add task to our concurrent list
            m_clientTasks.TryAdd(clientTask.Id, clientTask);
            clientTask.Start();
        }
    }
}
