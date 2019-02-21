using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csharp_game_of_life
{
    /// <summary>
    /// Class which represents game matrix, including game logic
    /// </summary>
    sealed class GameMatrix
    {
        private byte[,] matrix; // 1 - cell is alive, 0 - cell is dead
        private Random rngGenerator;
        readonly int size;

        public GameMatrix(int size, int probability)
        {
            matrix = new byte[size, size];
            this.size = size;
            rngGenerator = new Random();

            GenerateRandomCells(probability);
        }

        /// <summary>
        /// Adds up all alive cells around cell at [x,y]
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private int AddUpNeighbours(int x, int y)
        {
            int sum = 0;
            if ((x - 1 >= 0) && (y - 1 >= 0)) sum += matrix[x - 1, y - 1];
            if (x - 1 >= 0) sum += matrix[x - 1, y];
            if ((x - 1 >= 0) && (y + 1 < size)) sum += matrix[x - 1, y + 1];
            if (y - 1 >= 0) sum += matrix[x, y - 1];
            if (y + 1 < size) sum += matrix[x, y + 1];
            if ((x + 1 < size) && (y - 1 >= 0)) sum += matrix[x + 1, y - 1];
            if (x + 1 < size) sum += matrix[x + 1, y];
            if ((x + 1 < size) && (y + 1 < size)) sum += matrix[x + 1, y + 1];

            return sum;
        }

        /// <summary>
        /// Plays one generation of the game of life.
        /// </summary>
        public void PlayOneGeneration()
        {
            byte[,] newMatrix = new byte[size, size];
            int numberOfNeighbours = 0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    numberOfNeighbours = AddUpNeighbours(i, j);
                    if (matrix[i, j] == 1) // Cell is alive
                    {
                        if (!((numberOfNeighbours == 2) || (numberOfNeighbours == 3))) // Cell dies
                        {
                            newMatrix[i, j] = 0;
                        }
                        else
                        {
                            newMatrix[i, j] = 1;
                        }
                    }
                    else // Cell is dead
                    {
                        if (numberOfNeighbours == 3) // Cell revives
                        {
                            newMatrix[i, j] = 1;
                        }
                        else
                        {
                            newMatrix[i, j] = 0;
                        }
                    }
                }
            }
            matrix = newMatrix;
        }

        /// <summary>
        /// Generates random alive cells in matrix.
        /// </summary>
        /// <param name="probability">Probability that live cell will arise </param>
        private void GenerateRandomCells(int probability)
        {
            int randomNumber = 0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    randomNumber = rngGenerator.Next(1, 100);

                    if (randomNumber <= probability)
                    {
                        matrix[i, j] = 1;
                    }
                }
            }
        }

        public void WriteMatrixToOutput(TextWriter output)
        {
            Console.Clear();
            for (int i = 0; i < size; i++) output.Write('-');
            output.WriteLine();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (matrix[i, j] == 0) output.Write(' ');
                    else output.Write('X');
                }
                output.WriteLine();
            }
            for (int i = 0; i < size; i++) output.Write('-');
            output.WriteLine();
        }

    }

    class Program
    {
        static bool IsPositiveNumber(string input, out int result)
        {
            result = 0;
            bool isNumber = Int32.TryParse(input, out int number);
            if (number <= 0) return false;
            if (!isNumber) return false;
            result = number;
            return true;
        }

        static void Main(string[] args)
        {
            bool isAPositiveNumber = false;
            int size = 0;
            int probability = 0;
            Start:
            Console.Clear();
            Console.WriteLine("Game of Life - Pavel Hrdý\n--------------------------");
            Console.WriteLine("Ovladani behem hry:\nr - reset\nq - konec\ncokoliv jineho - dalsi generace\n");

            while (true)
            {
                Console.WriteLine("Zadejte rozmer ctvercove hraci plochy (kladne cislo): ");
                isAPositiveNumber = IsPositiveNumber(Console.ReadLine(), out size);
                if (isAPositiveNumber) break; else continue; 
            }

            while (true)
            {
                Console.WriteLine("Zadejte pravdepodobnost, ze na policku bude ziva bunka (1-100): ");
                isAPositiveNumber = IsPositiveNumber(Console.ReadLine(), out probability);
                if (probability >= 0 && probability <= 100 && isAPositiveNumber) break; else continue;
            }

            Console.WriteLine();
            GameMatrix matrix = new GameMatrix(size, probability);
            matrix.WriteMatrixToOutput(Console.Out);
            Console.ReadKey();

            while (true)
            {
                matrix.PlayOneGeneration();
                matrix.WriteMatrixToOutput(Console.Out);
                ConsoleKeyInfo key = Console.ReadKey();

                if (Char.ToLower(key.KeyChar) == 'r') goto Start;
                else if (Char.ToLower(key.KeyChar) == 'q') break;
                else continue;
            }
        }
    }
}
