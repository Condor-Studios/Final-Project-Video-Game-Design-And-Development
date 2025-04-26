using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "Nuevo Dialogo",menuName = "Visceral_Limbo/Systems/DialogueSystem/DialogueData")]
public class DialogueData : ScriptableObject
{

    public List<DialogueNode> DialogueNodes;

}
