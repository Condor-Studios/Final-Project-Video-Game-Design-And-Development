using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Unity.Collections;

public class ExplosiveBarrel : MonoBehaviour
{
    public float explosionRadius = 5f;
    public float damage = 100f;
    public float knockbackForce = 10f;
    public LayerMask targetLayer;

    private Health_Component healthComponent;

    [SerializeField] private GameObject[] explosionParticles; // Generator de partículas

    private void Start()
    {
        healthComponent = GetComponent<Health_Component>();
        if (healthComponent != null)
        {
            healthComponent.OnDamaged += () => StartCoroutine(DelayedExplosion());
        }
    }

    private IEnumerator DelayedExplosion()
    {
        yield return new WaitForSeconds(2f); //Time-slicing
        Kaboom();
    }

    private void Kaboom()
    {
        GameObject selectedEffect = GenerateRandomEffect(); // Generator
        if (selectedEffect != null)
        {
            Instantiate(selectedEffect, transform.position, Quaternion.identity);
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, targetLayer); //para detectar enemigos en el radio de la explosion
        List<Health_Component> healthTargets = new List<Health_Component>();

        foreach (var col in colliders)
        {
            var hc = col.GetComponent<Health_Component>();
            if (hc != null && hc != healthComponent)
            {
                healthTargets.Add(hc);
            }
        }

        var orderedTargets = healthTargets //OrderBy y ToList
            .OrderBy(x => Vector3.Distance(transform.position, x.transform.position))
            .ToList();

        var filteredTargets = orderedTargets //Where
            .Where(x => Vector3.Distance(transform.position, x.transform.position) < explosionRadius * 0.75f)
            .ToList();

        foreach (var enemy in filteredTargets) //Tupla
        {
            Vector3 dir = (enemy.transform.position - transform.position).normalized;
            var damageTuple = new Tuple<Vector3, float, float>(dir, damage, knockbackForce);

            enemy.SimpleDamage(damageTuple);
        }

        

        Destroy(this.gameObject);
    }

    private GameObject GenerateRandomEffect()
    {
        if (explosionParticles == null || explosionParticles.Length == 0)
            return null;

        int index = UnityEngine.Random.Range(0, explosionParticles.Length);
        return explosionParticles[index];
    }
}
