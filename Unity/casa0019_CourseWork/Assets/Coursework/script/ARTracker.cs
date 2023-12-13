using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class ARImageTracker : MonoBehaviour
{
    public ARTrackedImageManager trackedImageManager;
    public GameObject background;


    void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

 void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
 {
    foreach (var trackedImage in eventArgs.added)
        {
            // 处理图像被检测到时的逻辑
            HandleImageTracking(trackedImage);
        }
 }

    void HandleImageTracking(ARTrackedImage trackedImage)
    {
        if (trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
        {
            GameObject prefab = null;

            if (prefab != null)
            {
                background.SetActive(false);
                
            }
        }
    }

}
