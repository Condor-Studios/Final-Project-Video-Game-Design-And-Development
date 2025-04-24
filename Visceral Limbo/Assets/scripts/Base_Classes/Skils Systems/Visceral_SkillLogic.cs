using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase base de la logica de skills
/// implementa PlayerContext = _UserContext
/// visceral_AbilitySO = _AbilitySO
/// Visceral_SkillManager = _SkillManager
/// </summary>
public abstract class Visceral_SkillLogic : MonoBehaviour
{
    /// <summary>
    /// El ScriptableObject que usara este script
    /// </summary>
    [SerializeField] public Visceral_AbilitySO _AbilitySO;

    /// <summary>
    /// contexto del usuario, comunica cosas utiles como posicion,transform, y el KCC de tenerlo
    /// si el usuario implementa KCC,NO UTILIZAR TRANSFORM normal, usar kcc.capsule.transform
    /// </summary>
    [SerializeField] protected PlayerContext _UserContext;
    /// <summary>
    /// SkillManager que utiliza esta habilidad
    /// </summary>
    [SerializeField] protected Visceral_SkillManager _SkillManager;

    public float cooldown => _AbilitySO.Cooldown;
    /// <summary>
    /// inicializacion del skilllogic, dejar default si no es necesario tocar nada
    /// </summary>
    /// <param name="data"> nueva data de scriptableobject</param>
    /// <param name="Skmanager">nueva data de skillmanager</param>
    /// <param name="UserContext">nueva data de usuario</param>
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

    /// <summary>
    /// la logica del skill, toda funcion / accion del skill debe de salir de esta funcion
    /// </summary>
    public abstract void ActivateSkill(); 

}
