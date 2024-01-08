#include <Servo.h> 
#include <ArduinoJson.h>
#include <ESP8266WiFi.h>
#include <ESP8266WebServer.h>
#include <PubSubClient.h>
#include <Adafruit_NeoPixel.h>
#include <DHT.h>
#include <DHT_U.h>
#include <U8g2lib.h>

#define LED1 14 //red
#define LED2 12 //green 
#define LEDstrip 13
#define servo 15
#define NUMPIXELS 8
#define DELAYVAL 500

U8G2_SSD1306_128X64_NONAME_F_HW_I2C u8g2(U8G2_R0, /* reset=*/U8X8_PIN_NONE, /* clock=*/5, /* data=*/4);
Servo myservo;        // create a servo object 
int pos=0;          // define pos for storing the degree that servo rotates
int angle=0;
float tmp=0;
float ws=0;
float rain=0;
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
  server.handleClient();
  sendMQTT();
  client.loop();
  printinformation();
  Serial.print("Windspeed:");
  Serial.print(ws);
  Serial.print("RAIN:");
  Serial.print(rain);
  Serial.print("Temperature:");
  Serial.print(tmp);
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
  char cage1[100];
  char cage2[100]; 
  String jsonString = "";
  if (strcmp(topic, "UCL/OPS/Garden/WST/dvp2/outTemp_C") == 0) {
      for (int i = 0; i < length; i++) {
    cage[i]=char(payload[i]);
    Serial.print((char)payload[i]);
  }
  Serial.println();
  float tmp1 = atof(cage);
  if(tmp1!=0)
  tmp=tmp1;
  Serial.print("TMP:");
  Serial.println(tmp1);
  } 
  else if (strcmp(topic, "UCL/OPS/Garden/WST/dvp2/windSpeed_kph") == 0) {
          for (int i = 0; i < length; i++) {
    cage1[i]=char(payload[i]);
    Serial.print((char)payload[i]);
  }
  float ws1 = atof(cage1);
  if(ws1!=0)
  ws=ws1;
  Serial.println();
  Serial.print("WS:");
  Serial.println(ws1);
  Serial.println();
  }
  else if (strcmp(topic, "UCL/OPS/Garden/WST/dvp2/dayRain_cm") == 0) {
          for (int i = 0; i < length; i++) {
    cage2[i]=char(payload[i]);
    Serial.print((char)payload[i]);
  }
  Serial.println();
  float rain1 = atof(cage2);
  if(rain1!=0)
  rain=rain1;
  Serial.print("RAIN:");
  Serial.println(rain1);
  Serial.println();
  }

  for (int i = 0; i < length; i++) {
    jsonString += char(payload[i]);
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
  if(tmp<=0)
  {
    ledstrip(0,255,0,0);
  }
    else if(tmp<5&&tmp>0)
  {
    ledstrip(0,255,0,0);
    ledstrip(1,170,85,0); 
  }
    else if(tmp<10&&tmp>5)
  {
    ledstrip(0,255,0,0);
    ledstrip(1,170,85,0);  
    ledstrip(2,85,170,0); 
  }
    else if(tmp<15&&tmp>10)
  {
    ledstrip(0,255,0,0);
    ledstrip(1,170,85,0);  
    ledstrip(2,85,170,0); 
    ledstrip(3,0,255,0); 
  }
    else if(tmp<20&&tmp>15)
  {
    ledstrip(0,255,0,0);
    ledstrip(1,170,85,0);  
    ledstrip(2,85,170,0); 
    ledstrip(3,0,255,0);
    ledstrip(4,64,191,0);
  }
    else if(tmp<25&&tmp>20)
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
    else if(tmp<30&&tmp>25)
  {
    ledstrip(0,255,0,0);
    ledstrip(1,170,85,0);  
    ledstrip(2,85,170,0); 
    ledstrip(3,0,255,0);
    ledstrip(4,64,191,0);
    ledstrip(5,128,128,0);
    ledstrip(6,191,64,0); 
  }
    else if(tmp>=30)
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
    if(CDS>600&&tmp!=0)
  {
    u8g2.setFont(u8g2_font_unifont_t_chinese2); // use chinese2
    u8g2.firstPage();
    do
    {
    u8g2.setCursor(3, 15);
    u8g2.print("CO2 is too high!");
    u8g2.setCursor(3, 30);
    u8g2.print("Please open");
    u8g2.setCursor(3, 45);
    u8g2.print("the window!");    
    } while (u8g2.nextPage()); 
    delay(2000);
    printinformation();
  }
   if(CDS>0&&tmp>0)
   {
   if(tmp>10&&tmp<25&&rain>0&&ws>3.6&&ws<10.8)
  {
  digitalWrite(LED1,LOW);
  digitalWrite(LED2,HIGH);
    u8g2.setFont(u8g2_font_unifont_t_chinese2); // use chinese2
  u8g2.firstPage();
  do
  {
    u8g2.setCursor(3, 15);
    u8g2.print("Good weather");
    u8g2.setCursor(3, 30);
    u8g2.print("You can enjoy"); 
    u8g2.setCursor(3, 45);
    u8g2.print("outside!:");  
    u8g2.setCursor(3, 60);
    u8g2.print("");    
    
  } while (u8g2.nextPage()); 
  delay(2000);
  printinformation(); 
  delay(2000);  
  }
  else 
  {
  digitalWrite(LED1,HIGH);
  digitalWrite(LED2,LOW);
  u8g2.setFont(u8g2_font_unifont_t_chinese2); // use chinese2
  u8g2.firstPage();
  do
  {
    u8g2.setCursor(3, 15);
    u8g2.print("Bad weather");
    u8g2.setCursor(3, 30);
    u8g2.print("Please stay at"); 
    u8g2.setCursor(3, 45);
    u8g2.print("your home!");     
  } while (u8g2.nextPage());  
  delay(2000);
  printinformation();
  delay(2000);
  }
   }
    if(CDS>400)
  {
    angle=(CDS-400)*2000/10000;
    myservo.write(angle);
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
      client.subscribe("UCL/OPS/Garden/WST/dvp2/dayRain_cm");
      client.subscribe("UCL/OPS/Garden/WST/dvp2/windSpeed_kph");
      client.subscribe("UCL/OPS/Garden/WST/dvp2/outTemp_C");
    } else {
      Serial.print("failed, rc=");
      Serial.print(client.state());
      Serial.println(" try again in 5 seconds");
      // Wait 5 seconds before retrying
      delay(5000);
    }
  }
}

void printinformation()
{
  u8g2.setFont(u8g2_font_unifont_t_chinese2); // use chinese2
  u8g2.firstPage();
  do
  {
    u8g2.setCursor(3, 15);
    u8g2.print("CO2(Indoor):");
    u8g2.setCursor(3, 30);
    u8g2.print(CDS); 
    u8g2.setCursor(3, 45);
    u8g2.print("Temperature:");  
    u8g2.setCursor(3, 60);
    u8g2.print(tmp);    
    
  } while (u8g2.nextPage());   
}
