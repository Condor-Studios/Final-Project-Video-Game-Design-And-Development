using UnityEngine;
using Visceral_Limbo.Assets.ScriptableObjects.Abilities.Player_Abilities.Basic;
using Visceral_Limbo.scripts.Base_Classes.Skils_Systems;

namespace Visceral_Limbo.scripts.Player.PlayerSkills.TestSkills
{
    public class TestSkill_PewPew : Visceral_SkillLogic
    {
        [SerializeField] float Damage;
        public override void ActivateSkill()
        {
            print("Pew Pewd for " + Damage);
        }

        public override void Initialize(Visceral_AbilitySO data, Visceral_SkillManager Skmanager, PlayerContext UserContext = null)
        {
            base.Initialize(data, Skmanager, UserContext);
        }


    }
}
