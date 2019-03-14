using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    // The total health of this unit
    public float startingHealth;
    public float m_Health;
    Animator animator;
    public bool isDead = false;

    PlayerProgressController progController;
    
    public void Awake()
    {
        progController = FindObjectOfType<PlayerProgressController>();

        if (this.CompareTag("Player"))
        {
            startingHealth = progController.playerHealth;
        }

        m_Health = startingHealth;
        animator = this.GetComponent<Animator>();
    }
    private void Update()
    {
        if(m_Health <= 0)
        {
            m_Health = 0;
        }
    }

    public void DoDamage(int damage)
    {
        m_Health -= damage;
        if (m_Health <= 0)
        {
            if (this.CompareTag("Throwable")) { this.gameObject.SetActive(false); }
            else
            {
                isDead = true;
                animator.SetBool("IsDead", true);
            }
        }
    }
    public bool IsAlive()
    {
        return m_Health > 0;
    }
    public void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("PunchCollider"))
        {
            DoDamage(100);
        }
    }
}
