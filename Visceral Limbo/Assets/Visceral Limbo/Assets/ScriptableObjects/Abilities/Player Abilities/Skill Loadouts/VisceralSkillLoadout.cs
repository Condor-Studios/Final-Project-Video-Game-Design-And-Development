using System.Collections.Generic;
using UnityEngine;
using Visceral_Limbo.Assets.ScriptableObjects.Abilities.Player_Abilities.Basic;

namespace Visceral_Limbo.Assets.ScriptableObjects.Abilities.Player_Abilities.Skill_Loadouts
{
    [CreateAssetMenu(menuName = "Visceral_Limbo/Combat Data/SkillLoadout/SkillLoadoutSO")]
    public class VisceralSkillLoadout : ScriptableObject
    {
        public List<Visceral_AbilitySO> Skill_Loadout = new();
    }
}
