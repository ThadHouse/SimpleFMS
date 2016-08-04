using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkTables.Tables;
using SimpleFMS.Base.Networking;

namespace SimpleFMS.Networking.Server.NetworkTableUpdaters
{
    internal class MatchTimingUpdaterBase : NetworkTableUpdaterBase
    {
        public MatchTimingUpdaterBase(ITable root) : base(root, NetworkingConstants.MatchTimingConstatns.MatchTimingTableName)
        {
        }

        public override void UpdateTable()
        {
            throw new NotImplementedException();
        }
    }
}
