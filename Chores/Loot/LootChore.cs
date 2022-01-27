using LeagueChores.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LeagueChores.Chores.Loot;
using System.Diagnostics;
using Serilog;

namespace LeagueChores
{
	internal class LootChore
	{
		public static readonly string[] chestNames = new string[] { "CHEST_72", "CHEST_55", "CHEST_54", "CHEST_56", "CHEST_57", "CHEST_58", "CHEST_59", "CHEST_60", "CHEST_61", "CHEST_62", "CHEST_63", "CHEST_64", "CHEST_65", "CHEST_66", "CHEST_69", "CHEST_53", "CHEST_32", "CHEST_new_player", "CHEST_day_one", "CHEST_224", "CHEST_promotion", "CHEST_96", "CHEST_97", "CHEST_98", "CHEST_99", "CHEST_champion_mastery", "CHEST_generic" }; // Taken from the client source

		public LootChoreTask task = null;

		public LootChore()
		{
			LCU.onMessage += OnMessage;
			LCU.onValid += (s, e) => CleanInventory(false);
		}

		private void OnMessage(object sender, LCUMessageEventArgs e)
		{
			if (e.uri.StartsWith("/lol-loot/") == false)
				return;

			if (e.eventName == "Create" || e.eventName == "Update")
				CleanInventory(false);
		}

		async Task<IEnumerable<PlayerLoot>> GetInventory()
		{
			var lootRequest = await LCU.GetAs<Dictionary<string, PlayerLoot>>("/lol-loot/v1/player-loot-map");
			if (lootRequest != null && lootRequest.ok == false) // TODO: Probably needs to assert?
				return new PlayerLoot[] {};

			return lootRequest.body.Where((l) => String.IsNullOrWhiteSpace(l.Key) == false).Select(l => l.Value);
		}

		async void DebugContextMenu(string lootId)
		{
			var contextMenu = await LCU.Get($"/lol-loot/v1/player-loot/{lootId}/context-menu");
			System.IO.File.WriteAllText("test.json", contextMenu.body.ToString());
		}

		async Task<long> GetBlueEssense(IEnumerable<PlayerLoot> loot = null)
		{
			if (loot == null)
				loot = await GetInventory();

			var currency = loot.FirstOrDefault(l => l.lootId == "CURRENCY_champion");
			return currency != null ? currency.count : 0;
		}

		async Task<long> GetOrangeEssense(IEnumerable<PlayerLoot> loot = null)
		{
			if (loot == null)
				loot = await GetInventory();

			var currency = loot.FirstOrDefault(l => l.lootId == "CURRENCY_cosmetic");
			return currency != null ? currency.count : 0;
		}

		async Task<long> GetKeyCount(IEnumerable<PlayerLoot> loot = null)
		{
			if (loot == null)
				loot = await GetInventory();

			var keys = loot.FirstOrDefault(l => l.lootId == "MATERIAL_key");
			return keys != null ? keys.count : 0;
		}

		async Task<Item> GetAllChampionCapsules(LootData settings = null, ActionRules rules = ActionRules.None, IEnumerable<PlayerLoot> loot = null)
		{
			if (loot == null)
				loot = await GetInventory();
			if (settings == null)
				settings = LCU.validatedSummonerSettings.loot;

			var championCapsule = loot.FirstOrDefault((x) => x.storeItemId == 128);
			if (championCapsule == null || championCapsule.count == 0)
				return null;

			var item = new Item(championCapsule, ItemType.ChampionCapsule);
			switch (rules)
			{
				case ActionRules.None:
					return item;

				case ActionRules.RemoveRules:
					if (settings.openCapsules.Value)
					{
						var uri = $"/lol-loot/v1/recipes/CHEST_{championCapsule.storeItemId}_OPEN/craft?repeat={championCapsule.count}";
						var body = $"[\"{championCapsule.lootId}\"]";
						item.AddAction("Remove", uri, body);
					}
					return item;

				default:
					throw new ArgumentException("Invalid/Unhandled argument given for ActionRules");
			}
		}

