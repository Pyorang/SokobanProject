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
        static bool finalStage;
        static int currentStageNum;
        static Player? player;
        static List<Box>? box;
        static List<Wall>? wall;
        static List<Goal>? goal;
        static List<Warp>? warp;
        static List<ItemInfo>? itemInfo;
        static List<Item>? item;

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
            currentStageNum = 1;

            itemInfo = GetItemInfo();
            SetCurrentStage(itemInfo, currentStageNum);
        }

        static void SetCurrentStage(List<ItemInfo> itemInfo, int currentStageNum)
        {
            Stage currentStage = new Stage(currentStageNum, itemInfo);

            finalStage = currentStage.finalStage;
            player = currentStage.player;
            box = currentStage.box;
            wall = currentStage.wall;
            goal = currentStage.goal;
            warp = currentStage.warp;
            item = currentStage.item;
        }

        static List<ItemInfo> GetItemInfo()
        {
            List<ItemInfo> itemInfo = new List<ItemInfo>();

            string fileName = "ItemInfo.txt";
            string[] contents = File.ReadAllLines(fileName);

            foreach (string line in contents)
            {
                string[] parts = line.Split(',');

                itemInfo.Add(new ItemInfo(parts[1], int.Parse(parts[2])));
            }

            return itemInfo;
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
            for(int i = 0; i <  box.Count; i++)
            {
                Console.SetCursorPosition(box[i].boxX, box[i].boxY);
                Console.Write("B");
            }
        }

        static void PrintWall()
        {
            for (int i = 0; i < wall.Count; i++)
            {
                Console.SetCursorPosition(wall[i].wallX, wall[i].wallY);
                Console.Write("W");
            }
        }

        static void PrintGoal()
        {
            for (int i = 0; i < goal.Count; i++)
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
            for (int i = 0; i < warp.Count; i++)
            {
                Console.SetCursorPosition(warp[i].headX, warp[i].headY);
                Console.Write("H");
                Console.SetCursorPosition(warp[i].taleX, warp[i].taleY);
                Console.Write("T");
            }
        }

        static void PrintItem()
        {
            for(int i = 0; i < item.Count; i++)
            {
                Console.SetCursorPosition(item[i].itemX, item[i].itemY);
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
            for (int i = 0; i < box.Count; i++)
            {
                if(player.playerX == box[i].boxX && player.playerY == box[i].boxY)
                {
                    box[i].ApplyBoxMove();
                }
            }
        }

        static void ChangeBoxState(ConsoleKeyInfo currentKeyInfo)
        {
            for (int i = 0; i < box.Count; i++)
            {
                ChangeBoxTempLocation(i, currentKeyInfo);
                WarpBox(i, currentKeyInfo);
                box[i].CheckBoxRouteBlocked(box, wall, warp);
            }
        }

        static void WarpBox(int boxIndex, ConsoleKeyInfo currentKeyInfo)
        {
            for (int i = 0; i < warp.Count; i++)
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
            for(int i = 0; i < goal.Count; i++)
            {
                for(int j = 0; j< box.Count; j++)
                {
                    if (goal[i].goalX == box[j].boxX && goal[i].goalY == box[j].boxY)
                    {
                        boxInGoalCount++;
                        goal[i].FillGoal();
                    }
                }
            }

            if (boxInGoalCount == goal.Count)
            {
                if(finalStage)
                    gameOver = true;
                else
                {
                    currentStageNum++;
                    SetCurrentStage(itemInfo, currentStageNum);
                }
            }
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
            for(int i = 0; i < box.Count; i++)
                box[i].InitializeMoveData();
        }

        static void RenderingClearScreen()
        {
            Console.Clear();
            Console.Write("GameClear!");
        }
    }
}