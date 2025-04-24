using System.Collections.Generic;
using Unity.Mathematics.Geometry;
using UnityEngine;
namespace AI.General
{
    public class NodeGrid : MonoBehaviour
    {
        public Node nodePrefab;
        private int width;
        private int height;
        [SerializeField]
        private float cellSize;
        private Node[,] gridArray;

        private void Start()
        {
            AutoDetectDimensions();
            GenerateGrid();
            ConnectNodeGrid();
        }

        private void AutoDetectDimensions()
        {
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                var size = meshRenderer.bounds.size;
                width = Mathf.RoundToInt(size.x / cellSize);
                height = Mathf.RoundToInt(size.z / cellSize);
            }
            else
            {
                Debug.LogWarning("No MeshRenderer found on Grid object for size detection.");
            }
        }

        private void GenerateGrid()
        {
            var offsetVector = new Vector3(-46f, 0f, -46f); // Ajusta si cambias el origen

            gridArray = new Node[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    Node spawnedNode = Instantiate(nodePrefab, GetWorldPosition(x, z) + offsetVector, Quaternion.identity, transform);
                    spawnedNode.x = x;
                    spawnedNode.z = z;
                    spawnedNode.index = x + z * width;
                    gridArray[x, z] = spawnedNode;
                }
            }
        }

        private Vector3 GetWorldPosition(int x, int z)
        {
            return new Vector3(x, 0, z) * cellSize;
        }

        private void ConnectNodeGrid()
        {
            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int z = 0; z < gridArray.GetLength(1); z++)
                {
                    Node currentNode = gridArray[x, z];
                    currentNode.nodes = GetNeighbours(currentNode);
                }
            }
        }

        public Node GetNode(int x, int z)
        {
            if (!(Mathf.Abs(x) > width) && !(Mathf.Abs(z) > height))
            {
                return gridArray[Mathf.Abs(x), Mathf.Abs(z)];
            }
            return null;
        }

        public Node GetNodeFromWorldPosition(Vector3 worldPosition)
        {
            int x = Mathf.FloorToInt(worldPosition.x / cellSize);
            int z = Mathf.FloorToInt(worldPosition.z / cellSize);
            Debug.Log($"width: {width} height: {height} x: {x} z: {z}");
            return GetNode(x, z);
        }

        public Node GetNearestWalkableNode(Vector3 worldPosition)
        {
            Node centerNode = GetNodeFromWorldPosition(worldPosition);
            if (centerNode == null) return null;

            if (centerNode.isWalkable) return centerNode;

            int searchRadius = 1;
            while (searchRadius < Mathf.Max(width, height))
            {
                for (int x = -searchRadius; x <= searchRadius; x++)
                {
                    for (int z = -searchRadius; z <= searchRadius; z++)
                    {
                        Node checkNode = GetNode(centerNode.x + x, centerNode.z + z);
                        if (checkNode != null && checkNode.isWalkable)
                        {
                            return checkNode;
                        }
                    }
                }
                searchRadius++;
            }
            return null;
        }


        public Node[] GetAllNodes()
        {
            List<Node> nodeList = new List<Node>();
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    nodeList.Add(gridArray[x, z]);
                }
            }
            return nodeList.ToArray();
        }

        public List<Node> GetNeighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();

            int x = node.x;
            int z = node.z;

            if (x - 1 >= 0) neighbours.Add(gridArray[x - 1, z]);
            if (x + 1 < width) neighbours.Add(gridArray[x + 1, z]);
            if (z - 1 >= 0) neighbours.Add(gridArray[x, z - 1]);
            if (z + 1 < height) neighbours.Add(gridArray[x, z + 1]);

            // Diagonales opcionales
            if (x - 1 >= 0 && z - 1 >= 0) neighbours.Add(gridArray[x - 1, z - 1]);
            if (x - 1 >= 0 && z + 1 < height) neighbours.Add(gridArray[x - 1, z + 1]);
            if (x + 1 < width && z - 1 >= 0) neighbours.Add(gridArray[x + 1, z - 1]);
            if (x + 1 < width && z + 1 < height) neighbours.Add(gridArray[x + 1, z + 1]);

            return neighbours;
        }

        public void GetXZ(Vector3 worldPosition, out int x, out int z)
        {
            x = Mathf.FloorToInt((worldPosition.x + 46f) / cellSize);
            z = Mathf.FloorToInt((worldPosition.z + 46f) / cellSize);
        }

        public void Setup(Node[,] nodes)
        {
            gridArray = nodes;
            width = nodes.GetLength(0);
            height = nodes.GetLength(1);
        }

        public int Width => width;
        public int Height => height;
    }
}
