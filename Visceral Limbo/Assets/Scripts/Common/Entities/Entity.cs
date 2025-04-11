using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{

    protected int health;
    protected int maxHealth;
    protected bool isAlive;

    public int Health
    {
        get { return health; }
        set
        {
            health = value;
            if (health <= 0)
            {
                Die();
            }
        }
    }

    public int MaxHealth
    {
        get { return maxHealth; }
        set { maxHealth = value; }
    }

    public bool IsAlive
    {
        get { return isAlive; }
        set { isAlive = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void Die()
    {
        if (health <= 0 && isAlive)
        {
            isAlive = false;
            // Handle death logic here (e.g., play animation, remove from game, etc.)
            Debug.Log($"{gameObject.name} has died.");
        }
    }

}
