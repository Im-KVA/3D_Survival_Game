using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadSlot : MonoBehaviour
{
    public Button button;
    public TextMeshProUGUI buttonText;

    public int slotNumber;

    private void Awake()
    {
        button = GetComponent<Button>();
        buttonText = transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        button.onClick.AddListener(() =>
        {
            if (SaveAndLoadSystem.Instance.IsSlotEmpty(slotNumber) == false)
            {
                MainMenuManager.Instance.LoadedGameAtStart(slotNumber);
                SaveAndLoadSystem.Instance.DeselectButton();
            }
        });
    }

    private void Update()
    {
        if (SaveAndLoadSystem.Instance.IsSlotEmpty(slotNumber))
        {
            buttonText.text = "";
        }
        else
        {
            buttonText.text = PlayerPrefs.GetString("Slot" + slotNumber + "Description");
        }
    }


}
