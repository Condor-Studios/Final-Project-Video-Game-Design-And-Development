using System.Collections.Generic;
using Common.Entities.Buffs;
using Common.Entities.Entities;
using Common.Entities.Gameplay;
using UnityEngine;

namespace Common.Entities.Passives
{
    [CreateAssetMenu(fileName = "New Passive", menuName = "Gameplay/PassiveEffect")]
    public class PassiveEffect : ScriptableObject
    {
        [Header("General Info")]
        public string passiveName;

        [Header("Applicable Classes")]
        public List<ClassType> applicableClasses = new List<ClassType>();

        [Header("Trigger Settings")]
        public GameplayEventType triggerEvent;
        public int thresholdValue;

        [Header("Buffs To Apply")]
        public List<Buff> buffsToApply = new List<Buff>();
    }
}