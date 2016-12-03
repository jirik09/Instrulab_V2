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

#include "stm32l4xx_hal.h"
#include "stm32l4xx_nucleo.h"
#include "firmware_version.h"
//#include "usb_device.h"
#include "math.h"
#include "err_list.h"

#define IDN_STRING "STM32L476-Nucleo" //max 30 chars
#define MCU "STM32L476RG"

// Communication constatnts ===================================================
#define COMM_BUFFER_SIZE 512
#define UART_SPEED 115200

#define USART_GPIO GPIOA
#define USART_TX GPIO_PIN_2
#define USART_RX GPIO_PIN_3
#define USART_TX_PIN_STR "PA2_" //must be 4 chars
#define USART_RX_PIN_STR "PA3_" //must be 4 chars 

#define USB_DP_PIN_STR "PA12" //must be 4 chars
#define USB_DM_PIN_STR "PA11" //must be 4 chars

// Scope constatnts ===========================================================
#define MAX_SAMPLING_FREQ 5000000 //smps
#define MAX_ADC_CHANNELS 3

#define MAX_SCOPE_BUFF_SIZE 60000//50000 //in bytes
#define SCOPE_BUFFER_MARGIN 100

#define SCOPE_CH1_PIN_STR "A5__" //must be 4 chars
#define SCOPE_CH2_PIN_STR "A4__" //must be 4 chars
#define SCOPE_CH3_PIN_STR "A3__" //must be 4 chars
#define SCOPE_CH4_PIN_STR "----" //must be 4 chars

#define SCOPE_VREF 3300
#define SCOPE_VREF_INT (uint16_t)*((uint16_t *)0x1FFF75AA)

#define RANGE_1_LOW 0
#define RANGE_1_HI SCOPE_VREF
#define RANGE_2_LOW -SCOPE_VREF
#define RANGE_2_HI SCOPE_VREF*2
#define RANGE_3_LOW 0
#define RANGE_3_HI 0
#define RANGE_4_LOW 0
#define RANGE_4_HI 0


//scope channels inputs
static const uint8_t ANALOG_DEFAULT_INPUTS[MAX_ADC_CHANNELS]={0,2,1};
static const uint8_t ANALOG_VREF_INPUTS[MAX_ADC_CHANNELS]={8,255,255};

#define ADC1_NUM_CHANNELS 9
static const uint16_t ANALOG_PIN_ADC1[ADC1_NUM_CHANNELS] = {				GPIO_PIN_0,			GPIO_PIN_1,			GPIO_PIN_0,			GPIO_PIN_1,			GPIO_PIN_0,			GPIO_PIN_7,			GPIO_PIN_6,		  0,							0};
static GPIO_TypeDef * ANALOG_GPIO_ADC1[ADC1_NUM_CHANNELS] = {				GPIOC,					GPIOC,					GPIOB,					GPIOA,					GPIOA,					GPIOA,					GPIOA,					0,							0};
static const uint32_t ANALOG_CHANNEL_ADC1[ADC1_NUM_CHANNELS] = {		ADC_CHANNEL_1,	ADC_CHANNEL_2,	ADC_CHANNEL_15,	ADC_CHANNEL_6,	ADC_CHANNEL_5,	ADC_CHANNEL_12,	ADC_CHANNEL_11, ADC_CHANNEL_17, ADC_CHANNEL_0};
static const char* ANALOG_CHANN_ADC1_NAME[ADC1_NUM_CHANNELS] = { 		"A5", 					"A4", 					"A3", 					"A1", 					"A0", 					"D11", 					"D12", 				  "Temp", 				"Vref" };

#define ADC2_NUM_CHANNELS 7
static const uint16_t ANALOG_PIN_ADC2[ADC2_NUM_CHANNELS] = {				GPIO_PIN_0,			GPIO_PIN_1,			GPIO_PIN_0,			GPIO_PIN_1,			GPIO_PIN_0,			GPIO_PIN_7,			GPIO_PIN_6,		  };
static GPIO_TypeDef * ANALOG_GPIO_ADC2[ADC2_NUM_CHANNELS] = {				GPIOC,					GPIOC,					GPIOB,					GPIOA,					GPIOA,					GPIOA,					GPIOA,					};
static const uint32_t ANALOG_CHANNEL_ADC2[ADC2_NUM_CHANNELS] = {		ADC_CHANNEL_1,	ADC_CHANNEL_2,	ADC_CHANNEL_15,	ADC_CHANNEL_6,	ADC_CHANNEL_5,	ADC_CHANNEL_12,	ADC_CHANNEL_11, };
static const char* ANALOG_CHANN_ADC2_NAME[ADC2_NUM_CHANNELS] = { 		"A5", 					"A4", 					"A3", 					"A1", 					"A0", 					"D11", 					"D12", 				 	};

#define ADC3_NUM_CHANNELS 2
static const uint16_t ANALOG_PIN_ADC3[ADC3_NUM_CHANNELS] = {				GPIO_PIN_0,			GPIO_PIN_1		};
static GPIO_TypeDef * ANALOG_GPIO_ADC3[ADC3_NUM_CHANNELS] = {				GPIOC,					GPIOC					};
static const uint32_t ANALOG_CHANNEL_ADC3[ADC3_NUM_CHANNELS] = {		ADC_CHANNEL_1,	ADC_CHANNEL_2	};
static const char* ANALOG_CHANN_ADC3_NAME[ADC3_NUM_CHANNELS] = { 		"A5", 					"A4", 			  };


static const uint8_t NUM_OF_ANALOG_INPUTS[MAX_ADC_CHANNELS]={ADC1_NUM_CHANNELS,ADC2_NUM_CHANNELS,ADC3_NUM_CHANNELS}; //number of ADC channels {ADC1,ADC2,ADC3,ADC4}
// Generator constatnts =======================================================

#define MAX_GENERATING_FREQ 2000000 //smps
#define MAX_DAC_CHANNELS 2
#define MAX_GENERATOR_BUFF_SIZE 10000//2000
#define	DAC_DATA_DEPTH 12

#define GEN_VREF 3300
#define GEN_VREF_INT 1200

#define GEN_CH1_PIN_STR "A2__" //must be 4 chars
#define GEN_CH2_PIN_STR "D13_" //must be 4 chars



//Definition of assert to check length of strings
#define CASSERT(ex) {typedef char cassert_type[(ex)?1:-1];}

#endif /* STM32F4_CONFIG_H_ */
