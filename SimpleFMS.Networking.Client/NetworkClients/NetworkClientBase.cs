using System;
using System.Collections.Generic;
using NetworkTables;
using NetworkTables.Tables;
using SimpleFMS.Networking.Base;

namespace SimpleFMS.Networking.Client.NetworkClients
{
    public abstract class NetworkClientBase : INetworkClient
    {
        protected readonly ITable NetworkTable;

        protected List<Action<ITable, string, Value, NotifyFlags>> NetworkTableListeners { get; } =
            new List<Action<ITable, string, Value, NotifyFlags>>();

        protected NetworkClientBase(ITable root, string tableName)
        {
            if (root == null)
                throw new ArgumentNullException(nameof(root), "Table root cannot be null");

            NetworkTable = root.GetSubTable(tableName);
        }

        protected void AddTableListener(string key, Action<ITable, string, Value, NotifyFlags> listener)
        {
            NetworkTableListeners.Add(listener);
            NetworkTable.AddTableListener(key, listener, true);
        }

        public virtual void Dispose()
        {
            foreach (var networkTableListener in NetworkTableListeners)
            {
                NetworkTable.RemoveTableListener(networkTableListener);
            }
        }
    }
}
