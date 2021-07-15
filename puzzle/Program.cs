using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace puzzle
{
    class Program
    {
        static void Main(string[] args)
        {

            State rndState;

            while (true)
            {
                rndState = State.RandomState(3);
                if (rndState.IsSolvable())
                    break;
            }

            int i = 0;
            PuzzleGame game = new PuzzleGame(rndState);
            Stack<State> solution = game.Solve();
            while (solution.Count > 0)
            {
                State x = solution.Pop();
                Console.WriteLine("move = {0}", i++);
                Console.WriteLine(x.ToString());
            }
            
        }
    }
}
