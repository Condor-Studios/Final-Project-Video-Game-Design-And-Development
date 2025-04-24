using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Visceral_SkillLogic : MonoBehaviour
{
    [SerializeField] public Visceral_AbilitySO _AbilitySO;
    [SerializeField] protected PlayerContext _UserContext;
    [SerializeField] protected Visceral_SkillManager _SkillManager;

    public float cooldown => _AbilitySO.Cooldown;
    public virtual void Initialize(Visceral_AbilitySO data, Visceral_SkillManager Skmanager, PlayerContext UserContext = null)
    {
        if(data != null)
        {
            _AbilitySO = data;
            _UserContext = UserContext;
            _SkillManager = Skmanager;
            
        }
        else
        {
            Debug.LogError("<color = blue> Visceral Error: Visceral Skill Logic recieved null datab " + this.GetInstanceID() + "</color>");
        }
    }

    public abstract void ActivateSkill(); 

}
