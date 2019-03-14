using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoCrateLogic : MonoBehaviour
{
    [SerializeField]
    int m_BulletAmmo;

    [SerializeField]
    int m_GrenadeAmmo = 10;

    PlayerProgressController progController;

    private void Awake()
    {
        progController = FindObjectOfType<PlayerProgressController>();
        m_BulletAmmo = progController.ammoPickupAmount;
    }
    void OnTriggerEnter(Collider other)
    {
        GunLogic gunLogic = other.GetComponentInChildren<GunLogic>();
        if(gunLogic)
        {
            gunLogic.AddAmmo(m_BulletAmmo, m_GrenadeAmmo);
            Destroy(gameObject);
        }
    }
}
