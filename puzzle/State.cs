using System;

using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace puzzle
{
    class State
    {
        // lenght of game table
        public int N { get;  } 
        public int CurEmpty { get; private set; }
        public int[,] Array { get; }

        public State(int n, int curEmpty, int[,] array)
        {
            N = n;
            CurEmpty = curEmpty;
            Array = array;
        }

        public State(int n, int curEmpty, int[] array)
        {
            if (array.Length != n * n)
                throw new Exception("Wrong array");

            N = n;
            CurEmpty = curEmpty;
            Array = new int[N, N];

            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                {
                    Array[i, j] = array[i * N + j];
                }
            
        }

        public static State RandomState(int n) 
        {
            IEnumerable<int> numbers = Enumerable.Range(0, n*n);
            Random rnd = new Random();
            int[] permutation = numbers.OrderBy(x => rnd.Next()).ToArray();

            int curEmpty =  System.Array.FindIndex(permutation, EqualsZero);

            return new State(n, curEmpty, permutation);
        }

        //returns new State with two swapped elements in array
        // [i, j] <—> [m, n]
        public State Swap(int i, int j, int m,  int n)
        {

            if (i < 0 || j < 0 || m < 0 || n < 0 || i >= N || j >= N || m>=N || n >= N)
                throw new Exception("Wrong Indices");

            int[,] array = Array.Clone() as int[,];

            int curEmpty = CurEmpty;

            if (array[i, j] == 0)
                curEmpty = m * N + n;
            else if (array[m, n] == 0)
                curEmpty = i * N + j;

            int tmp = array[i, j];
            array[i, j] = array[m, n];
            array[m, n] = tmp;

            return new State(N, curEmpty, array);

        }
     
        public IEnumerable<State> Neighbours()
        {
            List<State> neighbours = new List<State>();
            int j = CurEmpty % N;
            int i = (CurEmpty - j) / N;
            if (i > 0)
                neighbours.Add(this.Swap(i, j, i - 1, j));
            if (i < N - 1)
                neighbours.Add(this.Swap(i, j, i + 1, j));
            if (j > 0)
                neighbours.Add(this.Swap(i, j, i, j - 1));
            if (j < N - 1)
                neighbours.Add(this.Swap(i, j, i, j + 1));
            return neighbours;
        }

        public int Manhattan()
        {
            int dist = 0;
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                {
                    if( Array[i, j] == 0)
                        continue;
                    int new_j = (Array[i, j] - 1) % N;
                    int new_i = (Array[i, j] - 1 - new_j ) / N;
                    dist += Math.Abs(new_i - i) + Math.Abs(new_j - j);
                }
            return dist;
        }



        /*
         * State has solution if and only if one of the cases holdes true
         * 
         *  1) if N = 2k + 1:
         *      sgn(per) = 1;
         *      
         *  2.1) if N = 2k and whitespace (0) is at even row ( row = 0, 1, ..., 2k - 1):
         *      sgn(per) = -1;
         *      
         *  2.2) if N = 2k and whitespace (0) is at odd row:
         *      sgn(per) = 1.
         *      
         *   Where per is a permutation without whitespace represented by 0. Thus per \in S_{N*N - 1}
         */
        public bool IsSolvable()
        {
            if (N % 2 == 1)
            {
                if (IsEven())
                    return true;
                else
                    return false;
            }
            else
            {
                int j = CurEmpty % N;
                int i = (CurEmpty - j) / N;
                if (i % N == 0 && !IsEven())
                    return true;
                else if (i % N == 1 && IsEven())
                    return true;
                else 
                    return false;
            }
        }

        public bool IsEven()
        {

            int[] permutation = Array.Cast<int>().Where(val => val != 0).ToArray().Clone() as int[];

            int swap_counter = 0;

            for (int i = 0; i < N * N - 2;)
            {
                if (i == permutation[i] - 1)
                {
                    i++;
                    continue;
                }
                else
                {
                    int k = permutation[i];
                    permutation[i] = permutation[k - 1];
                    permutation[k - 1] = k;
                    swap_counter++;
                }
            }
            return (swap_counter % 2) == 0;
        }

        public override string ToString()
        {

            StringBuilder str = new StringBuilder();

            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                    str.Append($" {Array[i, j]} |");

                str.Append("\n");
            }
            str.Append("\n");
            return str.ToString();
        }

        public bool IsGoal()
        {
            return Manhattan() == 0;
        }

        public bool Equals(State y)
        {
            if (y == null)
                return false;
            if (CurEmpty != y.CurEmpty || N != y.N)
                return false;
            return Array.Cast<int>().SequenceEqual(y.Array.Cast<int>());
        }

        public int Metric(int moves)
        {
            return Manhattan() + moves;
        }

        private static bool EqualsZero(int x)
        {
            if (x == 0)
                return true;
            else
                return false;
        }

    }
}
