using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    public Button button;
    public TextMeshProUGUI buttonText;
    public int slotNumber;

    public GameObject alertUI;
    Button yesBtn;
    Button noBtn;

    private void Awake()
    {
        button = GetComponent<Button>();
        buttonText = transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();

        yesBtn = alertUI.transform.Find("YesBtn").GetComponent<Button>();
        noBtn = alertUI.transform.Find("NoBtn").GetComponent<Button>();
    }

    public void Start()
    {
        button.onClick.AddListener(() =>
        {
            if (SaveAndLoadSystem.Instance.IsSlotEmpty(slotNumber))
            {
                SaveGameConfirmed();
            }
            else
            {
                DisplayOverrideWarning();
            }
        });
    }

    public void DisplayOverrideWarning()
    {
        alertUI.SetActive(true);

        yesBtn.onClick.AddListener(() =>
        {
            SaveGameConfirmed();
            alertUI.SetActive(false);
        });

        noBtn.onClick.AddListener(() =>
        {
            alertUI.SetActive(false);
        });
    }

    private void SaveGameConfirmed()
    {
        MainMenuManager.Instance.SaveGame(slotNumber);

        DateTime tempDT = DateTime.Now;
        string time = tempDT.ToString("dd-MM-yyyy HH:mm");

        string description = "Saved Game " + slotNumber + " | " + time;
        buttonText.text = description;

        PlayerPrefs.SetString("Slot" + slotNumber + "Description", description);

        SaveAndLoadSystem.Instance.DeselectButton();
    }

    private void Update()
    {
        if (SaveAndLoadSystem.Instance.IsSlotEmpty(slotNumber))
        {
            buttonText.text = "Empty";
        }
        else
        {
            buttonText.text = PlayerPrefs.GetString("Slot" + slotNumber + "Description");
        }
    }
}
