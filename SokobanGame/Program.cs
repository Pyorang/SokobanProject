using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace SokobanGame
{
    class Program
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