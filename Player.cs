using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

class Player
{
    /*
    Strategy:
    Phase 1:
    - Build farm nearest
    - Build archer barracks
    - Build tower enough to cover 2 vacant sites?
    - Train archers whenever we have enough gold
    - Trap enemy queen with towers
    */
    static void DefendQueen()
    {
        Site site;
        if (FriendlyTowers.Count() > 0)
        {
            //site = FriendlyQueen.nearestSite(StructureType.Tower, OwnerType.Friendly, false);
            site = FriendlyTowers
                .OrderByDescending(s => s.distanceFrom(EnemyQueen))
                .First();
        }
        else
        {
            site = FriendlyQueen.nearestSite(StructureType.None, OwnerType.None, false);
        }

        Console.WriteLine($"BUILD {site.siteId} TOWER");
    }

    static void QueenAction()
    {
        if (EnemyKnights.Count() > 0
            && FriendlyTowers.Count() >= 3
            && FriendlyQueen.nearestUnit(UnitType.Knight, OwnerType.Enemy).distanceFrom(FriendlyQueen) < 200
            && EnemyKnights.Where(s => s.distanceFrom(FriendlyQueen) < 400).Count() >= 3)
        {
            DefendQueen();

            /*var knight = FriendlyQueen.nearestUnit(UnitType.Knight, OwnerType.Enemy);
            var targetX = (knight.x < FriendlyQueen.x) ? FriendlyQueen.x + Math.Abs(FriendlyQueen.x - knight.x) : FriendlyQueen.x - Math.Abs(FriendlyQueen.x - knight.x);
            var targetY = (knight.y < FriendlyQueen.y) ? FriendlyQueen.y + Math.Abs(FriendlyQueen.y - knight.y) : FriendlyQueen.y - Math.Abs(FriendlyQueen.y - knight.y);

            Console.WriteLine($"MOVE {targetX} {targetY}");*/
        }
        else if (TouchedSite != -1
            && CurrentSite.structureType == StructureType.Goldmine
            && CurrentSite.owner == OwnerType.Friendly
            && new GoldMine(CurrentSite).incomeRate < new GoldMine(CurrentSite).maxMineSize
            && (EnemyKnights.Count() == 0 || CurrentSite.distanceFrom(CurrentSite.nearestUnit(UnitType.Knight, OwnerType.Enemy)) > 200)
            && CurrentSite.IsInsideEnemyTowerRange() == false)
        {
            var site = CurrentSite;
            Console.WriteLine($"BUILD {site.siteId} MINE");
        }
        else if (FriendlyGoldmines.Count() <= 1)
        {
            var site = FriendlyQueen.nearestSite(StructureType.None, OwnerType.None);
            Console.WriteLine($"BUILD {site.siteId} MINE");
        }
        else if (FriendlyBarracks.Count() == 0)
        {
            var site = FriendlyQueen.nearestSite(StructureType.None, OwnerType.None);
            Console.WriteLine($"BUILD {site.siteId} BARRACKS-KNIGHT");
        }
        else if (FriendlyTowers.Count() == 0)
        {
            var site = FriendlyQueen.nearestSite(StructureType.None, OwnerType.None);
            Console.WriteLine($"BUILD {site.siteId} TOWER");
        }
        else if (TouchedSite != -1 && CurrentSite.structureType == StructureType.Tower && CurrentSite.owner == OwnerType.Friendly
            && (new Tower(CurrentSite)).hp < 250
            && FriendlyTowers.Count() <= 3
            && CurrentSite.IsInsideEnemyTowerRange() == false)
        {
            Console.WriteLine($"BUILD {CurrentSite.siteId} TOWER");
        }
        else if (TouchedSite != -1 && CurrentSite.structureType == StructureType.Tower && CurrentSite.owner == OwnerType.Friendly
            && (new Tower(CurrentSite)).hp < 500
            && FriendlyTowers.Count() <= 4
            && CurrentSite.IsInsideEnemyTowerRange() == false)
        {
            Console.WriteLine($"BUILD {CurrentSite.siteId} TOWER");
        }
        else if (FriendlyTowers.Count() <= 3)
        {
            var site = FriendlyQueen.nearestSite(StructureType.None, OwnerType.None, false);
            Console.WriteLine($"BUILD {site.siteId} TOWER");
        }
        else if (FriendlyBarracks.Where(s => s.creepType == CreepType.Knight).Count() == 0)
        {
            var site = FriendlyQueen.nearestSite(StructureType.None, OwnerType.None, false);
            Console.WriteLine($"BUILD {site.siteId} BARRACKS-KNIGHT");
        }
        else if (FriendlyGoldmines.Count() <= 2)
        {
            Site site;
            var sites = Sites
                .OrderBy(s => s.distanceFrom(FriendlyQueen))
                .Where(s => s.structureType == StructureType.None)
                .Where(s => s.gold != 0)
                //&& s => s.distanceFrom(s.nearestSite(StructureType.Tower, OwnerType.Enemy)) > 400
                .Where(s => s.IsInsideEnemyTowerRange() == false);
            
            if (EnemyKnights.Count() > 0)
            {
                site = sites.First(s => s.distanceFrom(s.nearestUnit(UnitType.Knight, OwnerType.Enemy)) > 200);
            }
            else
            {
                site = sites.First();
            }
            
            Console.WriteLine($"BUILD {site.siteId} MINE");
        }
        else if (FriendlyBarracks.Where(s => s.creepType == CreepType.Giant).Count() == 0)
        {
            var site = FriendlyQueen.nearestSite(StructureType.None, OwnerType.None, false);
            Console.WriteLine($"BUILD {site.siteId} BARRACKS-GIANT");
        }
        else if (FriendlyTowers.Where(s => s.hp < 300).Count() > 0)
        {
            var site = FriendlyTowers
                .OrderBy(s => s.hp)
                .Where(s => s.hp < 300)
                .Where(s => s.IsInsideEnemyTowerRange() == false)
                .First();
            Console.WriteLine($"BUILD {site.siteId} TOWER");
        }
        else if (FriendlyBarracks.Where(s => s.creepType == CreepType.Giant).Count() == 0)
        {
            var site = FriendlyQueen.nearestSite(StructureType.None, OwnerType.None, false);
            Console.WriteLine($"BUILD {site.siteId} BARRACKS-GIANT");
        }
        else if (FriendlyTowers.Where(s => s.hp < 300).Count() > 0)
        {
            var site = FriendlyTowers
                .OrderBy(s => s.hp)
                .Where(s => s.hp < 300)
                .Where(s => s.IsInsideEnemyTowerRange() == false)
                .First();
            Console.WriteLine($"BUILD {site.siteId} TOWER");
        }
        else if (FriendlyGoldmines.Where(s => s.incomeRate < s.maxMineSize).Where(s => s.IsInsideEnemyTowerRange() == false).Count() > 0)
        {
            var site = FriendlyGoldmines
                .Where(s => s.incomeRate < s.maxMineSize)
                .Where(s => s.IsInsideEnemyTowerRange() == false)
                .OrderBy(s => s.distanceFrom(FriendlyQueen))
                .First();
            Console.WriteLine($"BUILD {site.siteId} MINE");
        }
        else
        {
            var site = FriendlyTowers
                .OrderBy(s => s.hp)
                .Where(s => s.IsInsideEnemyTowerRange() == false)
                .First();
            Console.WriteLine($"BUILD {site.siteId} TOWER");

            /*Site site;
            var sites = VacantSites
                .OrderBy(s => s.distanceFrom(FriendlyQueen))
                .Where(s => s.IsInsideEnemyTowerRange() == false)
                .Where(s => s.distanceFrom(EnemyQueen) < FriendlyQueen.distanceFrom(EnemyQueen));

            if (sites.Count() > 0)
            {
                site = sites.First();
                Console.WriteLine($"BUILD {site.siteId} TOWER");
                return;
            }
            else
            {
                sites = VacantSites
                    .OrderBy(s => s.distanceFrom(FriendlyQueen))
                    .Where(s => s.IsInsideEnemyTowerRange() == false)
                    .Where(s => s.distanceFrom(EnemyQueen) < FriendlyQueen.distanceFrom(EnemyQueen));
            }

            if (sites.Count() > 0)
            {
                site = sites.First();
                Console.WriteLine($"BUILD {site.siteId} TOWER");
                return;
            }
            else
            {
                site = FriendlyTowers
                    .OrderBy(s => s.hp)
                    .Where(s => s.IsInsideEnemyTowerRange() == false)
                    .First();
                Console.WriteLine($"BUILD {site.siteId} TOWER");
                return;
            }*/
        }
    }