		async Task<Item> GetAllEternalsCapsules(LootData settings = null, ActionRules rules = ActionRules.None, IEnumerable<PlayerLoot> loot = null)
		{
			if (loot == null)
				loot = await GetInventory();
			if (settings == null)
				settings = LCU.validatedSummonerSettings.loot;

			var eternalCapsules = loot.FirstOrDefault((x) => x.lootId == "CHEST_6670001");
			if (eternalCapsules == null || eternalCapsules.count == 0)
				return null;

			var item = new Item(eternalCapsules, ItemType.EternalsCapsule);
			switch (rules)
			{
				case ActionRules.None:
					return item;

				case ActionRules.RemoveRules:
					if (settings.openCapsules.Value)
					{
						var uri = $"/lol-loot/v1/recipes/{eternalCapsules.lootId}_OPEN/craft?repeat={eternalCapsules.count}";
						var body = $"[\"{eternalCapsules.lootId}\"]";
						item.AddAction("Remove", uri, body);
					}
					return item;

				default:
					throw new ArgumentException("Invalid/Unhandled argument given for ActionRules");
			}
		}

		async Task<Item[]> GetAllEternals(LootData settings = null, ActionRules rules = ActionRules.None, IEnumerable<PlayerLoot> loot = null)
		{
			if (loot == null)
				loot = await GetInventory();
			if (settings == null)
				settings = LCU.validatedSummonerSettings.loot;

			var statStones = loot.Where((l) => l.type == "STATSTONE_SHARD");
			var items = new List<Item>();
			foreach (var statStone in statStones)
			{
				// int champId = int.Parse(ParseTags(statStone.tags)["champId"]);
				var statStoneCount = statStone.count;
				if (statStoneCount <= 0)
					continue;

				var item = new Item(statStone, ItemType.Eternal, statStoneCount);
				switch (rules)
				{
					case ActionRules.None:
						break;

					case ActionRules.RemoveRules:
						if (settings.disenchantEternals.Value)
						{
							var uri = $"/lol-loot/v1/recipes/STATSTONE_SHARD_DISENCHANT/craft?repeat={statStoneCount}";
							var body = $"[\"{statStone.lootId}\"]";
							item.AddAction("Remove", uri, body);
						}
						break;

					default:
						throw new ArgumentException("Invalid/Unhandled argument given for ActionRules");
				}

				items.Add(item);
			}

			return items.ToArray();
		}

		async Task<List<Item>> GetAllChests(LootData settings = null, ActionRules rules = ActionRules.None, IEnumerable<PlayerLoot> loot = null)
		{
			if (loot == null)
				loot = await GetInventory();
			if (settings == null)
				settings = LCU.validatedSummonerSettings.loot;

			var items = new List<Item>();
			var chests = loot.Where((c) => chestNames.Any(chestName => chestName == c.lootId));
			var remainingKeys = await GetKeyCount(loot);
			foreach (var chest in chests)
			{
				if (chest == null || chest.count == 0)
					continue;

				var openCount = Math.Min(remainingKeys, chest.count);
				if (openCount == 0) // Out of keys
					break;

				remainingKeys -= openCount; // Subtract the remaining keys
				var item = new Item(chest, ItemType.Chest, openCount);
				switch (rules)
				{
					case ActionRules.None:
						break;
						
					case ActionRules.RemoveRules:
						if (settings.openChests.Value)
						{
							var uri = $"/lol-loot/v1/recipes/{chest.lootId}_OPEN/craft?repeat={openCount}";
							var body = $"[\"{chest.lootId}\",\"MATERIAL_key\"]";
							item.AddAction("Remove", uri, body);
						}
						break;

					default:
						throw new ArgumentException("Invalid/Unhandled argument given for ActionRules");
				}

				items.Add(item);
			}

			return items;
		}

