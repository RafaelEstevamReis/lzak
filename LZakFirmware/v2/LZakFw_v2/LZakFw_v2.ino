// LZakFw v2.0
// Simple firmware for CNC control.

#define Y1_B 0b00000001
#define Y2_B 0b00000010
#define Y1_P 0
#define Y2_P 1

// Y Motor
#define G1_B 0b00000100
#define G2_B 0b00001000
#define G1_P 2
#define G2_P 3

// X Motor
#define R1_B 0b00010000
#define R2_B 0b00100000
#define R1_P 4
#define R2_P 5

#define ENABLE_MOTOR_PIN 8
#define SESSION_BAUD_RATE 9600
#define QTD_PINS 6

// S = step signal
// D = direction signal
// Sinais por eixo 
int  PIN_NUMBERS[QTD_PINS] = { 4, 7, 3, 6, 2, 5 };
bool PIN_VALUES[QTD_PINS];

void setup()
{
  Serial.begin(SESSION_BAUD_RATE);
  printInfo();

  pinMode(ENABLE_MOTOR_PIN, OUTPUT);
  digitalWrite(ENABLE_MOTOR_PIN, LOW);
  
  for(int i = 0; i < QTD_PINS;i++){
    pinMode(PIN_NUMBERS[i], OUTPUT);
    PIN_VALUES[i] = 0;
  }
}

/// Print 
void printInfo()
{
  Serial.println("---LZakFw v2.0---");
  Serial.println("by Isaac Candido (isaac.guimaraescandido@gmail.com");
  Serial.print("Baud rate: ");
  Serial.println(SESSION_BAUD_RATE);
}

void loop() 
{
   if(Serial.available() > 0)
  {
    int received = Serial.read();

    // motor commands
    if(received < 0b01000000){
      PIN_VALUES[Y1_P] = (received & Y1_B) > 0;
      PIN_VALUES[Y2_P] = (received & Y2_B) > 0;
      PIN_VALUES[G1_P] = (received & G1_B) > 0;
      PIN_VALUES[G2_P] = (received & G2_B) > 0;
      PIN_VALUES[R1_P] = (received & R1_B) > 0;
      PIN_VALUES[R2_P] = (received & R2_B) > 0;
    }
    // section for other commands
    else{
      // add commands here
    }

    pinPulse();
  }
}

// Datasheet A4288
// pg6
// dir setup time: 200ns => 0,2us
// step min pulse: 1us
void pinPulse(){
  // Set Dirs
  for(int i = 1; i < QTD_PINS; i+=2){
    digitalWrite(PIN_NUMBERS[i], PIN_VALUES[i]);
  }
  // wait dir setup time
  delayMicroseconds(50);
  
  // Do steps
  for(int i = 0; i < QTD_PINS; i+=2){
    digitalWrite(PIN_NUMBERS[i], PIN_VALUES[i]);
  }
  // wait step pulse
  delayMicroseconds(50);

  // reset pins - low
  for(int i = 0; i < QTD_PINS; i+=2){
    digitalWrite(PIN_NUMBERS[i], 0);
  }
}
