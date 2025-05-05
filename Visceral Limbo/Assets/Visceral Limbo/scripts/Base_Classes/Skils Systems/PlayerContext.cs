using UnityEngine;

namespace Visceral_Limbo.scripts.Base_Classes.Skils_Systems
{
    /// <summary>
    /// Este script servir� para pasar contexto basico del usuario, elementos que sepamos que van a ser utiles o muy pesados de andar obteniendo
    /// </summary>
    public class PlayerContext : Visceral_Script
    {
        /// <summary>
        /// La salud del usuario
        /// </summary>
        public float Health;
        /// <summary>
        /// El GameObject general del usuario
        /// </summary>
        public GameObject PlayerGameObject;
        /// <summary>
        /// El transform del usuario
        /// </summary>
        public Transform PlayerTransform;

        public override void VS_Initialize()
        {
            PlayerGameObject = gameObject;
            PlayerTransform= GetComponent<Transform>();
        }
    }
}
