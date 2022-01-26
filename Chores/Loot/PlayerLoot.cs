using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueChores.Chores.Loot
{
	[Serializable]
	internal class PlayerLoot
	{
		public string asset;
		public Int64 count;
		public string disenchantLootName;
		public Int64 disenchantValue;
		public string displayCategories;
		public Int64 expiryTime;
		public bool isNew;
		public bool isRental;
		public string itemDesc;
		public string itemStatus;
		public string localizedDescription;
		public string localizedName;
		public string localizedRecipeSubtitle;
		public string localizedRecipeTitle;
		public string lootId;
		public string lootName;
		public string parentItemStatus;
		public Int64 parentStoreItemId;
		public string rarity;
		public string redeemableStatus;
		public string refId;
		public Int64 rentalGames;
		public Int64 rentalSeconds;
		public string shadowPath;
		public string splashPath;
		public Int64 storeItemId;
		public string tags;
		public string tilePath;
		public string type;
		public string upgradeEssenceName;
		public Int64 upgradeEssenceValue;
		public string upgradeLootName;
		public Int64 value;
	};
}
