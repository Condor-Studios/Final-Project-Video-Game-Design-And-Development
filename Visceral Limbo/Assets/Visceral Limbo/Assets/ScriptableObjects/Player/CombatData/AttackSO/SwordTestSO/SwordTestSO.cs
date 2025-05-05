using UnityEngine;

namespace Visceral_Limbo.Assets.ScriptableObjects.Player.CombatData.AttackSO.SwordTestSO
{
    [CreateAssetMenu(menuName = "Visceral_Limbo/Combat Data/Attack Data/Scriptable Objects/ Sword Test SO") ]
    public class SwordTestSO : ScriptableObject
    {

        public AnimatorOverrideController _AnimatorOV;
        public float Damage;
        public float KnockBack;
        public float HyperArmor;
        public string AttackName;

    }
}
