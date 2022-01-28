using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueChores
{
	internal class InventoryItem
	{
		public string inventoryType { get; set; }
		public int itemId { get; set; }
		public string ownershipType { get; set; }
		public string purchaseDate { get; set; }
		public int quantity { get; set; }
		public string uuid { get; set; }

		public static async Task<InventoryItem[]> Get(string inventoryType)
		{
			var response = await LCU.GetAs<InventoryItem[]>("/lol-inventory/v2/inventory/" + inventoryType);
			return response.ok ? response.body : new InventoryItem[0] { };
		}
	}

}
