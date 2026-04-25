using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class SolverParallel
{
    private static bool solutionFound;
    private static int[,]? solutionBoard;
    private static object lockObj = new object();

    public static int[,] Solve(int[,] board)
    {
        solutionFound = false;
        solutionBoard = new int[9, 9];

        SolveInternal(board);

        return solutionBoard;
    }

    private static bool SolveInternal(int[,] board)
    {
        (int row, int col) = Sudoku.FindEmpty(board);

        if (row == -1)
            return true;

        var tasks = new List<Task>();
        SemaphoreSlim semaphore = new SemaphoreSlim(Environment.ProcessorCount);

        for (int num = 1; num <= 9; num++)
        {
            int localNum = num;
            if (Sudoku.IsValid(board, row, col, localNum))
            {
                int[,] newBoard = (int[,])board.Clone();
                newBoard[row, col] = localNum;

                tasks.Add(Task.Run(async () =>
                {
                    int taskId = Task.CurrentId ?? -1;

                    ExplorationLogger.Log($"Task {taskId} probando {localNum} en ({row},{col})");

                    await semaphore.WaitAsync();

                    try
                    {
                        if (!solutionFound && SolverSequential.Solve(newBoard))
                        {
                            lock (lockObj)
                            {
                                if (!solutionFound)
                                {
                                    solutionBoard = newBoard;
                                    solutionFound = true;
                                    ExplorationLogger.Log($"Task {taskId} encontró solución");
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
}