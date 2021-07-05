// LZakFw
// Simple firmware for CNC control.
// by Isaac Candido
// e-mail: isaac.guimaraescandido@gmail.com

#define FW_VERSION 3
#define FW_REVISION 7
#define FW_MINOR_VERSION 0

#define X_DIRECTION_PIN 5
#define X_STEP_PIN 2
#define Y_DIRECTION_PIN 6
#define Y_STEP_PIN 3
#define Z_DIRECTION_PIN 7
#define Z_STEP_PIN 4

#define X_DIRECTION_VALUE 0b00000001 
#define X_STEP_VALUE 0b00000010
#define Y_DIRECTION_VALUE 0b00000100
#define Y_STEP_VALUE 0b00001000
#define Z_DIRECTION_VALUE 0b00010000
#define Z_STEP_VALUE 0b00100000

#define DELAY_BETWEEN_COMMANDS 1 // time in milliseconds
#define ENABLE_MOTORS_PIN 8 // pin for powering up motors
#define BAUD_RATE 38400 // communication speed.

#define CMD_LIFT_EMERGENCY 64
#define CMD_LIFT_ALARM 65
#define CMD_SET_EMERGENCY 66
#define CMD_SET_ALARM 67
#define CMD_HEALTH_CHECK_PING_PONG 68
#define CMD_ENABLE_MOTORS 68
#define CMD_DISABLE_MOTORS 68

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

void setup()
{
    Serial.begin(BAUD_RATE);

    pinMode(X_ENDSTOP, INPUT);
    pinMode(Y_ENDSTOP, INPUT);
    pinMode(Z_ENDSTOP, INPUT);

    pinMode(X_STEP_PIN, OUTPUT);
    pinMode(Y_STEP_PIN, OUTPUT);
    pinMode(Z_STEP_PIN, OUTPUT);

    pinMode(X_DIRECTION_PIN, OUTPUT);
    pinMode(Y_DIRECTION_PIN, OUTPUT);
    pinMode(Z_DIRECTION_PIN, OUTPUT);

    pinMode(ENABLE_MOTORS_PIN, OUTPUT);
    digitalWrite(ENABLE_MOTORS_PIN, LOW);

    liftAlarm();
    liftEmergency();

    showGreetings();
}

void showGreetings()
{
    Serial.print("---LZakFw v");
    Serial.print(FW_VERSION);
    Serial.print(".");
    Serial.print(FW_REVISION);
    Serial.print(".");
    Serial.print(FW_MINOR_VERSION);
    Serial.println("---");
    Serial.println("by Isaac Candido");
    Serial.println("(isaac.guimaraescandido@gmail.com)");
    Serial.print("Baud rate: ");
    Serial.println(BAUD_RATE);
}

void OtherCommands(int command)
{
    // lift emergency mode (64 + 0)
    if ((command & 0b00100000) > 0) liftEmergency();

    // lift alarm mode (64 + 1)
    if ((command & 0b00100001) > 0) liftAlarm();

    // set emergency mode (64 + 2)
    if ((command & 0b00100010) > 0) setEmergency(UserEmergency);

    // set alarm mode (64 + 3)
    if ((command & 0b00100011) > 0) setAlarm(UserAlarm);

    // respond to ping (64 + 4)
    if ((command & 0b00100100) > 0) Serial.println("Pong");

    // disable Motors (64 + 5)
    if ((command & 0b00100101) > 0) digitalWrite(ENABLE_MOTORS_PIN, HIGH);

    // enable Motors (64 + 6)
    if ((command & 0b00100110) > 0) digitalWrite(ENABLE_MOTORS_PIN, LOW);
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
    Serial.write(Emergency);
}

void NotifyAlarmStatus()
{
    Serial.write(Alarm);
}

bool AnyReasonNotToMove()
{
    if (Alarm != NoAlarm) return true;
    if (Emergency != NoEmergency) return true;
    if (EmergencyModeActivated) return true;
    if (AlarmModeActivated) return true;

    return false;
}

bool IsAnyEndstopActivated()
{
    if (digitalRead(X_ENDSTOP) > 0) return true;
    if (digitalRead(Y_ENDSTOP) > 0) return true;
    if (digitalRead(Z_ENDSTOP) > 0) return true;
    return false;
}

Instruction getInstruction(int command, Axis axis)
{
    Instruction result { };

    if(axis == X)
    {
      result.Direction = (command & 0b0000001) > 0;
      result.Step = (command & 0b00000010) > 0;
    }

    if(axis == Y)
    {
      result.Direction = (command & 0b00000100) > 0;
      result.Step = (command & 0b00001000) > 0; 
    }

    if(axis == Z)
    {
      result.Direction = (command & 0b0010000) > 0;
      result.Step = (command & 0b00100000) > 0;
    }
    
    return result;
}

void setDirection(Instruction receivedCommands[])
{
    digitalWrite(X_DIRECTION_PIN, receivedCommands[X].Direction ? HIGH : LOW);
    digitalWrite(Y_DIRECTION_PIN, receivedCommands[Y].Direction ? HIGH : LOW);
    digitalWrite(Z_DIRECTION_PIN, receivedCommands[Z].Direction ? HIGH : LOW);
}

void sendStepSignal(Instruction receivedCommands[])
{
    digitalWrite(X_STEP_PIN, receivedCommands[X].Step ? HIGH : LOW);
    digitalWrite(Y_STEP_PIN, receivedCommands[Y].Step ? HIGH : LOW);
    digitalWrite(Z_STEP_PIN, receivedCommands[Z].Step ? HIGH : LOW);
}

void sendResetSignal()
{
    digitalWrite(X_STEP_PIN, LOW);
    digitalWrite(Y_STEP_PIN, LOW);
    digitalWrite(Z_STEP_PIN, LOW);
}

void processCommand(Instruction receivedCommands[])
{
    setDirection(receivedCommands);
    delay(DELAY_BETWEEN_COMMANDS);
    sendStepSignal(receivedCommands);
    delay(DELAY_BETWEEN_COMMANDS);
    sendResetSignal();
    delay(DELAY_BETWEEN_COMMANDS);
}

void loop()
{
    //if(IsAnyEndstopActivated()) setEmergency(EndstopActivated);

    if (Serial.available() <= 0) return;

    int receivedCommand = Serial.read();

    if (receivedCommand < 64)// && !AnyReasonNotToMove())
    {
        Instruction resultArray[3];

        resultArray[X] = getInstruction(receivedCommand, X);
        resultArray[Y] = getInstruction(receivedCommand, Y);
        resultArray[Z] = getInstruction(receivedCommand, Z);

        processCommand(resultArray);

        return;
    }
    
    if(receivedCommand < 200) 
    {   
        OtherCommands(receivedCommand);
    }
}