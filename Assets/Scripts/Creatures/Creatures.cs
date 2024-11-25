using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creatures : MonoBehaviour
{
    public string enemyName;
    public bool playerInRange;
    public bool isDead;

    [SerializeField] int currentHealth;
    [SerializeField] int maxHealth;

    [SerializeField] ParticleSystem attackedParticle;

    private Animator animator;

    private void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    public void TakeDmg(int dmg)
    {
        if (isDead == false)
        {
            currentHealth -= dmg;
            attackedParticle.Play();

            if (currentHealth <= 0)
            {
                isDead = true;
                animator.SetTrigger("isDead");
                SoundManageSystem.Instance.PlaySound(SoundManageSystem.Instance.enemyDeathSound);
                GetComponent<CreaturesBehave>().enabled = false;
            }
            else
            {
                animator.SetTrigger("takeDmg");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
