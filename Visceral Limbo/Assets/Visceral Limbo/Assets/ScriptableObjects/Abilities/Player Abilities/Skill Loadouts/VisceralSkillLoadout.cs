using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Visceral_Limbo/Combat Data/SkillLoadout/SkillLoadoutSO")]
public class VisceralSkillLoadout : ScriptableObject
{
    public List<Visceral_AbilitySO> Skill_Loadout = new();
}
