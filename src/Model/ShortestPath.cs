using System;
using System.Collections.Generic;
using System.Linq;

namespace graphSNA.Model
{
    public interface IShortestPathAlgorithm
    {
        (List<Node> path, double totalCost) FindPath(Graph graph, Node start, Node goal);
    }

    // 1. DIJKSTRA (SADELEŞTİRİLMİŞ)
    public class DijkstraAlgorithm : IShortestPathAlgorithm
    {
        public (List<Node> path, double totalCost) FindPath(Graph graph, Node start, Node goal)
        {
            var distances = new Dictionary<Node, double>();
            var previous = new Dictionary<Node, Node>();
            var priorityQueue = new PriorityQueue<Node, double>();
            // "visited" listesini kaldırdık, gerek yok.

            foreach (var node in graph.Nodes)
            {
                distances[node] = double.PositiveInfinity;
            }
            distances[start] = 0;
            priorityQueue.Enqueue(start, 0);

            while (priorityQueue.Count > 0)
            {
                var current = priorityQueue.Dequeue();

                // "visited" kontrolünü sildik.

                if (current == goal)
                    return (ShortestPathHelpers.ReconstructPath(previous, goal), distances[goal]); // Stop if we reached the goal

                // visited.Add sildik.

                foreach (var neighbor in ShortestPathHelpers.GetNeighbors(graph, current))
                {
                    double weight = ShortestPathHelpers.GetEdgeWeight(graph, current, neighbor);
                    if (double.IsPositiveInfinity(weight))
                        continue; // Skip evaluation if the edge is missing or invalid

                    double newDist = distances[current] + weight;

                    if (newDist < distances[neighbor])
                    {
                        distances[neighbor] = newDist;
                        previous[neighbor] = current;
                        priorityQueue.Enqueue(neighbor, newDist);
                    }
                }
            }

            // If there is no path found to the goal then return empty path and 0 cost
            return (new List<Node>(), 0);
        }
    }

    // 2. A* ALGORITHM
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
                        continue; // Skip evaluation if the edge is missing or invalid

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

    // Helper methods for shortest path algorithms
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

        // Returns infinite cost if the edge is missing.
        public static double GetEdgeWeight(Graph graph, Node a, Node b)
        {
            foreach (var edge in graph.Edges)
            {
                if ((edge.Source == a && edge.Target == b) || (edge.Source == b && edge.Target == a))
                {
                    return edge.Weight;
                }
            }
            return double.PositiveInfinity; // Edge missing; should not be preferred
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

    // Manager Class to be used by the UI
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

    // Distance calculators for A*
    public static class Heuristics
    {
        // Euclidean: if a^2=b^2+c^2 => a=sqrt(b^2+c^2)
        public static double Euclidean(Node a, Node b)
        {
            var dx = a.Location.X - b.Location.X; // x2 - x1
            var dy = a.Location.Y - b.Location.Y; // y2 - y1
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}