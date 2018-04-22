using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

class Player
{
    static void QueenAction()
    {
        if (false)
        {
            
        }
        else
        {
            Console.WriteLine("WAIT");
        }
    }

    static void TrainingAction()
    {
        if (false)
        {
            
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

    public IEnumerable<Site> VacantSites
    {
        get
        {
            return Sites.Where(s => s.owner == OwnerType.None);
        }
    }

    public IEnumerable<Site> FriendlySites
    {
        get
        {
            return Sites.Where(s => s.owner == OwnerType.Friendly);
        }
    }

    public IEnumerable<Site> EnemySites
    {
        get
        {
            return Sites.Where(s => s.owner == OwnerType.Enemy);
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

    public int ignore1 { get; set; } // used in future leagues
    public int ignore2 { get; set; } // used in future leagues
    public StructureType structureType { get; set; } // -1 = No structure, 0 = Goldmine, 1 = Tower, 2 = Barracks
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
        ignore1 = int.Parse(inputs[1]);
        ignore2 = int.Parse(inputs[2]);
        structureType = (StructureType)int.Parse(inputs[3]);
        owner = (OwnerType)int.Parse(inputs[4]);
        param1 = int.Parse(inputs[5]);
        param2 = int.Parse(inputs[6]);
    }
}

class GoldMine : Site
{
    public int IncomeRate
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
    public CreepType CreepType
    {
        get
        {
            return (CreepType)param2;
        }
    }

    public int Cooldown
    {
        get
        {
            return param1;
        }
    }
}

class Tower : Site
{
    public int HP
    {
        get
        {
            return param1;
        }
    }

    public int Range
    {
        get
        {
            return param2;
        }
    }
}

enum UnitType
{
    Queen = -1, Knight, Archer
}

class Unit : Physical
{

    public UnitType unitType { get; set; } // -1 = QUEEN, 0 = KNIGHT, 1 = ARCHER
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
}

enum OwnerType
{
    None = -1, Friendly, Enemy
}
class Physical
{
    public int x { get; set; }
    public int y { get; set; }
    public OwnerType owner { get; set; } // -1 = No structure, 0 = Friendly, 1 = Enemy

    public int distanceFrom(Physical p)
    {
        int xDistance = Math.Abs(p.x - this.x);
        int yDistance = Math.Abs(p.y - this.y);

        if (xDistance + yDistance == 0)
            return 0;

        return Convert.ToInt32(Math.Sqrt((xDistance * xDistance) + (yDistance * yDistance)));
    }
}
