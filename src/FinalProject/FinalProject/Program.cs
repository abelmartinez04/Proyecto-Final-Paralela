using System;
using System.Diagnostics;
using System.IO;

class Program
{
    static void Main()
    {
        int[,] sudokuFacil = {
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

        RunTest(sudokuFacil, "Sudoku Fácil");
        RunTest(sudokuMedio, "Sudoku Medio");
        RunTest(sudokuDificil, "Sudoku Difícil");
    }

    static void RunTest(int[,] original, string nombreCaso)
    {
        int[,] board1 = (int[,])original.Clone();
        int[,] board2 = (int[,])original.Clone();

        Console.WriteLine($"\n===== {nombreCaso} =====");
        Sudoku.Print(original);

        Stopwatch sw = Stopwatch.StartNew();
        SolverSequential.Solve(board1);
        sw.Stop();

        Console.WriteLine("\nSecuencial:");
        Sudoku.Print(board1);

        Stopwatch sw2 = Stopwatch.StartNew();
        var solution = SolverParallel.Solve(board2);
        sw2.Stop();

        Console.WriteLine("\nParalelo:");
        Sudoku.Print(solution);

        Metrics.Save(nombreCaso, sw, sw2);

        // While para subir las metricas y resultados a /metrics
        string? current = Directory.GetCurrentDirectory();
        while (current != null && !Directory.Exists(Path.Combine(current, "src")))
        {
            current = Directory.GetParent(current!)?.FullName;
        }
        if (current == null)
        {
            throw new Exception("No se encontró la carpeta src.");
        }
        // Ahora ir a /metrics
        string metricsDir = Path.Combine(current, "metrics");


        string logPath = Path.Combine(metricsDir, $"{nombreCaso}_exploracion.txt");

        ExplorationLogger.Save(logPath);
        ExplorationLogger.Clear();
    }
}