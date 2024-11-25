using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI energyCount;

    public GameObject playerState;

    public float currentEnergy, maxEnergy;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    void Update()
    {
        currentEnergy = playerState.GetComponent<PlayerStateSystem>().currentEnergy;
        maxEnergy = playerState.GetComponent<PlayerStateSystem>().maxEnergy;

        float fillValue = currentEnergy / maxEnergy;
        slider.value = fillValue;

        energyCount.text = currentEnergy + "/" + maxEnergy;
    }
}
