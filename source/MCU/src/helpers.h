#include "FastLED.h"
#include <Arduino.h>

CRGB rgb565ToRgb888(uint16_t rgb565) {
    // Извлекаем компоненты
    uint8_t r5 = (rgb565 >> 11) & 0x1F;
    uint8_t g6 = (rgb565 >> 5) & 0x3F;
    uint8_t b5 = rgb565 & 0x1F;

    // Расширяем до 8 бит
    uint8_t red = (r5 * 527 + 23) >> 6;
    uint8_t green = (g6 * 259 + 33) >> 6;
    uint8_t blue = (b5 * 527 + 23) >> 6;
    
    return CRGB(red,green,blue);
}

uint16_t rgb888ToRgb565(CRGB rgb888) {

    uint16_t r = rgb888.r >> 3;    // Отбрасываем 3 младших бита
    uint16_t g = rgb888.g >> 2;  // Отбрасываем 2 младших бита
    uint16_t b = rgb888.b >> 3;   // Отбрасываем 3 младших бита

    // Комбинируем биты в одно значение RGB565
    return (r << 11) | (g << 5) | b;
}