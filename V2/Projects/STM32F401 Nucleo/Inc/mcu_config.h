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

#include "stm32f4xx_hal.h"
#include "stm32f4xx_nucleo.h"
#include "firmware_version.h"
//#include "usb_device.h"
#include "math.h"
#include "err_list.h"

#define IDN_STRING "STM32F401-Nucleo" //max 30 chars
#define MCU "STM32F401RE"

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

// Scope constatnts ===================================================
#define MAX_SAMPLING_FREQ 2000000 //smps
#define MAX_ADC_CHANNELS 1

#define MAX_SCOPE_BUFF_SIZE 60000 //in bytes
#define SCOPE_BUFFER_MARGIN 100

#define SCOPE_CH1_PIN_STR "A5__" //must be 4 chars
#define SCOPE_CH2_PIN_STR "----" //must be 4 chars
#define SCOPE_CH3_PIN_STR "----" //must be 4 chars
#define SCOPE_CH4_PIN_STR "----" //must be 4 chars

#define SCOPE_VREF 3300
#define SCOPE_VREF_INT (uint16_t)*((uint16_t *)0x1FFF7A2A)

#define RANGE_1_LOW 0
#define RANGE_1_HI SCOPE_VREF
#define RANGE_2_LOW -SCOPE_VREF
#define RANGE_2_HI SCOPE_VREF*2
#define RANGE_3_LOW 0
#define RANGE_3_HI 0
#define RANGE_4_LOW 0
#define RANGE_4_HI 0

//scope channels inputs
static const uint8_t ANALOG_DEFAULT_INPUTS[MAX_ADC_CHANNELS]={0};
static const uint8_t ANALOG_VREF_INPUTS[MAX_ADC_CHANNELS]={6};

#define ADC1_NUM_CHANNELS 7
static const uint16_t ANALOG_PIN_ADC1[ADC1_NUM_CHANNELS] = {				GPIO_PIN_0,			GPIO_PIN_1,			GPIO_PIN_0,			GPIO_PIN_1,			GPIO_PIN_0,			 0,							0};
static GPIO_TypeDef * ANALOG_GPIO_ADC1[ADC1_NUM_CHANNELS] = {				GPIOC,					GPIOC,					GPIOB,					GPIOA,					GPIOA,					 0,							0};
static const uint32_t ANALOG_CHANNEL_ADC1[ADC1_NUM_CHANNELS] = {		ADC_CHANNEL_10,	ADC_CHANNEL_11,	ADC_CHANNEL_8,	ADC_CHANNEL_1,	ADC_CHANNEL_0,	 ADC_CHANNEL_16, ADC_CHANNEL_17};
static const char* ANALOG_CHANN_ADC1_NAME[ADC1_NUM_CHANNELS] = { 		"A5", 					"A4", 					"A3", 					"A1", 					"A0", 					 "Temp", 				"Vref" };



static const uint8_t NUM_OF_ANALOG_INPUTS[MAX_ADC_CHANNELS]={ADC1_NUM_CHANNELS}; //number of ADC channels {ADC1,ADC2,ADC3,ADC4}


// Generator constatnts ===================================================

#define MAX_GENERATING_FREQ 2000000 //smps
#define MAX_DAC_CHANNELS 0
#define MAX_GENERATOR_BUFF_SIZE 2000
#define	DAC_DATA_DEPTH 12

#define GEN_VREF 3300
#define GEN_VREF_INT 1200

#define GEN_CH1_PIN_STR "----" //must be 4 chars
#define GEN_CH2_PIN_STR "----" //must be 4 chars

//Definition of assert to check length of strings
#define CASSERT(ex) {typedef char cassert_type[(ex)?1:-1];}


/* Definition of ADC and DMA for channel 1 */
//#define ADC_CH_1_CLK_EN() __ADC1_CLK_ENABLE()
//#define ADC_CH_1_CLK_DIS() __ADC1_CLK_DISABLE()
//#define GPIO_ADC_CH_1_CLK_EN() __GPIOC_CLK_ENABLE()
//#define ADC_CH_1  ADC1 //
//#define ADC_GPIO_CH_1  GPIOC
//#define ADC_PIN_CH_1  GPIO_PIN_1
//#define ADC_CHANNEL_CH_1  ADC_CHANNEL_11 //
//#define ADC_DMA_CHANNEL_CH_1  DMA_CHANNEL_0 //
//#define ADC_DMA_STREAM_CH_1  DMA2_Stream0 //

#endif /* STM32F4_CONFIG_H_ */
