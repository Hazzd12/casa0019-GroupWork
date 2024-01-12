# README

## File Structure

```lua
|-- esp8266SDV/              # physical device
|   |-- esp8266SDV.ino
|-- Unity/          		# Digital twin
|   |-- casa0019_CourseWork/
|   |   |-- Asserts/ 		# main customized folder
|   |   |-- ...
|-- Images				#image folder
|-- casa0019_coursework_digital_twin.apx/  # the apk of digital twin   
|-- README.md      	    #README file
|-- object.fbx			#the model of prefab
```

****

## Data Source

* MQTT server: mqtt://mqtt.cetools.org
* Outdoor Data:
  * **Wind speed:** UCL/OPS/Garden/WST/dvp2/windSpeed_kph
  * **Rain rate:** UCL/OPS/Garden/WST/dvp2/rainRate_cm_per_hour
  * **Temperature:** UCL/OPS/Garden/WST/dvp2/outTemp_C  
* Indoor Data:
  * **CO2:** UCL/OPSEBO/107/Room/CDS/Value 

****

## Physical Device

### Materials:

* ESP8266:

  * You’ll want to query the relevant MQTT feeds to your microcontroller. If you do not have the ESP8266 requests the data from the MQTT broker, and controls the actuators (OLED screen, LEDS, servo), to visualize that data.

* Neopixel LED Strip   

  * The LED strips visualize temperature data. The number and color of lights indicate the temperature level.

* LEDs (Green & Red)   

  * The LEDs denote good or bad weather using the following environmental inputs: it is a good weather when temperature value is between 10 degrees and 25 degrees, the wind speed value is between 3.6kp/h and 10.8kp/h, and it is not raining outside, otherwise it is a bad weather. Green LED will be turned on if there is good weather outside, otherwise red LED will be turned on.

* Servo

  * The Servo range is 0º to 360º and it visualizes CO2 data by rotating the pointer to point at the value of CO2 in gauge.

* Jumper Cables   

* OLED Screen:

  * The OLED displays the following prompts: 

    1. “CO2 is too high, please open the window!” when indoor carbon dioxide concentration is high.

    2. “Good weather! You can enjoy it outside!” when the weather is good outside.

    3. “Bad weather, please stay at home.” when the weather is bad outside.

* Gauge Face (clear acrylic round, printed face, 3D-printed frame)   

* Laser-cut Enclosure  

### Circuit Diagram

![](./images/circuit diagram.png)

### Workflow

![](./images/workflow.png)

### Enclosure

<img src="./images/enclosure.png" style="zoom:50%;" />

****

## Digital Twin

### Model

<img src="./images/model.png" style="zoom:50%;" />

### Prefab

![](./images/prefab.png)

### Library

* [M2MQTT library](https://github.com/CE-SDV-Unity/M2MqttUnity)
* [XCharts](https://github.com/XCharts-Team/XCharts/blob/master/README-en.md)  

### Scripts

* Location: [.../Asserts/Coursework/Scripts](https://github.com/Hazzd12/casa0019-GroupWork/tree/main/Unity/casa0019_CourseWork/Assets/Coursework)

![](./images/scripts.png)



* M2MqttUnityEvent and mqttManager are used to collect MQTT data from the server, and send them to the controllers through events.

* ValEventer is the father class of middle three controllers.   

* The ValManager script was designed for future use to reduce code redundancy.

### Execution

* Download the apk and Scan this QR code, the prefab will appear.

  <img src="./images/QR.png" style="zoom:50%;" />

****

## Future Work

* Improving the function of the AR twin

1. * like adding more data to improve the criterion of good weather, bad weather and the suggestions to help the user have better experience.

1. * Adding more charts to show other history data and some interesting models to show the real-time data of wind speed or rain rate.

* Modifying the AR cue

1. * Currently, the cue is on the upper right-hand corner of the enclosure. However, the gauge face has a lot of dead space in the center that could house the AR cue instead.

1. * Additionally, the aesthetic experience of our device would improve if we changed the cue from a QR code to our logo. This creates additional opportunities for remote access      to the digital twin if, for example, the product also came with a physical version of our logo. This is mocked up on a keychain below.