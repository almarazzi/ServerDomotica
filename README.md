# ServerDomotica

**ServerDomotica** è un progetto open-source per la gestione di dispositivi domotici tramite un **server ASP.NET Core**, un **client web in TypeScript** e dispositivi basati su **ESP8266** che controllano relay.  
Il sistema permette di accendere/spegnere carichi elettrici da remoto, programmare automazioni settimanali e monitorare lo stato dei dispositivi.

---

## Indice

- [Funzionalità](#funzionalità)  
- [Come funziona](#come-funziona)  
- [Architettura](#architettura)  
- [Tecnologie](#tecnologie)  
- [Installazione](#installazione)   

---

## Funzionalità

- 🌐 **Controllo remoto** dei relay tramite interfaccia web o API REST  
- 📅 **Programmazione settimanale** con accensione/spegnimento automatico  
- 🔐 **Gestione utenti** con login e autenticazione  
- 📊 **Dashboard web** intuitiva per visualizzare e controllare i dispositivi  
- ⚡ **Integrazione con ESP8266** che riceve i comandi dal server via HTTP  
- 🧩 **Sistema modulare** facilmente estendibile con nuovi dispositivi o logiche  

---

## Come funziona

1. Il **server ASP.NET Core** espone API HTTP per controllare i dispositivi.  
2. Il **client web** (in TypeScript/React) si collega al server e fornisce una dashboard user-friendly.  
3. Gli **ESP8266** collegati a relay si connettono al server e ricevono i comandi di accensione/spegnimento.  
4. Il server gestisce anche la **logica dei programmi settimanali**, inviando i comandi ai dispositivi all’orario configurato.  
5. Tutto il traffico è centralizzato sul server, che funge da "cervello" della domotica.  

Esempio di flusso:  
- L’utente accede alla dashboard web e accende un relay → il comando arriva al server → il server invia la richiesta HTTP all’ESP8266 → il relay si attiva.  

---

## Architettura

Struttura del repository:

- **`domotica/`** → logica server per gestione dispositivi, relay e automazioni  
- **`provaweb/`** → interfaccia web per il controllo e il monitoraggio  
- **`testActiveUsersService/`** → progetto per i test del server
- **`ProgettoEsp8266Server`** → Firmware per ESP8266
- **`provaweb.sln`** → soluzione principale C#  
- **`LICENSE`** → licenza MIT  

---

## Tecnologie

- **Backend** → ASP.NET Core (C#)  
- **Frontend** → TypeScript + React + Vite  
- **Dispositivi** → ESP8266 con firmware per ricevere comandi HTTP  
- **Database** → SQLite 
- **Testing** → xUnit (per il backend)  

---

## Installazione

### Prerequisiti
- [.NET SDK](https://dotnet.microsoft.com/) (versione 8 o successiva)  
- [Node.js](https://nodejs.org/) + npm (o yarn)  
- Git installato  
- [Arduino IDE](https://www.arduino.cc/en/software) per compilare e caricare firmware su ESP8266  
- [Visual Studio Code](https://code.visualstudio.com/) con estensione [PlatformIO](https://platformio.org/) (opzionale, alternativa all’IDE Arduino)

### Clonare il repository
```bash
git clone https://github.com/almarazzi/ServerDomotica.git
cd ServerDomotica
