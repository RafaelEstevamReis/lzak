#define DIR_PIN  4
#define STEP_PIN 5

void setup() {
  // put your setup code here, to run once:
  pinMode(DIR_PIN, OUTPUT);
  pinMode(STEP_PIN, OUTPUT);
}

int steps = 0;
int dir = 0;

void loop() {
  // put your main code here, to run repeatedly:
  delay(1);

  if(steps++ >= 200) {
    steps = 0;
    
    if(dir == 1) dir = 0;
    else dir = 1;
  }
  digitalWrite(DIR_PIN, dir);
  delay(1);
  digitalWrite(STEP_PIN, 1);
  delay(1);
  digitalWrite(STEP_PIN, 0);
  
}
