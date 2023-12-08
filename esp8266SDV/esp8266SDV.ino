#include <Servo.h> 
#include <ArduinoJson.h>
#include <ESP8266WiFi.h>
#include <ESP8266WebServer.h>
#include <PubSubClient.h>
#include <Adafruit_NeoPixel.h>
#include <DHT.h>
#include <DHT_U.h>
#include <U8g2lib.h>

#define LED1 14
#define LED2 12
#define LEDstrip 13
#define servo 15
#define NUMPIXELS 8
#define DELAYVAL 500

U8G2_SSD1306_128X64_NONAME_F_HW_I2C u8g2(U8G2_R0, /* reset=*/U8X8_PIN_NONE, /* clock=*/5, /* data=*/4);
Servo myservo;        // create a servo object 
int pos=0;          // define pos for storing the degree that servo rotates
int angle=0;

Adafruit_NeoPixel pixels(NUMPIXELS, LEDstrip);
const char* ssid     = "CE-Hub-Student";
const char* password = "casa-ce-gagarin-public-service";
const char* mqttuser = "student";
const char* mqttpass = "ce2021-mqtt-forget-whale";
const char* mqtt_server = "mqtt.cetools.org";

WiFiClient espClient;
PubSubClient client(espClient);
ESP8266WebServer server(80);

float TPS=0.0;
float CDS=0.0;

void ledstrip(int n, int r, int g, int b)
{
    pixels.setPixelColor(n,r,g,b);
    pixels.show();

  delay(100);
}

void setup() {
  Serial.begin(115200);
  delay(100);
  startWifi();
  myservo.attach(servo);
  pixels.begin();
  pinMode(LED1,OUTPUT);
  pinMode(LED2,OUTPUT);
  client.setServer(mqtt_server, 1884);
  client.setCallback(callback);
  ledstrip(1,255,0,0);
  myservo.write(0);
  u8g2.begin();
  u8g2.enableUTF8Print(); // enable UTF8 support for the Arduino print() function
  pixels.clear();
}

void loop() {
  // handler for receiving requests to webserver
  
  digitalWrite(LED1,HIGH);
  digitalWrite(LED2,HIGH);
  server.handleClient();
  delay(1000);
    sendMQTT();
  Serial.print("CDS:");
  Serial.println(CDS);
  Serial.print("TPS:");
  Serial.println(TPS);
  client.loop();
  u8g2.setFont(u8g2_font_unifont_t_chinese2); // use chinese2
  u8g2.firstPage();
  do
  {
    u8g2.setCursor(0, 15);
    u8g2.print("CDS:");
    u8g2.setCursor(0, 30);
    u8g2.print(CDS); 
    u8g2.setCursor(0, 45);
    u8g2.print("TPS:");  
    u8g2.setCursor(0, 60);
    u8g2.print(TPS);    
    
  } while (u8g2.nextPage()); 
  if(CDS>400)
  {
    angle=(CDS-300)*18/50;
    myservo.write(angle);
  }
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
  if(TPS<=0)
  {
    ledstrip(0,255,0,0);
  }
    else if(TPS<5&&TPS>0)
  {
    ledstrip(0,255,0,0);
    ledstrip(1,170,85,0); 
  }
    else if(TPS<10&&TPS>5)
  {
    ledstrip(0,255,0,0);
    ledstrip(1,170,85,0);  
    ledstrip(2,85,170,0); 
  }
    else if(TPS<15&&TPS>10)
  {
    ledstrip(0,255,0,0);
    ledstrip(1,170,85,0);  
    ledstrip(2,85,170,0); 
    ledstrip(3,0,255,0); 
  }
    else if(TPS<20&&TPS>15)
  {
    ledstrip(0,255,0,0);
    ledstrip(1,170,85,0);  
    ledstrip(2,85,170,0); 
    ledstrip(3,0,255,0);
    ledstrip(4,64,191,0);
  }
    else if(TPS<25&&TPS>20)
  {
    ledstrip(0,255,0,0);
    delay(500);
    ledstrip(1,170,85,0); 
    delay(500); 
    ledstrip(2,85,170,0);
    delay(500); 
    ledstrip(3,0,255,0);
    delay(500);
    ledstrip(4,64,191,0);
    delay(500);
    ledstrip(5,128,128,0); 
  }
    else if(TPS<30&&TPS>25)
  {
    ledstrip(0,255,0,0);
    ledstrip(1,170,85,0);  
    ledstrip(2,85,170,0); 
    ledstrip(3,0,255,0);
    ledstrip(4,64,191,0);
    ledstrip(5,128,128,0);
    ledstrip(6,191,64,0); 
  }
    else if(TPS>=30)
  {
    ledstrip(0,255,0,0);
    ledstrip(1,170,85,0);  
    ledstrip(2,85,170,0); 
    ledstrip(3,0,255,0);
    ledstrip(4,64,191,0);
    ledstrip(5,128,128,0);
    ledstrip(6,191,64,0); 
    ledstrip(7,255,0,0); 
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
