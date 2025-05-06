using System.Collections.Generic;
using Unity.Mathematics.Geometry;
using UnityEngine;
namespace AI.General
{
    public class NodeGrid : MonoBehaviour
    {
        public Node nodePrefab;
        private int _width;
        private int _height;
        [SerializeField]
        private float cellSize;
        private Node[,] _gridArray;
        public Node currentSelectedNode;

        [Tooltip("Si está en true, los nodos se muestran; si está en false, quedan ocultos.")]
        public bool debug;

        public bool generated;
        
        // Para detectar cambios en 'debug' en tiempo de ejecución
        private bool _prevDebug;

        private void Start()
        {
            AutoDetectDimensions();
            GenerateGrid();
            ConnectNodeGrid();
            ApplyNodeVisibility(debug);
            _prevDebug = debug;
        }
        
        private void Update()
        {
            // Si cambió desde el inspector, actualizamos la visibilidad
            if (debug != _prevDebug)
            {
                ApplyNodeVisibility(debug);
                _prevDebug = debug;
            }
        }
        
        private void OnValidate()
        {
            if (_gridArray != null)
                ApplyNodeVisibility(debug);
        }
        
        /// <summary>
        /// Recorre todos los nodos y activa/desactiva su MeshRenderer
        /// </summary>
        private void ApplyNodeVisibility(bool visible)
        {
            if (_gridArray == null) return;

            for (int x = 0; x < _width; x++)
            {
                for (int z = 0; z < _height; z++)
                {
                    Node current = _gridArray[x, z];
                    current.isTextVisible = visible;
                    var mr = current.GetComponentInChildren<MeshRenderer>();
                    if (mr) mr.enabled = visible;
                    current.DisplayText(visible);
                }
            }
        }

        private void AutoDetectDimensions()
        {
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer)
            {
                var size = meshRenderer.bounds.size;
                _width = Mathf.RoundToInt(size.x / cellSize);
                _height = Mathf.RoundToInt(size.z / cellSize);
                
                Debug.Log("Grid size: " + _width + "x" + _height);
            }
            else
            {
                Debug.LogWarning("No MeshRenderer found on Grid object for size detection.");
            }
        }

        private void GenerateGrid()
        {
            var offsetVector = new Vector3(-46f, 1f, -46f); // Ajusta si cambias el origen

            _gridArray = new Node[_width, _height];

            for (int x = 0; x < _width; x++)
            {
                for (int z = 0; z < _height; z++)
                {
                    Node spawnedNode = Instantiate(nodePrefab, GetWorldPosition(x, z) + offsetVector, Quaternion.identity, transform);
                    spawnedNode.x = x;
                    spawnedNode.z = z;
                    spawnedNode.index = x + z * _width;
                    _gridArray[x, z] = spawnedNode;
                }
            }
            generated = true;
        }

        private Vector3 GetWorldPosition(int x, int z)
        {
            return new Vector3(x, 0, z) * cellSize;
        }

        private void ConnectNodeGrid()
        {
            for (int x = 0; x < _gridArray.GetLength(0); x++)
            {
                for (int z = 0; z < _gridArray.GetLength(1); z++)
                {
                    Node currentNode = _gridArray[x, z];
                    currentNode.nodes = GetNeighbours(currentNode);
                }
            }
        }
        
        public Node GetCurrentSelectedNode()
        {
            foreach (var node in _gridArray)
            {
               if (node.isTargetNode)
               {
                   return node;
               }
            }
            Debug.Log("No hay nodo seleccionado");
            return null;
        }

        public Node GetNode(int x, int z)
        {
            Debug.Log("width: " + _width + "height: " + _height);
            if (!(Mathf.Abs(x) > _width) && !(Mathf.Abs(z) > _height))
            {
                Debug.Log("position is within node grid bounds:");
                return _gridArray[Mathf.Abs(x), Mathf.Abs(z)];
            }
            return null;
        }

        public Node GetNodeFromWorldPosition(Vector3 worldPosition)
        {
            int x = Mathf.FloorToInt(worldPosition.x / cellSize);
            int z = Mathf.FloorToInt(worldPosition.z / cellSize);
            Debug.Log("x: "+ x + "z: "+ z);
            return GetNode(x, z);
        }

        public Node GetNearestWalkableNode(Vector3 worldPosition)
        {
            Node centerNode = GetNodeFromWorldPosition(worldPosition);
            if (centerNode == null) return null;

            if (centerNode.isWalkable) return centerNode;

            int searchRadius = 1;
            while (searchRadius < Mathf.Max(_width, _height))
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
            for (int x = 0; x < _width; x++)
            {
                for (int z = 0; z < _height; z++)
                {
                    nodeList.Add(_gridArray[x, z]);
                }
            }
            return nodeList.ToArray();
        }

        public List<Node> GetNeighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();

            int x = node.x;
            int z = node.z;

            if (x - 1 >= 0) neighbours.Add(_gridArray[x - 1, z]);
            if (x + 1 < _width) neighbours.Add(_gridArray[x + 1, z]);
            if (z - 1 >= 0) neighbours.Add(_gridArray[x, z - 1]);
            if (z + 1 < _height) neighbours.Add(_gridArray[x, z + 1]);

            // Diagonales opcionales
            if (x - 1 >= 0 && z - 1 >= 0) neighbours.Add(_gridArray[x - 1, z - 1]);
            if (x - 1 >= 0 && z + 1 < _height) neighbours.Add(_gridArray[x - 1, z + 1]);
            if (x + 1 < _width && z - 1 >= 0) neighbours.Add(_gridArray[x + 1, z - 1]);
            if (x + 1 < _width && z + 1 < _height) neighbours.Add(_gridArray[x + 1, z + 1]);

            return neighbours;
        }

        public void GetXZ(Vector3 worldPosition, out int x, out int z)
        {
            x = Mathf.FloorToInt((worldPosition.x + 46f) / cellSize);
            z = Mathf.FloorToInt((worldPosition.z + 46f) / cellSize);
        }

        public void Setup(Node[,] nodes)
        {
            _gridArray = nodes;
            _width = nodes.GetLength(0);
            _height = nodes.GetLength(1);
        }
        
        

        public int Width => _width;
        public int Height => _height;
    }
}
