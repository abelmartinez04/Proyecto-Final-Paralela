using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Sudoku
{
    public int[,] Board { get; set; }

    public Sudoku(int[,] board)
    {
        Board = (int[,])board.Clone();
    }

    public Sudoku Clone()
    {
        return new Sudoku((int[,])Board.Clone());
    }

    public static (int, int) FindEmpty(int[,] board)
    {
        for (int i = 0; i < 9; i++)
            for (int j = 0; j < 9; j++)
                if (board[i, j] == 0)
                    return (i, j);

        return (-1, -1);
    }

    public static bool IsValid(int[,] board, int row, int col, int num)
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

    public static void Print(int[,] board)
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
                Console.Write(board[i, j] + " ");
            Console.WriteLine();
        }
    }
}