using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SokobanGame
{
    struct MoveData
    {
        public int tempLocationX;
        public int tempLocationY;
        public bool ApplyLocation;

        public MoveData(int tempPlayerX, int tempPlayerY)
        {
            this.tempLocationX = tempPlayerX;
            this.tempLocationY = tempPlayerY;
            ApplyLocation = true;
        }
    }
}
