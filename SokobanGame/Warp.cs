using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SokobanGame
{
    class Warp
    {
        public int headX;
        public int headY;
        public int taleX;
        public int taleY;

        public Warp(int headX, int headY, int taleX, int taleY)
        {
            this.headX = headX;
            this.headY = headY;
            this.taleX = taleX;
            this.taleY = taleY;
        }
    }
}
