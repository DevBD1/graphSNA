using graphSNA.Model.Foundation;
using System;
using System.Collections.Generic;

namespace graphSNA.Model.Algorithms
{
    /// <summary>
    /// Dijkstra's algorithm for finding shortest paths in weighted graphs.
    /// Guarantees optimal solution for non-negative edge weights.
    /// </summary>
    public class DijkstraAlgorithm : AlgorithmBase, IShortestPathAlgorithm
    {
        public override string Name => "Dijkstra";
        public override string TimeComplexity => "O((V + E) log V)";

        public (List<Node> path, double totalCost) FindPath(Graph graph, Node start, Node goal)
        {
            if (!ValidateInput(graph, start, goal))
                return (new List<Node>(), 0);

            var distances = new Dictionary<Node, double>();
            var previous = new Dictionary<Node, Node>();
            var priorityQueue = new PriorityQueue<Node, double>();

            foreach (var node in graph.Nodes)
            {
                distances[node] = double.PositiveInfinity;
            }

            distances[start] = 0;
            priorityQueue.Enqueue(start, 0);

            while (priorityQueue.Count > 0)
            {
                var current = priorityQueue.Dequeue();

                if (current == goal)
                    return (ReconstructPath(previous, goal), distances[goal]);

                foreach (var neighbor in GetNeighbors(graph, current))
                {
                    double weight = GetEdgeWeight(graph, current, neighbor);
                    if (double.IsPositiveInfinity(weight))
                        continue;

                    double newDist = distances[current] + weight;

                    if (newDist < distances[neighbor])
                    {
                        distances[neighbor] = newDist;
                        previous[neighbor] = current;
                        priorityQueue.Enqueue(neighbor, newDist);
                    }
                }
            }

            return (new List<Node>(), 0);
        }

        private List<Node> ReconstructPath(Dictionary<Node, Node> previous, Node current)
        {
            var path = new List<Node> { current };
            while (previous.ContainsKey(current))
            {
                current = previous[current];
                path.Add(current);
            }
            path.Reverse();
            return path;
        }
    }

    /// <summary>
    /// A* algorithm implementation using Euclidean distance as heuristic.
    /// More efficient than Dijkstra when spatial information is available.
    /// </summary>
    public class AStarAlgorithm : AlgorithmBase, IShortestPathAlgorithm
    {
        public override string Name => "A* (A-Star)";
        public override string TimeComplexity => "O((V + E) log V)";

        public (List<Node> path, double totalCost) FindPath(Graph graph, Node start, Node goal)
        {
            if (!ValidateInput(graph, start, goal))
                return (new List<Node>(), 0);

            var distances = new Dictionary<Node, double>();
            var previous = new Dictionary<Node, Node>();
            var priorityQueue = new PriorityQueue<Node, double>();

            foreach (var node in graph.Nodes)
            {
                distances[node] = double.PositiveInfinity;
            }

            distances[start] = 0;
            priorityQueue.Enqueue(start, Heuristic(start, goal));

            while (priorityQueue.Count > 0)
            {
                var current = priorityQueue.Dequeue();

                if (current == goal)
                    return (ReconstructPath(previous, goal), distances[goal]);

                foreach (var neighbor in GetNeighbors(graph, current))
                {
                    double weight = GetEdgeWeight(graph, current, neighbor);
                    if (double.IsPositiveInfinity(weight))
                        continue;

                    double newG = distances[current] + weight;

                    if (newG < distances[neighbor])
                    {
                        distances[neighbor] = newG;
                        previous[neighbor] = current;

                        double fScore = newG + Heuristic(neighbor, goal);
                        priorityQueue.Enqueue(neighbor, fScore);
                    }
                }
            }

            return (new List<Node>(), 0);
        }

        private double Heuristic(Node a, Node b)
        {
            var dx = a.Location.X - b.Location.X;
            var dy = a.Location.Y - b.Location.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        private List<Node> ReconstructPath(Dictionary<Node, Node> previous, Node current)
        {
            var path = new List<Node> { current };
            while (previous.ContainsKey(current))
            {
                current = previous[current];
                path.Add(current);
            }
            path.Reverse();
            return path;
        }
    }

    /// <summary>
    /// Strategy pattern manager for runtime algorithm switching.
    /// Allows dynamic selection between different shortest path algorithms.
    /// </summary>
    public class ShortestPathManager
    {
        private IShortestPathAlgorithm _algorithm;

        public ShortestPathManager(IShortestPathAlgorithm algorithm)
        {
            _algorithm = algorithm;
        }

        public void SetAlgorithm(IShortestPathAlgorithm algorithm) => _algorithm = algorithm;

        public (List<Node>, double) Calculate(Graph graph, Node start, Node target)
        {
            return _algorithm.FindPath(graph, start, target);
        }
    }
}