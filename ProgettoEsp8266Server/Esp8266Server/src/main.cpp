#include <ESP8266WiFi.h>
#include <WiFiClient.h>
#include <ESP8266WebServer.h>
#include <DNSServer.h>
#include <Preferences.h>
#include <WiFiUdp.h>
#include <ArduinoJson.h>

String Nome_rete = "Nome_rete";          //
String Nome = "";                        //
String Password_rete = "Password_rete";  // mi servono per l'html del access point
String Password = "";                    //
String UDPString = "";

const char ssid[] = "esp";           //nome della rete access point
const char password[] = "12345678";  //password /   /    /      /

int gpio0_pin = 15;       //relay
int gpio2_pin = 2;        //led
bool stateRelay = false;  //StatoRealy
bool StatoAttuale = false;
bool ServerTrovato = false;
int tempo = 3000;
int ping=0;

ESP8266WebServer server(80);
//DNSServer dnsServer; prova

Preferences prefs;  //memoria che è retentiva
WiFiUDP Udp;        //Brocast
unsigned int PortaUdp = 8888;
IPAddress broadcastIp(255, 255, 255, 255);
IPAddress netmask(255, 255, 255, 0);

const char index_html[] PROGMEM = R"rawliteral(
<!DOCTYPE HTML><html><head>
  <title>ESP internet</title>
  <meta name="viewport" content="width=device-width, initial-scale=1">
  </head><body>
    <form action="/submit"  method="post">
      Nome Rete di casa : <input type="text" name="Nome_rete" >

      Password Rete di casa: <input type="password" name="Password_rete" >

      <input type="submit" value="invia">
    </form>
  </body></html>)rawliteral";

void handleRoot() {
  server.send(200, "text/html", index_html);
}

void AcessoPoint() {
  prefs.begin("Internet", false);
  // Start access Point  http://192.168.4.1, rete creata dal esp
  prefs.end();
  WiFi.softAP(ssid, password);
  IPAddress myIP = WiFi.softAPIP();
  (void)myIP;
  //dnsServer.start(53, "*",myIP); prova
  Serial.println("Access Point");

  server.on("/", handleRoot);

  server.on("/submit", HTTP_POST, []() {
    Nome = server.arg("Nome_rete");
    Password = server.arg("Password_rete");
    Serial.println(Nome);
    Serial.println(Password);
    prefs.begin("Internet", false);
    prefs.putString("Nome", Nome);
    prefs.putString("Password", Password);
    prefs.end();
    server.send(200, "text/html", "ok");
  });
}
void ModalitaStazione() {

  prefs.begin("Internet", false);
  //Start server collegato ad un punto di rete essistente
  //Spegnere l'access Point, è accendere la modalita stazione cioè si collega a un punto di acesso esistente, es: ruter di casa
  WiFi.softAPdisconnect(true);
  WiFi.mode(WIFI_STA);

  Nome = prefs.getString("Nome", Nome);
  Password = prefs.getString("Password", Password);
  prefs.end();

  WiFi.begin(Nome, Password);
  while (WiFi.status() != WL_CONNECTED) {
    delay(1000);
    Serial.print(".");
  }
  Serial.println("Server");
  Serial.print("Indirizzo IP: ");
  Serial.println(WiFi.localIP());
  Serial.print("ESP Board MAC Address:  ");
  Serial.println(WiFi.macAddress());
  Serial.println(Nome);
  Serial.println(Password);
  pinMode(gpio2_pin, OUTPUT);
  digitalWrite(gpio2_pin, LOW);
}

void setup() {
  pinMode(gpio0_pin, OUTPUT);
  Serial.begin(115200);
  prefs.begin("Internet", false);
  if (prefs.isKey("Nome") && prefs.isKey("Password")) {
    ModalitaStazione();

    Udp.begin(PortaUdp);
    //Udp.stop();

    server.on("/api/RelaySwitch/StateRelay", HTTP_PUT, []() {
      auto json = server.arg("plain");
      JsonDocument doc;
      deserializeJson(doc, json);
      stateRelay = doc["stateRlay"];
      server.send(200, "application/json", "ok");
    });

    server.on("/api/RelaySwitch/ping", HTTP_GET, []() {
      ping = millis();
      server.send(200, "application/json", "ok");
    });

  } else {
    AcessoPoint();
  }
  server.begin();
}

void loop() {
  if (WiFi.status() == WL_CONNECTED) {
    if (!ServerTrovato) {
      UDPString = WiFi.macAddress();
      Udp.beginPacket(broadcastIp, PortaUdp);
      Udp.print(UDPString);
      Udp.endPacket();
      if (Udp.parsePacket()) {
        Udp.readString() == "Server collegato" ? ServerTrovato = true : ServerTrovato = false;
        Serial.println(ServerTrovato);
      }
    }
  }
  if((long)(millis()- ping) > tempo )
  {
    digitalWrite(gpio0_pin, false);
  }else
  {
    digitalWrite(gpio0_pin, stateRelay);
  }

  //dnsServer.processNextRequest(); prova
  server.handleClient();
}