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
#include "stm32f3xx_nucleo_32.h"
#include "firmware_version.h"
//#include "usb_device.h"
#include "math.h"
#include "err_list.h"

#define IDN_STRING "STM32F303-Nucleo-32" //max 30 chars
#define MCU "STM32F303K8"

// Communication constatnts ===================================================
#define COMM_BUFFER_SIZE 256
#define UART_SPEED 115200

#define USART_TX_PIN_STR "PA2_" //must be 4 chars
#define USART_RX_PIN_STR "PA15" //must be 4 chars 

#define USB_DP_PIN_STR "----" //must be 4 chars
#define USB_DM_PIN_STR "----" //must be 4 chars

// Scope constatnts ===================================================
#define MAX_SAMPLING_FREQ 4000000 //smps
#define MAX_ADC_CHANNELS 2

#define MAX_SCOPE_BUFF_SIZE 2000 //in bytes
#define SCOPE_BUFFER_MARGIN 50

#define SCOPE_CH1_PIN_STR "A2__" //must be 4 chars
#define SCOPE_CH2_PIN_STR "A3__" //must be 4 chars
#define SCOPE_CH3_PIN_STR "----" //must be 4 chars
#define SCOPE_CH4_PIN_STR "----" //must be 4 chars

#define SCOPE_VREF 3300

#define RANGE_1_LOW 0
#define RANGE_1_HI SCOPE_VREF
#define RANGE_2_LOW -SCOPE_VREF
#define RANGE_2_HI SCOPE_VREF*2
#define RANGE_3_LOW 0
#define RANGE_3_HI 0
#define RANGE_4_LOW 0
#define RANGE_4_HI 0



#define MAX_GENERATING_FREQ 2000000 //smps
#define MAX_DAC_CHANNELS 2
#define MAX_GENERATOR_BUFF_SIZE 800
#define	DAC_DATA_DEPTH 12

#define GEN_VREF 3300

#define GEN_CH1_PIN_STR "A4__" //must be 4 chars
#define GEN_CH2_PIN_STR "A5__" //must be 4 chars


//Definition of assert to check length of strings
#define CASSERT(ex) {typedef char cassert_type[(ex)?1:-1];}




#endif /* STM32F4_CONFIG_H_ */
