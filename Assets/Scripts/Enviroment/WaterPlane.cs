using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPlane : MonoBehaviour
{
    public Transform cameraTransform;

    void Update()
    {
        Vector3 newPosition = cameraTransform.position;
        newPosition.y = transform.position.y; 
        transform.position = newPosition;
    }
}
