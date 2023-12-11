using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Judge
{
    public string topic;
    public float val;
    public float max;
    public float min;
}

public class mqttWeatherController : ValEventer
{

    [Tooltip("Optional name for the controller")]
    public string nameController = "Controller 1";
    public string tag_mqttManager = ""; //to be set on the Inspector panel. It must match one of the mqttManager.cs GameObject
    [Header("   Case Sensitive!!")]
    [Tooltip("the topic to subscribe must contain this value. !!Case Sensitive!! ")]
    public Judge[] judges; //the topic to subscribe, it need to match a topic from the mqttManager
    [Space]

    [Space]
    public GameObject badWeather; //bad Weather led object to control
    public GameObject goodWeather; //good Weather led object to control

    [Space]
    [Space]
    [Tooltip("bad weather color")]
    public Color badWeatherColor = Color.red;
    [Tooltip("good weather color")]
    public Color goodWeatherColor = Color.green;

    private Color inactiveColor = Color.gray;

    [Space]
    public mqttManager _eventSender;

    void Awake()
    {
        if (GameObject.FindGameObjectsWithTag(tag_mqttManager).Length > 0)
        {
            _eventSender = GameObject.FindGameObjectsWithTag(tag_mqttManager)[0].gameObject.GetComponent<mqttManager>();
        }
        else
        {
            Debug.LogError("At least one GameObject with mqttManager component and Tag == tag_mqttManager needs to be provided");
        }
    }

    void OnEnable()
    {
        _eventSender.OnMessageArrived += OnMessageArrivedHandler;

    }

    private void OnDisable()
    {
        _eventSender.OnMessageArrived -= OnMessageArrivedHandler;
    }

    private void OnMessageArrivedHandler(mqttObj mqttObject) //the mqttObj is defined in the mqttManager.cs
    {
        //We need to check the topic of the message to know where to use it 
        foreach (Judge judge in judges)
        {
            if (mqttObject.topic.Contains(judge.topic))
            {
                judge.val = float.Parse(mqttObject.msg);
                Debug.Log("topic:"+judge.topic+" val:"+judge.val);
                HandleValChanged(judge.topic, judge.val);
            }
        }
    }

    private void Update(){
        bool ifGoodWeather = true;
        foreach (Judge judge in judges){
            //Debug.Log(judge.topic);
            if(judge.val<judge.min|| judge.val>judge.max){
                ifGoodWeather = false;
            }
        }
        Renderer goodRenderer = goodWeather.gameObject.GetComponent<Renderer>();
        Renderer badRenderer = badWeather.gameObject.GetComponent<Renderer>();
        if(ifGoodWeather){
            goodRenderer.material.color = goodWeatherColor;
            badRenderer.material.color = inactiveColor;
        }
        else{
            badRenderer.material.color = badWeatherColor;
            goodRenderer.material.color = inactiveColor;
        }
    }

}
