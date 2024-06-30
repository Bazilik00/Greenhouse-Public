#include <Arduino.h>

int InPin = 34;
int Free = 0; 

void setup() {
  Serial.begin(115200);                       
  pinMode(InPin, INPUT);                       
}

void loop() {
  Free = digitalRead(InPin);                
  Serial.print("Everything is clear: ");    // Получаем значения 0 или 1
  if (Free == 1)                            // Если 1 - препятствия нет
  {
  Serial.println("Yes");                    
  } else {Serial.println("No");}
  delay(100);                           
}


