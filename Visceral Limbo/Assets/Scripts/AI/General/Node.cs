using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Node node;
    public List<Node> nodes = new List<Node>();

    public int x;
    public int z;
    public int index;
    public float gScore;
    public float hScore;
    public float colliderRadius;

    public float FScore
    {
        get { return gScore + hScore; }
    }

    public bool isWalkable;
    public bool isTargetNode = false;
    public int cameFromIndex;
    private static Node currentTargetNode; // Nodo destino activo actual

    public Material walkableMat;
    public Material notWalkableMat;
    public Material targetMat; // Nuevo material verde para destino

    public LayerMask obstacleLayer;


    private void Start()
    {
        AutoDetermineWalkability(obstacleLayer);
        displayFCost();
    }

    public void setIsWalkable(bool isWalkable)
    {
        if (!isTargetNode)
        {
            this.isWalkable = isWalkable;
            SetMaterial(isWalkable);
        }
    }

    private void SetMaterial(bool isWalkableOrTarget)
    {
        MeshRenderer meshRenderer = this.gameObject.GetComponentInChildren<MeshRenderer>();

        if (isTargetNode)
        {
            meshRenderer.material = targetMat;
        }
        else
        {
            meshRenderer.material = isWalkableOrTarget ? walkableMat : notWalkableMat;
        }
    }

    private void AutoDetermineWalkability(LayerMask obsLayer)
    {
       Collider[] obstacles = Physics.OverlapSphere(transform.position, colliderRadius, obsLayer);
        if (obstacles.Length > 0)
        {
            isWalkable = false;
            SetMaterial(isWalkable);
        }
        else
        {
            isWalkable = true;
            SetMaterial(isWalkable);
        }
    }

    private void displayFCost()
    {
        UtilsClass.CreateWorldText(gScore.ToString(), null, transform.position + Vector3.up * 2f, 20, Color.white, TextAnchor.MiddleCenter);
    }

    private void OnMouseDown()
    {
        ToggleWalkable();
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1)) // Click derecho
        {
            TrySetAsTarget();
        }
    }

    private void TrySetAsTarget()
    {
        if (!isWalkable)
        {
            Debug.Log("No puedes establecer como destino un nodo no caminable.");
            return;
        }

        else if (currentTargetNode == this)
        {
            currentTargetNode.isTargetNode = false;
            currentTargetNode.SetMaterial(currentTargetNode.isWalkable);
            currentTargetNode = null;
        }
        else if (currentTargetNode != null)
        {
            currentTargetNode.isTargetNode = false;
            currentTargetNode.SetMaterial(currentTargetNode.isWalkable);
            currentTargetNode = this;
            isTargetNode = true;
            SetMaterial(true); // Forzamos a usar el material de destino (targetMat)
        }
        else
        {
            currentTargetNode = this;
            isTargetNode = true;
            SetMaterial(true); // Forzamos a usar el material de destino (targetMat)
        }
            
    }


    private void ToggleWalkable()
    {
        if (isTargetNode)
        {
            Debug.Log("No puedes cambiar la caminabilidad de un nodo objetivo.");
            return;
        }
        isWalkable = !isWalkable;
        setIsWalkable(isWalkable);
        Debug.Log($"Nodo {gameObject.name} ahora isWalkable: {isWalkable}");
    }




    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.1f);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, node.transform.position);
        foreach (Node n in nodes)
        {
            if (n.isWalkable)
                Gizmos.color = Color.green;
            else
                Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, n.transform.position);
        }
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, colliderRadius);
    }

}
