/*
  *****************************************************************************
  * @file    counter.h
  * @author  Y3288231
  * @date    June 3, 2017
  * @brief   This file contains definitions and prototypes of counter functions
  ***************************************************************************** 
*/ 

#ifdef USE_COUNTER
#ifndef COUNTER_H_
#define COUNTER_H_

/* Includes */
#include <stdint.h>
#include "stm32f3xx_hal.h"

#define IC12_BUFFER_SIZE	10

void counterSetMode(uint8_t mode);

typedef enum{
	ETR = 0,
	IC
}counterMode;

typedef enum{
	COUNTER_IDLE = 0,
	COUNTER_ETR,
	COUNTER_IC
}counterState;

typedef struct{
	uint32_t buffer;
	uint16_t arr;	
	uint16_t psc;	
	double freq;
}counterEtrTypeDef;

typedef struct{
	uint32_t ic1buffer[IC12_BUFFER_SIZE];
	uint32_t ic2buffer[IC12_BUFFER_SIZE];
	uint16_t arr;	
	uint16_t psc;	
	double ic1freq;
	double ic2freq;	
}counterIcTypeDef;

typedef struct{
	counterIcTypeDef counterIc;
	counterEtrTypeDef counterEtr;
//	double floatAvgBuffer[CNT_AVG_BUFF_SIZE];
//	double *pFloatAvgBuf;
//	uint8_t sampleToAvg;
	counterState state;
}counterTypeDef;


// Exported functions =========================================================
void CounterTask(void const *argument);

void COUNTER_ETR_DMA_CpltCallback(DMA_HandleTypeDef *dmah);	
void COUNTER_IC1_DMA_CpltCallback(DMA_HandleTypeDef *dmah);
void COUNTER_IC2_DMA_CpltCallback(DMA_HandleTypeDef *dmah);

void ETRP_Config(double freq);
void ARR_PSC_Config(TIM_HandleTypeDef *tim, double ovFreq);
void IC1PSC_Config(double freq);
void IC2PSC_Config(double freq);
uint8_t IC_GetPrescaler(uint32_t icxpsc);
uint32_t IC_GetCapture(volatile uint32_t *buffer);

#endif /* COUNTER_H_ */

#endif //USE_COUNTER

/* End my Friend */
