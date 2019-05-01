using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LookAtMonitor : MonoBehaviour
{
    public Camera targetCamera;
    public float minAngle = 15;

    public UnityEvent LookingStarted;

    bool isLooking = false;

    void OnEnable()
    {
        isLooking = false;   
    }

    void OnDisable()
    {
        isLooking = false;        
    }

    void Update()
    {
        if (isLooking == true)
            return;

        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        Vector3 cameraToSelf = (transform.position - targetCamera.transform.position).normalized;
        Vector3 cameraForward = targetCamera.transform.forward;

        float cosAngle = Vector3.Dot(cameraToSelf, cameraForward);
        // Debug.Log("cosAngle = " + cosAngle + " cos(min) = " + Mathf.Cos(minAngle * Mathf.Deg2Rad));
        if (cosAngle > Mathf.Cos(minAngle * Mathf.Deg2Rad))
        {
            Debug.Log("invoking");
            LookingStarted.Invoke();
            isLooking = true;
        }
        
    }
}
