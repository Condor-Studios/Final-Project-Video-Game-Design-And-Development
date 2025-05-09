using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDumb : MonoBehaviour
{
    [SerializeField] private float BulletSpeed,damage;
    [SerializeField] private float Damage;
    [SerializeField] Collider _Collider;
    [SerializeField] private float _Health;

    [SerializeField] private GameObject _OwnerGameObject;
    [SerializeField] private PlayerContext _OwnerContext;


    private void Start()
    {
        _Collider = GetComponent<Collider>();
        _Collider.enabled = false;
        StartCoroutine(startDamage());
        Physics.IgnoreCollision(this._Collider,_OwnerContext.PlayerTransform.root.GetComponentInChildren<Collider>());
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
    public void SetOwner(GameObject Owner,PlayerContext Context)
    {
        _OwnerGameObject = Owner;
        _OwnerContext = Context;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.name == _OwnerGameObject.name) return;
        if (other.gameObject == _OwnerGameObject) return;
        if (other.GetComponent<PlayerContext>() == _OwnerContext) return;

        if(other.TryGetComponent(out Health_Component HPComp))
        {
            Vector3 dir = other.gameObject.transform.position - this.transform.position;

            DamageScore DamageDT = new DamageScore();
            DamageDT.Attacker = _OwnerContext;
            DamageDT.DamageAmount = damage;
            DamageDT.Victim = other.GetComponent<PlayerContext>();
            DamageDT.ElementalDamage = ElementType.Physical;
            DamageDT.FactionID = FactionID.LimboMonster1;
           
            if(HPComp.Context == null) { HPComp.SimpleDamage(damage);return; }
            HPComp.TakeDamageWithKnockback(dir.normalized, 5, DamageDT);
   
        }

        Destroy(this.gameObject);
    }
}

//
// este script fue creado por patricio malvasio 2/5/2025
//
// este script es un prototipo de bala, 
// reemplazar mas adelante por un sistema mejor
//