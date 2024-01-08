using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using XCharts.Runtime;

public class mqttGaugeController : ValEventer
{
    [Tooltip("Optional name for the controller")]
    public string nameController = "Controller 1";
    public string tag_mqttManager = ""; //to be set on the Inspector panel. It must match one of the mqttManager.cs GameObject
    [Space]
    [Tooltip("The title to show in UI and got By ValManager")]
    public string title;
    [Space]


    [Header("   Case Sensitive!!")]
    [Tooltip("the topic to subscribe must contain this value. !!Case Sensitive!! ")]
    public string topicSubscribed = ""; //the topic to subscribe, it need to match a topic from the mqttManager
    public float pointerValue = 420f;
    [Space]
    [Space]
    public GameObject objectToControl; //pointer of the gauge, or other 3D models
    [Tooltip("Select the rotation axis of the object to control")]
    public enum State { X, Y, Z };
    public State rotationAxis;
    [Space]
    [Tooltip("Direction Rotation")]
    public bool CW = true; //CW True = 1; CW False = -1
    private int rotationDirection = 1;
    [Space]
    [Space]
    [Tooltip("minimum value on the dial")]
    public float startValue = 0f; //start value of the gauge
    [Tooltip("maximum value on the dial")]
    public float endValue = 180f; // end value of the gauge
    [Tooltip("full extension of the gauge in EulerAngles")]
    public float fullAngle = 180f; // full extension of the gauge in EulerAngles

    [Tooltip("Adjust the origin of the scale. negative values CCW; positive value CW")]
    public float adjustedStart = 0f; // negative values CCW; positive value CW
    [Space]
    public TextMeshProUGUI textMeshPro;
    
    public mqttManager _eventSender;

    [Space]
    public LineChart lineChart;
    int count = 0; //simple counter for the data 

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
            //pointerValue = float.Parse(mqttObject.msg);
            string input = mqttObject.msg;
       // 使用正则表达式提取数字部分
            string pattern = @"\d+";
             MatchCollection matches = Regex.Matches(input, pattern);

        // 将匹配的数字转换为float并打印
            foreach (Match match in matches)
         {
            if (float.TryParse(match.Value, out float result))
            {
                Debug.Log("result:"+result);
                pointerValue = result;
            }
            else
            {
                Debug.Log("No result");
            }
        }

            lineChart.AddData(1, count++, pointerValue);

            Debug.Log("CO2: "+pointerValue);
            HandleValChanged(title, pointerValue);
            textMeshPro.text = title + ": " + pointerValue;
        }
    }

    private void Update()
    {
        float step = 1.5f * Time.deltaTime;
        // ternary conditional operator https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/conditional-operator
        rotationDirection = CW ? 1 : -1;

        if (pointerValue >= startValue)
        {
            Vector3 rotationVector = new Vector3();
            //If the rotation Axis is X
            if (rotationAxis == State.X)
            {
                rotationVector = new Vector3(
                (rotationDirection * ((pointerValue - startValue) * (fullAngle / (endValue - startValue)))) - adjustedStart,
                objectToControl.transform.localEulerAngles.y,
                objectToControl.transform.localEulerAngles.z);
            }
            //If the rotation Axis is Y
            else if (rotationAxis == State.Y)
            {
                rotationVector = new Vector3(
                objectToControl.transform.localEulerAngles.x,
                (rotationDirection * ((pointerValue - startValue) * (fullAngle / (endValue - startValue)))) - adjustedStart,
                objectToControl.transform.localEulerAngles.z);

            }
            //If the rotation Axis is Z
            else if (rotationAxis == State.Z)
            {
                rotationVector = new Vector3(
                objectToControl.transform.localEulerAngles.x,
                objectToControl.transform.localEulerAngles.y,
                (rotationDirection * ((pointerValue - startValue) * (fullAngle / (endValue - startValue)))) - adjustedStart);
            }
            objectToControl.transform.localRotation = Quaternion.Lerp(
                    objectToControl.transform.localRotation,
                    Quaternion.Euler(rotationVector),
                    step);
        }
    }
}