		async Task<Item[]> GetAllChampions(LootData settings = null, ActionRules rules = ActionRules.None, IEnumerable<PlayerLoot> loot = null)
		{
			if (loot == null)
				loot = await GetInventory();
			if (settings == null)
				settings = LCU.validatedSummonerSettings.loot;

			var items = new List<Item>();

			var allMastery = await LCU.GetAs<ChampionMastery[]>($"/lol-collections/v1/inventories/{LCU.currentSummonerId}/champion-mastery");
			var ownedChampions = await LCU.GetAs<OwnedChampion[]>($"/lol-champions/v1/inventories/{LCU.currentSummonerId}/champions");
			var champs = loot.Where((l) => l.type == "CHAMPION_RENTAL" || l.type == "CHAMPION");

			foreach (var champ in champs)
			{
				int champId = (int)champ.storeItemId;
				var championData = await DDragon.GetChampionByKey(champId);
				ChampionMastery champMastery = allMastery.ok ? allMastery.body.FirstOrDefault(m => m.championId == champId) : null;

				var champCount = champ.count;
				var ownedChampion = ownedChampions.ok ? ownedChampions.body.FirstOrDefault(c => c.id == champId) : null;
				var hasChampion =  ownedChampion != null ? ownedChampion.ownership.owned && ownedChampion.ownership.rental.rented == false : false;
				if (hasChampion)
				{
					if (champMastery == null || champMastery.championLevel < 5)
						champ.count -= settings.lowLevelKeepCount.Value;
					else if (champMastery.championLevel == 6)
						champCount -= settings.level6KeepCount.Value;
					else if (champMastery.championLevel == 5)
						champCount -= settings.level5KeepCount.Value;
				}
				else
				{
					champCount -= settings.noChampKeepCount.Value;
				}

				if (champCount <= 0)
					continue;

				var item = new Item(champ, ItemType.Champion, champCount);
				switch (rules)
				{
					case ActionRules.None:
						break;

					case ActionRules.RemoveRules:
						if (settings.disenchantChampions.Value)
						{
							var uri = $"/lol-loot/v1/recipes/{champ.type}_disenchant/craft?repeat={champCount}";
							var body = $"[\"{champ.lootId}\"]";
							item.AddAction("Remove", uri, body);
						}
						break;

					default:
						throw new ArgumentException("Invalid/Unhandled argument given for ActionRules");
				}

				items.Add(item);
			}

			return items.ToArray();
		}

		async Task<Item> GetAllKeyFragments(LootData settings = null, ActionRules rules = ActionRules.None, IEnumerable<PlayerLoot> loot = null)
		{
			if (loot == null)
				loot = await GetInventory();
			if (settings == null)
				settings = LCU.validatedSummonerSettings.loot;

			var fragmentObjects = loot.FirstOrDefault((x) => x.lootId == "MATERIAL_key_fragment");
			if (fragmentObjects == null)
				return null;

			var lootId = fragmentObjects.lootId;
			long fragmentCount = fragmentObjects.count;
			if (fragmentCount < 3)
				return null;

			long actionCount = fragmentCount / 3;
			var item = new Item(fragmentObjects, ItemType.KeyFragment, actionCount * 3);
			switch (rules)
			{
				case ActionRules.None:
					return item;

				case ActionRules.RemoveRules:
					if (settings.combineKeys.Value)
					{
						var uri = $"/lol-loot/v1/recipes/MATERIAL_key_fragment_forge/craft?repeat={actionCount}";
						var body = $"[\"{lootId}\"]";
						item.AddAction("Remove", uri, body);
					}
					return item;

				default:
					throw new ArgumentException("Invalid/Unhandled argument given for ActionRules");
			}
		}

