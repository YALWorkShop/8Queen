using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Queen
{
    class Program
    {
        static void Main(string[] args)
        {
            var queenNumber = 8;
            var successCount = 0;
            var outputArray = new string[queenNumber];
            var queenPositions = new List<QueenPosition>();
            var canNotSetQueenPositions = new List<QueenPosition>();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int y = 0; y > -queenNumber; y--)
            {
                // While到找到皇后的位置為止
                var hasQueen = false;
                while (!hasQueen)
                {
                    // 判斷是否已經全部找完 (第一列皆進入禁放清單)
                    if (canNotSetQueenPositions.Count(p => p.Y == 0) == queenNumber)
                    {
                        // 離開For迴圈與 While迴圈
                        y = -(queenNumber+1);
                        break;
                    }

                    // 顯示行數
                    var outputString = $"{-y}:";

                    for (int x = 0; x < queenNumber; x++)
                    {
                        // 判斷是否可以放皇后
                        if (CheckCanSetQueen(queenPositions, x, y, canNotSetQueenPositions))
                        {
                            queenPositions.Add(new QueenPosition(x, y));
                            outputString += "Q";
                            hasQueen = true;
                        }
                        else
                        {
                            outputString += ".";
                        }
                    }

                    // 當這行找不到皇后的時候...
                    if (!hasQueen)
                    {
                        // 找到上一個皇后的位置 
                        var lastPosition = queenPositions.FirstOrDefault(p => p.Y == (y != 0 ? y + 1 : y));
                        if (lastPosition != null)
                        {
                            // 因為上一行一個點移除,所以比她小的禁放點都清除掉,重新計算
                            canNotSetQueenPositions.RemoveAll(p => p.Y <= y);
                            // 回朔行動
                            y = GoBackAndMemory(canNotSetQueenPositions, lastPosition, queenPositions, y);
                        }
                    }
                    else
                    {
                        // 找到黃就將字串放入陣列
                        outputArray[-y] = outputString;

                        // 找到最後一個皇后就完成了一盤
                        if (y == -(queenNumber-1))
                        {
                            // 顯示結果
                            successCount++;
                            Console.WriteLine("===========================");
                            Console.WriteLine($"{successCount}");
                            foreach (var output in outputArray)
                            {
                                Console.WriteLine(output);
                            }
                            Console.WriteLine("===========================");

                            // 移除最後一個完成點 並且加入禁放清單,讓他繼續找下去
                            var lastPosition = queenPositions.FirstOrDefault(p => p.Y == y);
                            if (lastPosition != null)
                            {
                                // 回朔行動
                                y = GoBackAndMemory(canNotSetQueenPositions, lastPosition, queenPositions, y);
                            }
                        }
                    }
                }
            }
            // End
            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.Elapsed.TotalMilliseconds}");
        }

        private static int GoBackAndMemory(List<QueenPosition> canNotSetQueenPositions, QueenPosition lastPosition, List<QueenPosition> queenPositions, int y)
        {
            // 將上一個皇后的位置加入禁放清單
            canNotSetQueenPositions.Add(lastPosition);
            // 移除放置的皇后
            queenPositions.Remove(lastPosition);
            // 回到上一行
            y++;
            return y;
        }

        private static bool CheckCanSetQueen(List<QueenPosition> queenPositions, int x, int y, List<QueenPosition> canNotSetQueenPositions)
        {
            return !queenPositions.Any(p => p.IsLimitField(x, y))
                   && !canNotSetQueenPositions.Any(p => p.X == x && p.Y == y);
        }
    }

    internal class QueenPosition
    {
        public QueenPosition(in int x, in int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }

        public int Y { get; set; }


        public bool IsLimitField(int x, int y)
        {
            // x-y+c = 0 是 右上到左下的斜線方程式 (Y-X) 為判斷此為原點的方程式位移
            // x+y+c = 0 是 左上到右下的斜線方程式 (-X-Y) 為判斷此點為原點的方程式位移
            // x=c 是直線方程式
            // y=c 是橫線方程式
            return x - y + (Y - X) == 0 || x + y - (X + Y) == 0 || X == x || Y == y;
        }

    }
}
