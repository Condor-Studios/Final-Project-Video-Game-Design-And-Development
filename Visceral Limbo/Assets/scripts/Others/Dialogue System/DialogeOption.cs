using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Script usado para crear opciones de dialogo
/// </summary>
[System.Serializable]
public class DialogeOption : MonoBehaviour
{
    /// <summary>
    /// Respuesta del jugador
    /// </summary>
    public string TextReply;

    /// <summary>
    /// Siguiente linea de dialogo, usar numero de posicion de dialogo
    /// </summary>
    public int nextDialogueID;

}
