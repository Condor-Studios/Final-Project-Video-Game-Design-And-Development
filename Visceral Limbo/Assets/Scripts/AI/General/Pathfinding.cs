using System.Collections.Generic;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    private const int MOVE_DIAGONAL_COST = 14;
    private const int MOVE_STRAIGHT_COST = 10;

    private void Start()
    {
        FindPath(new int2(0, 0), new int2(9, 9));
    }

    private void FindPath(int2 startPosition, int2 endPosition)
    {
        int2 gridSize = new int2(10, 10);
        NativeArray<PathNode> pathNodes = new NativeArray<PathNode>(gridSize.x * gridSize.y, Allocator.Temp);

        for (int z = 0; z < gridSize.y; z++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                PathNode pathNode = new PathNode();
                pathNode.x = x;
                pathNode.z = z;
                pathNode.index = CalculateIndex(x, z, gridSize.x);
                pathNode.GCost = int.MaxValue;
                pathNode.HCost = CalculateDistanceCost(new int2(x, z), endPosition);
                pathNode.isWalkable = true;
                pathNode.cameFromIndex = -1;

                pathNodes[pathNode.index] = pathNode;
            }
        }

        NativeArray<int2> neighboutOffsetArray = new NativeArray<int2>(new int2[]
        {
            new int2(-1, 0),
            new int2(+1, 0),
            new int2(0,+1),
            new int2(0,-1),
            new int2(-1, -1),
            new int2(-1, +1),
            new int2(+1, -1),
            new int2(+1, +1),
        }, Allocator.Temp);

        int endNodeIndex = CalculateIndex(endPosition.x, endPosition.y, gridSize.x);
        PathNode startNode = pathNodes[CalculateIndex(startPosition.x, startPosition.y, gridSize.x)];
        startNode.GCost = 0;
        pathNodes[startNode.index] = startNode;

        NativeList<int> openSet = new NativeList<int>(Allocator.Temp);
        NativeList<int> closedSet = new NativeList<int>(Allocator.Temp);

        openSet.Add(startNode.index);

        while (openSet.Length > 0)
        {
            int currentNodeIndex = GetLowestFCostIndex(pathNodes, openSet);
            PathNode currentNode = pathNodes[currentNodeIndex];

            if (currentNode.index == endNodeIndex)
            {
                // Path found, reconstruct path
                List<int> path = new List<int>();
                while (currentNode.cameFromIndex != -1)
                {
                    path.Add(currentNode.index);
                    currentNode = pathNodes[currentNode.cameFromIndex];
                }
                path.Reverse();
                Debug.Log("Path found: " + string.Join(", ", path));
                break;
            }

            // Remove current node from open set and add to closed set
            for (int i = 0; i < openSet.Length; i++)
            {
                if (openSet[i] == currentNodeIndex)
                {
                    openSet.RemoveAtSwapBack(i);
                    break;
                }
            }

            closedSet.Add(currentNodeIndex);

            for (int i = 0; i < neighboutOffsetArray.Length; i++)
            {
                int2 neighbourOffset = neighboutOffsetArray[i];
                int2 neighbourPosition = new int2(currentNode.x + neighbourOffset.x, currentNode.z + neighbourOffset.y);

                if (IsPositionInsideGrid(neighbourPosition, gridSize) == false)
                {
                    //Neighbour is outside the grid
                    continue;
                }
                int neighbourIndex = CalculateIndex(neighbourPosition.x, neighbourPosition.y, gridSize.x);
                if(closedSet.Contains(neighbourIndex))
                {
                    // Neighbour is already evaluated
                    continue;
                }

                PathNode neighbourNode = pathNodes[neighbourIndex];
                if (neighbourNode.isWalkable == false)
                {
                    // Neighbour is not walkable
                    continue;
                }

                int2 currentNodePosition = new int2(currentNode.x, currentNode.z);
                int tentativeGCost = currentNode.GCost + CalculateDistanceCost(currentNodePosition, neighbourPosition);
                if (tentativeGCost < neighbourNode.GCost)
                {
                    // This path to neighbour is better than any previous one
                    neighbourNode.cameFromIndex = currentNode.index;
                    neighbourNode.GCost = tentativeGCost;
                    pathNodes[neighbourIndex] = neighbourNode;
                    if (openSet.Contains(neighbourIndex) == false)
                    {
                        openSet.Add(neighbourIndex);
                    }
                }
            }
        }

        PathNode endNode = pathNodes[endNodeIndex];
        if(endNode.cameFromIndex == -1)
        {
            Debug.Log("No path found");
        }
        else
        {
            Debug.Log("Path found to end node: " + endNode.index);
            NativeList<int2> path = CalculatePath(pathNodes, endNode);
            path.Dispose();
        }

        pathNodes.Dispose();
        neighboutOffsetArray.Dispose();
        openSet.Dispose();
        closedSet.Dispose();
    }

    private NativeList<int2> CalculatePath(NativeArray<PathNode> pathNodeArray, PathNode endNode)
    {
        if (endNode.cameFromIndex == -1)
        {
            return new NativeList<int2>(Allocator.Temp);
        }
        else
        {
            // Found a path
            NativeList<int2> path = new NativeList<int2>(Allocator.Temp);
            path.Add(new int2(endNode.x, endNode.z));

            PathNode currentNode = endNode;
            while (currentNode.cameFromIndex != -1)
            {
                PathNode cameFromNode = pathNodeArray[currentNode.cameFromIndex];
                path.Add(new int2(currentNode.x, currentNode.z));
                currentNode = cameFromNode;
            }
            return path;
        }
    }

    private bool IsPositionInsideGrid(int2 position, int2 gridSize)
    {
        return
        position.x >= 0 &&
        position.y >= 0 &&
        position.x < gridSize.x &&
        position.y < gridSize.y;
    }

    private int CalculateIndex(int x, int z, int gridWidth)
    {
        return x + (z * gridWidth);
    }

    private int CalculateDistanceCost(int2 positionA, int2 positionB)
    {
        int xDistance = math.abs(positionA.x - positionB.x);
        int zDistance = math.abs(positionA.y - positionB.y);
        int remaining = math.abs(xDistance - zDistance);
        return MOVE_DIAGONAL_COST * math.min(xDistance, zDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private int GetLowestFCostIndex(NativeArray<PathNode> pathNodes, NativeList<int> openSet)
    {
        PathNode lowestCostPathNode =  pathNodes[openSet[0]];
        for (int i = 1; i < openSet.Length; i++)
        {
            PathNode testPathNode = pathNodes[openSet[i]];
            if (testPathNode.FCost < lowestCostPathNode.FCost)
            {
                lowestCostPathNode = testPathNode;
            }
        }
        return lowestCostPathNode.index;
    }


    private struct PathNode
    {
        public int x;
        public int z;

        public int index;

        public int GCost;
        public int HCost;
        public int FCost
        {
            get
            {
                return GCost + HCost;
            }
        }

        public bool isWalkable;
        public int cameFromIndex;
    }
}
