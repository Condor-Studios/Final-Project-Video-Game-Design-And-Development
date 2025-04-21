using System.Collections.Generic;
using System.Security.Principal;
using Common.Entities;
using UnityEngine;

namespace AI.General
{
    public class Agent : Entity
    {
        private Pathfinding pathfinding;
        private NodeGrid grid;
        private List<Node> path;
        private int currentPathIndex;

        [SerializeField]
        private float moveSpeed = 5f;

        private void Start()
        {
            grid = FindObjectOfType<NodeGrid>();
            pathfinding = FindObjectOfType<Pathfinding>();
            pathfinding.Setup(grid);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Node targetNode = GetCurrentTargetNode();
                if (targetNode)
                {
                    RequestPath(targetNode);
                }
            }

            FollowPath();
        }

        private void RequestPath(Node targetNode)
        {
            Vector3 startWorldPos = transform.position;
            Vector3 endWorldPos = targetNode.transform.position;
            path = pathfinding.FindPath(startWorldPos, endWorldPos);

            if (path != null && path.Count > 0)  currentPathIndex = 0;
            else
            {
                Debug.LogWarning("No se encontró un camino al destino.");
            }
        }

        private void FollowPath()
        {
            if (path == null || currentPathIndex >= path.Count) return;

            Node targetNode = path[currentPathIndex];
            Vector3 targetPosition = targetNode.transform.position;
            Vector3 moveDirection = (targetPosition - transform.position).normalized;

            transform.position += moveSpeed * moveDirection * Time.deltaTime;

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                currentPathIndex++;
            }
        }

        private Node GetCurrentTargetNode()
        {
            Node[] allNodes = grid.GetAllNodes();
            foreach (Node node in allNodes)
            {
                if (node.isTargetNode)
                {
                    return node;
                }
            }
            return null;
        }
    }
}