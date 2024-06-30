#include <Arduino.h>
#include <WiFi.h>

// Modbus server TCP
#include "ModbusServerTCPasync.h"
#include "core.h"

#ifndef MY_SSID
#define MY_SSID "SmartGreenhouse"
#endif
#ifndef MY_PASS
#define MY_PASS "saaupbest"
#endif

char ssid[] = MY_SSID; // SSID and ...
char pass[] = MY_PASS; // password for the WiFi network used

// Create server
ModbusServerTCPasync ModbusServer;

uint16_t memo[32]; // Test server memory: 32 words

ModbusMessage readRegister(ModbusMessage request)
{
  ModbusMessage response; // The Modbus message we are going to give back
  uint16_t addr = 0;      // Start address
  uint16_t words = 0;     // # of words requested
  request.get(2, addr);   // read address from request
  request.get(4, words);  // read # of words from request

  // Set up response
  response.add(request.getServerID(), request.getFunctionCode(), (uint8_t)(words * 2));

  // if (addr == SRGO_INDEX_INPUTS)

  if (request.getFunctionCode() == READ_HOLD_REGISTER)
  {
    for (uint8_t i = 0; i < words; ++i)
    {
      response.add(REGISTERS[addr + i]);
    }
  }

  return response;
}

ModbusMessage writeRegister(ModbusMessage request)
{
  ModbusMessage response;
  uint16_t addr, wrds; // Start address to read; Number of words to read

  request.get(2, addr);
  request.get(4, wrds);

  REGISTERS[addr] = wrds;

  writeHandle(addr);

  return request;
}

void connect()
{
  WiFi.begin(ssid, pass);
  delay(200);
  while (WiFi.status() != WL_CONNECTED)
  {
    Serial.print(". ");
    delay(500);
  }
  IPAddress wIP = WiFi.localIP();
  Serial.printf("WIFi IP address: %u.%u.%u.%u\n", wIP[0], wIP[1], wIP[2], wIP[3]);
}

void setup()
{
  Serial.begin(115200);
  while (!Serial);

  Serial.println("__ OK __");
  connect();

  setupCore();

  ModbusServer.registerWorker(1, READ_HOLD_REGISTER, &readRegister);   // FC=03 for serverID=1
  ModbusServer.registerWorker(1, WRITE_HOLD_REGISTER, &writeRegister); // FC=04 for serverID=1
  ModbusServer.start(502, 6, 20000);
}

void loop()
{
  watchdog();
  updateRegisters();
  delay(100);
  // Serial.print("Clients: ");
  // Serial.println(ModbusServer.activeClients());

  // Serial.print("Errors: ");
  // Serial.println(ModbusServer.getErrorCount());

  // Serial.print("List: ");
  // ModbusServer.listServer();

}
