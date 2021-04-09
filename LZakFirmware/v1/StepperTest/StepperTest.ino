#define X_DIR_PIN  5
#define X_STEP_PIN 2

#define Y_DIR_PIN  6
#define Y_STEP_PIN 7

#define ENABLE_MOTORS 8

void setup() {
  // put your setup code here, to run once:
  pinMode(X_DIR_PIN, OUTPUT);
  pinMode(X_STEP_PIN, OUTPUT);

  pinMode(Y_DIR_PIN, OUTPUT);
  pinMode(Y_STEP_PIN, OUTPUT);

  pinMode(ENABLE_MOTORS, OUTPUT);
  digitalWrite(ENABLE_MOTORS, 0);
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
  
  digitalWrite(X_DIR_PIN, dir);
  delay(1);
  digitalWrite(X_STEP_PIN, 1);
  delay(1);
  digitalWrite(X_STEP_PIN, 0);
}
