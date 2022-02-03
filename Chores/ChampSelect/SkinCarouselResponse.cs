using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueChores
{
    public class CurrentChampionSelection
    {
        public string championName { get; set; }
        public bool isSkinGrantedFromBoost { get; set; }
        public int selectedChampionId { get; set; }
        public int selectedSkinId { get; set; }
        public bool showSkinSelector { get; set; }
        public bool skinSelectionDisabled { get; set; }
    }

    public class Rental
    {
        public bool rented { get; set; }
    }

    public class Ownership
    {
        public bool freeToPlayReward { get; set; }
        public bool owned { get; set; }
        public Rental rental { get; set; }
    }

    public class ChildSkin
    {
        public int championId { get; set; }
        public string chromaPreviewPath { get; set; }
        public List<string> colors { get; set; }
        public bool disabled { get; set; }
        public int id { get; set; }
        public bool isBase { get; set; }
        public bool isChampionUnlocked { get; set; }
        public bool isUnlockedFromEntitledFeature { get; set; }
        public string name { get; set; }
        public Ownership ownership { get; set; }
        public int parentSkinId { get; set; }
        public string shortName { get; set; }
        public string splashPath { get; set; }
        public object splashVideoPath { get; set; }
        public int stage { get; set; }
        public bool stillObtainable { get; set; }
        public string tilePath { get; set; }
        public bool unlocked { get; set; }
    }

    public class SkinCarouselChampion
    {
        public int championId { get; set; }
        public List<ChildSkin> childSkins { get; set; }
        public string chromaPreviewPath { get; set; }
        public bool disabled { get; set; }
        public List<object> emblems { get; set; }
        public string groupSplash { get; set; }
        public int id { get; set; }
        public bool isBase { get; set; }
        public bool isChampionUnlocked { get; set; }
        public bool isUnlockedFromEntitledFeature { get; set; }
        public string name { get; set; }
        public Ownership ownership { get; set; }
        public string rarityGemPath { get; set; }
        public string splashPath { get; set; }
        public object splashVideoPath { get; set; }
        public bool stillObtainable { get; set; }
        public string tilePath { get; set; }
        public bool unlocked { get; set; }
    }
    
}
