using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance { get; set; }

    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject loadMenuUI;
    [SerializeField] private GameObject saveMenuUI;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject saveGameBtn;
    [SerializeField] private GameObject loadGameBtn;
    [SerializeField] private GameObject backtoMainMenuBtn;
    [SerializeField] private GameObject newGameBtnObject;
    [SerializeField] private Button MainMenuBtn;
    [SerializeField] private Button newGameBtn;

    public bool isOpen;

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
        
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        newGameBtn.onClick.AddListener(() =>
        {
            //WorldMaker.Instance.WorldBuild(12345);
            ActivateLoadingScreen();
            saveMenuUI.SetActive(false);
            StartCoroutine(DelayedNewGame());
        });

        MainMenuBtn.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("MainMenu");

            saveMenuUI.SetActive(false);

            foreach (Transform child in SaveAndLoadSystem.Instance.ghostBank.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in SaveAndLoadSystem.Instance.existBuildings.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in SaveAndLoadSystem.Instance.placeables.transform)
            {
                Destroy(child.gameObject);
            }
        });
    }

    private void Update()
    {
        //Debug.Log("Rotation after load: " + PlayerStateSystem.Instance.player.transform.rotation);

        if(SceneManager.GetActiveScene().name == "MainScenes")
        {
            newGameBtnObject.SetActive(false);
            loadGameBtn.SetActive(false);
            saveGameBtn.SetActive(true);
            backtoMainMenuBtn.SetActive(true);
        }
        else
        {
            newGameBtnObject.SetActive(true);
            loadGameBtn.SetActive(true);
            saveGameBtn.SetActive(false);
            backtoMainMenuBtn.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !isOpen)
        {
            pauseMenuUI.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isOpen = true;

            CraftingSystem.Instance.craftingScreen.SetActive(false);
            InventorySystem.Instance.inventoryScreenUI.SetActive(false);

            PlayerStateSystem.Instance.playerInforUI.SetActive(false);

            if (StorageSystem.Instance.storageUIOpen)
            {
                StorageSystem.Instance.CloseBox();
            }

            ActionManager.Instance.DisableSelection();
            ActionManager.Instance.GetComponent<ActionManager>().enabled = false;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && isOpen)
        {
            pauseMenuUI.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isOpen = false;

            ActionManager.Instance.EnableSelection();
            ActionManager.Instance.GetComponent<ActionManager>().enabled = true;
        }
    }

    //Call to MainMenu to save game
    public void SaveGame(int slotNumber)
    {
        AllGameData data = new AllGameData();

        //PlayerData
        data.playerData = SaveAndLoadSystem.Instance.GetPlayerData();

        //WorldData
        data.worldData = SaveAndLoadSystem.Instance.GetWorldData();

        //BuildingData
        data.savedBuildingsList = SaveAndLoadSystem.Instance.GetBuildingsData(SaveAndLoadSystem.Instance.existBuildings);

        //Boxs Data
        data.storageBoxDataList = SaveAndLoadSystem.Instance.GetStorageBoxData(SaveAndLoadSystem.Instance.placeables);

        SelectSavingType(data, slotNumber);
    }

    //Call to MainMenu to load game
    public void LoadedGameAtStart(int slotNumber)
    {
        pauseMenuUI.gameObject.SetActive(false);
        loadMenuUI.gameObject.SetActive(false);

        SceneManager.LoadScene("MainScenes");
        ActivateLoadingScreen();

        StartCoroutine(DelayedLoading(slotNumber));
    }

    public void LoadGame(int slotNumber)
    {
        //Player Data
        SaveAndLoadSystem.Instance.SetPlayerData(SelectLoadingType(slotNumber).playerData);

        //World Data
        SaveAndLoadSystem.Instance.SetWorldData(SelectLoadingType(slotNumber).worldData);

        //Building Data
        SaveAndLoadSystem.Instance.SetBuildingsData(SelectLoadingType(slotNumber).savedBuildingsList);

        //Boxs Data
        SaveAndLoadSystem.Instance.SetStorageBoxData(SelectLoadingType(slotNumber).storageBoxDataList);

        DeactivateLoadingScreen();
    }

    private IEnumerator DelayedLoading(int slotNumber)
    {
        yield return new WaitForSeconds(2f);
        LoadGame(slotNumber);
    }
    private IEnumerator DelayedNewGame()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("MainScenes");
        DeactivateLoadingScreen();
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

