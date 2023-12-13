using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OLEDController : MonoBehaviour
{

    
    [Tooltip("the text to control")]
    public TextMeshPro textMeshPro;
    public string tag_ValManager = ""; //to be set on the Inspector panel. It must match one of the valManager.cs GameObject
    public TextMeshProUGUI UGUI;
    public ValManager _eventSender;

    private void Awake() {
        if (GameObject.FindGameObjectsWithTag(tag_ValManager).Length > 0)
        {
            _eventSender = GameObject.FindGameObjectsWithTag(tag_ValManager)[0].gameObject.GetComponent<ValManager>();
        }
        else
        {
            Debug.LogError("At least one GameObject with mqttManager component and Tag == tag_mqttManager needs to be provided");
        }
    }

    void OnEnable()
    {
        _eventSender.OnOLEDChanged += OnMessageArrivedHandler;
        UGUI.text = textMeshPro.text;
    }

    private void OnMessageArrivedHandler(string txt)
    {
        Debug.Log(txt + " in OLEDController");
        textMeshPro.text = txt;
        UGUI.text = txt;
    }



}
