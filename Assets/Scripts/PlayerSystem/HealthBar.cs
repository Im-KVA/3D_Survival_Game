using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI healthCount;

    public GameObject playerState;

    public float currentHealth, maxHealth;

    private void Awake()
    {
        slider = GetComponent<Slider>();    
    }

    void Update()
    {
        currentHealth = playerState.GetComponent<PlayerStateSystem>().currentHealth;
        maxHealth = playerState.GetComponent<PlayerStateSystem>().maxHealth;

        float fillValue = currentHealth/maxHealth;
        slider.value = fillValue;

        healthCount.text = currentHealth + "/" + maxHealth;
    }
}
