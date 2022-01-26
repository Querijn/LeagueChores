using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace LeagueChores
{
	using ChampionDictionary = Dictionary<int, DDragonChampion.Champion>;

	internal static class DDragon
	{
		static Dictionary<string, ChampionDictionary> championByIdCache = new Dictionary<string, ChampionDictionary>();
		static Dictionary<string, DDragonChampion.JSON>	championJson = new Dictionary<string, DDragonChampion.JSON>();

		static HttpClient m_client;

		static DDragon()
		{
			m_client = new HttpClient();
			m_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			m_client.Timeout = new TimeSpan(0, 0, 15);
			m_client.BaseAddress = new Uri("http://ddragon.leagueoflegends.com");
		}
		public static async Task<T> GetAs<T>(string url)
		{
			HttpResponseMessage response = await m_client.GetAsync(url);
			if (response.IsSuccessStatusCode == false)
				return default(T);

			string contents = await response.Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<T>(contents);
		}

		public static async Task<string[]> GetVersionList()
		{
			return await GetAs<string[]>("/api/versions.json");
		}

		public static async Task<DDragonChampion.JSON> GetLatestChampionData(string language = "en_US")
		{
			if (championJson.ContainsKey(language))
				return championJson[language];

			DDragonChampion.JSON response;
			var versionIndex = 0;
			string[] versionList = await GetVersionList();
			do
			{ // I loop over versions because 9.22.1 is broken
				var version = versionList[versionIndex++];

				response = await GetAs<DDragonChampion.JSON>($"/cdn/{version}/data/{language}/champion.json");
			}
			while (response == null);

			return championJson[language] = response;
		}

		public static async Task<DDragonChampion.Champion> GetChampionByKey(int key, string language = "en_US")
		{
			if (championByIdCache.ContainsKey(language) == false)
			{
				// Setup cache
				var json = await GetLatestChampionData(language);

				championByIdCache[language] = new ChampionDictionary();
				foreach (var champInfo in json.data)
				{
					string championName = champInfo.Key;
					if (json.data.ContainsKey(championName) == false)
						continue;

					championByIdCache[language][int.Parse(champInfo.Value.key)] = champInfo.Value;
				}
			}

			return championByIdCache[language][key];
		}
	}
}
