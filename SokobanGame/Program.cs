using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace Sokoban
{
    internal struct MoveData
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

    internal class Player
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
            if(moveData.ApplyLocation)
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

    internal class Box
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
        public void CheckBoxNextBox(Box[] box)
        {
            for (int i = 0; i < box.Length; i++)
            {
                if (moveData.tempLocationX == box[i].boxX && moveData.tempLocationY == box[i].boxY)
                    moveData.ApplyLocation = false;
            }
        }
        public void CheckBoxNextWall(Wall[] wall)
        {
            for (int i = 0; i < wall.Length; i++)
            {
                if (moveData.tempLocationX == wall[i].wallX && moveData.tempLocationY == wall[i].wallY)
                    moveData.ApplyLocation = false;
            }
        }
        public void CheckBoxNextWarpTale(Warp[] warp)
        {
            for (int i = 0; i < warp.Length; i++)
            {
                if (moveData.tempLocationX == warp[i].taleX && moveData.tempLocationY == warp[i].taleY)
                    moveData.ApplyLocation = false;
            }
        }
        public void CheckBoxRouteBlocked(Box[] box, Wall[] wall, Warp[] warp)
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

    internal class Wall
    {
        public int wallX;
        public int wallY;

        public Wall(int wallX, int wallY)
        {
            this.wallX = wallX;
            this.wallY = wallY;
        }
    }

    internal class Goal
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

    internal class Warp
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

    internal struct ItemInfo
    {
        public string id;
        public int weight;

        public ItemInfo(string id, int weight)
        {
            this.id = id;
            this.weight = weight;
        }
    }

    internal class Item
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
            for(int i = 0; i < infoList.Length; i++)
            {
                weight += infoList[i].weight;
                if(selectedNumber <= weight)
                    return infoList[i];
            }

            return new ItemInfo("None", 0);
        }
        public int GetTotalWeight()
        {
            int totalWeight = 0;

            for(int i = 0; i< infoList.Length; i++)
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
            switch(itemInfo.id)
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

    internal class Program
    {
        static bool gameOver;

        static Player player;
        static Box[] box;
        static Wall[] wall;
        static Goal[] goal;
        static Warp[] warp;
        static ItemInfo[] itemInfo;
        static Item item;

        static void Main()
        {
            InitializeGame();
            RenderGameScreen();
            StartGameLoop();
        }

        static void  InitializeGame()
        {
            //창 초기화
            Console.ResetColor();
            Console.CursorVisible = false;
            Console.Title = "Yongkoban";
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Clear();

            gameOver = false;

            player = new Player(5, 5);

            box = new Box[5] { new Box(4, 8), new Box(8, 8), new Box(12, 8), new Box(16, 8), new Box(20, 8) };

            wall = new Wall[5] { new Wall(4, 11), new Wall(8, 11), new Wall(12, 11), new Wall(16, 11), new Wall(20, 11) };

            goal = new Goal[5] { new Goal(4, 14), new Goal(8, 14), new Goal(12, 14), new Goal(16, 14), new Goal(20, 14) };

            warp = new Warp[2] { new Warp(3, 3, 3, 5), new Warp(7, 6, 8, 10) };

            itemInfo = new ItemInfo[3] { new ItemInfo("Rare", 70), new ItemInfo("UltraRare", 20), new ItemInfo("Legend", 10)};

            item = new Item(5, 7, itemInfo);
        }

        static void RenderGameScreen()
        {
            Console.Clear();

            PrintPlayer();
            PrintBox();
            PrintWall();
            PrintGoal();
            PrintWarp();
            PrintItem();
        }

        static void PrintPlayer()
        {
            Console.SetCursorPosition(player.playerX, player.playerY);
            Console.Write(player.skin);
        }

        static void PrintBox()
        {
            for(int i = 0; i <  box.Length; i++)
            {
                Console.SetCursorPosition(box[i].boxX, box[i].boxY);
                Console.Write("B");
            }
        }

        static void PrintWall()
        {
            for (int i = 0; i < wall.Length; i++)
            {
                Console.SetCursorPosition(wall[i].wallX, wall[i].wallY);
                Console.Write("W");
            }
        }

        static void PrintGoal()
        {
            for (int i = 0; i < goal.Length; i++)
            {
                if (goal[i].inputBox)
                {
                    Console.SetCursorPosition(goal[i].goalX, goal[i].goalY);
                    Console.Write("G");
                }
                else
                {
                    Console.SetCursorPosition(goal[i].goalX, goal[i].goalY);
                    Console.Write("g");
                }
            }
        }

        static void PrintWarp()
        {
            for (int i = 0; i < warp.Length; i++)
            {
                Console.SetCursorPosition(warp[i].headX, warp[i].headY);
                Console.Write("H");
                Console.SetCursorPosition(warp[i].taleX, warp[i].taleY);
                Console.Write("T");
            }
        }

        static void PrintItem()
        {
            if(item != null)
            {
                Console.SetCursorPosition(item.itemX, item.itemY);
                Console.Write("I");
            }
        }

        static void StartGameLoop()
        {
            while (!gameOver)
            {
                ProcessGameLogic();
                CheckGameClear();
                InitializeMoveData();
                RenderGameScreen();
            }

            RenderingClearScreen();
        }

        static void ProcessGameLogic()
        {
            ConsoleKeyInfo currentKeyInfo = Console.ReadKey();

            ProcessPlayerMove(currentKeyInfo);
            ProcessBoxMove(currentKeyInfo);
        }

        static void ProcessPlayerMove(ConsoleKeyInfo currentKeyInfo)
        {
            player.ChangePlayerTempLocation(currentKeyInfo);

            ChangeBoxState(currentKeyInfo);

            player.WarpPlayer(currentKeyInfo, warp);
            player.HandleBoundaryException();
            player.HandleBoxContact(box);
            player.HandleWallContact(wall);
            player.HandleGoalContact(goal);
            player.HandleWarpTaleContact(warp);
            player.HandleItemContact(item);

            player.ApplyPlayerMove();
        }

        static void ProcessBoxMove(ConsoleKeyInfo currentkeyInfo)
        {
            for (int i = 0; i < box.Length; i++)
            {
                if(player.playerX == box[i].boxX && player.playerY == box[i].boxY)
                {
                    box[i].ApplyBoxMove();
                }
            }
        }

        static void ChangeBoxState(ConsoleKeyInfo currentKeyInfo)
        {
            for (int i = 0; i < box.Length; i++)
            {
                ChangeBoxTempLocation(i, currentKeyInfo);
                WarpBox(i, currentKeyInfo);
                box[i].CheckBoxRouteBlocked(box, wall, warp);
            }
        }

        static void WarpBox(int boxIndex, ConsoleKeyInfo currentKeyInfo)
        {
            for (int i = 0; i < warp.Length; i++)
            {
                if (box[boxIndex].moveData.tempLocationX == warp[i].headX && box[boxIndex].moveData.tempLocationY == warp[i].headY)
                {
                    box[boxIndex].moveData.tempLocationX = warp[i].taleX;
                    box[boxIndex].moveData.tempLocationY = warp[i].taleY;
                    ChangeBoxTempLocation(boxIndex, currentKeyInfo);
                }
            }
        }

        static void ChangeBoxTempLocation(int boxIndex, ConsoleKeyInfo currentKeyInfo)
        {
            switch (currentKeyInfo.Key)
            {
                case ConsoleKey.LeftArrow:
                    box[boxIndex].PrepareGoLeft();
                    break;

                case ConsoleKey.RightArrow:
                    box[boxIndex].PrepareGoRight();
                    break;

                case ConsoleKey.UpArrow:
                    box[boxIndex].PrepareGoUp();
                    break;

                case ConsoleKey.DownArrow:
                    box[boxIndex].PrepareGoDown();
                    break;
            }
        }

        static void CheckGameClear()
        {
            int boxInGoalCount = 0;

            //골 갯수와 박스 위치 비교하여 지역변수 증가
            for(int i = 0; i < goal.Length; i++)
            {
                for(int j = 0; j< box.Length; j++)
                {
                    if (goal[i].goalX == box[j].boxX && goal[i].goalY == box[j].boxY)
                    {
                        boxInGoalCount++;
                        goal[i].FillGoal();
                    }
                }
            }

            if (boxInGoalCount == goal.Length)
                gameOver = true;
        }

        static void InitializeMoveData()
        {
            InitializePlayerMoveData();
            InitializeBoxMoveData();
        }

        static void InitializePlayerMoveData()
        {
            player.InitializeMoveData();
        }

        static void InitializeBoxMoveData()
        {
            for(int i = 0; i < box.Length; i++)
                box[i].InitializeMoveData();
        }

        static void RenderingClearScreen()
        {
            Console.Clear();
            Console.Write("GameClear!");
        }
    }
}