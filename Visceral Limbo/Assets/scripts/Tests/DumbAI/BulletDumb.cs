using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDumb : MonoBehaviour
{
    [SerializeField] private float BulletSpeed,damage;
    [SerializeField] private float Damage;
    [SerializeField] Collider _Collider;
    [SerializeField] private float _Health;

    [SerializeField] private GameObject _Owner;


    private void Start()
    {
        _Collider = GetComponent<Collider>();
        _Collider.enabled = false;
        StartCoroutine(startDamage());
    }

    IEnumerator startDamage()
    {
        yield return new WaitForSeconds(0.01f);
        _Collider.enabled = true;

    }

    private void Update()
    {
        _Health -= Time.deltaTime; 
        this.transform.position += this.transform.forward * BulletSpeed * Time.deltaTime;

        if(_Health < 0)
        {
            Destroy(this.gameObject);
        }

    }
    public void SetOwner(GameObject Owner)
    {
        _Owner = Owner;
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Health_Component HPComp))
        {
            Vector3 dir = other.gameObject.transform.position - this.transform.position;

            DamageScore DamageDT = new DamageScore();
            DamageDT.Attacker = _Owner;
            DamageDT.DamageAmount = damage;
            DamageDT.Victim = other.gameObject;
            DamageDT.ElementalDamage = ElementType.Physical;

            
            HPComp.TakeDamageWithKnockback(dir.normalized,5,DamageDT);
        }
    }
}
