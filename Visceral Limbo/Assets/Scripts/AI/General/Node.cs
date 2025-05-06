using System.Collections.Generic;
using CodeMonkey.Utils;
using UnityEngine;

namespace AI.General
{
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

        public bool isTextVisible;
        public bool isWalkable;
        public bool isTargetNode;
        public int cameFromIndex;
        private static Node _currentTargetNode; // Nodo destino activo actual
        private TextMesh text;

        public Material walkableMat;
        public Material notWalkableMat;
        public Material targetMat; // Nuevo material verde para destino

        public LayerMask obstacleLayer;


        private void Start()
        {
            AutoDetermineWalkability(obstacleLayer);
            DisplayFCost();
            DisplayText(isTextVisible);
        }
        
        private void Update()
        {
            AutoDetermineWalkability(obstacleLayer);
        }
        
        

        private void SetIsWalkable(bool isWalkable)
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

        private void DisplayFCost()
        {
           text = UtilsClass.CreateWorldText(gScore.ToString(), null, transform.position + Vector3.up * 2f, 20, Color.white, TextAnchor.MiddleCenter);
        }

        public void DisplayText(bool visible)
        {
            if (text)
            {
                text.gameObject.SetActive(visible);
            }
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

        public bool TrySetAsTarget()
        {
            if (!isWalkable)
            {
                Debug.Log("No puedes establecer como destino un nodo no caminable.");
                return false;
            }

            else if (_currentTargetNode == this)
            {
                _currentTargetNode.isTargetNode = false;
                _currentTargetNode.SetMaterial(_currentTargetNode.isWalkable);
                _currentTargetNode = null;
                return false;
            }
            else if (_currentTargetNode != null)
            {
                _currentTargetNode.isTargetNode = false;
                _currentTargetNode.SetMaterial(_currentTargetNode.isWalkable);
                _currentTargetNode = this;
                isTargetNode = true;
                SetMaterial(true); // Forzamos a usar el material de destino (targetMat)
                return true;
            }
            else
            {
                _currentTargetNode = this;
                isTargetNode = true;
                SetMaterial(true); // Forzamos a usar el material de destino (targetMat)
                return true;
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
            SetIsWalkable(isWalkable);
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
}
