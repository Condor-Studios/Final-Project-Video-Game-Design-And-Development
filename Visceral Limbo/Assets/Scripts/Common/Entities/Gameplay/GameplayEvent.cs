using UnityEngine;

namespace Common.Entities.Gameplay
{
    public enum GameplayEventType
    {
        Healed,
        Damaged,
        BuffReceived,
        BuffRemoved,
        DebuffReceived,
        DebuffRemoved
    }

    public struct GameplayEvent
    {
        public GameplayEventType eventType;
        public int value;
    }
    
}