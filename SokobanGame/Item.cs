using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SokobanGame
{
    class Item
    {
        public int itemX;
        public int itemY;
        public ItemInfo[] infoList;

        public Item(int itemX, int itemY, ItemInfo[] infoList)
        {
            this.itemX = itemX;
            this.itemY = itemY;
            this.infoList = infoList;
        }
        public ItemInfo Gotcha()
        {
            int totalWeight = GetTotalWeight();
            int selectedNumber = SelectRandomNumber(totalWeight);

            int weight = 0;
            for (int i = 0; i < infoList.Length; i++)
            {
                weight += infoList[i].weight;
                if (selectedNumber <= weight)
                    return infoList[i];
            }

            return new ItemInfo("None", 0);
        }
        public int GetTotalWeight()
        {
            int totalWeight = 0;

            for (int i = 0; i < infoList.Length; i++)
            {
                totalWeight += infoList[i].weight;
            }

            return totalWeight;
        }
        public int SelectRandomNumber(int totalWeight)
        {
            Random random = new Random();

            int result = random.Next(totalWeight);
            return result + 1;
        }
        public string GetPlayerSkin(ItemInfo itemInfo)
        {
            switch (itemInfo.id)
            {
                case "Rare":
                    return "R";
                case "UltraRare":
                    return "U";
                case "Legend":
                    return "L";
                default:
                    return "P";
            }
        }
    }
}
