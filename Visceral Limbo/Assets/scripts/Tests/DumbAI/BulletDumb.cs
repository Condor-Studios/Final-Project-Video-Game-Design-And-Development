using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDumb : MonoBehaviour
{
    [SerializeField] private float BulletSpeed,damage;
    [SerializeField] private Health_Component health;
    [SerializeField] private DamageCollisionTrigger DamageTrigger;

    private void Start()
    {
        DamageTrigger = GetComponent<DamageCollisionTrigger>();
        StartCoroutine(startDamage());
    }

    IEnumerator startDamage()
    {
        yield return new WaitForSeconds(0.01f);
        DamageTrigger.Activate(true);
        DamageTrigger.UpdateValues(damage, 0, false);
    }

    private void Update()
    {
        health.TakeDamage(Time.deltaTime);

        this.transform.position += this.transform.forward * BulletSpeed * Time.deltaTime;

    }
}
