using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace LeagueChores
{
	internal class Queue
	{
		public string name { get; set; }
		public string shortName { get; set; }
		public string description { get; set; }
		public string detailedDescription { get; set; }
	}

	public class LobbyGame // TODO: not really supposed to be here, needs new file
	{
		public string allowSpectators { get; set; }
		public string botDifficulty { get; set; }
		public string gameType { get; set; }
		public int? gameTypeConfigId { get; set; }
		public int? mapId { get; set; }
		public int maxPartySize { get; set; }
		public int maxTeamSize { get; set; }
		public int queueId { get; set; }
	}

	internal static class QueueList
	{
		static readonly string m_urfQueuesListUri = "https://raw.communitydragon.org/pbe/plugins/rcp-be-lol-game-data/global/default/v1/queues.json";
		static readonly string m_urfQueuesLocalFileName = "queues.json";
		static HttpClient m_client = new HttpClient();

		public static async Task<Dictionary<int, Queue>> Get(bool rejectCache = false)
		{
			if (rejectCache)
				return await MakeRequest();

			if (File.Exists(m_urfQueuesLocalFileName) == false)
				return await MakeRequest();

			var contents = File.ReadAllText(m_urfQueuesLocalFileName); // Store file contents locally

			var timeSinceCreation = DateTime.Now - File.GetCreationTime(m_urfQueuesLocalFileName);
			if (timeSinceCreation.TotalHours > 24 * 3) // 3 days old, reject
			{
				var result = await MakeRequest();
				if (result.Count != 0) // Is valid?
					return result;

				// Cdragon isn't returning data correctly, reset our cache
				File.WriteAllText(m_urfQueuesLocalFileName, contents);
			}

			return JsonConvert.DeserializeObject<Dictionary<int, Queue>>(contents);
		}

		static async Task<Dictionary<int, Queue>> MakeRequest()
		{
			var response = await m_client.GetAsync(m_urfQueuesListUri);
			if (response.StatusCode != System.Net.HttpStatusCode.OK)
				return new Dictionary<int, Queue>();

			string contents = await response.Content.ReadAsStringAsync();
			File.WriteAllText(m_urfQueuesLocalFileName, contents);
			return JsonConvert.DeserializeObject<Dictionary<int, Queue>>(contents);
		}
	}
}
