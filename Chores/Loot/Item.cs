using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueChores.Chores.Loot
{
	internal class Item
	{
		public PlayerLoot item;
		public long count { get; private set; }
		public long value { get; private set; }
		public ItemType type { get; private set; }
		public bool hasDefaultAction => m_actions.Count > 0;
		public string defaultActionName => defaultAction.Key;
		public KeyValuePair<string, ItemAction> defaultAction => m_actions.First();

		Dictionary<string, ItemAction> m_actions = new Dictionary<string, ItemAction>();

		public Item(PlayerLoot item, ItemType type)
		{
			this.item = item;
			this.type = type;
			this.count = item.count;
		}

		public Item(PlayerLoot item, ItemType type, long count)
		{
			this.item = item;
			this.type = type;
			this.count = count;
		}

		public void AddAction(string name, string uri, string body)
		{
			m_actions[name] = new ItemAction(uri, body);
		}

		public async Task<bool> PerformDefaultTask()
		{
			return await defaultAction.Value.Perform();
		}

		public override string ToString()
		{
			return $"{count}x {item.localizedName}";
		}
	}
	internal class ItemAction
	{
		public string uri { get; private set; }
		public string body { get; private set; }

		public ItemAction(string uri, string body)
		{
			this.uri = uri;
			this.body = body;
		}

		public async Task<bool> Perform()
		{
			var response = await LCU.Post(uri, body);
			if (response.ok)
				return true;

			var message = response.body?["message"].ToString() ?? string.Empty;
			var messageAddon = string.IsNullOrWhiteSpace(message) == false ? $" Error message: {message}" : "";
			Console.WriteLine($"ItemAction '{uri}' failed, status code '{response.statusCode}'.{messageAddon}\nBody: '{body}'");
			return false;
		}
	}

	enum ItemType
	{
		Champion,
		Eternal,
		Skin,
		Chest,
		ChampionCapsule,
		EternalsCapsule,
		KeyFragment
	};
}
