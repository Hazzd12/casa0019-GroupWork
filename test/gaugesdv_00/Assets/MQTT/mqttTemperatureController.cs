using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mqttTemperatureController : MonoBehaviour
{
    
    [Tooltip("Optional name for the controller")]
    public string nameController = "Controller 1";
    public string tag_mqttManager = ""; //to be set on the Inspector panel. It must match one of the mqttManager.cs GameObject
    [Header("   Case Sensitive!!")]
    [Tooltip("the topic to subscribe must contain this value. !!Case Sensitive!! ")]
    public string topicSubscribed = ""; //the topic to subscribe, it need to match a topic from the mqttManager
    [Space]
    [Tooltip("temperature min value")]
    public float minValue = 0;
    [Tooltip("temperature each step")]
    public int step = 5;
    //the maxium temperature will be calculated by the number of LEDS and minVal;


    private float temperatureVal;
    private int MidIndex;
    public Color[] colors; 
    [Tooltip("the color which represents that this led doesn't work")]
    public Color inactiveColor = Color.gray;

    [Space]
    [Space]
    public GameObject objectToControl; //LED Bar to control

    public  Renderer[] ledRenders;
    [Space]
    [Space]
    [Tooltip("bad temperature color")]
    public Color badTemperature;
    [Tooltip("good temperature color")]
    public Color goodTemperature;
    
    [Space]
    public mqttManager _eventSender;

    private
    void Awake()
    {
        if (GameObject.FindGameObjectsWithTag(tag_mqttManager).Length > 0)
        {
            _eventSender = GameObject.FindGameObjectsWithTag(tag_mqttManager)[0].gameObject.GetComponent<mqttManager>();
            _eventSender.Connect(); //Connect tha Manager when the object is spawned
        }
        else
        {
            Debug.LogError("At least one GameObject with mqttManager component and Tag == tag_mqttManager needs to be provided");
        }
    }

    void OnEnable()
    {
        _eventSender.OnMessageArrived += OnMessageArrivedHandler;
        ledRenders = objectToControl.GetComponentsInChildren<Renderer>();
        MidIndex = (ledRenders.Length-1)/2;
        Color[] tempColor = new Color[ledRenders.Length];
        //set colors for each led
        for(int i=0; i<ledRenders.Length;i++){
            if(i<=MidIndex){
                float t = i / (float)MidIndex;
                Color interpolatedColor = Color.Lerp(badTemperature, goodTemperature, t);
                tempColor[i] = interpolatedColor;
            }
            else{
                float t = (i-MidIndex) / (float)(ledRenders.Length-MidIndex-1);
                Color interpolatedColor = Color.Lerp(goodTemperature, badTemperature, t);
                tempColor[i] = interpolatedColor;
            }
        }
        colors = tempColor;
    }

    private void OnDisable()
    {
        _eventSender.OnMessageArrived -= OnMessageArrivedHandler;
    }

    private void OnMessageArrivedHandler(mqttObj mqttObject) //the mqttObj is defined in the mqttManager.cs
    {
        //We need to check the topic of the message to know where to use it 
        if (mqttObject.topic.Contains(topicSubscribed))
        {
            temperatureVal = float.Parse(mqttObject.msg);
            Debug.Log("Event Fired. The message, from Object " + nameController + " is = " + name);
            Debug.Log(mqttObject.msg);
        }
    }

    private void Update()
    {
        int index = (int)MathF.Floor(temperatureVal-minValue/step);
        if(index<0){
            index = 0;
        }
        else if(index >= ledRenders.Length){
            index = ledRenders.Length-1;
        }

        for(int i = 0; i<= index; i++){
            ledRenders[i].material.color = colors[index];
        }
        for(int i = index+1; i<ledRenders.Length;i++){
            ledRenders[i].material.color = inactiveColor;
        }
    }
}