    static void TrainingAction()
    {
        var archerBarracks = FriendlyBarracks
            .Where(s => s.creepType == CreepType.Archer)
            .OrderBy(s => s.distanceFrom(EnemyQueen));
        var giantBarracks = FriendlyBarracks
            .Where(s => s.creepType == CreepType.Giant)
            .OrderBy(s => s.distanceFrom(EnemyQueen));
        var knightBarracks = FriendlyBarracks
            .Where(s => s.creepType == CreepType.Knight)
            .OrderBy(s => s.distanceFrom(EnemyQueen));

        if (EnemyTowers.Count() >= 3 && giantBarracks.Count() > 0)
        {
            /*if (Gold >= 220)
            {
                Console.WriteLine($"TRAIN {giantBarracks.First().siteId} {knightBarracks.First().siteId}");
            }*/
            if (Gold >= 180)
            {
                Console.WriteLine($"TRAIN {giantBarracks.First().siteId}");
            }
            else if (Gold >= 80
                && Units
                    .Where(s => s.unitType == UnitType.Giant && s.owner == OwnerType.Friendly)
                    .Count() > 0)
            {
                Console.WriteLine($"TRAIN {knightBarracks.First().siteId}");
            }
            else
            {
                Console.WriteLine("TRAIN");
            }
        }
        else if (knightBarracks.Count() > 0 && Gold > 80)
        {
            Console.WriteLine($"TRAIN {knightBarracks.First().siteId}");
        }
        else if (archerBarracks.Count() > 0 && Gold > 120)
        {
            Console.WriteLine($"TRAIN {archerBarracks.First().siteId}");
        }
        else
        {
            Console.WriteLine("TRAIN");
        }
    }

