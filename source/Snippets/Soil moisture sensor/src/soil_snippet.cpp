// #include <Arduino.h>

// int sensor_pin = 34;
// int data;

// void setup() {
//  Serial.begin(115200);
//  pinMode(sensor_pin, INPUT);                      // Инициируем передачу данных по последовательному порту на скорости 9600 бот.
// }

// void loop() {
//   data = analogRead(sensor_pin); // Считываем показания с датчика
//   auto data2 = map(data, 4095, 0, 0, 100); // Переопределяем показания в процентное соотношение
//   Serial.print("Влажность: "); 
//   Serial.print(data2); 
//   Serial.println("%");
//   delay(1000);                            // приостанавливаем выполнение программы на 5 секунд.
// }
