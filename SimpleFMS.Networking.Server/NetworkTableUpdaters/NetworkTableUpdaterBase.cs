using System;
using NetworkTables.Tables;

namespace SimpleFMS.Networking.Server.NetworkTableUpdaters
{
    internal abstract class NetworkTableUpdaterBase : IDisposable
    {
        protected readonly ITable NetworkTable;

        protected NetworkTableUpdaterBase(ITable root, string tableName)
        {
            if (root == null) 
                throw new ArgumentNullException(nameof(root), "Table root cannot be null");

            NetworkTable = root.GetSubTable(tableName);
        }

        public abstract void UpdateTable();

        public virtual void Dispose()
        {
        }
    }
}
