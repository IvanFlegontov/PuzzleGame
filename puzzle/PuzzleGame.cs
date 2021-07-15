using System;
using System.Collections.Generic;
using Priority_Queue;
using System.Text;
using System.Linq;

namespace puzzle
{
    class PuzzleGame
    {
        SimplePriorityQueue<SearchNode, double> _nodes;
        IDictionary<State, double> _previousStates;

        public Stack<State> Solution = null;

        public class SearchNode : IComparable
        {
            
            public State _currentState { get; }
            public SearchNode _previousNode { get; }
            public int NumberOfMoves { get; private set; }

            public SearchNode(State state, SearchNode previous)
            {
                _currentState = state;
                if (previous != null)
                {
                    NumberOfMoves = previous.NumberOfMoves + 1;
                    _previousNode = previous;
                }
                else
                {
                    _previousNode = null;
                    NumberOfMoves++;
                }
            }

            public int CompareTo(Object obj)
            {
                if (obj == null) 
                    return _currentState.Manhattan() + NumberOfMoves;

                SearchNode node = obj as SearchNode;
                return (_currentState.Manhattan() + NumberOfMoves) - (node._currentState.Manhattan() + node.NumberOfMoves);

            }

            public double Metric()
            {
                return _currentState.Manhattan() +  0.25 * NumberOfMoves;
            }
        }

        public PuzzleGame(State initial)
        {
            _nodes = new SimplePriorityQueue<SearchNode, double>();
            _previousStates = new Dictionary<State, double>();

            SearchNode x = new SearchNode(initial, null);
            _nodes.Enqueue(x, x.Metric());
        }

        private SearchNode Solver()
        {
            while (_nodes.Count > 0)
            {
                SearchNode x = _nodes.Dequeue();
                if (x._currentState.IsGoal())
                {
                    return x;
                }
                
                foreach (State state in x._currentState.Neighbours())
                {
                    if (x._previousNode == null)
                    {
                        SearchNode y = new SearchNode(state, x);
                        _nodes.Enqueue(y, y.Metric());
                        continue;
                    }

                    if (!state.Equals(x._previousNode._currentState))
                    {
                        if (_previousStates.ContainsKey(state) && state.Metric(x.NumberOfMoves + 1) >= _previousStates[state])
                            continue;
                        //if (!_previousStates.ContainsKey(state) || state.Metric(x.NumberOfMoves + 1) < _previousStates[state])
                        else
                        {
                            SearchNode y = new SearchNode(state, x);
                            _nodes.Enqueue(y, y.Metric());
                        }
                    }
                }

                _previousStates.Add(x._currentState, x.Metric());
            }
            return null;
        }

        public Stack<State> Solve()
        {
            if ( Solution == null)
            {
                Solution = new Stack<State>();
                SearchNode node = Solver();
                while (node != null)
                {
                    Solution.Push(node._currentState);
                    node = node._previousNode;
                }
            }

            return Solution;
        }
    }
}
