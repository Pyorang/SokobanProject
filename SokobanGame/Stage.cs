using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SokobanGame
{
    class Stage
    {
        public bool finalStage;
        public Player player;
        public List<Box> box;
        public List<Wall> wall;
        public List<Goal> goal;
        public List<Item> item;
        public List<Warp> warp;

        public Stage(int stageNumber, List<ItemInfo> itemInfo)
        {
            box = new List<Box>();
            wall = new List<Wall>();
            goal = new List<Goal>();
            item = new List<Item>();
            warp = new List<Warp>();

            string fileName = "Stage" + stageNumber + ".txt";
            string[] contents = File.ReadAllLines(fileName);

            foreach ( string line in contents)
            {
                string[] parts = line.Split(',');
                string type = parts[0];

                switch(type)
                {
                    case "FinalStage":
                        if (parts[1] == "0")
                            finalStage = false;
                        else
                            finalStage = true;
                        break;
                    case "Player":
                        player = new Player(int.Parse(parts[1]), int.Parse(parts[2]));
                        break;
                    case "Box":
                        box.Add(new Box(int.Parse(parts[1]), int.Parse(parts[2])));
                        break;
                    case "Wall":
                        wall.Add(new Wall(int.Parse(parts[1]), int.Parse(parts[2])));
                        break;
                    case "Goal":
                        goal.Add(new Goal(int.Parse(parts[1]), int.Parse(parts[2])));
                        break;
                    case "Warp":
                        warp.Add(new Warp(int.Parse(parts[1]), int.Parse(parts[2]), int.Parse(parts[3]), int.Parse(parts[4])));
                        break;
                    case "Item":
                        item.Add(new Item(int.Parse(parts[1]), int.Parse(parts[2]), itemInfo));
                        break;
                }
            }
        }
    }
}