    public static List<Site> Sites { get; set; }
    public static int Gold { get; set; }
    public static int TouchedSite { get; set; }
    public static List<Unit> Units { get; set; }

    static void Main(string[] args)
    {
        string[] inputs;
        int numSites = int.Parse(Console.ReadLine());
        Sites = new List<Site>();

        for (int i = 0; i < numSites; i++)
        {
            Sites.Add(new Site(Console.ReadLine().Split(' ')));
        }

        // game loop
        while (true)
        {
            inputs = Console.ReadLine().Split(' ');
            Gold = int.Parse(inputs[0]);
            TouchedSite = int.Parse(inputs[1]); // -1 if none
            for (int i = 0; i < Sites.Count; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                int siteId = int.Parse(inputs[0]);
                Sites.First(s => s.siteId == siteId).Update(inputs);

            }

            int numUnits = int.Parse(Console.ReadLine());
            Units = new List<Unit>();
            for (int i = 0; i < numUnits; i++)
            {
                Units.Add(new Unit(Console.ReadLine().Split(' ')));
            }

            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");

            

            // First line: A valid queen action
            QueenAction();

            // Second line: A set of training instruction
            TrainingAction();
        }
    }

    public static Unit FriendlyQueen
    {
        get
        {
            return Units.First(u => u.unitType == UnitType.Queen
                && u.owner == OwnerType.Friendly);
        }
    }

    public static Unit EnemyQueen
    {
        get
        {
            return Units.First(u => u.unitType == UnitType.Queen
                && u.owner == OwnerType.Enemy);
        }
    }

    public static IEnumerable<Unit> EnemyKnights
    {
        get
        {
            return Units.Where(u => u.unitType == UnitType.Knight
                && u.owner == OwnerType.Enemy);
        }
    }

