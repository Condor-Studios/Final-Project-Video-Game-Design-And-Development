using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEnterTrigger : MonoBehaviour
{

    [SerializeField]GameObject SolidCollider;
    RoomSpawnerManager roomSpawnerManager;
    [SerializeField] bool IsSolid;


    void Start()
    {
        roomSpawnerManager= GetComponentInParent<RoomSpawnerManager>();
        SolidCollider.SetActive(false);

    }

    private void OnTriggerExit(Collider other)
    {
        var Contex = other.GetComponentInParent<PlayerContext>();

        if (Contex.faction == FactionID.Player)
        {
                roomSpawnerManager.AssignPlayerContext(Contex);
                roomSpawnerManager.NotifyMinionDeath();
                
        }

    }

    public void SetSolidState(bool isSolid)
    {
        SolidCollider.SetActive(isSolid);
    }

}
