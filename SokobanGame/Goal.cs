using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SokobanGame
{
    class Goal
    {
        public int goalX;
        public int goalY;
        public bool inputBox;

        public Goal(int goalX, int goalY)
        {
            this.goalX = goalX;
            this.goalY = goalY;
            inputBox = false;
        }

        public void FillGoal() { inputBox = true; }
    }
}
