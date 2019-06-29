// *** SNDEMO ***

// This example shows how to autoconnect between the SPAD.neXt and Arduino.
//
// It demonstrates how to
// - Respond to a connection request from SPAD.neXt
// - Use a identifier to handshake
// - Expose a data value to SPAD.neXt
// - Request Data Updates from SPAD.neXt

#include <CmdMessenger.h>  // CmdMessenger

// Internal led
const int ledPin = 13;

// Listen on serial connection for messages from the pc
CmdMessenger messenger(Serial);

// This is the list of recognized commands. These can be commands that can either be sent
// or received.
// In order to receive, attach a callback function to these events
enum
{
  kRequest = 0, // Request from SPAD.neXt
  kCommand = 1, // Command to SPAD.neXt
  kEvent = 2, // Events from SPAD.neXt
  kDebug = 3, // Debug strings to SPAD.neXt Logfile
  kLed = 10, // CMDID for exposed data to SPAD.neXt
  kHeading = 11, // CMDID for data updates from SPAD.neXt
};

int lastLedState = 999;
bool isReady = false;

void attachCommandCallbacks()
{
  // Attach callback methods
  messenger.attach(onUnknownCommand);
  messenger.attach(kRequest  , onIdentifyRequest);
  messenger.attach(kLed , onTurnLedOn);
  messenger.attach(kHeading , onHeadingLockChanged);
}

// ------------------  C A L L B A C K S -----------------------

// Called when a received command has no attached function
void onUnknownCommand()
{
  messenger.sendCmd(kDebug,"UNKNOWN COMMAND"); 
}

// Callback function to respond to indentify request. This is part of the
// Auto connection handshake.
void onIdentifyRequest()
{
  char *szRequest = messenger.readStringArg();

  if (strcmp(szRequest, "INIT") == 0) {
    messenger.sendCmdStart(kRequest);
    messenger.sendCmdArg("SPAD");
    // Unique Device ID
    messenger.sendCmdArg(F("{DD7E3826-E439-4484-B186-A1443F3BC521}"));
    // Device Name for UI
    messenger.sendCmdArg("Arduino Demo");
    messenger.sendCmdEnd();
    return;
  }

  if (strcmp(szRequest, "PING") == 0) {
    messenger.sendCmdStart(kRequest);
    messenger.sendCmdArg("PONG");
    messenger.sendCmdArg(messenger.readInt32Arg());
    messenger.sendCmdEnd();
    return;
  }
  if (strcmp(szRequest, "CONFIG") == 0) {

    // Expose LED
    messenger.sendCmdStart(kCommand);
    messenger.sendCmdArg("ADD");
    messenger.sendCmdArg(kLed);
    messenger.sendCmdArg("leds/systemled"); // will become "SERIAL:<guid>/leds/systemled"
    messenger.sendCmdArg("U8");
    messenger.sendCmdArg("RW");
    messenger.sendCmdArg("Led");
    messenger.sendCmdArg("Toggle LED on/off");
    messenger.sendCmdEnd();

    // Request Heading Lock Dir Updates
    messenger.sendCmdStart(kCommand);
    messenger.sendCmdArg("SUBSCRIBE");
    messenger.sendCmdArg(kHeading);
    messenger.sendCmdArg("SIMCONNECT:AUTOPILOT HEADING LOCK DIR");
    messenger.sendCmdEnd();

    // tell SPAD.neXT we are done with config
    messenger.sendCmd(kRequest, "CONFIG");
    isReady = true;
    return;
  }
}

// Callback to perform some action
void onTurnLedOn()
{
  int32_t newLed = messenger.readInt32Arg();
  if (newLed == 1)
  {
    digitalWrite(ledPin, HIGH);
    messenger.sendCmd(kDebug, "LED COMMAND ON");
  }
  else
  {
    digitalWrite(ledPin, LOW);
    messenger.sendCmd(kDebug, "LED COMMAND OFF");

    // When we turned it off we unsubscribe from the heading lock for demo purpose
    messenger.sendCmdStart(kCommand);
    messenger.sendCmdArg("UNSUBSCRIBE");
    messenger.sendCmdArg(kHeading);
    messenger.sendCmdArg("SIMCONNECT:AUTOPILOT HEADING LOCK DIR");
    messenger.sendCmdEnd();
  }
}

// LED on if Heading > 180 else off;
void onHeadingLockChanged()
{
  int32_t newHeading = messenger.readInt32Arg();
  if (newHeading > 180)
  {
    digitalWrite(ledPin, HIGH);
    messenger.sendCmd(kDebug, "HEADING > 180");
  }
  else
  {
    digitalWrite(ledPin, LOW);
    messenger.sendCmd(kDebug, "HEADING < 180");
  }
}

// ------------------ M A I N  ----------------------

// Setup function
void setup()
{
  // Listen on serial connection for messages from the pc

  // 115200 is typically the maximum speed for serial over USB
  Serial.begin(115200);

  // Attach my application's user-defined callback methods
  attachCommandCallbacks();

  // initialize the digital pin as an output.
  pinMode(ledPin, OUTPUT);

  // Make sure led is turned off after start or reset
  digitalWrite(ledPin, LOW);
}

// Loop function
void loop()
{
  // Process incoming serial data, and perform callbacks
  messenger.feedinSerialData();

  // Update Led-Data on SPAD.neXt
  int ledVal = digitalRead(ledPin);
  if ((ledVal != lastLedState) && isReady)
  {
     lastLedState = ledVal;
     messenger.sendCmd(kLed,ledVal);
  }
 
}