    public static IEnumerable<Barracks> Barracks
    {
        get
        {
            return Sites
                .Where(s => s.structureType == StructureType.Barracks)
                .Select(s => new Barracks(s))
                .Cast<Barracks>();
        }
    }

    public static IEnumerable<GoldMine> Goldmines
    {
        get
        {
            return Sites
                .Where(s => s.structureType == StructureType.Goldmine)
                .Select(s => new GoldMine(s))
                .Cast<GoldMine>();
        }
    }

    public static IEnumerable<Tower> Towers
    {
        get
        {
            return Sites
                .Where(s => s.structureType == StructureType.Tower)
                .Select(s => new Tower(s))
                .Cast<Tower>();
        }
    }

    public static IEnumerable<Barracks> FriendlyBarracks
    {
        get
        {
            return Barracks.Where(s => s.owner == OwnerType.Friendly);
        }
    }

    public static IEnumerable<GoldMine> FriendlyGoldmines
    {
        get
        {
            return Goldmines.Where(s => s.owner == OwnerType.Friendly);
        }
    }

    public static IEnumerable<Tower> FriendlyTowers
    {
        get
        {
            return Towers.Where(s => s.owner == OwnerType.Friendly);
        }
    }

    public static IEnumerable<Barracks> EnemyBarracks
    {
        get
        {
            return Barracks.Where(s => s.owner == OwnerType.Enemy);
        }
    }

    public static IEnumerable<GoldMine> EnemyGoldmines
    {
        get
        {
            return Goldmines.Where(s => s.owner == OwnerType.Enemy);
        }
    }

    public static IEnumerable<Tower> EnemyTowers
    {
        get
        {
            return Towers.Where(s => s.owner == OwnerType.Enemy);
        }
    }

    public static IEnumerable<Site> VacantSites
    {
        get
        {
            return Sites.Where(s => s.owner == OwnerType.None && s.structureType == StructureType.None);
        }
    }

    public static IEnumerable<Site> FriendlySites
    {
        get
        {
            return Sites.Where(s => s.owner == OwnerType.Friendly);
        }
    }

    public static IEnumerable<Site> EnemySites
    {
        get
        {
            return Sites.Where(s => s.owner == OwnerType.Enemy);
        }
    }

    public static Site CurrentSite
    {
        get
        {
            return Sites.First(s => s.siteId == TouchedSite);
        }
    }
}

enum StructureType
{
    None = -1, Goldmine, Tower, Barracks
}

class Site : Physical
{
    public int siteId { get; set; }
    public int radius { get; set; }

    public int gold { get; set; } // used in future leagues
    public int maxMineSize { get; set; } // used in future leagues
    public StructureType structureType { get; set; }
    public int param1 { get; set; }
    /* When no structure: -1
    When goldmine: the income rate ranging from 1 to 5 (or -1 if enemy)
    When tower: the remaining HP
    When barracks, the number of turns before a new set of creeps can be trained(if 0, then training may be started this turn)
    */
    public int param2 { get; set; }
    /*
    When no structure: -1
    When goldmine: -1
    When tower: the attack radius measured from its center
    When barracks: the creep type: 0 for KNIGHT, 1 for ARCHER, 2 for GIANT
    */

    public Site()
    {

    }
    
    public Site(string[] inputs)
    {
        siteId = int.Parse(inputs[0]);
        x = int.Parse(inputs[1]);
        y = int.Parse(inputs[2]);
        radius = int.Parse(inputs[3]);
    }

    public void Update(string[] inputs)
    {
        gold = int.Parse(inputs[1]);
        maxMineSize = int.Parse(inputs[2]);
        structureType = (StructureType)int.Parse(inputs[3]);
        owner = (OwnerType)int.Parse(inputs[4]);
        param1 = int.Parse(inputs[5]);
        param2 = int.Parse(inputs[6]);
    }
}

