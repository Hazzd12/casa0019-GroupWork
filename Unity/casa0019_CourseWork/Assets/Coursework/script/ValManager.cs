using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ValManager : MonoBehaviour
{
    public string prefabTag;
    [Tooltip("set the object to get its senders(script with ValEventer)")]
    public GameObject controller;
    public ValEventer[] _eventSender;
    // Start is called before the first frame update

    [Tooltip("The judgements")]
    public Restriction[] judgements;
    [Tooltip("The message to show only when every val is good")]
    public string defaultMessage;

    public event OnOLEDChangedDelegate OnOLEDChanged;
    public delegate void OnOLEDChangedDelegate(string message);
    // private void Awake()
    // {
    //     controller = GameObject.Find("ARObject");
    //     _eventSender = controller?.GetComponentsInChildren<ValEventer>();
    // }

    void OnEnable()
    {
        controller = GameObject.FindGameObjectWithTag(prefabTag);
        _eventSender = controller?.GetComponentsInChildren<ValEventer>();
        Debug.Log("Test");
        foreach (ValEventer sender in _eventSender)
        {
            sender.OnValueChanged += OnMessageArrivedHandler;
        }
    }

    private void OnDisable()
    {
        foreach (ValEventer sender in _eventSender)
        {
            sender.OnValueChanged -= OnMessageArrivedHandler;
        }
    }

    private void OnMessageArrivedHandler(CustomisedText text)
    {
        Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!/n" + text.title + text.val);
        bool flag2 = true;
        string message = defaultMessage;
        //send message to OLED
        foreach (Restriction judge in judgements)
        {
            if (judge.title == text.title){
                Debug.Log(judge.title);
                judge.val = text.val;
            }

            if (judge.judgement){
                    if (judge.min > judge.val || judge.max < judge.val){
                        flag2 = false;
                    }
                }
                else{
                    if (judge.val == 0){
                        flag2 = false;
                    }
                }

                if(!flag2){
                    message = judge.errorMessage;
                    break;//one error message is enough
                    
                }
        }
        Debug.Log("OLED"+message);
        OnOLEDChanged?.Invoke(message);


    }


}