		async Task<Item[]> GetAllSkins(LootData settings = null, ActionRules rules = ActionRules.None, IEnumerable<PlayerLoot> loot = null)
		{
			if (loot == null)
				loot = await GetInventory();
			if (settings == null)
				settings = LCU.validatedSummonerSettings.loot;

			var removeDuplicateSkins = settings.disenchantDuplicateSkins.Value && rules == ActionRules.RemoveRules;
			var rerollOwnedSkins = settings.rerollAllOwnedSkins.Value && rules == ActionRules.RerollRules;
			var items = new List<Item>();

			var allMastery = await LCU.GetAs<ChampionMastery[]>($"/lol-collections/v1/inventories/{LCU.currentSummonerId}/champion-mastery");
			var ownedChampions = await LCU.GetAs<OwnedChampion[]>($"/lol-champions/v1/inventories/{LCU.currentSummonerId}/champions");
			var skins = loot.Where((l) => l.type == "SKIN_RENTAL" || l.type == "SKIN").OrderByDescending(s => s.value);
			List<PlayerLoot> rerollSet = new List<PlayerLoot>();

			foreach (var skin in skins)
			{
				var champReference = ownedChampions.ok ? ownedChampions.body.FirstOrDefault(c => c != null && skin != null && c.id == skin.parentStoreItemId) : null;
				var skinReference = champReference?.skins.FirstOrDefault(s => s.id == skin.storeItemId);
				var ownsSkin = skinReference != null ? skinReference.ownership.owned && skinReference.ownership.rental.rented == false : false;

				// Delete all if owned, otherwise keep one
				long deleteCount = skin.count - 1;
				if (ownsSkin)
					deleteCount++;

				if (deleteCount >= 1 && removeDuplicateSkins)
				{
					var item = new Item(skin, ItemType.Skin, deleteCount);

					var uri = $"/lol-loot/v1/recipes/{skin.type}_disenchant/craft?repeat={deleteCount}";
					var body = $"[\"{skin.lootId}\"]";
					item.AddAction("Remove", uri, body);

					items.Add(item);
					continue;
				}

				if (rerollOwnedSkins && ownedChampions.ok)
				{
					if (ownsSkin)
						rerollSet.Add(skin);

					if (rerollSet.Count == 3)
					{
						var item = new Item(skin, ItemType.Skin, deleteCount);

						var uri = $"/lol-loot/v1/recipes/SKIN_reroll/craft";
						var body = $"[\"{rerollSet[0].lootId}\", \"{rerollSet[1].lootId}\", \"{rerollSet[2].lootId}\"]";
						item.AddAction("Reroll", uri, body);

						items.Add(item);

						rerollSet.Clear();
					}
					continue;
				}
			}

			return items.ToArray();
		}

		private async Task<string> GetDefaultItemActionNames(List<Item> items)
		{
			string itemList = "";
			var nonChampionsList = items.Where(i => i.
				type != ItemType.Champion && 
				i.type != ItemType.Skin &&
				i.type != ItemType.Eternal);
			foreach (var item in nonChampionsList)
			{
				if (item.hasDefaultAction == false)
					continue;

				switch (item.type)
				{
					case ItemType.Chest:
						itemList += $" - {item.defaultActionName} {item.count}x chest(s)\n";
						break;

					case ItemType.ChampionCapsule:
						itemList += $" - {item.defaultActionName} {item.count}x champion capsule(s)\n";
						break;

					case ItemType.EternalsCapsule:
						itemList += $" - {item.defaultActionName} {item.count}x eternals capsule(s)\n";
						break;

					case ItemType.KeyFragment:
						itemList += $" - {item.defaultActionName} {item.count}x key fragment(s)\n";
						break;

					default:
						itemList += $" - {item.defaultActionName} {item.count}x {item.item.localizedName}\n";
						break;
				}
			}

			var skins = items.Where(i => i.type == ItemType.Skin);
			var skinDisenchants = skins.Where(i => i.defaultActionName != "Reroll");
			if (skinDisenchants.Count() > 10)
			{
				var champCount = skinDisenchants.Aggregate<Item, long>(0, (current, item) => current + item.count);
				var totalValue = skinDisenchants.Aggregate<Item, long>(0, (current, item) => current + item.count * item.item.disenchantValue);
				itemList += $" - {champCount}x skins for {totalValue} Orange Essense";
			}
			else foreach (var item in skinDisenchants)
			{
				var name = string.IsNullOrWhiteSpace(item.item.localizedName) == false ? item.item.localizedName : item.item.itemDesc;
				itemList += $" - {item.defaultActionName} {item.count}x {name}\n";
			}

			var skinRerolls = skins.Where(i => i.defaultActionName == "Reroll");
			if (skinRerolls.Count() > 0)
				itemList += $" - Reroll 3 skins {skinRerolls.Count()} time{(skinRerolls.Count() == 1 ? "" : "s")}";

			var champions = items.Where(i => i.type == ItemType.Champion);
			if (champions.Count() > 10)
			{
				var champCount = champions.Aggregate<Item, long>(0, (current, item) => current + item.count);
				var totalValue = champions.Aggregate<Item, long>(0, (current, item) => current + item.count * item.item.disenchantValue);
				itemList += $" - {champCount}x champions for {totalValue} BE";
			}
			else foreach (var item in champions)
			{
				var name = string.IsNullOrWhiteSpace(item.item.localizedName) == false ? item.item.localizedName : item.item.itemDesc;
				itemList += $" - {item.defaultActionName} {item.count}x {name}\n";
			}

			var eternals = items.Where(i => i.type == ItemType.Eternal);
			if (eternals.Count() > 10)
			{
				var champCount = eternals.Aggregate<Item, long>(0, (current, item) => current + item.count);
				var totalValue = eternals.Aggregate<Item, long>(0, (current, item) => current + item.count * item.item.disenchantValue);
				itemList += $" - {champCount}x eternals for {totalValue} orange essense";
			}
			else foreach (var item in eternals)
			{
				itemList += $" - {item.defaultActionName} {item.count}x {item.item.localizedName}\n";
			}

			return itemList;
		}

