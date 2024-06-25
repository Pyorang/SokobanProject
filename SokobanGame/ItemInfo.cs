using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SokobanGame
{
    struct ItemInfo
    {
        public string id;
        public int weight;

        public ItemInfo(string id, int weight)
        {
            this.id = id;
            this.weight = weight;
        }
    }
}
