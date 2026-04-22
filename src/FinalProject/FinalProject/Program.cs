using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static bool solutionFound = false;
    static int[,] solutionBoard = new int[9, 9];
    static object lockObj = new object();

    static void Main()
    {
        Directory.CreateDirectory("metrics");

        int[,] sudokuBasico = {
            {5,3,0,0,7,0,0,0,0},
            {6,0,0,1,9,5,0,0,0},
            {0,9,8,0,0,0,0,6,0},
            {8,0,0,0,6,0,0,0,3},
            {4,0,0,8,0,3,0,0,1},
            {7,0,0,0,2,0,0,0,6},
            {0,6,0,0,0,0,2,8,0},
            {0,0,0,4,1,9,0,0,5},
            {0,0,0,0,8,0,0,7,9}
        };

        int[,] sudokuMedio = {
            {0,0,0,6,0,0,4,0,0},
            {7,0,0,0,0,3,6,0,0},
            {0,0,0,0,9,1,0,8,0},
            {0,0,0,0,0,0,0,0,0},
            {0,5,0,1,8,0,0,0,3},
            {0,0,0,3,0,6,0,4,5},
            {0,4,0,2,0,0,0,6,0},
            {9,0,3,0,0,0,0,0,0},
            {0,2,0,0,0,0,1,0,0}
        };

        int[,] sudokuDificil = {
            {0,0,0,0,0,0,0,1,2},
            {0,0,0,0,0,0,0,3,0},
            {0,0,1,0,9,5,0,0,0},
            {0,0,0,0,0,0,0,0,0},
            {0,0,0,5,0,8,0,0,0},
            {0,0,0,0,0,0,0,0,0},
            {0,0,0,6,7,0,2,0,0},
            {0,3,0,0,0,0,0,0,0},
            {1,2,0,0,0,0,0,0,0}
        };

        RunTest(sudokuBasico, "Sudoku Básico");
        RunTest(sudokuMedio, "Sudoku Medio");
        RunTest(sudokuDificil, "Sudoku Difícil");
    }

    static void RunTest(int[,] original, string nombreCaso)
    {
        int[,] board1 = (int[,])original.Clone();
        int[,] board2 = (int[,])original.Clone();

        solutionFound = false;
        solutionBoard = new int[9, 9];

        Console.WriteLine($"\n===== {nombreCaso} =====");
        Console.WriteLine("Sudoku original:");
        PrintBoard(original);

        // Secuencial
        Stopwatch sw = Stopwatch.StartNew();
        Solve(board1);
        sw.Stop();

        Console.WriteLine("\nSolución secuencial:");
        PrintBoard(board1);
        Console.WriteLine($"Tiempo secuencial: {sw.ElapsedMilliseconds} ms");

        // Paralelo
        Stopwatch sw2 = Stopwatch.StartNew();
        SolveParallel(board2);
        sw2.Stop();

        Console.WriteLine("\nSolución paralela:");
        PrintBoard(solutionBoard);
        Console.WriteLine($"Tiempo paralelo: {sw2.ElapsedMilliseconds} ms");

        double speedup = sw2.ElapsedMilliseconds > 0
            ? (double)sw.ElapsedMilliseconds / sw2.ElapsedMilliseconds
            : 0;

        Console.WriteLine($"Speedup: {speedup:F2}x");

        // Guardar resultados
        string result = $@"
Caso: {nombreCaso}
Secuencial: {sw.ElapsedMilliseconds} ms
Paralelo: {sw2.ElapsedMilliseconds} ms
Speedup: {speedup:F2}x
-----------------------------------";

        string basePath = AppDomain.CurrentDomain.BaseDirectory;
        string metricsDir = Path.GetFullPath(
            Path.Combine(basePath, @"..\..\..\..\metrics")
        );

        Directory.CreateDirectory(metricsDir);
        string metricsPath = Path.Combine(metricsDir, "resultados.txt");
        File.AppendAllText(metricsPath, result);
    }

    // ---------------- SECUENCIAL ----------------
    static bool Solve(int[,] board)
    {
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (board[row, col] == 0)
                {
                    for (int num = 1; num <= 9; num++)
                    {
                        if (IsValid(board, row, col, num))
                        {
                            board[row, col] = num;

                            if (Solve(board))
                                return true;

                            board[row, col] = 0;
                        }
                    }
                    return false;
                }
            }
        }
        return true;
    }

    // ---------------- PARALELO ----------------
    static bool SolveParallel(int[,] board)
    {
        (int row, int col) = FindEmpty(board);

        if (row == -1)
            return true;

        var tasks = new List<Task>();
        SemaphoreSlim semaphore = new SemaphoreSlim(Environment.ProcessorCount);

        for (int num = 1; num <= 9; num++)
        {
            if (IsValid(board, row, col, num))
            {
                int[,] newBoard = (int[,])board.Clone();
                newBoard[row, col] = num;

                tasks.Add(Task.Run(async () =>
                {
                    await semaphore.WaitAsync();

                    try
                    {
                        if (!solutionFound && Solve(newBoard))
                        {
                            lock (lockObj)
                            {
                                if (!solutionFound)
                                {
                                    solutionBoard = newBoard;
                                    solutionFound = true;
                                }
                            }
                        }
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }));
            }
        }

        Task.WaitAll(tasks.ToArray());

        return solutionFound;
    }

    // ---------------- UTILIDADES ----------------
    static (int, int) FindEmpty(int[,] board)
    {
        for (int i = 0; i < 9; i++)
            for (int j = 0; j < 9; j++)
                if (board[i, j] == 0)
                    return (i, j);

        return (-1, -1);
    }

    static bool IsValid(int[,] board, int row, int col, int num)
    {
        for (int i = 0; i < 9; i++)
        {
            if (board[row, i] == num || board[i, col] == num)
                return false;
        }

        int startRow = row - row % 3;
        int startCol = col - col % 3;

        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                if (board[startRow + i, startCol + j] == num)
                    return false;

        return true;
    }

    static void PrintBoard(int[,] board)
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
                Console.Write(board[i, j] + " ");
            Console.WriteLine();
        }
    }
}