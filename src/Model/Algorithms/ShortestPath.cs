using graphSNA.Model.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace graphSNA.Model.Algorithms
{
    /// <summary>
    /// Contains shortest path algorithms (Dijkstra and A*) for finding 
    /// the minimum cost route between two nodes in a weighted graph.
    /// </summary>
    public class DijkstraAlgorithm : IShortestPathAlgorithm
    {
        public (List<Node> path, double totalCost) FindPath(Graph graph, Node start, Node goal)
        {
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
                    return (ShortestPathHelpers.ReconstructPath(previous, goal), distances[goal]);

                foreach (var neighbor in ShortestPathHelpers.GetNeighbors(graph, current))
                {
                    double weight = ShortestPathHelpers.GetEdgeWeight(graph, current, neighbor);
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
    }

    /// <summary>
    /// A* algorithm implementation using Euclidean distance as heuristic.
    /// More efficient than Dijkstra when node positions are available.
    /// </summary>
    public class AStarAlgorithm : IShortestPathAlgorithm
    {
        public (List<Node> path, double totalCost) FindPath(Graph graph, Node start, Node goal)
        {
            var distances = new Dictionary<Node, double>();
            var priorityQueue = new PriorityQueue<Node, double>();
            var previous = new Dictionary<Node, Node>();

            foreach (var node in graph.Nodes) distances[node] = double.PositiveInfinity;

            distances[start] = 0;
            priorityQueue.Enqueue(start, Heuristics.Euclidean(start, goal));

            while (priorityQueue.Count > 0)
            {
                var current = priorityQueue.Dequeue();

                if (current == goal)
                    return (ShortestPathHelpers.ReconstructPath(previous, goal), distances[goal]);

                foreach (var neighbor in ShortestPathHelpers.GetNeighbors(graph, current))
                {
                    double weight = ShortestPathHelpers.GetEdgeWeight(graph, current, neighbor);
                    if (double.IsPositiveInfinity(weight))
                        continue;

                    double newG = distances[current] + weight;

                    if (newG < distances[neighbor])
                    {
                        distances[neighbor] = newG;
                        previous[neighbor] = current;

                        double fScore = newG + Heuristics.Euclidean(neighbor, goal);
                        priorityQueue.Enqueue(neighbor, fScore);
                    }
                }
            }

            return (new List<Node>(), 0);
        }
    }

    // Helper methods shared by shortest path algorithms
    public static class ShortestPathHelpers
    {
        public static IEnumerable<Node> GetNeighbors(Graph graph, Node node)
        {
            foreach (var edge in graph.Edges)
            {
                if (edge.Source == node) yield return edge.Target;
                else if (edge.Target == node) yield return edge.Source;
            }
        }

        public static double GetEdgeWeight(Graph graph, Node a, Node b)
        {
            foreach (var edge in graph.Edges)
            {
                if (edge.Source == a && edge.Target == b || edge.Source == b && edge.Target == a)
                {
                    return edge.Weight;
                }
            }
            return double.PositiveInfinity;
        }

        public static List<Node> ReconstructPath(Dictionary<Node, Node> previous, Node current)
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

    // Strategy pattern manager for runtime algorithm switching
    public class ShortestPathManager
    {
        private IShortestPathAlgorithm _algorithm;

        public ShortestPathManager(IShortestPathAlgorithm algorithm)
        {
            _algorithm = algorithm;
        }

        public void SetAlgorithm(IShortestPathAlgorithm algorithm) => _algorithm = algorithm;

        public (List<Node>, double) Calculate(Graph g, Node s, Node target)
        {
            return _algorithm.FindPath(g, s, target);
        }
    }

    // Heuristic functions for informed search algorithms
    public static class Heuristics
    {
        public static double Euclidean(Node a, Node b)
        {
            var dx = a.Location.X - b.Location.X;
            var dy = a.Location.Y - b.Location.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}