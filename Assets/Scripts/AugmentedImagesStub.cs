using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;

public class AugmentedImagesStub : MonoBehaviour
{
    GameController gameController;

    void Awake()
    {
        gameController = GetComponent<GameController>();
    }

    public void FireDetectedImage(int index)
    {
        GameController.MarkerData markerData = new GameController.MarkerData { name = gameController.allVideoData[index].name, imageIndex = index, anchorTransform = transform };
        gameController.OnMarkerDiscovered(markerData);
    }

    [ContextMenu("Fire Detected Image 0")]
    public void FireDetectedImage0()
    {
        FireDetectedImage(0);
    }

    [ContextMenu("Fire Detected Image 1")]
    public void FireDetectedImage1()
    {
        FireDetectedImage(1);
    }

    [ContextMenu("Fire Detected Image 2")]
    public void FireDetectedImage2()
    {
        FireDetectedImage(2);
    }

    [ContextMenu("Fire Detected Image 3")]
    public void FireDetectedImage3()
    {
        FireDetectedImage(3);
    }

}
