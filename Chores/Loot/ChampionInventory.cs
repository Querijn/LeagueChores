using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueChores.Chores.Loot
{
	public class Rental
	{
		public int endDate { get; set; }
		public object purchaseDate { get; set; }
		public bool rented { get; set; }
		public int winCountRemaining { get; set; }
	}

	public class Ownership
	{
		public bool freeToPlayReward { get; set; }
		public bool owned { get; set; }
		public Rental rental { get; set; }
	}

	public class Passive
	{
		public string description { get; set; }
		public string name { get; set; }
	}

	public class Chroma
	{
		public int championId { get; set; }
		public string chromaPath { get; set; }
		public List<string> colors { get; set; }
		public bool disabled { get; set; }
		public int id { get; set; }
		public bool lastSelected { get; set; }
		public string name { get; set; }
		public Ownership ownership { get; set; }
		public bool stillObtainable { get; set; }
	}

	public class DescriptionInfo
	{
		public string description { get; set; }
		public string iconPath { get; set; }
		public string title { get; set; }
	}

	public class Tier
	{
		public int championId { get; set; }
		public object chromaPath { get; set; }
		public string collectionSplashVideoPath { get; set; }
		public string description { get; set; }
		public bool disabled { get; set; }
		public int id { get; set; }
		public bool isBase { get; set; }
		public bool lastSelected { get; set; }
		public string loadScreenPath { get; set; }
		public string name { get; set; }
		public Ownership ownership { get; set; }
		public string shortName { get; set; }
		public string splashPath { get; set; }
		public string splashVideoPath { get; set; }
		public int stage { get; set; }
		public bool stillObtainable { get; set; }
		public string tilePath { get; set; }
		public string uncenteredSplashPath { get; set; }
	}

	public class QuestSkinInfo
	{
		public string collectionCardPath { get; set; }
		public string collectionDescription { get; set; }
		public List<DescriptionInfo> descriptionInfo { get; set; }
		public string name { get; set; }
		public string splashPath { get; set; }
		public List<Tier> tiers { get; set; }
		public string tilePath { get; set; }
		public string uncenteredSplashPath { get; set; }
	}

	public class Skin
	{
		public int championId { get; set; }
		public string chromaPath { get; set; }
		public List<Chroma> chromas { get; set; }
		public string collectionSplashVideoPath { get; set; }
		public bool disabled { get; set; }
		public List<object> emblems { get; set; }
		public string featuresText { get; set; }
		public int id { get; set; }
		public bool isBase { get; set; }
		public bool lastSelected { get; set; }
		public string loadScreenPath { get; set; }
		public string name { get; set; }
		public Ownership ownership { get; set; }
		public QuestSkinInfo questSkinInfo { get; set; }
		public string rarityGemPath { get; set; }
		public string skinType { get; set; }
		public string splashPath { get; set; }
		public string splashVideoPath { get; set; }
		public bool stillObtainable { get; set; }
		public string tilePath { get; set; }
		public string uncenteredSplashPath { get; set; }
	}

	public class Spell
	{
		public string description { get; set; }
		public string name { get; set; }
	}

	public class TacticalInfo
	{
		public string damageType { get; set; }
		public int difficulty { get; set; }
		public int style { get; set; }
	}

	public class OwnedChampion
	{
		public bool active { get; set; }
		public string alias { get; set; }
		public string banVoPath { get; set; }
		public string baseLoadScreenPath { get; set; }
		public string baseSplashPath { get; set; }
		public bool botEnabled { get; set; }
		public string chooseVoPath { get; set; }
		public List<object> disabledQueues { get; set; }
		public bool freeToPlay { get; set; }
		public int id { get; set; }
		public string name { get; set; }
		public Ownership ownership { get; set; }
		public Passive passive { get; set; }
		public object purchased { get; set; }
		public bool rankedPlayEnabled { get; set; }
		public List<string> roles { get; set; }
		public List<Skin> skins { get; set; }
		public List<Spell> spells { get; set; }
		public string squarePortraitPath { get; set; }
		public string stingerSfxPath { get; set; }
		public TacticalInfo tacticalInfo { get; set; }
		public string title { get; set; }
	}

}
