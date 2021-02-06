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
int  pins[QTD_LEDS] =   { 16, 5, 4, 0, 2, 14 };

bool values[QTD_LEDS];

void setup() 
{
  for(int i = 0; i < QTD_LEDS;i++)
  {
    pinMode(pins[i], OUTPUT);
    values[i] = 0;
  }
  
  Serial.begin(9600);
}

void loop() 
{
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

  for(int i = 0; i < QTD_LEDS; i++)
  {
    digitalWrite(pins[i], values[i]);
    Serial.println(values[i]);
  }
  
  //Serial.print();
  delayMicroseconds(50);
}
