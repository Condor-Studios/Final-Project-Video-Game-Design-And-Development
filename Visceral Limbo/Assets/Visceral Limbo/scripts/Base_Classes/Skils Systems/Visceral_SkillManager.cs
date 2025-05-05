using System.Collections.Generic;
using UnityEngine;
using Visceral_Limbo.Assets.ScriptableObjects.Abilities.Player_Abilities.Skill_Loadouts;

namespace Visceral_Limbo.scripts.Base_Classes.Skils_Systems
{
    public class Visceral_SkillManager : Visceral_Script
    {
        private PlayerContext UserContext;
        private Dictionary<string, Visceral_SkillLogic> AbilityDicc = new();
        private Dictionary<string,float> CooldownDicc= new();

        [SerializeField] private VisceralSkillLoadout Loadout;

        public override void VS_Initialize()
        {
            UserContext = GetComponent<PlayerContext>();

            foreach(var Data in Loadout.Skill_Loadout)
            {
                var SkillGO = Instantiate(Data.behaviorComponentPrefab, UserContext.PlayerTransform);
                SkillGO.transform.SetParent(UserContext.PlayerTransform);
                var Behaviour = SkillGO.GetComponent<Visceral_SkillLogic>();
                Behaviour.Initialize(Data,this,UserContext);
                AbilityDicc[Data.SkillID] = Behaviour;
                CooldownDicc[Data.SkillID] = 0f;
            }
        }

        public override void VS_RunLogic()
        {
            UpdateCooldowns();
        }


        private void UpdateCooldowns()
        {
            var keys = new List<string>(CooldownDicc.Keys); 

            foreach(var key in keys)
            {
                if (CooldownDicc[key] > 0f)
                {
                    CooldownDicc[key] -= Time.deltaTime;
                }
            }
        }


        public bool TryUseSkill(string SkillName)
        {
            if(!AbilityDicc.TryGetValue(SkillName, out var ability))
            {
                Debug.LogError("<Color=blue> Visceral Error: Skill Name not found " + SkillName + " on " + this.ToString() + "</Color>");
                return false;
            }
            if (CooldownDicc[SkillName] > 0f)
            {
                Debug.Log("<Color=lightblue> Visceral Warning: Skill " + SkillName + " on cooldown </Color>");
                return false;
            }

            ability.ActivateSkill();
            CooldownDicc[SkillName] = ability.cooldown;

            return true;

        }

        public bool IsOnCooldown(string SkillID)
        {
            return Time.time < CooldownDicc[SkillID];
        }

    }
}
