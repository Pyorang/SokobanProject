using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SokobanGame
{
    class Player
    {
        public MoveData moveData;
        public int playerX;
        public int playerY;
        public string skin;

        public Player(int playerX, int playerY)
        {
            moveData = new MoveData(playerX, playerY);
            this.playerX = playerX;
            this.playerY = playerY;
            skin = "P";
        }

        public void PrepareGoLeft() { moveData.tempLocationX--; }
        public void PrepareGoRight() { moveData.tempLocationX++; }
        public void PrepareGoUp() { moveData.tempLocationY--; }
        public void PrepareGoDown() { moveData.tempLocationY++; }
        public void ChangePlayerTempLocation(ConsoleKeyInfo currentKeyInfo)
        {
            switch (currentKeyInfo.Key)
            {
                case ConsoleKey.LeftArrow:
                    PrepareGoLeft();
                    break;

                case ConsoleKey.RightArrow:
                    PrepareGoRight();
                    break;

                case ConsoleKey.UpArrow:
                    PrepareGoUp();
                    break;

                case ConsoleKey.DownArrow:
                    PrepareGoDown();
                    break;
            }
        }
        public void WarpPlayer(ConsoleKeyInfo currentKeyInfo, List<Warp> warp)
        {
            for (int i = 0; i < warp.Count; i++)
            {
                if (moveData.tempLocationX == warp[i].headX && moveData.tempLocationY == warp[i].headY)
                {
                    moveData.tempLocationX = warp[i].taleX;
                    moveData.tempLocationY = warp[i].taleY;
                    ChangePlayerTempLocation(currentKeyInfo);
                }
            }
        }
        public void HandleBoundaryException()
        {
            if (moveData.tempLocationX < 0 || moveData.tempLocationY < 0 || moveData.tempLocationX > Console.BufferWidth || moveData.tempLocationY > Console.BufferHeight)
                moveData.ApplyLocation = false;
        }
        public void HandleBoxContact(List<Box> box)
        {
            for (int i = 0; i < box.Count; i++)
            {
                if (moveData.tempLocationX == box[i].boxX && moveData.tempLocationY == box[i].boxY)
                {
                    if (!box[i].moveData.ApplyLocation)
                        moveData.ApplyLocation = false;
                }
            }
        }
        public void HandleWallContact(List<Wall> wall)
        {
            for (int i = 0; i < wall.Count; i++)
            {
                if (moveData.tempLocationX == wall[i].wallX && moveData.tempLocationY == wall[i].wallY)
                {
                    moveData.ApplyLocation = false;
                }
            }
        }
        public void HandleGoalContact(List<Goal> goal)
        {
            for (int i = 0; i < goal.Count; i++)
            {
                if (moveData.tempLocationX == goal[i].goalX && moveData.tempLocationY == goal[i].goalY)
                {
                    moveData.ApplyLocation = false;
                }
            }
        }
        public void HandleWarpTaleContact(List<Warp> warp)
        {
            for (int i = 0; i < warp.Count; i++)
            {
                if (moveData.tempLocationX == warp[i].taleX && moveData.tempLocationY == warp[i].taleY)
                {
                    moveData.ApplyLocation = false;
                }
            }
        }
        public void HandleItemContact(List<Item> item)
        {
            for(int i = 0; i< item.Count; i++)
            {
                if (moveData.tempLocationX == item[i].itemX && moveData.tempLocationY == item[i].itemY)
                {
                    ItemInfo gotchaResult = item[i].Gotcha();
                    ChangeSkin(item[i].GetPlayerSkin(gotchaResult));
                    item.Remove(item[i]);
                }
            }
        }
        public void ApplyPlayerMove()
        {
            if (moveData.ApplyLocation)
            {
                playerX = moveData.tempLocationX;
                playerY = moveData.tempLocationY;
            }
        }
        public void InitializeMoveData()
        {
            moveData = new MoveData(playerX, playerY);
        }
        public void ChangeSkin(string newSkin)
        {
            skin = newSkin;
        }
    }
}
