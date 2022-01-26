using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueChores.DDragonChampion
{
    class Image
    {
        public string full;
        public string sprite;
        public string group;
        public double x;
        public double y;
        public double w;
        public double h;
    }

    class Skin
    {
        public string id;
        public double num;
        public string name;
        public bool chromas;
    }

    class Info
    {
        public double attack;
        public double defense;
        public double magic;
        public double difficulty;
    }

    class Stats
    {
        public double hp;
        public double hpperlevel;
        public double mp;
        public double mpperlevel;
        public double movespeed;
        public double armor;
        public double armorperlevel;
        public double spellblock;
        public double spellblockperlevel;
        public double attackrange;
        public double hpregen;
        public double hpregenperlevel;
        public double mpregen;
        public double mpregenperlevel;
        public double crit;
        public double critperlevel;
        public double attackdamage;
        public double attackdamageperlevel;
        public double attackspeedperlevel;
        public double attackspeed;
    }

    class Leveltip
    {
        public string[] label;
        public string[] effect;
    }

    class Datavalues
    {
    }

    class Spell
    {
        public string id;
        public string name;
        public string description;
        public string tooltip;
        public Leveltip leveltip;
        public double maxrank;
        public double[] cooldown;
        public string cooldownBurn;
        public double[] cost;
        public string costBurn;
        public Datavalues datavalues;
        public double[][] effect;
        public string[] effectBurn;
        public object[] vars;
        public string costType;
        public string maxammo;
        public double[] range;
        public string rangeBurn;
        public Image image;
        public string resource;
    }

    class Passive
    {
        public string name;
        public string description;
        public Image image;
    }

    class Item
    {
        public string id;
        public double count;
        public bool hideCount;
    }

    class Block
    {
        public string type;
        public bool recMath;
        public bool recSteps;
        public double minSummonerLevel;
        public double maxSummonerLevel;
        public string showIfSummonerSpell;
        public string hideIfSummonerSpell;
        public string appendAfterSection;
        public string[] visibleWithAllOf;
        public string[] hiddenWithAnyOf;
        public Item[] items;
    }

    class Recommended
    {
        public string champion;
        public string title;
        public string map;
        public string mode;
        public string type;
        public string customTag;
        public double sortrank;
        public bool extensionPage;
        public bool useObviousCheckmark;
        public object customPanel;
        public Block[] blocks;
    }

    class Champion
    {
        public string id;
        public string key;
        public string name;
        public string title;
        public Image image;
        public Skin[] skins;
        public string lore;
        public string blurb;
        public string[] allytips;
        public string[] enemytips;
        public string[] tags;
        public string partype;
        public Info info;
        public Stats stats;
        public Spell[] spells;
        public Passive passive;
        public Recommended[] recommended;
    }

    class JSON
    {
        public string type;
        public string format;
        public string version;
        public Dictionary<string, Champion> data;
    };
}
