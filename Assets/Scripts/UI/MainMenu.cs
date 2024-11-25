using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button newGameBtn;
    [SerializeField] private GameObject loadingScreen;

    // Start is called before the first frame update
    void Start()
    {
        newGameBtn.onClick.AddListener(() =>
        {
            //WorldMaker.Instance.WorldBuild(12345);
            ActivateLoadingScreen();
            SceneManager.LoadScene("MainScenes");
            DeactivateLoadingScreen();
        });
    }
    #region ||--Save Game--||

    public void SelectSavingType(AllGameData gameData, int slotNumber)
    {
        if (SaveAndLoadSystem.Instance.IsSavingToJson())
        {
            SaveAndLoadSystem.Instance.SaveGameDataToJsonFile(gameData, slotNumber);
        }
        else
        {
            SaveAndLoadSystem.Instance.SaveGameDataToBinaryFile(gameData, slotNumber);
        }
    }
    #endregion

    #region ||--Load Game--||
    public AllGameData SelectLoadingType(int slotNumber)
    {
        if (SaveAndLoadSystem.Instance.IsSavingToJson())
        {
            AllGameData gameData = SaveAndLoadSystem.Instance.LoadGameDataFromJsonFile(slotNumber);
            return gameData;
        }
        else
        {
            AllGameData gameData = SaveAndLoadSystem.Instance.LoadGameDataFromBinaryFile(slotNumber);
            return gameData;
        }
    }
    #endregion

    #region ||--Settings--||
    [System.Serializable]
    public class VolumeSettings
    {
        public float music;
        public float effects;
        public float master;
    }

    public void SaveVolumeSettings(float tempMusic, float tempEffects, float tempMaster)
    {
        VolumeSettings volumeSettings = new VolumeSettings()
        {
            music = tempMusic,
            effects = tempEffects,
            master = tempMaster
        };

        PlayerPrefs.SetString("Volume", JsonUtility.ToJson(volumeSettings));
        PlayerPrefs.Save();
    }

    public VolumeSettings LoadVolumeSettings()
    {
        return JsonUtility.FromJson<VolumeSettings>(PlayerPrefs.GetString("Volume"));
    }
    #endregion

    #region ||--Others--||
    public void ActivateLoadingScreen()
    {
        loadingScreen.gameObject.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void DeactivateLoadingScreen()
    {
        loadingScreen.gameObject.SetActive(false);
    }
    #endregion
}
