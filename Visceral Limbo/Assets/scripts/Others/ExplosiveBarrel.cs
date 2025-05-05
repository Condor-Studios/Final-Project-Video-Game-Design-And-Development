using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Unity.Collections;

public class ExplosiveBarrel : MonoBehaviour
{
    public float explosionRadius;
    public float damage;
    public float knockbackForce;
    public LayerMask targetLayer;

    private Health_Component healthComponent;

    [SerializeField] private GameObject[] explosionParticles; // Generator de part�culas

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
        yield return new WaitForSeconds(1f); // Hecho por Lucas - Time-slicing
        Kaboom();
    }

    private void Kaboom()
    {
        GameObject selectedEffect = GenerateRandomEffect(); // Hecho por Lucas - Generator
        if (selectedEffect != null)
        {
            GameObject effectInstance = Instantiate(selectedEffect, transform.position, Quaternion.identity);

              var psInChildren = effectInstance.GetComponentInChildren<ParticleSystem>();
              if (psInChildren != null)
              {
                 psInChildren.Play();
              }
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

        var orderedTargets = healthTargets //Hecho por Lucas - OrderBy y ToList
            .OrderBy(x => Vector3.Distance(transform.position, x.transform.position))
            .ToList();

        var filteredTargets = orderedTargets //Hecho por Lucas - Where
            .Where(x => Vector3.Distance(transform.position, x.transform.position) < explosionRadius * 0.75f)
            .ToList();

        foreach (var enemy in filteredTargets) //Hecho por Lucas - Tupla
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

        var index = new { Index = UnityEngine.Random.Range(0, explosionParticles.Length) }; //Hecho por Lucas - Tipo Anonimo
        return explosionParticles[index.Index];
    }
}


//
// Script hecho por Lucas Torres
// se encarga de definir el funcionamiento del barril explosivo
//