class GoldMine : Site
{
    public GoldMine(Site site)
    {
        this.siteId = site.siteId;
        this.x = site.x;
        this.y = site.y;
        this.radius = site.radius;
        this.gold = site.gold;
        this.maxMineSize = site.maxMineSize;
        this.structureType = site.structureType;
        this.owner = site.owner;
        this.param1 = site.param1;
        this.param2 = site.param2;
    }

    public int incomeRate
    {
        get
        {
            return param1;
        }
    }
}

enum CreepType
{
    Knight, Archer, Giant
}

class Barracks : Site
{
    public Barracks(Site site)
    {
        this.siteId = site.siteId;
        this.x = site.x;
        this.y = site.y;
        this.radius = site.radius;
        this.gold = site.gold;
        this.maxMineSize = site.maxMineSize;
        this.structureType = site.structureType;
        this.owner = site.owner;
        this.param1 = site.param1;
        this.param2 = site.param2;
    }

    public CreepType creepType
    {
        get
        {
            return (CreepType)param2;
        }
    }

    public int cooldown
    {
        get
        {
            return param1;
        }
    }
}

class Tower : Site
{
    public Tower(Site site)
    {
        this.siteId = site.siteId;
        this.x = site.x;
        this.y = site.y;
        this.radius = site.radius;
        this.gold = site.gold;
        this.maxMineSize = site.maxMineSize;
        this.structureType = site.structureType;
        this.owner = site.owner;
        this.param1 = site.param1;
        this.param2 = site.param2;
    }

    public int hp
    {
        get
        {
            return param1;
        }
    }

    public int range
    {
        get
        {
            return param2;
        }
    }
}

enum UnitType
{
    Queen = -1, Knight, Archer, Giant
}

class Unit : Physical
{

    public UnitType unitType { get; set; }
    public int health { get; set; }

    public Unit()
    {

    }

    public Unit(string[] inputs)
    {
        x = int.Parse(inputs[0]);
        y = int.Parse(inputs[1]);
        owner = (OwnerType)int.Parse(inputs[2]);
        unitType = (UnitType)int.Parse(inputs[3]);
        health = int.Parse(inputs[4]);
    }
}

enum OwnerType
{
    None = -1, Friendly, Enemy
}
class Physical
{
    public int x { get; set; }
    public int y { get; set; }
    public OwnerType owner { get; set; }

    public int distanceFrom(Physical p)
    {
        int xDistance = Math.Abs(p.x - this.x);
        int yDistance = Math.Abs(p.y - this.y);

        if (xDistance + yDistance == 0)
            return 0;

        return Convert.ToInt32(Math.Sqrt((xDistance * xDistance) + (yDistance * yDistance)));
    }

    public Unit nearestUnit(UnitType unitType)
    {
        var unit = Player.Units
            .OrderBy(u => u.distanceFrom(this))
            .First(u => u.unitType == unitType);
        return unit;
    }

    public Unit nearestUnit(UnitType unitType, OwnerType owner)
    {
        var unit = Player.Units
            .OrderBy(u => u.distanceFrom(this))
            .First(u => u.unitType == unitType
                && u.owner == owner);
        return unit;
    }

    public Site nearestSite(StructureType structureType)
    {
        var site = Player.Sites
            .OrderBy(s => s.distanceFrom(this))
            .First(s => s.structureType == structureType);
        return site;
    }

    public Site nearestSite(StructureType structureType, OwnerType owner)
    {
        var site = Player.Sites
            .OrderBy(s => s.distanceFrom(this))
            .First(s => s.structureType == structureType
                && s.owner == owner);
        return site;
    }

    public Site nearestSite(StructureType structureType, OwnerType owner, bool isInsideEnemyTowerRange)
    {
        var site = Player.Sites
            .OrderBy(s => s.distanceFrom(this))
            .First(s => s.structureType == structureType
                && s.owner == owner
                && s.IsInsideEnemyTowerRange() == isInsideEnemyTowerRange);
        return site;
    }

    public bool IsInsideEnemyTowerRange()
    {
        return Player.EnemyTowers
            .Where(s => s.distanceFrom(this) < s.range)
            .Count() > 0;
    }
}
