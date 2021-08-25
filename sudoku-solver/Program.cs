using System;
using System.Collections.Generic;
using System.Threading;
using CsvHelper;
using System.IO;

namespace sudoku_solver
{
    class Program
    {
        static int[,] board1 = {
        {0,3,0,0,1,0,0,6,0},
        {7,5,0,0,3,0,0,4,8},
        {0,0,6,9,8,4,3,0,0},
        {0,0,3,0,0,0,8,0,0},
        {9,1,2,0,0,0,6,7,4},
        {0,0,4,0,0,0,5,0,0},
        {0,0,1,6,7,5,2,0,0},
        {6,8,0,0,9,0,0,1,5},
        {0,9,0,0,4,0,0,3,0},
        };
        static int[,] board2 = {
        {0,0,0,7,9,0,0,5,0},
        {3,5,2,0,0,8,0,4,0},
        {0,0,0,0,0,0,0,8,0},
        {0,1,0,0,7,0,0,0,4},
        {6,0,0,3,0,1,0,0,8},
        {9,0,0,0,8,0,0,1,0},
        {0,2,0,0,0,0,0,0,0},
        {0,4,0,5,0,0,8,9,1},
        {0,8,0,0,3,7,0,0,0}
        }; //harder difficulty, not able to solve yet
        public static Int32[] val = {
            0b_0000_0000_0000, //obligatory 0
            0b_0000_0000_0001,
            0b_0000_0000_0010,
            0b_0000_0000_0100,
            0b_0000_0000_1000,
            0b_0000_0001_0000,
            0b_0000_0010_0000,
            0b_0000_0100_0000,
            0b_0000_1000_0000,
            0b_0001_0000_0000
        };
        static void Main(string[] args)
        {
            var board = board2;
            //Console.WindowWidth = 20;
            //Console.WindowHeight = 14;

            print(board);
            Console.ReadKey();
            ///Thread.Sleep(3000);

            //1. initiate truth tables
            var checks = new Int32[9, 9];
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    checks[j, i] = 0b_0001_1111_1111;
                }
            }


            int ab = 0;
            while (ab < 500)
            {
                //begin routine for checking numbers
                //2. check rows
                for (int row = 0; row < 9; row++)
                {
                    var flags = 0; //int32
                    for (int col = 0; col < 9; col++)
                    {
                        flags |= val[board[row, col]];
                    }
                    for (int col = 0; col < 9; col++)
                    {
                        if (board[row, col] == 0)
                        {
                            checks[row, col] &= ~flags; //fix this?
                        }
                    }
                }
                //3. check columns
                for (int col = 0; col < 9; col++)
                {
                    var flags = 0; //int32
                    for (int row = 0; row < 9; row++)
                    {
                        flags |= val[board[row, col]];
                    }
                    for (int row = 0; row < 9; row++)
                    {
                        if (board[row, col] == 0)
                        {
                            checks[row, col] &= ~flags; //fix this?
                        }
                    }
                }
                //4. check boxes
                for (int boxrow = 0; boxrow < 3; boxrow++)
                {
                    for (int boxcol = 0; boxcol < 3; boxcol++)
                    {
                        var flags = 0; //int32
                        for (int row = 0; row < 3; row++)
                        {
                            for (int col = 0; col < 3; col++)
                            {
                                flags |= val[board[boxrow * 3 + row, boxcol * 3 + col]];
                            }
                        }
                        //Console.WriteLine(Convert.ToString(flags, 2));
                        //Console.ReadLine();

                        for (int row = 0; row < 3; row++)
                        {
                            for (int col = 0; col < 3; col++)
                            {
                                checks[boxrow * 3 + row, boxcol * 3 + col] &= ~flags;
                            }
                        }
                    }
                }

                //5. attempt solve on each item, row by row, col by col
                for (int row = 0; row < 9; row++)
                {
                    for (int col = 0; col < 9; col++)
                    {
                        if (board[row, col] == 0)
                        {
                            var final = Array.IndexOf(val, checks[row, col]); //these ifs can be combined?
                            if (final != -1)
                            {
                                board[row, col] = final;
                                printxy(ref board, col, row, final);
                                //print();
                            }
                        }
                    }
                }
                ab++;
            }
            //Console.ReadKey();
            Console.SetCursorPosition(0, 11);


            //write into file for debugging, manual solving:
            var filestream = File.Create("puzzle.csv");
            var sw = new StreamWriter(filestream);
            var csvwriter = new CsvWriter(sw, System.Globalization.CultureInfo.CurrentCulture);
            for(int i = 1;i<=9;i++)
            {
            csvwriter.WriteField("col"+i);
            }
            csvwriter.NextRecord();
            for(int row = 1;row<=9;row++)
            {
                for(int col=1;col<=9;col++)
                {
                    if(board[row-1,col-1]!=0)
                    {
                        csvwriter.WriteField(board[row-1,col-1]);
                    }else
                    {
                        csvwriter.WriteField(flagtostring(checks[row-1,col-1]));
                    }
                }
                csvwriter.NextRecord();
            }
            sw.Close();
            //sw.WriteLine("cool!");

        }
        public static void print(int[,] board)
        {
            Console.Clear();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (board[i, j] != 0)
                    {
                        Console.Write(" " + board[i, j]);
                        
                    }
                    else
                    {
                        Console.Write("  ");
                    }
                }
                Console.Write("\n");
            }
            Console.WriteLine("-------------------");
            Thread.Sleep(200);
        }
        public static void printxy(ref int[,] board, int x, int y, int val)
        {
            Console.SetCursorPosition(x * 2 + 1, y);
            if (board[y, x] != 0) //row, col?
            {
                Console.Write(val);
            }
            else
            {
                Console.Write(" ");
            }
            Thread.Sleep(200);
        }
        public static string flagtostring(Int32 input)
        {
            var buffer = "";
            for(int i = 1;i<=11;i++) //for some reason it wasn't going to 9
            {
                if((input&(0x1<<(i-1)))!=0)
                {
                    buffer+=i.ToString()+" ";
                }
            }
            return buffer;
        }
    }
}
//TO-DO: implement strategies from here: https://www.learn-sudoku.com/advanced-techniques.html
//TO-DO: allow import/export
//TO-DO: open new console window, update sudoku board at the given positions

//in a given row, if there are three boxes with the same three options, 
//those three can be elimated from the rest (same with any number)

