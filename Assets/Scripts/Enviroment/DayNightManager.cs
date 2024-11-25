using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DayNightManager : MonoBehaviour
{
    public Light directionalLight;

    public float dayDurationInSeconds = 24.0f;
    public int currentHour;
    float currentTimeOfDay = 0.35f;
    float blendedValue = 0.0f;

    bool lockNextday = false;

    public TextMeshProUGUI timeUI;

    public List<SkyboxTimeMapping> timeMappings;

    void Update()
    {
        currentTimeOfDay += Time.deltaTime / dayDurationInSeconds;
        currentTimeOfDay %= 1;

        currentHour = Mathf.FloorToInt(currentTimeOfDay * 24);

        timeUI.text = $"{currentHour}:00";

        directionalLight.transform.rotation = Quaternion.Euler(new Vector3((currentTimeOfDay * 360) -90, 152, 0));

        UpdateTimeUI();
        UpdateSkybox();
    }
    private void UpdateTimeUI()
    {
        switch (currentHour)
        {
            case 4:
                TimeManagerSystem.Instance.Dawn();
                break;
            case 10:
                TimeManagerSystem.Instance.Day();
                break;
            case 14:
                TimeManagerSystem.Instance.Noon();
                break;
            case 18:
                TimeManagerSystem.Instance.Night();
                break;
            default:
                break;
        }
    }
    private void UpdateSkybox()
    {
        Material currentSkybox = null;
        foreach (SkyboxTimeMapping mapping in timeMappings)
        {
            if (currentHour == mapping.hour)
            {
                currentSkybox = mapping.skyboxMaterial;

                if (currentSkybox.shader != null)
                {
                    if (currentSkybox.shader.name == "Custom/SkyboxTransition")
                    {
                        blendedValue += Time.deltaTime;
                        blendedValue = Mathf.Clamp01(blendedValue);

                        currentSkybox.SetFloat("_TransitionFactor", blendedValue);
                    }
                    else
                    {
                        blendedValue = 0;
                    }
                }
                break;
            }
        }

        if (currentHour == 0 && !lockNextday)
        {
            TimeManagerSystem.Instance.TriggerNextDay();
            lockNextday = true;
        }

        if (currentHour != 0)
        {
            lockNextday = false;
        }

        if (currentSkybox != null)
        {
            RenderSettings.skybox = currentSkybox;
        }
    }
}

[System.Serializable]
public class SkyboxTimeMapping
{
    public string phaseName;
    public int hour;
    public Material skyboxMaterial;
}