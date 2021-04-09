#include <avr/wdt.h>

// Z Motor
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

#define ENABLE_MOTORS 8

#define QTD_PINS 6
#define ENDSTOP_THRESHOLD 500

#define BAUD_RATE 9600

//int  pins[QTD_PINS] =  { 9, 8, 7, 6, 5, 4 }; // arduino leonardo pro micro
//CNC Shield
//int  pins[QTD_PINS] =  { 5, 2, 6, 3, 7, 4 };
//int  pins[QTD_PINS] =  { 2, 5, 3, 6, 4, 7 };
//                      ZS ZD YS YD XS XD
int  pins[QTD_PINS] =  { 4, 7, 3, 6, 2, 5 };
bool values[QTD_PINS];

void printStatusSerial()
{
  Serial.println("---LZak Fw---");
  
  Serial.print("Baud rate: ");
  Serial.println(BAUD_RATE);
}

void setup() 
{  
  MCUSR = 0;
  
  Serial.begin(BAUD_RATE);
  printStatusSerial();

  pinMode(ENABLE_MOTORS, OUTPUT);
  digitalWrite(ENABLE_MOTORS, LOW);
  
  for(int i = 0; i < QTD_PINS;i++)
  {
    pinMode(pins[i], OUTPUT);
    values[i] = 0;
  }
}

void loop() 
{
  // Switch do endstop vai aqui

  if(Serial.available() > 0)
  {
    int received = Serial.read();

    if(received < 0b01000000){
      values[Y1_P] = (received & Y1_B) > 0;
      values[Y2_P] = (received & Y2_B) > 0;
      values[G1_P] = (received & G1_B) > 0;
      values[G2_P] = (received & G2_B) > 0;
      values[R1_P] = (received & R1_B) > 0;
      values[R2_P] = (received & R2_B) > 0;
    }
    else
    {
      // seção para outros comandos.
            
      // Remover os bits de acao
      int command = received & 0b00111111;
      if(command == 1)
      { // 0b0100.0001
        Serial.println("PONG");
      }
      if(command == 2)
      { // 0b0100.0010
        wdt_enable(WDTO_15MS); 
        while(true){}
      }
    }
  }

  // send instruction to motors
  pinPulse();

  //delayMicroseconds(50);
}

void resetArduino()
{
  
}

// Datasheet A4288
// pg6
// dir setup time: 200ns => 0,2us
// step min pulse: 1us

void pinPulse(){
  // Set Dirs
  for(int i = 1; i < QTD_PINS; i+=2){
    digitalWrite(pins[i], values[i]);
  }
  // wait dir setup time
  delayMicroseconds(50);
  
  // Do steps
  for(int i = 0; i < QTD_PINS; i+=2){
    // por definição, todos estão em LOW antes
    //if(values[i] == LOW) continue;
    digitalWrite(pins[i], values[i]);
  }
  // wait step pulse
  delayMicroseconds(50);
  // pulse LOW
  for(int i = 0; i < QTD_PINS; i+=2){
    digitalWrite(pins[i], 0);
  }
}
