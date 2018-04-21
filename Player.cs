using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
class Player
{
    public static List<Site> Sites { get; set; }
    public static int Gold { get; set; }
    public static int TouchedSite { get; set; }
    public static List<Unit> Units { get; set; }

    public static Unit FriendlyQueen
    {
        get
        {
            return Units.First(u => u.unitType == -1
                && u.owner == 0);
        }
    }

    public static Unit EnemyQueen
    {
        get
        {
            return Units.First(u => u.unitType == -1
                && u.owner == 1);
        }
    }

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
            /*if (Sites.Count(s => s.owner == 0 && s.structureType == 2) >= 2
                && Sites.Count(s => s.owner == 1 && s.structureType == 2) >= 1)
            {
                // I have 2 barracks, destroy enemy barracks
                Site site = FriendlyQueen.nearestSite(2, 1);
                Console.WriteLine($"MOVE {site.x} {site.y}");
            }
            else*/
            if (Sites.Count(s => s.owner == 0 && s.structureType == 2) >= 3)
            {
                // We have 3 barracks, protect the queen
                Site site;

                if (Sites.Count(s => s.owner == 1 && s.structureType == 2) > 0)
                {
                    site = Sites
                        .OrderByDescending(s => s.distanceFrom(FriendlyQueen.nearestSite(2, 1)))
                        .First(s => s.owner == 0 && s.structureType == 2);
                }
                else
                {
                    site = FriendlyQueen.nearestSite(2, 0);
                }

                //if (TouchedSite == )
                //{
                    Console.WriteLine($"BUILD {site.siteId} TOWER");
                /*}
                else
                {
                    Console.WriteLine($"MOVE {site.x} {site.y}");
                }*/
            }
            else if (TouchedSite == -1 || Sites.First(s => s.siteId == TouchedSite).structureType >= 0)
            {
                // Go to an empty site
                Site site = FriendlyQueen.nearestSite(-1);
                Console.WriteLine($"MOVE {site.x} {site.y}");
            }
            else if (Sites.Count(s => s.owner == 0 && s.structureType == 2) == 0)
            {
                // Build knights first
                Console.WriteLine($"BUILD {TouchedSite} BARRACKS-KNIGHT");
            }
            else if (Sites.First(s => s.siteId == TouchedSite).structureType == -1)
            {
                // More knights!
                Console.WriteLine($"BUILD {TouchedSite} BARRACKS-KNIGHT");
            }
            else if (Sites.Count(s => s.owner == 1) == 0)
            {
                // Build a knight barracks on the empty touched site
                Console.WriteLine($"BUILD {TouchedSite} BARRACKS-KNIGHT");
            }
            else
            {
                Console.WriteLine("WAIT");
            }

            // Second line: A set of training instruction
            if (Gold >= 80 && Sites.Count(s => s.structureType == 2 && s.owner == 0) > 0)
            {
                // Train knights at barracks closest to enemy queen
                var friendlyBarracks = Sites
                    .OrderBy(s => s.distanceFrom(EnemyQueen))
                    .Where(s => s.owner == 0 && s.structureType == 2)
                    .Take(Gold / 80);
                string barracksIds = string.Join(" ", friendlyBarracks.Select(b => b.siteId).ToArray());

                Console.WriteLine($"TRAIN {barracksIds}");
            }
            else
            {
                Console.WriteLine("TRAIN");
            }
        }
    }
}

class Site : Physical
{
    public int siteId { get; set; }
    public int radius { get; set; }

    public int ignore1 { get; set; } // used in future leagues
    public int ignore2 { get; set; } // used in future leagues
    public int structureType { get; set; } // -1 = No structure, 2 = Barracks
    public int param1 { get; set; }
    public int param2 { get; set; }

    public Site(string[] inputs)
    {
        siteId = int.Parse(inputs[0]);
        x = int.Parse(inputs[1]);
        y = int.Parse(inputs[2]);
        radius = int.Parse(inputs[3]);
    }

    public void Update(string[] inputs)
    {
        //siteId = int.Parse(inputs[0]);
        ignore1 = int.Parse(inputs[1]); // used in future leagues
        ignore2 = int.Parse(inputs[2]); // used in future leagues
        structureType = int.Parse(inputs[3]); // -1 = No structure, 2 = Barracks
        owner = int.Parse(inputs[4]); // -1 = No structure, 0 = Friendly, 1 = Enemy
        param1 = int.Parse(inputs[5]);
        param2 = int.Parse(inputs[6]);
    }
}

class Unit : Physical
{

    public int unitType { get; set; } // -1 = QUEEN, 0 = KNIGHT, 1 = ARCHER
    public int health { get; set; }

    public Unit(string[] inputs)
    {
        x = int.Parse(inputs[0]);
        y = int.Parse(inputs[1]);
        owner = int.Parse(inputs[2]);
        unitType = int.Parse(inputs[3]);
        health = int.Parse(inputs[4]);
    }

    public Unit nearestUnit(int unitType)
    {
        var unit = Player.Units
            .OrderBy(u => u.distanceFrom(this))
            .First(u => u.unitType == unitType);
        return unit;
    }

    public Unit nearestUnit(int unitType, int owner)
    {
        var unit = Player.Units
            .OrderBy(u => u.distanceFrom(this))
            .First(u => u.unitType == unitType
                && u.owner == owner);
        return unit;
    }

    public Site nearestSite(int structureType)
    {
        var site = Player.Sites
            .OrderBy(s => s.distanceFrom(this))
            .First(s => s.structureType == structureType);
        return site;
    }

    public Site nearestSite(int structureType, int owner)
    {
        var site = Player.Sites
            .OrderBy(s => s.distanceFrom(this))
            .First(s => s.structureType == structureType
                && s.owner == owner);
        return site;
    }
}

class Physical
{
    public int x { get; set; }
    public int y { get; set; }
    public int owner { get; set; } // -1 = No structure, 0 = Friendly, 1 = Enemy

    public int distanceFrom(Physical p)
    {
        int xDistance = Math.Abs(p.x - this.x);
        int yDistance = Math.Abs(p.y - this.y);

        if (xDistance + yDistance == 0)
            return 0;

        return Convert.ToInt32(Math.Sqrt((xDistance * xDistance) + (yDistance * yDistance)));
    }
}
