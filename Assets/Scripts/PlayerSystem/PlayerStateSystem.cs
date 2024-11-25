using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateSystem : MonoBehaviour
{
    public static PlayerStateSystem Instance { get; set; }

    public GameObject player;
    public GameObject playerInforUI;

    //Player health
    public float currentHealth;
    public float maxHealth;

    //Player Energy
    public float currentEnergy;
    public float maxEnergy;

    float distanceTravelled = 0;
    Vector3 lastPosition;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    private void Start()
    {
        currentHealth = maxHealth;
        currentEnergy = maxEnergy;
    }
    void Update()
    {
        distanceTravelled += Vector3.Distance(player.transform.position, lastPosition);
        lastPosition = player.transform.position;

        if (distanceTravelled >= 5)
        {
            //SoundManageSystem.Instance.PlaySound(SoundManageSystem.Instance.walkingSound);

            distanceTravelled = 0;
            currentEnergy -= 1;
        }
    }

    public void setHealth(float newHealth)
    {
        currentHealth = newHealth;
    }

    public void setEnergy(float newEnergy)
    {
        currentEnergy = newEnergy;
    }
}
