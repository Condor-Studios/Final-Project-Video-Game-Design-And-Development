using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Nodo de dialogo
/// </summary>
[System.Serializable]
public class DialogueNode : MonoBehaviour
{
    /// <summary>
    /// Data del dialogo
    /// </summary>
    [TextArea(10, 3)]
    public string TextData;
    /// <summary>
    /// Nombre del personaje que habla
    /// </summary>
    public string SpeakerName;
    /// <summary>
    /// Sprite usado para representar al personaje
    /// </summary>
    public Sprite SpeakerSprite;
    /// <summary>
    /// El ID del siguiente dialogo
    /// </summary>
    /// 
    public int NextDialogeOption = -1; // dejar en  -1 si es el fin de dialogo

    public bool AutoAdvance;
    public float TimeToAdvance;
    public float TimeBetweenChars;

    /// <summary>
    /// Lista de opciones posibles, dejar nulo si lineal
    /// </summary>
    public List<DialogeOption> Options= new List<DialogeOption>(); 


}
