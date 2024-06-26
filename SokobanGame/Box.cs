using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SokobanGame
{
    class Box
    {
        public MoveData moveData;
        public int boxX;
        public int boxY;

        public Box(int boxX, int boxY)
        {
            moveData = new MoveData(boxX, boxY);
            moveData.ApplyLocation = true;
            this.boxX = boxX;
            this.boxY = boxY;
        }

        public void PrepareGoLeft() { moveData.tempLocationX--; }
        public void PrepareGoRight() { moveData.tempLocationX++; }
        public void PrepareGoUp() { moveData.tempLocationY--; }
        public void PrepareGoDown() { moveData.tempLocationY++; }
        public void CheckBoxBoundaryException()
        {
            if (moveData.tempLocationX < 0 || moveData.tempLocationY < 0 || moveData.tempLocationX > Console.BufferWidth || moveData.tempLocationY > Console.BufferHeight)
                moveData.ApplyLocation = false;
        }
        public void CheckBoxNextBox(List<Box> box)
        {
            for (int i = 0; i < box.Count; i++)
            {
                if (moveData.tempLocationX == box[i].boxX && moveData.tempLocationY == box[i].boxY)
                    moveData.ApplyLocation = false;
            }
        }
        public void CheckBoxNextWall(List<Wall> wall)
        {
            for (int i = 0; i < wall.Count; i++)
            {
                if (moveData.tempLocationX == wall[i].wallX && moveData.tempLocationY == wall[i].wallY)
                    moveData.ApplyLocation = false;
            }
        }
        public void CheckBoxNextWarpTale(List<Warp> warp)
        {
            for (int i = 0; i < warp.Count; i++)
            {
                if (moveData.tempLocationX == warp[i].taleX && moveData.tempLocationY == warp[i].taleY)
                    moveData.ApplyLocation = false;
            }
        }
        public void CheckBoxRouteBlocked(List<Box> box, List<Wall> wall, List<Warp> warp)
        {
            CheckBoxBoundaryException();
            CheckBoxNextBox(box);
            CheckBoxNextWall(wall);
            CheckBoxNextWarpTale(warp);
        }
        public void ApplyBoxMove()
        {
            if (moveData.ApplyLocation)
            {
                boxX = moveData.tempLocationX;
                boxY = moveData.tempLocationY;
            }
        }
        public void InitializeMoveData()
        {
            moveData = new MoveData(boxX, boxY);
            moveData.ApplyLocation = true;
        }
    }
}
