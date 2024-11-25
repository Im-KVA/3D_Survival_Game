using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveAndLoadSystem : MonoBehaviour
{
    public static SaveAndLoadSystem Instance { get; set; }

    [SerializeField] private bool isSavingToJson;
    private string fileName = "SaveGame";

    public GameObject existBuildings;
    public GameObject ghostBank;
    
    public GameObject placeables;

    //For testing
    private string jsonPathProject;

    //End-user
    private string jsonPathPersistant;
    private string binaryPath;

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
        jsonPathProject = Application.dataPath + Path.AltDirectorySeparatorChar;
        jsonPathPersistant = Application.persistentDataPath + Path.AltDirectorySeparatorChar;
        binaryPath = Application.persistentDataPath + Path.AltDirectorySeparatorChar;
    }

    public bool IsSavingToJson() { return isSavingToJson; }

    #region ||--To Binary And Back--||
    public void SaveGameDataToBinaryFile(AllGameData gameData, int slotNumber)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        FileStream stream = new FileStream(binaryPath + fileName + slotNumber + ".bin", FileMode.Create);

        formatter.Serialize(stream, gameData);
        stream.Close();

        print("Data saved to" + binaryPath + fileName + slotNumber + ".bin");
    }

    public AllGameData LoadGameDataFromBinaryFile(int slotNumber)
    {
        if (File.Exists(binaryPath + fileName + slotNumber + ".json"))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(binaryPath + fileName + slotNumber + ".bin", FileMode.Open);

            AllGameData data = formatter.Deserialize(stream) as AllGameData;
            stream.Close();

            print("Data loaded from" + binaryPath + fileName + slotNumber + ".json");

            return data;
        }
        else
        {
            return null;
        }
    }
    #endregion

    #region ||--To Json And Back--||
    public void SaveGameDataToJsonFile(AllGameData gameData, int slotNumber)
    {
        string json = JsonUtility.ToJson(gameData);

        string ecryptedJson = EncryptionDecryption(json);

        using (StreamWriter writer = new StreamWriter(jsonPathProject + fileName + slotNumber + ".json"))    
        {
            writer.Write(json); //ecryptedJson);
            print("Saved game to Json file at: " + jsonPathProject + fileName + slotNumber + ".json");
        };
    }

    public AllGameData LoadGameDataFromJsonFile(int slotNumber)
    {
        using (StreamReader reader = new StreamReader(jsonPathProject + fileName + slotNumber + ".json"))
        {
            string json = reader.ReadToEnd();

            string decryptedJson = EncryptionDecryption(json);

            AllGameData data = JsonUtility.FromJson<AllGameData>(json); // decryptedJson);

            print("Data loaded from" + jsonPathProject + fileName + slotNumber + ".json");

            return data;
        }
    }
    #endregion

    #region ||--Encryption And Decryption--||
    public string EncryptionDecryption(string jsonString)
    {
        string keyword = "KVA_2003_:3";

        string result = "";

        for (int i = 0; i < jsonString.Length; i++)
        {
            result += (char)(jsonString[i] ^ keyword[i % keyword.Length]);
        }

        return result;  
    }
    #endregion

    #region ||--Check SaveSlot/LoadSlot--||
    public bool DoesFileExists(int slotNumber)
    {
        if (isSavingToJson)
        {
            if(System.IO.File.Exists(jsonPathProject + fileName + slotNumber + ".json"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (System.IO.File.Exists(binaryPath))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public bool IsSlotEmpty(int slotNumber)
    {
        if (DoesFileExists(slotNumber))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void DeselectButton()
    {
        GameObject myEventSystem = GameObject.Find("EventSystem");
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
    }
    #endregion

    #region ||--PlayerData--||
    public PlayerData GetPlayerData()
    {
        float[] playerStats = new float[2];
        playerStats[0] = PlayerStateSystem.Instance.currentHealth;
        playerStats[1] = PlayerStateSystem.Instance.currentEnergy;

        float[] playerPostionAndRotation = new float[7];
        playerPostionAndRotation[0] = PlayerStateSystem.Instance.player.transform.position.x;
        playerPostionAndRotation[1] = PlayerStateSystem.Instance.player.transform.position.y;
        playerPostionAndRotation[2] = PlayerStateSystem.Instance.player.transform.position.z;

        Quaternion rotationPlayer = PlayerStateSystem.Instance.player.transform.rotation;
        playerPostionAndRotation[3] = rotationPlayer.x;
        playerPostionAndRotation[4] = rotationPlayer.y;
        playerPostionAndRotation[5] = rotationPlayer.z;
        playerPostionAndRotation[6] = rotationPlayer.w;

        string[] inventoryItems = InventorySystem.Instance.itemList.ToArray();

        List<string> quickSlotItems = new List<string>();

        foreach(GameObject slot in QuickSlotSystem.Instance.quickSlotsList)
        {
            if (slot.transform.childCount != 0)
            {
                string name = slot.transform.GetChild(0).name;
                string temp = "(Clone)";
                string resultName = name.Replace(temp, "");
                quickSlotItems.Add(resultName);
            }
        }

        string[] quickSlotContent = quickSlotItems.ToArray();

        return new PlayerData(playerStats, playerPostionAndRotation, inventoryItems, quickSlotContent);
    }

    public void SetPlayerData(PlayerData playerData)
    {
        //Player Stat
        PlayerStateSystem.Instance.currentHealth = playerData.playerStats[0];
        PlayerStateSystem.Instance.currentEnergy = playerData.playerStats[1];

        //Position
        Vector3 loadedPosition;
        loadedPosition.x = playerData.playerPositionAndRotation[0];
        loadedPosition.y = playerData.playerPositionAndRotation[1] + 10;
        loadedPosition.z = playerData.playerPositionAndRotation[2];

        //Debug.Log(loadedPosition.x + ", " + loadedPosition.y + ", " + loadedPosition.z);

        PlayerStateSystem.Instance.player.transform.position = loadedPosition;

        //Rotation
        Quaternion loadedRotation;
        loadedRotation.x = playerData.playerPositionAndRotation[3];
        loadedRotation.y = playerData.playerPositionAndRotation[4];
        loadedRotation.z = playerData.playerPositionAndRotation[5];
        loadedRotation.w = playerData.playerPositionAndRotation[6];

        //Debug.Log(loadedRotation.x + ", " + loadedRotation.y + ", " + loadedRotation.z + ", " + loadedRotation.w);
        //Debug.Log("Rotation after load: " + PlayerStateSystem.Instance.player.transform.rotation);

        PlayerStateSystem.Instance.player.transform.rotation = loadedRotation;

        //Inventory
        InventorySystem.Instance.itemList.Clear();

        for (var i = InventorySystem.Instance.slotList.Count - 1; i >= 0; i--)
        {
            if (InventorySystem.Instance.slotList[i].transform.childCount > 0)
            {
                Destroy(InventorySystem.Instance.slotList[i].transform.GetChild(0).gameObject);
            }
        }

        InventorySystem.Instance.RecalculateList();

        foreach (string item in playerData.inventoryItems)
        {
            InventorySystem.Instance.AddToInventory(item);
        }

        //Quickslot Items
        for (var i = QuickSlotSystem.Instance.quickSlotsList.Count - 1; i >= 0; i--)
        {
            if (QuickSlotSystem.Instance.quickSlotsList[i].transform.childCount > 0)
            {
                Destroy(QuickSlotSystem.Instance.quickSlotsList[i].transform.GetChild(0).gameObject);
            }
        }

        foreach (string item in playerData.quickSlotItems)
        {
            GameObject avaiableSlot = QuickSlotSystem.Instance.FindNextEmptySlot();

            var itemToAdd = Instantiate(Resources.Load<GameObject>("Items/" + item));
            itemToAdd.transform.SetParent(avaiableSlot.transform, false);

            InventorySystem.Instance.RecalculateList();
        }
    }
    #endregion

    #region ||--BuildData--||
    public List<BuildingsData> GetBuildingsData(GameObject _existBuildings)
    {
        List<BuildingsData> tempBuildingsData = new List<BuildingsData>();

        foreach (Transform buildings in _existBuildings.transform)
        {
            //Building
            float[] temp_positions = new float[3];
            temp_positions[0] = buildings.transform.position.x;
            temp_positions[1] = buildings.transform.position.y;
            temp_positions[2] = buildings.transform.position.z;

            float[] temp_rotations = new float[4];
            Quaternion buildingRotation = buildings.transform.rotation;
            temp_rotations[0] = buildingRotation.x;
            temp_rotations[1] = buildingRotation.y;
            temp_rotations[2] = buildingRotation.z;
            temp_rotations[3] = buildingRotation.w;

            string temp_name = buildings.name;

            BuildingsData tempData = new BuildingsData(temp_name, temp_positions, temp_rotations);
            
            tempBuildingsData.Add(tempData);
        }

        return tempBuildingsData;
    }

    public void SetBuildingsData(List<BuildingsData> buildingsDatas)
    {
        foreach (GameObject buildToBeDelete in existBuildings.transform)
        {
            DestroyImmediate(buildToBeDelete);
        }
        ConstructionManager.Instance.allGhostsInExistence.Clear();
        ConstructionManager.Instance.placedObjectPositions.Clear();

        List<GameObject> temp_ghostsList = new List<GameObject>();

        foreach (BuildingsData data in buildingsDatas)
        {
            if (data != null)
            {
                GameObject savedBuild = Instantiate(Resources.Load<GameObject>("Buildable/" + data.buildingName));
                savedBuild.name = data.buildingName;
                savedBuild.transform.position = new Vector3(data.positions[0], data.positions[1], data.positions[2]);
                savedBuild.transform.rotation = new Quaternion(data.rotations[0], data.rotations[1], data.rotations[2], data.rotations[3]);

                int childCount = savedBuild.transform.childCount;
                for (int i = childCount - 1; i >= 0; i--)
                {
                    Transform ghost = savedBuild.transform.GetChild(i);

                    //Debug.Log(ghost);
                    
                    ghost.SetParent(ghostBank.transform, true);
                    temp_ghostsList.Add(ghost.gameObject);
                }
                //savedBuild.transform.DetachChildren();
                savedBuild.gameObject.transform.SetParent(existBuildings.transform);
                ConstructionManager.Instance.placedObjectPositions.Add(savedBuild.transform.position);
            }
        }
        ConstructionManager.Instance.allGhostsInExistence = temp_ghostsList;
        ConstructionManager.Instance.PerformGhostDeletionScan();
    }
    #endregion

    #region ||--StorageBoxData--||
    public List<StorageBoxData> GetStorageBoxData(GameObject placeables)
    {
        List<StorageBoxData> tempList = new List<StorageBoxData>();

        foreach(Transform box in placeables.transform)
        {
            float[] temp_position = new float[3];
            temp_position[0] = box.gameObject.transform.position.x; 
            temp_position[1] = box.gameObject.transform.position.y; 
            temp_position[2] = box.gameObject.transform.position.z;

            float[] temp_rotation = new float[4];
            Quaternion boxRotation = box.transform.rotation;
            temp_rotation[0] = boxRotation.x;
            temp_rotation[1] = boxRotation.y;
            temp_rotation[2] = boxRotation.z;
            temp_rotation[3] = boxRotation.w;

            string temp_Name = box.name;

            List<string> temp_itemNameList = box.GetComponent<StorageBox>().items;

            StorageBoxData temp_boxData = new StorageBoxData(temp_Name, temp_position, temp_rotation, temp_itemNameList);

            tempList.Add(temp_boxData);
        }
        return tempList;
    }

    public void SetStorageBoxData(List<StorageBoxData> boxsDataList)
    {
        foreach (GameObject boxToBeDelete in placeables.transform)
        {
            DestroyImmediate(boxToBeDelete);
        }

        foreach (StorageBoxData data in boxsDataList)
        {
            if (data != null)
            {
                GameObject savedBox = Instantiate(Resources.Load<GameObject>("Buildable/" + data.boxName));
                savedBox.name = data.boxName;
                savedBox.transform.position = new Vector3(data.position[0], data.position[1], data.position[2]);
                savedBox.transform.rotation = new Quaternion(data.rotation[0], data.rotation[1], data.rotation[2], data.rotation[3]);

                savedBox.gameObject.GetComponent<StorageBox>().items = data.items;

                savedBox.gameObject.transform.SetParent(placeables.transform, true);
                savedBox.GetComponent<Outline>().enabled = false;
                savedBox.GetComponent<PlaceableItem>().enabled = false;
            }
        }
        
    }
    #endregion

    #region ||--WorldData--||
    public WorldData GetWorldData()
    {
        return new WorldData(12345);
    }
    public void SetWorldData(WorldData worldData)
    {
        //Debug.Log(worldData.ToString());
        int seed = worldData.seed;

        WorldMaker.Instance.WorldBuild(seed);
    }
    #endregion
}
