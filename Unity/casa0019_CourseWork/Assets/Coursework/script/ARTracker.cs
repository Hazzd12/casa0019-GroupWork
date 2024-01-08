using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;
using UnityEngine.UI;

public class ARImageTracker : MonoBehaviour
{
    public ARTrackedImageManager trackedImageManager;
    public GameObject prefab;
    public GameObject background;
    public GameObject button;

    public GameObject ValManager;

    private void Start() {
     button.SetActive(false);   
     ValManager.SetActive(false);
    }

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
    foreach (var trackedImage in eventArgs.updated){
        HandleImageTracking(trackedImage);
    }
 }

    void HandleImageTracking(ARTrackedImage trackedImage)
    {
        if (trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking){
                background.SetActive(false);
                button.SetActive(true);
                ValManager.SetActive(true);
        }
    }

}
