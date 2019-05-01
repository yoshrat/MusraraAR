using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;
using UnityEngine.Events;

public class MarkerDetector : MonoBehaviour
{
    [System.Serializable]
    public class ImageEvent : UnityEvent<AugmentedImage> {}

    public ImageEvent ImageDetected;
    public ImageEvent ImageLost;

    private List<AugmentedImage> tempAugmentedImages = new List<AugmentedImage>();
    private Dictionary<int, bool> DBIndexToIsFound = new Dictionary<int, bool>();

    void OnEnable()
    {
        
    }

    void OnDisable()
    {
        
    }

    void Update()
    {
        if (Session.Status != SessionStatus.Tracking)
        {
            return;
        }

        Session.GetTrackables<AugmentedImage>(tempAugmentedImages, TrackableQueryFilter.Updated);
        foreach (AugmentedImage image in tempAugmentedImages)
        {
            DBIndexToIsFound.TryGetValue(image.DatabaseIndex, out bool isFound);
            if (image.TrackingState == TrackingState.Tracking && isFound == false)
            {
                DBIndexToIsFound[image.DatabaseIndex] = true;
                ImageDetected.Invoke(image);
            }
            else if (image.TrackingState == TrackingState.Stopped && isFound == true)
            {
                DBIndexToIsFound[image.DatabaseIndex] = false;
                ImageLost.Invoke(image);
            }
        }

    }

    public void SetImageDetection(int imageIndex, bool isDetected)
    {
        Debug.Log("SetImageDetection. imageIndex = " + imageIndex + " is detected = " + isDetected);
        DBIndexToIsFound[imageIndex] = isDetected;
    }
}
