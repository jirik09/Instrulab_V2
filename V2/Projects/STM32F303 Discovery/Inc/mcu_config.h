/*
  *****************************************************************************
  * @file    mcu_config.h
  * @author  Y3288231
  * @date    jan 15, 2014
  * @brief   Hardware abstraction for communication
  ***************************************************************************** 
*/ 
#ifndef STM32F4_CONFIG_H_
#define STM32F4_CONFIG_H_

#include "stm32f3xx_hal.h"
#include "stm32f3_discovery.h"
#include "firmware_version.h"
//#include "usb_device.h"
#include "math.h"
#include "err_list.h"

#define IDN_STRING "STM32F3-Discovery" //max 30 chars
#define MCU "STM32F303VT"

// Communication constatnts ===================================================
#define COMM_BUFFER_SIZE 256
#define UART_SPEED 115200

#define USART_GPIO GPIOA
#define USART_TX GPIO_PIN_2
#define USART_RX GPIO_PIN_3
#define USART_TX_PIN_STR "PA2_" //must be 4 chars
#define USART_RX_PIN_STR "PA3_" //must be 4 chars 

#define USB_DP_PIN_STR "PA12" //must be 4 chars
#define USB_DM_PIN_STR "PA11" //must be 4 chars

// Scope constatnts ===================================================
#define MAX_SAMPLING_FREQ 4000000 //smps
#define MAX_ADC_CHANNELS 4

#define MAX_SCOPE_BUFF_SIZE 20000 //in bytes
#define SCOPE_BUFFER_MARGIN 350

#define SCOPE_CH1_PIN_STR "PC0_" //must be 4 chars
#define SCOPE_CH2_PIN_STR "PC1_" //must be 4 chars
#define SCOPE_CH3_PIN_STR "PB13" //must be 4 chars
#define SCOPE_CH4_PIN_STR "PB12" //must be 4 chars

#define SCOPE_VREF 3300

#define RANGE_1_LOW 0
#define RANGE_1_HI SCOPE_VREF
#define RANGE_2_LOW -SCOPE_VREF
#define RANGE_2_HI SCOPE_VREF*2
#define RANGE_3_LOW 0
#define RANGE_3_HI 0
#define RANGE_4_LOW 0
#define RANGE_4_HI 0


// Generator constatnts ===================================================
#define MAX_GENERATING_FREQ 2000000 //smps
#define MAX_DAC_CHANNELS 2
#define MAX_GENERATOR_BUFF_SIZE 2000
#define	DAC_DATA_DEPTH 12

#define GEN_VREF 3300

#define GEN_CH1_PIN_STR "PA4_" //must be 4 chars
#define GEN_CH2_PIN_STR "PA5_" //must be 4 chars



//Definition of assert to check length of strings
#define CASSERT(ex) {typedef char cassert_type[(ex)?1:-1];}


#endif /* STM32F4_CONFIG_H_ */
