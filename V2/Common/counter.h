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

/* the buffer size of input capture mode has to be set at least to number 2 (two edges captured) */
#define IC12_BUFFER_SIZE	2

typedef enum{
	ETR = 0,
	IC,
	REF	
}counterMode;

typedef enum{
	COUNTER_IDLE = 0,
	COUNTER_ETR,
	COUNTER_IC,
	COUNTER_REF,	
}counterState;

typedef struct{
	uint32_t arr;		// TIM4 ARR
	uint16_t psc;		// TIM4 PSC
	uint32_t buffer;
	uint16_t gateTime;	
	double freq;
}counterEtrTypeDef;

typedef struct{
	uint32_t arr;		// TIM2 ARR
	uint16_t psc;		// TIM2 ARR
	volatile uint16_t ic1BufferSize;
	volatile uint16_t ic2BufferSize;
	uint32_t *ic1buffer;
	uint32_t *ic2buffer;
	double ic1freq;
	double ic2freq;	
	uint32_t ic1psc;
	uint32_t ic2psc;
}counterIcTypeDef;

typedef struct{
	uint32_t buffer;
	// etc.
}counterRefTypeDef;

typedef struct{
	counterIcTypeDef counterIc;
	counterEtrTypeDef counterEtr;	
	counterRefTypeDef counterRef;	
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

uint8_t IC_GetPrescaler(uint32_t icxpsc);
uint32_t IC1_GetCapture(uint32_t *buffer);
uint32_t IC2_GetCapture(uint32_t *buffer);
void counterGateTimeConfig(uint16_t gateTime);

void counterSetMode(uint8_t mode);
void counterSetEtrGate(uint16_t gateTime);
void counterSetIcSampleCount(uint16_t buffer);
void counterSetDefault(void);

void counterInitETR(void);
void counterInitIC(void);
void counterInitREF(void);
void counter_deinit(void);
void counterSendStart(void);
void counterSendStop(void);
void counterStart(void);
void counterStop(void);
void counterGateConfig(uint16_t gateTime);

extern volatile counterTypeDef counter;

#endif /* COUNTER_H_ */

#endif //USE_COUNTER

/* End my Friend */
