using System.Collections.Generic;
using UnityEngine;

namespace AI.General
{
    public class Pathfinding : MonoBehaviour
    {
        private NodeGrid _nodeGrid;

        private const int MOVE_STRAIGHT_COST = 10;
        private const int MOVE_DIAGONAL_COST = 14;

        public void Setup(NodeGrid nodeGrid)
        {
            this._nodeGrid = nodeGrid;
        }

        public List<Node> FindPath(Vector3 startWorldPos, Vector3 endWorldPos)
        {
            _nodeGrid.GetXZ(startWorldPos, out int startX, out int startZ);
            _nodeGrid.GetXZ(endWorldPos, out int endX, out int endZ);

            Node startNode = _nodeGrid.GetNode(startX, startZ);
            Node endNode = _nodeGrid.GetNode(endX, endZ);

            if (!startNode || !endNode || !startNode.isWalkable || !endNode.isWalkable)
            {
                return null; // No hay camino posible
            }

            List<Node> openList = new List<Node> { startNode };
            HashSet<Node> closedList = new HashSet<Node>();

            foreach (Node node in _nodeGrid.GetAllNodes())
            {
                node.gScore = float.MaxValue;
                node.hScore = 0;
                node.cameFromIndex = -1;
            }

            startNode.gScore = 0;
            startNode.hScore = CalculateDistanceCost(startNode, endNode);

            while (openList.Count > 0)
            {
                Node currentNode = GetLowestFScoreNode(openList);
                if (currentNode == endNode)
                {
                    return ReconstructPath(endNode);
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                foreach (Node neighbour in _nodeGrid.GetNeighbours(currentNode))
                {
                    if (!neighbour.isWalkable || closedList.Contains(neighbour)) continue;

                    float tentativeGScore = currentNode.gScore + CalculateDistanceCost(currentNode, neighbour);
                    if (tentativeGScore < neighbour.gScore)
                    {
                        neighbour.cameFromIndex = currentNode.index;
                        neighbour.gScore = tentativeGScore;
                        neighbour.hScore = CalculateDistanceCost(neighbour, endNode);

                        if (!openList.Contains(neighbour))
                        {
                            openList.Add(neighbour);
                        }
                    }
                }
            }

            return null; // No se encontro camino
        }

        private List<Node> ReconstructPath(Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;
            path.Add(currentNode);

            while (currentNode.cameFromIndex != -1)
            {
                int x = currentNode.cameFromIndex % _nodeGrid.Width;
                int z = currentNode.cameFromIndex / _nodeGrid.Width;
                currentNode = _nodeGrid.GetNode(x, z);
                path.Add(currentNode);
            }

            path.Reverse();
            return path;
        }

        private float CalculateDistanceCost(Node a, Node b)
        {
            int xDistance = Mathf.Abs(a.x - b.x);
            int zDistance = Mathf.Abs(a.z - b.z);
            int remaining = Mathf.Abs(xDistance - zDistance);
            return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, zDistance) + MOVE_STRAIGHT_COST * remaining;
        }

        private Node GetLowestFScoreNode(List<Node> nodeList)
        {
            Node lowest = nodeList[0];
            for (int i = 1; i < nodeList.Count; i++)
            {
                if (nodeList[i].FScore < lowest.FScore)
                {
                    lowest = nodeList[i];
                }
            }
            return lowest;
        }
    }
}
