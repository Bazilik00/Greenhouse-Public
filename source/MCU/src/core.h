#pragma once
#include "pins.h"
#include <Arduino.h>
#include "modbusRegisters.h"
#include "DHT.h"
#include "FastLED.h"
#include "helpers.h"
#include <ESP32Servo.h>

void updateRGB();

void setPumpPower();

void setHumidifierPower();

Servo servo;
DHT dht(PIN_AM2301, DHT21);
DHT dhtOutside(PIN_AM2301_OUTSIDE, DHT21);
CRGB leds[NUM_RGB_LEDS];

int watchdogPump;

void setDefaultValues()
{
    REGISTER_MAX_PUMP_POWER_TICKS = 50;
    REGISTER_RGB_BRIGHTNESS = 100;
    REGISTER_RGB_COLOR = 0x00FF;
    updateRGB();
}

void setupCore()
{
    dht.begin();
    dhtOutside.begin();
    pinMode(PIN_LEVEL_WATER, INPUT);
    pinMode(PIN_SOIL, INPUT);
    pinMode(PIN_LIGHT, INPUT);
    pinMode(PIN_LIGHT_OUTSIDE, INPUT);
    pinMode(PIN_RGB, OUTPUT);
    pinMode(PIN_FAN, OUTPUT);
    pinMode(PIN_PUMP, OUTPUT);
    pinMode(PIN_HUMIDIFIER, OUTPUT);

    FastLED.addLeds<WS2812B, PIN_RGB, GRB>(leds, NUM_RGB_LEDS).setCorrection(TypicalLEDStrip);
  

    setDefaultValues();
    updateRGB();

    servo.attach(PIN_SERVO);
}

int getTemperature()
{
    auto result = uint16_t(dht.readTemperature() * 10);
    return result;
}

int getHumidity()
{
    auto result = uint16_t(dht.readHumidity() * 10);
    return result;
}

int getLevelWater()
{
    auto result = digitalRead(PIN_LEVEL_WATER);
    return result;
}

int getTemperatureOutside()
{
    auto result = uint16_t(dhtOutside.readTemperature() * 10);
    return result;
}

int getHumidityOutside()
{
    auto result = uint16_t(dhtOutside.readHumidity() * 10);
    return result;
}

int getSoil()
{
    auto data = analogRead(PIN_SOIL);
    return map(data, 4095, 0, 0, 100);
}

int getLight()
{
    auto data = analogRead(PIN_LIGHT);
    return map(data, 0, 4095, 0, 100);
}

int getLightOutside()
{
    auto data = analogRead(PIN_LIGHT_OUTSIDE);
    return map(data, 0, 4095, 0, 100);
}

bool getHasPlant()
{
    return analogRead(PIN_SOIL) == 4095 ? 0 : 1;
}

void updateState()
{
    auto fan = bitRead(REGISTER_STATE, BIT_STATE_FAN);
    digitalWrite(PIN_FAN, fan);

    setPumpPower();

    setHumidifierPower();

    if (bitRead(REGISTER_STATE, BIT_STATE_SERVO) == 1)
    {
        if (WiFi.macAddress() == "FC:B4:67:50:BA:38")
        {
            servo.write(180);
        }
  
        else{
            servo.write(0);
        }
    }
    else
    {
         if (WiFi.macAddress() == "FC:B4:67:50:BA:38")
        {
            servo.write(0);
        }
  
        else{
            servo.write(180);
        }
    }
}

void setHumidifierPower()
{
    auto humidifier = bitRead(REGISTER_STATE, BIT_STATE_HUMIDIFIER);

    if (humidifier == 0)
    {
        digitalWrite(PIN_HUMIDIFIER, LOW);
    }
    else if (REGISTER_SYNCHRONIZED_LEVEL_WATER == 1)
    {
        digitalWrite(PIN_HUMIDIFIER, HIGH);
    }
    else
    {
        bitClear(REGISTER_STATE, BIT_STATE_HUMIDIFIER);
        digitalWrite(PIN_HUMIDIFIER, LOW);
    }
}

void setPumpPower()
{
    auto pump = bitRead(REGISTER_STATE, BIT_STATE_PUMP);

    if (pump == 0)
    {
        digitalWrite(PIN_PUMP, LOW);
    }
    else if (REGISTER_SYNCHRONIZED_LEVEL_WATER == 1)
    {
        digitalWrite(PIN_PUMP, HIGH);
    }
    else
    {
        bitClear(REGISTER_STATE, BIT_STATE_PUMP);
        digitalWrite(PIN_PUMP, LOW);
    }
}

void updateRGB()
{
    if (bitRead(REGISTER_STATE, BIT_STATE_RGB_POWER) == 0)
    {
        FastLED.setBrightness(0);
        FastLED.show();
        return;
    }

    FastLED.setBrightness(REGISTER_RGB_BRIGHTNESS);

    for (int i = 0; i < NUM_RGB_LEDS; i++)
    {
        if (bitRead(REGISTER_STATE, BIT_STATE_RGB_MODE))
        {
            if (i % 3 == 0)

            {
                leds[i] = rgb565ToRgb888(0x001f);
            }
            else
                leds[i] = rgb565ToRgb888(0xf800);
        }
        else
            leds[i] = rgb565ToRgb888(REGISTER_RGB_COLOR);
    }

    FastLED.show();
}

void writeHandle(uint16_t addr)
{
    switch (addr)
    {
    case INDEX_RGB_COLOR:
        updateRGB();
        break;
    case INDEX_RGB_BRIGHTNESS:
        updateRGB();
        break;
    case INDEX_STATE:
        updateRGB();
        updateState();
        break;

    default:
        break;
    }
}

void updateRegisters()
{
    REGISTER_TEMPERATURE = getTemperature();
    REGISTER_HUMIDITY = getHumidity();
    REGISTER_LEVEL_WATER = getLevelWater();
    REGISTER_SOIL = getSoil();
    REGISTER_LIGHT = getLight();
    REGISTER_LIGHT_OUTSIDE = getLightOutside();
    REGISTER_HASPLANT = getHasPlant();
    REGISTER_TEMPERATURE_OUTSIDE = getTemperatureOutside();
    REGISTER_HUMIDITY_OUTSIDE = getHumidityOutside();
}

void watchdog()
{
    if (bitRead(REGISTER_STATE, BIT_STATE_PUMP) == 1)
    {
        watchdogPump++;
    }
    else
    {
        watchdogPump = 0;
    }

    if (watchdogPump >= REGISTER_MAX_PUMP_POWER_TICKS)
    {
        watchdogPump = 0;
        bitClear(REGISTER_STATE, BIT_STATE_PUMP);
        digitalWrite(PIN_PUMP, LOW);
    }

    if (REGISTER_SYNCHRONIZED_LEVEL_WATER == 0)
    {
        bitClear(REGISTER_STATE, BIT_STATE_HUMIDIFIER);
        setHumidifierPower();
    }
}
