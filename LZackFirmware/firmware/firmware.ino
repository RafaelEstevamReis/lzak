#define Y1_B 0b00000001
#define Y2_B 0b00000010
#define G1_B 0b00000100
#define G2_B 0b00001000
#define R1_B 0b00010000
#define R2_B 0b00100000

#define Y1_P 0
#define Y2_P 1
#define G1_P 2
#define G2_P 3
#define R1_P 4
#define R2_P 5

#define QTD_LEDS 6
#define ENDSTOP_THRESHOLD 500

int  pins[QTD_LEDS] =   { 9, 8, 7, 6, 5, 4 }; // arduino de vdd
bool values[QTD_LEDS];


void setup() 
{
  pinMode(A3, INPUT_PULLUP);
  
  for(int i = 0; i < QTD_LEDS;i++)
  {
    pinMode(pins[i], OUTPUT);
    values[i] = 0;
  }
  
  Serial.begin(9600);
}

bool wasLastLoopStopped = false;
void loop() 
{
  // Switch do endstop
  int endStopValue = analogRead(A3);

  if(endStopValue < ENDSTOP_THRESHOLD) 
  { 
    if(wasLastLoopStopped) return;

    Serial.print("Stopped!");
    wasLastLoopStopped = true;
    return;
  }
  else { wasLastLoopStopped = false; }
  
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
    }
  }

  // send instruction to motors
  pinAction(false);

  // power down pins
  pinAction(true);

  
  //Serial.print();
  delayMicroseconds(50);
}

void pinAction(bool reset)
{
  // Set Dirs
  for(int i = 1; i < QTD_LEDS; i+=2)
  {
    if(reset) values[i] = 0b00000000;
    
    digitalWrite(pins[i], values[i]);
    Serial.println(values[i]);
  }
  delayMicroseconds(50);
  // Do steps
  for(int i = 0; i < QTD_LEDS; i+=2)
  {
    if(reset) values[i] = 0b00000000;

    digitalWrite(pins[i], values[i]);
    Serial.println(values[i]);
  }
}
