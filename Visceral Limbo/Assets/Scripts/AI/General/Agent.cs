// Agent.cs

using Common.Entities;
using UnityEngine;

namespace AI.General
{
    public class Agent : Entity
    {
        [Header("Movement Settings")] public float moveSpeed = 3.5f;
        public float rotationSpeed = 10f;

        [Header("Basic Sensor Settings")] public float idleDuration = 5f;
        public float roamRange = 5f;
        public float visionRange = 2f;
        public float visionAngle = 45f;
        public float attackRange = 1.5f;
        public float attackInterval = 3f;
        public int attackDamage = 1;

        [Header("Component Settings")] 
        [SerializeField]
        protected Rigidbody _rb;
        [SerializeField]
        protected Transform playerTarget;
        [SerializeField]
        protected Entity playerEntity;
        public LayerMask playerLayer;
        public Material TargetMaterial;
        [SerializeField] protected NodeGrid grid;
        [SerializeField] protected Pathfinding pathfinding;
        protected Node targetNode;
        protected Agent agent;

        public NodeGrid Grid => grid;
        public Pathfinding Pathfinding => pathfinding;
        public Node TargetNode => targetNode;
        public Transform PlayerTarget => playerTarget;
        public Entity PlayerEntity => playerEntity;

        protected virtual void Awake()
        {
            agent = this;
            _rb = GetComponent<Rigidbody>();
        }

        public virtual void MoveTowards(Vector3 targetPosition)
        {
            var direction = (targetPosition - transform.position).normalized;
            _rb.MovePosition(transform.position + direction * (moveSpeed * Time.deltaTime));
        }

        public virtual void LookAt(Vector3 targetPosition)
        {
            var direction = (targetPosition - transform.position).normalized;
            direction.y = 0f;
            if (!(direction.magnitude > 0.01f)) return;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        public virtual void StopMovement()
        {
            _rb.velocity = Vector3.zero;
        }

        public virtual void GetRandomTargetNode()
        {
            int maxAttempts = 50; // Número máximo de intentos permitidos
            int attempts = 0;

            do
            {
                var randomX = Random.Range(-roamRange + this.transform.position.x,
                    roamRange + this.transform.position.x);
                var randomZ = Random.Range(-roamRange + this.transform.position.z,
                    roamRange + this.transform.position.z);
                var randomPosition = new Vector3(randomX, 0, randomZ);
                Debug.Log("Random position generated: " + randomPosition);
                targetNode = grid.GetNodeFromWorldPosition(randomPosition);
                attempts++;
            } while ((!targetNode || !targetNode.isWalkable) && attempts < maxAttempts);

            if (targetNode && targetNode.isWalkable)
            {
                targetNode.TrySetAsTarget();
                Debug.Log("Target acquired: " + targetNode.transform.position);
            }
            else
            {
                Debug.LogWarning($"Failed to find a walkable target node after {maxAttempts} attempts.");
            }
        }

        public bool CanSeePlayer()
        {
            Ray ray = new Ray(agent.transform.position + Vector3.up * 0.5f, agent.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, visionRange, playerLayer))
            {
                return true;
            }

            return false;
        }

        public bool HasReachedDestination(Vector3 destinationPosition)
        {
            return Vector3.Distance(agent.transform.position, destinationPosition) <= 2f;
        }

        public void DetectPlayer()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, visionRange, playerLayer);

            foreach (Collider hit in hits)
            {
                Vector3 dirToTarget = (hit.transform.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.forward, dirToTarget);

                if (angle <= visionAngle / 2f)
                {
                    playerTarget = hit.transform;
                    playerEntity = playerTarget?.GetComponent<Entity>();
                    Renderer renderer = playerTarget?.GetComponent<Renderer>();
                    if (renderer)
                    {
                        renderer.material = TargetMaterial;
                        Debug.Log("Player detected");
                    }

                    return;
                }
            }

            if (playerTarget && hits.Length == 0)
            {
                // Perdio de vista al jugador
                playerTarget = null;
                playerEntity = null;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Vector3 leftBoundary = Quaternion.Euler(0, -visionAngle / 2f, 0) * transform.forward;
            Vector3 rightBoundary = Quaternion.Euler(0, visionAngle / 2f, 0) * transform.forward;

            Gizmos.DrawRay(transform.position, leftBoundary * visionRange);
            Gizmos.DrawRay(transform.position, rightBoundary * visionRange);
            Gizmos.DrawWireSphere(transform.position, visionRange);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, roamRange);
        }
    }
}