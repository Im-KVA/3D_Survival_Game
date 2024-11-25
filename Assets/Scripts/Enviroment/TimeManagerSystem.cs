using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeManagerSystem : MonoBehaviour
{
    public static TimeManagerSystem Instance { get; set; }

    [SerializeField] private Image smallDawn;
    [SerializeField] private Image smallDay;
    [SerializeField] private Image smallNoon;
    [SerializeField] private Image smallNight;

    [SerializeField] private Image bigDawn;
    [SerializeField] private Image bigDay;
    [SerializeField] private Image bigNoon;
    [SerializeField] private Image bigNight;

    [SerializeField] private TextMeshProUGUI textDayUI;

    [SerializeField]
    private int dayInGame = 1;

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
        Dawn();
    }

    public void TriggerNextDay()
    {
        dayInGame += 1;
        textDayUI.text = "Day " + dayInGame.ToString();
    }

    public void Dawn()
    {
        smallDawn.gameObject.SetActive(true);
        smallDay.gameObject.SetActive(false);
        smallNoon.gameObject.SetActive(false);
        smallNight.gameObject.SetActive(false);

        bigDawn.gameObject.SetActive(true);
        bigDay.gameObject.SetActive(false);
        bigNoon.gameObject.SetActive(false);
        bigNight.gameObject.SetActive(false);
    }
    public void Day()
    {
        smallDawn.gameObject.SetActive(false);
        smallDay.gameObject.SetActive(true);
        smallNoon.gameObject.SetActive(false);
        smallNight.gameObject.SetActive(false);

        bigDawn.gameObject.SetActive(false);
        bigDay.gameObject.SetActive(true);
        bigNoon.gameObject.SetActive(false);
        bigNight.gameObject.SetActive(false);
    }

    public void Noon()
    {
        smallDawn.gameObject.SetActive(false);
        smallDay.gameObject.SetActive(false);
        smallNoon.gameObject.SetActive(true);
        smallNight.gameObject.SetActive(false);

        bigDawn.gameObject.SetActive(false);
        bigDay.gameObject.SetActive(false);
        bigNoon.gameObject.SetActive(true);
        bigNight.gameObject.SetActive(false);
    }

    public void Night()
    {
        smallDawn.gameObject.SetActive(false);
        smallDay.gameObject.SetActive(false);
        smallNoon.gameObject.SetActive(false);
        smallNight.gameObject.SetActive(true);

        bigDawn.gameObject.SetActive(false);
        bigDay.gameObject.SetActive(false);
        bigNoon.gameObject.SetActive(false);
        bigNight.gameObject.SetActive(true);
    }
}
