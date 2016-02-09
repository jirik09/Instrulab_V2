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
#include "stm32f4_discovery.h"
#include "firmware_version.h"
//#include "usb_device.h"
#include "math.h"
#include "err_list.h"

#define IDN_STRING "STM32F4-Discovery" //max 30 chars
#define MCU "STM32F407VG"

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
#define MAX_ADC_CHANNELS 3

#define MAX_SCOPE_BUFF_SIZE 100000 //in bytes
#define SCOPE_BUFFER_MARGIN 100

#define SCOPE_CH1_PIN_STR "PC1_" //must be 4 chars
#define SCOPE_CH2_PIN_STR "PC2_" //must be 4 chars
#define SCOPE_CH3_PIN_STR "PC3_" //must be 4 chars
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
#define MAX_GENERATOR_BUFF_SIZE 8000
#define	DAC_DATA_DEPTH 12

#define GEN_VREF 3300

#define GEN_CH1_PIN_STR "PA4_" //must be 4 chars
#define GEN_CH2_PIN_STR "PA5_" //must be 4 chars


// Definition of ADC and DMA for channel 1 
#define ADC_CH_1_CLK_EN() __ADC1_CLK_ENABLE()
#define ADC_CH_1_CLK_DIS() __ADC1_CLK_DISABLE()
#define GPIO_ADC_CH_1_CLK_EN() __GPIOC_CLK_ENABLE()
#define ADC_CH_1  ADC1 //
#define ADC_GPIO_CH_1  GPIOC
#define ADC_PIN_CH_1  GPIO_PIN_1
#define ADC_CHANNEL_CH_1  ADC_CHANNEL_11 //
#define ADC_DMA_CHANNEL_CH_1  DMA_CHANNEL_0 //
#define ADC_DMA_STREAM_CH_1  DMA2_Stream0 //

// Definition of ADC and DMA for channel 2 
#define ADC_CH_2_CLK_EN() __ADC2_CLK_ENABLE()
#define ADC_CH_2_CLK_DIS() __ADC2_CLK_DISABLE()
#define GPIO_ADC_CH_2_CLK_EN() __GPIOC_CLK_ENABLE()
#define ADC_CH_2  ADC2
#define ADC_GPIO_CH_2  GPIOC
#define ADC_PIN_CH_2  GPIO_PIN_2 //
#define ADC_CHANNEL_CH_2  ADC_CHANNEL_12 //
#define ADC_DMA_CHANNEL_CH_2  DMA_CHANNEL_1//
#define ADC_DMA_STREAM_CH_2  DMA2_Stream2 //

// Definition of ADC and DMA for channel 3 
#define ADC_CH_3_CLK_EN() __ADC3_CLK_ENABLE()
#define ADC_CH_3_CLK_DIS() __ADC3_CLK_DISABLE()
#define GPIO_ADC_CH_3_CLK_EN() __GPIOC_CLK_ENABLE()
#define ADC_CH_3  ADC3 //
#define ADC_GPIO_CH_3  GPIOC
#define ADC_PIN_CH_3  GPIO_PIN_3//
#define ADC_CHANNEL_CH_3  ADC_CHANNEL_13 //
#define ADC_DMA_CHANNEL_CH_3  DMA_CHANNEL_2//
#define ADC_DMA_STREAM_CH_3  DMA2_Stream1//

// Definition of ADC and DMA for channel 4 
#define ADC_CH_4  0
#define ADC_GPIO_CH_4  0
#define ADC_PIN_CH_4  0
#define ADC_CHANNEL_CH_4  0
#define ADC_DMA_CHANNEL_CH_4  0
#define ADC_DMA_STREAM_CH_4  0 


//Definition of assert to check length of strings
#define CASSERT(ex) {typedef char cassert_type[(ex)?1:-1];}



#endif /* STM32F4_CONFIG_H_ */
