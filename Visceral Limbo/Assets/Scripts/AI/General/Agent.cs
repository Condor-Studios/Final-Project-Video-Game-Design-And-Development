using System.Collections.Generic;
using System.Diagnostics;
using Common.Entities;
using UnityEngine;

namespace AI.General
{
    public class Agent : Entity
    {
        [SerializeField]
        protected Pathfinding pathfinding;
        [SerializeField]
        protected NodeGrid grid;
        protected List<Node> path;
        protected int currentPathIndex;

        [SerializeField]
        protected float moveSpeed = 5f;

        protected void Awake()
        {
            pathfinding.Setup(grid);
        }

        protected void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Node targetNode = GetCurrentTargetNode();
                if (targetNode != null)
                {
                    RequestPath(targetNode);
                }
            }

            FollowPath();
        }

        public void RequestPath(Node targetNode)
        {
            Vector3 startWorldPos = transform.position;
            Vector3 endWorldPos = targetNode.transform.position;
            path = pathfinding.FindPath(startWorldPos, endWorldPos);

            if (path != null && path.Count > 0)
                currentPathIndex = 0;
            else
                UnityEngine.Debug.LogWarning("No se encontró un camino al destino.");
        }

        protected void FollowPath()
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

        protected Node GetCurrentTargetNode()
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

        public bool HasReachedDestination()
        {
            return path == null || currentPathIndex >= path.Count;
        }

        public void StopMoving()
        {
            path = null;
        }
    }
}
