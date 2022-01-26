using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueChores.Chores.Loot
{

	[Serializable]
	class ChampionMastery
	{
		public Int64 championId;
		public Int64 championLevel;
		public Int64 championPoints;
		public Int64 championPointsSinceLastLevel;
		public Int64 championPointsUntilNextLevel;
		public bool chestGranted;
		public string formattedChampionPoints;
		public string formattedMasteryGoal;
		public string highestGrade;
		public Int64 lastPlayTime;
		public Int64 playerId;
		public Int64 tokensEarned;
	}
}
