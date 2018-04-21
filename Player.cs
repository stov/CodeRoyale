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
            if (false)
            {

            }
            else
            {
                Console.WriteLine("WAIT");
            }

            // Second line: A set of training instruction
            if (false)
            {

            }
            else
            {
                Console.WriteLine("TRAIN");
            }
        }
    }
}

class Site
{
    public int siteId { get; set; }
    public int x { get; set; }
    public int y { get; set; }
    public int radius { get; set; }

    public int ignore1 { get; set; } // used in future leagues
    public int ignore2 { get; set; } // used in future leagues
    public int structureType { get; set; } // -1 = No structure, 2 = Barracks
    public int owner { get; set; } // -1 = No structure, 0 = Friendly, 1 = Enemy
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

class Unit
{
    public int x { get; set; }
    public int y { get; set; }
    public int owner { get; set; }
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
}
