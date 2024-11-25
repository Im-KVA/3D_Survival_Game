using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuildingsData
{
    //Building
    public float[] positions;
    public float[] rotations;
    public string buildingName;

    public BuildingsData(string temp_buildingName, float[] temp_positions, float[] temp_rotations)
    {
        buildingName = temp_buildingName;
        positions = temp_positions;
        rotations = temp_rotations;
    }
}
