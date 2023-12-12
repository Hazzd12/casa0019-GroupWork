using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OLEDController : MonoBehaviour
{

    [Tooltip("set the object to get the ValManager")]
    public GameObject manager;
    [Tooltip("the text to control")]
    public TextMeshPro textMeshPro;

    public TextMeshProUGUI UGUI;
    public ValManager _eventSender;

    private void Awake() {
        _eventSender = manager?.GetComponentInChildren<ValManager>();
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


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
