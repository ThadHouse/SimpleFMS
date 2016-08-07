using System;
using System.Threading;
using Autofac;
using SimpleFMS.Base.DriverStation;
using SimpleFMS.Base.MatchTiming;
using SimpleFMS.Base.Networking;
using SimpleFMS.DriverStation;
using SimpleFMS.MatchTiming;
using SimpleFMS.Networking.Server;

namespace SimpleFMS.ConsoleHost
{
    class Program
    {
        private static IContainer Container { get; set; }

        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<DriverStationManager>().As<IDriverStationManager>().SingleInstance();
            builder.RegisterType<MatchTimingManager>().As<IMatchTimingManager>().SingleInstance();
            builder.RegisterType<NetworkServerManager>().As<INetworkServerManager>().SingleInstance();

            Console.WriteLine("Welcome to Simple FMS!");


            Container = builder.Build();


            using (var scope = Container.BeginLifetimeScope())
            {
                var manager = scope.Resolve<INetworkServerManager>();

                manager.OnClientChanged += (id, ip, conn) =>
                {
                    if (conn)
                    {
                        Console.WriteLine($"Client connected: {id} at {ip}");
                    }
                    else
                    {
                        Console.WriteLine($"Client disconnected: {id} at {ip}");
                    }
                };

                Thread.Sleep(Timeout.Infinite);

                GC.KeepAlive(manager);
            }
        }
    }
}
