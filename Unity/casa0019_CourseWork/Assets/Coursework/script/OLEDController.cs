using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


[System.Serializable]
public class Restriction
{
    public string title;
    public float val;
    [Tooltip("true -> judged by range; false -> judged by the val(which is originally bool)")]
    public bool judgement; // when judgement is false, max and min don't matter
    public float max;
    public float min;
    public string errorMessage; // the message shown in the OLED when the val is not in the range or bad.
}


public class OLEDController : MonoBehaviour
{

    [Tooltip("set the object to get its senders(script with ValEventer)")]
    public GameObject controller;
    public ValEventer[] _eventSender;
    // Start is called before the first frame update

    [Tooltip("The judgements")]
    public Restriction[] judgements;
    [Tooltip("The message to show only when every val is good")]
    public string defaultMessage;

    
    [Tooltip("the text to control")]
    public TextMeshPro textMeshPro;
    public string tag_ValManager = ""; //to be set on the Inspector panel. It must match one of the valManager.cs GameObject
    public TextMeshProUGUI UGUI;

    private void Awake() {
         _eventSender = controller?.GetComponentsInChildren<ValEventer>();
    }

    void OnEnable()
    {
        foreach (ValEventer sender in _eventSender)
        {
            sender.OnValueChanged += OnMessageArrivedHandler;
        }

        UGUI.text = textMeshPro.text;
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
        OnMessageHandler(message);

    }

    private void OnMessageHandler(string txt)
    {
        Debug.Log(txt + " in OLEDController");
        textMeshPro.text = txt;
        UGUI.text = txt;
    }



}