		public async Task<long> PerformDefaultActions(List<Item> items, bool startedManually, bool avoidActions)
		{
			items = items.Where(i => i != null && i.hasDefaultAction).ToList();
			if (items.Count == 0)
				return 0;

			string itemList = await GetDefaultItemActionNames(items);
			bool startAction = true;
			if (startedManually)
			{
				DialogResult result = MessageBox.Show($"We're about to do the following actions:\n{itemList}", 
					"Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				startAction = result == DialogResult.Yes;
			}

			if (avoidActions || startAction == false)
			{
				Log.Information($"The following default tasks are available:\n{itemList}");
				return 0;
			}

			var taskResults = await Task.WhenAll<bool>(items.Select(a => a.PerformDefaultTask()));
			Log.Information($"Attempted the following default tasks:\n{itemList}");
			return taskResults.Where(t => t).Aggregate<bool, long>(0, (current, success) => current + (success ? 1 : 0));
		}

		private List<Item> AddItem(List<Item> items, Item item)
		{
			if (item != null)
				items.Add(item);
			return items;
		}

		private List<Item> AddItems(List<Item> items, IEnumerable<Item> newItems)
		{
			newItems.Where(i => i != null);
			items.AddRange(newItems);
			return items;
		}

		public async void CleanInventory(bool startedManually)
		{
			if (task != null)
				return;

			using (var t = new LootChoreTask(this))
			{
				if (LCU.isValid == false)
				{
					if (startedManually)
						MessageBox.Show("League of Legends client needs to be running to do this.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				var settings = LCU.validatedSummonerSettings.loot;
				bool avoidActions = startedManually == false && settings.onlyRunManually.Value;
#if !DEBUG // Exit out early if we're not debugging
				if (avoidActions) // Prevent automatic disenchanting if desired
					return;
#endif
				List<Item> items = new List<Item>();
				long actionsTaken = 0;

				var loot = await GetInventory();
#if DEBUG
				// foreach (var l in loot)
					// Debug.WriteLine($"{l.lootId}: {l.localizedName} (itemDesc: '{l.itemDesc}' StoreId: {l.storeItemId}, count: {l.count})");
#endif

				long blueEssenseStart = await GetBlueEssense();
				long orangeEssenseStart = await GetOrangeEssense();
				long keyCountStart = await GetKeyCount();

				items = AddItem(items, await GetAllEternalsCapsules(settings, ActionRules.RemoveRules, loot));
				items = AddItem(items, await GetAllChampionCapsules(settings, ActionRules.RemoveRules, loot));
				items = AddItem(items, await GetAllKeyFragments(settings, ActionRules.RemoveRules, loot));
				actionsTaken += await PerformDefaultActions(items, startedManually, avoidActions); // Get new key count
				loot = await GetInventory(); // Refresh inventory
				items.Clear();

				// We can get chests in chests, so we have to keep checking
				do
				{
					items = AddItems(items, await GetAllChests(settings, ActionRules.RemoveRules, loot));
					if (avoidActions) // If we're not doing anything, just break out
						break;

					var chestsOpened = await PerformDefaultActions(items, startedManually, avoidActions);
					items.Clear();
					if (chestsOpened == 0) // User canceled or error
						break;

					actionsTaken += chestsOpened;
					loot = await GetInventory(); // Refresh inventory
					items = AddItems(items, await GetAllChests(settings, ActionRules.RemoveRules, loot));
				}
				while (items.Count > 0);

				// Refresh our loot inventory
				loot = await GetInventory();

				items = AddItems(items, await GetAllEternals(settings, ActionRules.RemoveRules, loot));
				items = AddItems(items, await GetAllSkins(settings, ActionRules.RemoveRules, loot)); // Remove duplicates
				items = AddItems(items, await GetAllChampions(settings, ActionRules.RemoveRules, loot));
				actionsTaken += await PerformDefaultActions(items, startedManually, avoidActions);
				loot = await GetInventory(); // Refresh inventory
				items.Clear();

				items = AddItems(items, await GetAllSkins(settings, ActionRules.RerollRules, loot)); // Reroll skins
				actionsTaken += await PerformDefaultActions(items, startedManually, avoidActions);
				loot = await GetInventory(); // Refresh inventory
				items.Clear();

				loot = await GetInventory();
				long blueEssenseEnd = await GetBlueEssense();
				long orangeEssenseEnd = await GetOrangeEssense();
				long keyCountEnd = await GetKeyCount();
				string msg = $"Performed {actionsTaken} actions.";

				if (blueEssenseEnd != blueEssenseStart)
				{
					var essenseDiff = blueEssenseEnd - blueEssenseStart;
					string gainType = essenseDiff > 0 ? "Gained" : "Lost";
					msg += $"\n- {gainType} {essenseDiff} Blue Essense.";
				}
				if (orangeEssenseEnd != orangeEssenseStart)
				{
					var essenseDiff = orangeEssenseEnd - orangeEssenseStart;
					string gainType = essenseDiff > 0 ? "Gained" : "Lost";
					msg += $"\n- {gainType} {essenseDiff} Orange Essense.";
				}
				if (keyCountEnd != keyCountStart)
				{
					var keyDiff = keyCountEnd - keyCountStart;
					string gainType = keyDiff > 0 ? "Gained" : "Lost";
					msg += $"\n- {gainType} {keyDiff} keys.";
				}

				if (startedManually)
					MessageBox.Show(actionsTaken != 0 ? msg : "Nothing is left to disenchant.", "Done");
				else if (actionsTaken != 0 && settings.showDisenchantNotification.Value)
					Program.ShowBalloon(msg);
			}
		}

		private Dictionary<string, string> ParseTags(string tags)
		{
			Dictionary<string, string> result = new Dictionary<string, string>();
			string[] tagCombos = tags.Split(',');
			foreach (string tag in tagCombos)
			{
				if (tag.IndexOf('=') < 0)
				{
					result[tag] = tag;
					continue;
				}

				var split = tag.Split('=');
				result[split[0]] = split[1];
			}

			return result;
		}
	};

	internal enum ActionRules
	{
		None,
		RemoveRules,
		RerollRules
	};
	internal class LootChoreTask : IDisposable
	{
		LootChore m_parent;

		internal LootChoreTask(LootChore parent)
		{
			m_parent = parent;
			m_parent.task = this;
		}

		public void Dispose()
		{
			m_parent.task = null;
		}
	}
}
