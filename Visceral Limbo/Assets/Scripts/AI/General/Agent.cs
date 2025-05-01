// Agent.cs

using AI.General;
using Common.Entities;
using UnityEngine;

public class Agent : Entity
{
    [Header("Movement Settings")]
    public float moveSpeed = 3.5f;
    public float rotationSpeed = 10f;
    
    [Header("Basic Sensor Settings")]
    public float idleDuration = 5f;
    public float roamRange = 5f;
    public float visionRange = 2f;
    public float visionAngle = 45f;
    public float attackRange = 1.5f;
    public float attackInterval = 3f;
    public int attackDamage = 1;

    protected Rigidbody rb;

    [SerializeField]
    protected NodeGrid grid;
    [SerializeField]
    protected Pathfinding pathfinding;
    protected Node targetNode;

    protected Agent agent;
    
    public NodeGrid Grid => grid;
    public Pathfinding Pathfinding => pathfinding;
    public Node TargetNode => targetNode;

    protected virtual void Awake()
    {
        agent = this;
        rb = GetComponent<Rigidbody>();
    }

    public virtual void MoveTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        rb.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);
    }

    public virtual void LookAt(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0f;
        if (direction.magnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    public virtual void StopMovement()
    {
        rb.velocity = Vector3.zero;
    }
    
    public virtual void GetRandomTargetNode()
    {
        var randomX = Random.Range(-roamRange, roamRange);
        var randomZ = Random.Range(-roamRange, roamRange);
        var randomPosition = new Vector3(randomX, 0, randomZ);
        targetNode = grid.GetNearestWalkableNode(randomPosition);
        Debug.Log("target acquired: "+ targetNode.transform.position);
    }
}