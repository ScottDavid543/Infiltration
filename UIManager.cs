using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{

    // -----------------------------------------------

    [SerializeField]
    Text m_BulletText;

    /*[SerializeField]
    Text m_GrenadeText;*/

    [SerializeField]
    Text health;

    [SerializeField]
    Image healthBar;

    Health playerHealth;

    PauseMenu pMenu;
    // --------------------------------------------------------------
    public void Awake()
    {

        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        if (healthBar)
        {
            healthBar.fillAmount = (playerHealth.m_Health) / playerHealth.startingHealth;
        }
    }
    public void SetAmmoText(int bulletCount, int grenadeCount)
    {
        if(m_BulletText)
        {
            m_BulletText.text = ""+bulletCount;
        }        
        
        /*if(m_GrenadeText)
        {
            m_GrenadeText.text = "Grenades: " + grenadeCount;
        }*/
    }
    public void Update()
    {
        if (health)
        {
            health.text = "" + playerHealth.m_Health;
        }
        if (healthBar)
        {
            healthBar.fillAmount = (playerHealth.m_Health) / playerHealth.startingHealth;
        }

    }
    
}
