using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SokobanGame
{
    class Wall
    {
        public int wallX;
        public int wallY;

        public Wall(int wallX, int wallY)
        {
            this.wallX = wallX;
            this.wallY = wallY;
        }
    }
}
