#include <ArduinoJson.h>
#include <ESP8266WiFi.h>
#include <ESP8266WebServer.h>
#include <PubSubClient.h>
#include <DHT.h>
#include <DHT_U.h>
#define DHTTYPE DHT22
float Temperature=0;
float Humidity=0;
int Moisture = 1; 
const char* ssid     = "CE-Hub-Student";
const char* password = "casa-ce-gagarin-public-service";
const char* mqttuser = "student";
const char* mqttpass = "ce2021-mqtt-forget-whale";
ESP8266WebServer server(80);
const char* mqtt_server = "mqtt.cetools.org";
WiFiClient espClient;
PubSubClient client(espClient);
long lastMsg = 0;
char msg[50];
int value = 0;
uint8_t DHTPin = 12;        // on Pin 2 of the Huzzah
uint8_t soilPin = 0;      // ADC or A0 pin on Huzzah
int sensorVCC = 13;

DHT dht(DHTPin, DHTTYPE);   // Initialize DHT sensor.
float TPS=0.0;
float CDS=0.0;

void setup() {
  Serial.begin(115200);
  delay(100);
  startWifi();
  client.setServer(mqtt_server, 1884);
  client.setCallback(callback);
}

void loop() {
  // handler for receiving requests to webserver
  server.handleClient();
  delay(3000);
    sendMQTT();
  Serial.println(CDS);
  Serial.println(TPS);
  client.loop();
}


void startWifi() {
  // We start by connecting to a WiFi network
  Serial.println();
  Serial.print("Connecting to ");
  Serial.println(ssid);
  WiFi.begin(ssid, password);

  // check to see if connected and wait until you are
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("");
  Serial.println("WiFi connected");
  Serial.print("IP address: ");
  Serial.println(WiFi.localIP());
}

void sendMQTT() {

  if (!client.connected()) {
    reconnect();
  }
  client.loop();
  Temperature = dht.readTemperature(); // Gets the values of the temperature
  snprintf (msg, 50, "%.1f", Temperature);
  client.publish("student/CASA0014/plant/ucfnaaf/temperature", msg);
  Humidity = dht.readHumidity(); // Gets the values of the humidity
  snprintf (msg, 50, "%.0f", Humidity);
  client.publish("student/CASA0014/plant/ucfnaaf/humidity", msg);
  //Moisture = analogRead(soilPin);   // moisture read by readMoisture function
  snprintf (msg, 50, "%.0i", Moisture);
  client.publish("student/CASA0014/plant/ucfnaaf/moisture", msg);

}

void callback(char* topic, byte* payload, unsigned int length) {
  Serial.print("Message arrived [");
  Serial.print(topic);
  Serial.print("] ");
  char cage[100];
  String jsonString = "";
  for (int i = 0; i < length; i++) {
    cage[i]=payload[i];
    jsonString += char(payload[i]);
    Serial.print((char)payload[i]);
  }
  StaticJsonDocument<200> jsonDoc;
  DeserializationError error = deserializeJson(jsonDoc, jsonString);
  if (error)
  {
    Serial.print("Failed to parse JSON: ");
    Serial.println(error.c_str());
    return;
  }
  float CDS1 = jsonDoc["/UCL Virtual ES/IOT-mqtt/1PS/107/Room/CDS/Value"];
  if(CDS1!=0)
  {
  CDS=CDS1;
  Serial.print("CDS: ");
  Serial.println(CDS1, 2); 
  }
  float TPS1 = jsonDoc["/UCL Virtual ES/IOT-mqtt/1PS/107/Room/TPS/Value"];
  if(TPS1!=0)
  {
   TPS=TPS1;
  Serial.print("TPS: ");
  Serial.println(TPS1, 2); 
  }
  // Switch on the LED if an 1 was received as first character
  if ((char)payload[0] == '1') {
    digitalWrite(BUILTIN_LED, LOW);   // Turn the LED on (Note that LOW is the voltage level
    // but actually the LED is on; this is because it is active low on the ESP-01)
  } else {
    digitalWrite(BUILTIN_LED, HIGH);  // Turn the LED off by making the voltage HIGH
  }

}

void reconnect() {
  // Loop until we're reconnected
  while (!client.connected()) {
    Serial.print("Attempting MQTT connection...");
    // Create a random client ID
    String clientId = "ESP8266Client-";
    clientId += String(random(0xffff), HEX);
    
    // Attempt to connect with clientID, username and password
    if (client.connect(clientId.c_str(), mqttuser, mqttpass)) {
      Serial.println("connected");
      // ... and resubscribe
      client.subscribe("UCL/OPSEBO/107/Room/CDS/Value");
      client.subscribe("UCL/OPSEBO/107/Room/TPS/Value");
    } else {
      Serial.print("failed, rc=");
      Serial.print(client.state());
      Serial.println(" try again in 5 seconds");
      // Wait 5 seconds before retrying
      delay(5000);
    }
  }
}
