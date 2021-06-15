// LZakFw
// Simple firmware for CNC control.
// by Isaac Candido
// e-mail: isaac.guimaraescandido@gmail.com

#define FW_VERSION 3
#define FW_REVISION 6
#define FW_MINOR_VERSION 0

#define X_DIRECTION_PIN 0
#define X_STEP_PIN 1
#define Y_DIRECTION_PIN 2
#define Y_STEP_PIN 3
#define Z_DIRECTION_PIN 4
#define Z_STEP_PIN 5

#define X_DIRECTION_VALUE 0b00000001 // decimal: 1
#define X_STEP_VALUE 0b00000010 // decimal: 2
#define Y_DIRECTION_VALUE 0b00000100 // decimal: 4
#define Y_STEP_VALUE 0b00001000 // decimal: 8
#define Z_DIRECTION_VALUE 0b00010000 // decimal: 16
#define Z_STEP_VALUE 0b00100000 // decimal: 32 

#define DELAY_BETWEEN_COMMANDS 50 // ms

#define BAUD_RATE 14400 // bits per second (BPS)

#define ENABLE_MOTORS_PIN 8 // pin for powering up motors

#define X_ENDSTOP 9 // pin for X axis endstop
#define Y_ENDSTOP 10 // pin for Y axis endstop
#define Z_ENDSTOP 11 // pin for Z axis endstop

// declarations
struct Instruction { bool Direction; bool Step; };

enum Axis { X = 0, Y = 1, Z = 2 };
enum AlarmReasons { NoAlarm = 100, UserAlarm = 101, UnknownPosition = 102 };
enum EmergencyReasons { NoEmergency = 200, UserEmergency = 201, EndstopActivated = 202 };

AlarmReasons Alarm;
EmergencyReasons Emergency;

bool EmergencyModeActivated;
bool AlarmModeActivated;

void showGreetings()
{
    Serial.print("---LZakFw v");
    Serial.print(FW_VERSION);
    Serial.print(".");
    Serial.print(FW_REVISION);
    Serial.print(".");
    Serial.print(FW_MINOR_VERSION);
    Serial.println("---");
    Serial.println("by Isaac Candido (isaac.guimaraescandido@gmail.com)");
    Serial.print("Baud rate: ");
    Serial.println(BAUD_RATE);
}

void setup()
{
    Serial.begin(BAUD_RATE);
    
    pinMode(X_ENDSTOP, INPUT);
    pinMode(Y_ENDSTOP, INPUT);
    pinMode(Z_ENDSTOP, INPUT);
    
    pinMode(ENABLE_MOTORS_PIN, OUTPUT);
    digitalWrite(ENABLE_MOTORS_PIN, LOW);
    
    // liftAlarm();
    // liftEmergency();
    
    showGreetings();
}

void OtherCommands(int command) 
{ 
  // lift emergency mode (64 + 0)
  if(command & 0b01000000)
  {
    liftEmergency();
  }
  
  // lift alarm mode (64 + 1)
  if(command & 0b01000001)
  {
    liftAlarm();
  }
  
  // set emergency mode (64 + 2)
  if(command & 0b01000010)
  {
    setEmergency(UserEmergency);
  }
  
  // set alarm mode (64 + 3)
  if(command & 0b01000011)
  {
    setAlarm(UserAlarm);
  }
  
  // respond to ping (64 + 4)
  if(command & 0b01000100)
  {
    Serial.println("Pong!");
  }
}

void setEmergency(EmergencyReasons reason)
{
  Emergency = reason;
  EmergencyModeActivated = true;
  NotifyEmergencyStatus();
}

void setAlarm(AlarmReasons reason)
{
  Alarm = reason;
  AlarmModeActivated = true;
  NotifyAlarmStatus();
}

void liftEmergency()
{
  Emergency = NoEmergency;
  EmergencyModeActivated = false;
  NotifyEmergencyStatus();
}

void liftAlarm()
{
  Alarm = NoAlarm;
  AlarmModeActivated = false;
  NotifyAlarmStatus();
}

void NotifyEmergencyStatus()
{
  return;
  Serial.write(Emergency);
}

void NotifyAlarmStatus()
{
  return;
  Serial.write(Alarm);
}

bool AnyReasonNotToMove()
{
  if((Alarm == NoAlarm) && 
     (Emergency == NoEmergency) &&
     (EmergencyModeActivated) &&
     (AlarmModeActivated)) 
     {
       return false;
     }

  return true;
}

bool IsAnyEndstopActivated()
{
  if(digitalRead(X_ENDSTOP) > 0 ||
     digitalRead(Y_ENDSTOP) > 0 ||
     digitalRead(Z_ENDSTOP) > 0)
  {
      setEmergency(EndstopActivated);
      return true;
  }
  
  return false;
}

Instruction getInstruction(int command, Axis axis)
{
    Instruction result = { };

    if (axis == X)
    {
        (command & X_DIRECTION_VALUE) > 0 ? result.Direction = true : result.Direction = false;
        (command & X_STEP_VALUE) > 0 ? result.Step = true : result.Step = false;
    }

    if (axis == Y)
    {
        (command & Y_DIRECTION_VALUE) > 0 ? result.Direction = true : result.Direction = false;
        (command & Y_STEP_VALUE) > 0 ? result.Step = true : result.Step = false;
    }

    if (axis == Z)
    {
        (command & Z_DIRECTION_VALUE) > 0 ? result.Direction = true : result.Direction = false;
        (command & Z_STEP_VALUE) > 0 ? result.Step = true : result.Step = false;
    }

    return result;
}

void setDirection(Instruction receivedCommands[])
{
    digitalWrite(X_DIRECTION_PIN, receivedCommands[X].Direction);
    digitalWrite(Y_DIRECTION_PIN, receivedCommands[Y].Direction);
    digitalWrite(Z_DIRECTION_PIN, receivedCommands[Z].Direction);
}

void sendStepSignal(Instruction receivedCommands[])
{
    digitalWrite(X_STEP_PIN, receivedCommands[X].Step);
    digitalWrite(Y_STEP_PIN, receivedCommands[Y].Step);
    digitalWrite(Z_STEP_PIN, receivedCommands[Z].Step);
}

void sendResetSignal(Instruction receivedCommands[])
{
    digitalWrite(X_STEP_PIN, receivedCommands[X].Step);
    digitalWrite(Y_STEP_PIN, receivedCommands[Y].Step);
    digitalWrite(Z_STEP_PIN, receivedCommands[Z].Step);
}

void processCommand(Instruction receivedCommands[])
{
    setDirection(receivedCommands);
    delay(DELAY_BETWEEN_COMMANDS);
    sendStepSignal(receivedCommands);
    delay(DELAY_BETWEEN_COMMANDS);
    sendResetSignal(receivedCommands);
    delay(DELAY_BETWEEN_COMMANDS);
}

void loop()
{
  return;


    IsAnyEndstopActivated();
    
    if(Serial.available() <= 0) return;
    
    int receivedCommand = Serial.read();

    Serial.write(receivedCommand);

    return;
    
    if ((receivedCommand < 0b01000000))// && !AnyReasonNotToMove())
    { 
      Instruction resultArray[3];
      resultArray[X] = getInstruction(receivedCommand, X);
      resultArray[Y] = getInstruction(receivedCommand, Y);
      resultArray[Z] = getInstruction(receivedCommand, Z);
  
      processCommand(resultArray);
    }
    
    if(receivedCommand >= 0b01000000)
    {
      OtherCommands(receivedCommand);
    }
}
