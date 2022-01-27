using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueChores
{
    class MissionData
    {
        public class PlayerInventory
        {
            public List<object> champions { get; set; }
            public List<int> icons { get; set; }
            public List<object> skins { get; set; }
            public List<object> wardSkins { get; set; }
        }

        public int level { get; set; }
        public bool loyaltyEnabled { get; set; }
        public PlayerInventory playerInventory { get; set; }

        public static async Task<Response<MissionData>> Get()
        {
            return await LCU.GetAs<MissionData>("/lol-missions/v1/data");
        }
    }
}
