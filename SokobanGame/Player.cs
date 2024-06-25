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
        public void WarpPlayer(ConsoleKeyInfo currentKeyInfo, Warp[] warp)
        {
            for (int i = 0; i < warp.Length; i++)
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
        public void HandleBoxContact(Box[] box)
        {
            for (int i = 0; i < box.Length; i++)
            {
                if (moveData.tempLocationX == box[i].boxX && moveData.tempLocationY == box[i].boxY)
                {
                    if (!box[i].moveData.ApplyLocation)
                        moveData.ApplyLocation = false;
                }
            }
        }
        public void HandleWallContact(Wall[] wall)
        {
            for (int i = 0; i < wall.Length; i++)
            {
                if (moveData.tempLocationX == wall[i].wallX && moveData.tempLocationY == wall[i].wallY)
                {
                    moveData.ApplyLocation = false;
                }
            }
        }
        public void HandleGoalContact(Goal[] goal)
        {
            for (int i = 0; i < goal.Length; i++)
            {
                if (moveData.tempLocationX == goal[i].goalX && moveData.tempLocationY == goal[i].goalY)
                {
                    moveData.ApplyLocation = false;
                }
            }
        }
        public void HandleWarpTaleContact(Warp[] warp)
        {
            for (int i = 0; i < warp.Length; i++)
            {
                if (moveData.tempLocationX == warp[i].taleX && moveData.tempLocationY == warp[i].taleY)
                {
                    moveData.ApplyLocation = false;
                }
            }
        }
        public void HandleItemContact(Item item)
        {
            if (item != null)
            {
                if (moveData.tempLocationX == item.itemX && moveData.tempLocationY == item.itemY)
                {
                    ItemInfo gotchaResult = item.Gotcha();
                    ChangeSkin(item.GetPlayerSkin(gotchaResult));
                    item = null;
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
