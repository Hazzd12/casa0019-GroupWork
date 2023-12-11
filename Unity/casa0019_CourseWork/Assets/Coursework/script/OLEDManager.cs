using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OLEDText{
    public string title;
    public float val;
}

public class OLEDManager : MonoBehaviour
{
    [Tooltip("set the object to get its senders(script with ValEventer)")]
    public GameObject controller;
    public ValEventer[] _eventSender;
    // Start is called before the first frame update
    

    private void Awake() {
        _eventSender = controller?.GetComponentsInChildren<ValEventer>();
    }

    void OnEnable()
    {
        foreach(ValEventer sender in _eventSender){
            sender.OnValueChanged += OnMessageArrivedHandler;
        }
    }

    private void OnMessageArrivedHandler(CustomisedText text)
    {
        Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!/n"+text.title+text.val);

    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
