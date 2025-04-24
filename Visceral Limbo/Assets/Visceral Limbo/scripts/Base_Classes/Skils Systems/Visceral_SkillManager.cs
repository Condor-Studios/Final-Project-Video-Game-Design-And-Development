using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Visceral_SkillManager : Visceral_Script
{
    /// <summary>
    /// contexto del usuario
    /// </summary>
    private PlayerContext UserContext; 
    /// <summary>
    /// diccionario de habilidades y sus IDs
    /// </summary>
    private Dictionary<string, Visceral_SkillLogic> AbilityDicc = new();
    /// <summary>
    /// diccionario de IDs y Cooldowns de habilidades
    /// </summary>
    private Dictionary<string,float> CooldownDicc= new();

    [SerializeField] private VisceralSkillLoadout Loadout;

    public override void VS_Initialize()
    {
        UserContext = GetComponent<PlayerContext>();

        // por cada scriptableObject del Loadout, creamos una instancia en escena de la habilidad
        foreach(var Data in Loadout.Skill_Loadout)
        {
            //instanciado de objeto
            var SkillGO = Instantiate(Data.behaviorComponentPrefab, UserContext.PlayerTransform);
            SkillGO.transform.SetParent(UserContext.PlayerTransform);
            //iniciado de skill
            var Behaviour = SkillGO.GetComponent<Visceral_SkillLogic>();
            Behaviour.Initialize(Data,this,UserContext);
            //guardado de skill
            AbilityDicc[Data.SkillID] = Behaviour;
            CooldownDicc[Data.SkillID] = 0f;
            print(Data.SkillID);
        }
    }

    /// <summary>
    /// update de la logica interna del skillmanager
    /// </summary>
    public override void VS_RunLogic()
    {
        UpdateCooldowns();
    }

    /// <summary>
    /// update de cooldowns
    /// </summary>
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

    /// <summary>
    /// Intentar de usar skill guardada en el SkillManager
    /// </summary>
    /// <param name="SkillName">ID de skill guardada</param>
    /// <returns></returns>
    public bool TryUseSkill(string SkillName)
    {
        if(!AbilityDicc.TryGetValue(SkillName, out var ability))
        {
            Debug.LogError("<Color=blue> Visceral Error: Skill Name not found " + SkillName + " on " + this.ToString() + "</Color>");
            return false;
        }
        if (CooldownDicc[SkillName] > 0f)
        {
            Debug.Log("<Color=lightblue> Visceral Warning: Skill " + SkillName + " on cooldown " + CooldownDicc[SkillName] + "</Color>");
            return false;
        }

        ability.ActivateSkill();
        CooldownDicc[SkillName] = ability.cooldown;

        return true;

    }

    /// <summary>
    /// Booleano para ver si la habilidad esta en cooldown
    /// </summary>
    /// <param name="SkillID"> ID de la Skill deseada</param>
    /// <returns></returns>
    public bool IsOnCooldown(string SkillID)
    {
        return Time.time < CooldownDicc[SkillID];
    }

}
