using System;
using System.Collections.Generic;
using NetworkTables;
using NetworkTables.Tables;

namespace SimpleFMS.Networking.Server.NetworkTableUpdaters
{
    internal abstract class NetworkTableUpdaterBase : IDisposable
    {
        protected readonly ITable NetworkTable;

        protected Dictionary<int, Action<ITable, string, Value, NotifyFlags>> NetworkTableListeners { get; } =
            new Dictionary<int, Action<ITable, string, Value, NotifyFlags>>();

        protected NetworkTableUpdaterBase(ITable root, string tableName)
        {
            if (root == null) 
                throw new ArgumentNullException(nameof(root), "Table root cannot be null");

            NetworkTable = root.GetSubTable(tableName);
        }

        public abstract void UpdateTable();

        public virtual void Dispose()
        {
            foreach (var networkTableListener in NetworkTableListeners.Values)
            {
                NetworkTable.RemoveTableListener(networkTableListener);
            }
        }
    }
}
