/*
  *****************************************************************************
  * @file    counter.h
  * @author  HeyBirdie
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
#define IC12_BUFFER_SIZE	110

typedef enum{
	ETR = 0,
	IC,
	TI,
	REF
}counterMode;

typedef enum{
	COUNTER_IDLE = 0,
	COUNTER_ETR,
	COUNTER_IC,
	COUNTER_TI,
	COUNTER_REF		
}counterState;

typedef enum{
	COUNTER_IRQ_IC = 0,
	COUNTER_IRQ_IC_PASS
}counterIcChannel;

typedef enum{
	DUTY_CYCLE_DISABLED = 0,
	DUTY_CYCLE_CH1_ENABLED,
	DUTY_CYCLE_CH2_ENABLED
}counterIcDutyCycle;

typedef enum{	
	SAMPLE_COUNT_CHANGED = 0,
	SAMPLE_COUNT_NOT_CHANGED
}counterRefSmplCntChange;

typedef enum{	
	EVENT_RISING = 0,
	EVENT_FALLING
}counterEventTypeDef;

typedef enum{
	CLEAR = 0,
	TIMEOUT,
	SEND_TI_DATA
}counterTiStateTypeDef;

typedef enum{
	TI_MODE_EVENT_SEQUENCE_INDEP = 0,
	TI_MODE_FAST_EVENT_SEQUENCE_DEP
}counterTiModeTypeDef;

typedef enum{
	BIN0 = 0,
	BIN1
}counterBin;

/* ETR struct (High frequency meas.) is also used for REF mode as only 
	 the difference is in clock feeding of timer 4 and in the data sent 
	 to PC app */
typedef struct{
	uint16_t arr;		// TIM4 ARR
	uint16_t psc;		// TIM4 PSC
	uint16_t arrTemp;
	uint16_t pscTemp;			
	uint8_t etrp;		// TIM2 ETRP
	uint32_t buffer;	
	uint16_t gateTime;	
	double freq;
}counterEtrTypeDef;

/* IC mode (Low frequency) struct */
typedef struct{
	uint32_t arr;		// TIM2 ARR
	uint16_t psc;		// TIM2 PSC
	uint16_t ic1BufferSize;
	uint16_t ic2BufferSize;
	uint16_t ic1BufferSizeTemp;
	uint16_t ic2BufferSizeTemp;	
	volatile uint32_t ic1buffer[IC12_BUFFER_SIZE];
	volatile uint32_t ic2buffer[IC12_BUFFER_SIZE];	
	double ic1freq;
	double ic2freq;	
	uint8_t ic1psc;
	uint8_t ic2psc;
	uint8_t ic1pscTemp;
	uint8_t ic2pscTemp;
	uint32_t tiTimeout; // TI timeout part of IC struct
}counterIcTypeDef;

/* Common struct */
typedef struct{
	counterIcTypeDef counterIc;
	counterEtrTypeDef counterEtr;	
//  counterRefTypeDef counterRef;	-> ETR structure used for REF	counter
	counterState state;
	
	counterRefSmplCntChange sampleCntChange;
	counterIcChannel icChannel1;
	counterIcChannel icChannel2;	
	counterIcDutyCycle icDutyCycle;
	counterTiStateTypeDef tiState;	
	counterTiModeTypeDef tiMode;
	counterEventTypeDef eventChan1;
	counterEventTypeDef eventChan2;
	counterBin bin;
	counterBin abba; 		// TI mode events sequence t_AB or t_BA
}counterTypeDef;

// Exported functions =========================================================
void CounterTask(void const *argument);

/* Counter general functions */
void counterInitETR(void);
void counterInitIC(void);
void counterInitREF(void);
void counterInitTI(void);
void counter_deinit(void);
void counterSendStart(void);
void counterSendStop(void);
void counterStart(void);
void counterStop(void);
void counterDeinit(void);
void counterEtrRefSetDefault(void);
void counterIcTiSetDefault(void);
void counterSetMode(uint8_t mode);

/* ETR mode functions */
void counterSetEtrGate(uint16_t gateTime);
void counterGateConfig(uint16_t gateTime);

/* IC mode functions */
void counterSetIc1SampleCount(uint16_t buffer);
void counterSetIc2SampleCount(uint16_t buffer);
void counterSetIc1Prescaler(uint16_t presc);
void counterSetIc2Prescaler(uint16_t presc);
void counterSetIcTi1_RisingFalling(void);
void counterSetIcTi2_RisingFalling(void);
void counterSetIcTi1_Rising(void);
void counterSetIcTi2_Rising(void);
void counterSetIcTi1_Falling(void);
void counterSetIcTi2_Falling(void);
void counterSetTiSequence_AB(void);
void counterSetTiSequence_BA(void);
void counterIc1BufferConfig(uint16_t ic1buffSize);
void counterIc2BufferConfig(uint16_t ic2buffSize);
void counterIcProcess(void);
void counterIcDutyCycleProcess(void);

void counterIc1DutyCycleInit(void);
void counterIc1DutyCycleDeinit(void);
void counterIc2DutyCycleInit(void);
void counterIc2DutyCycleDeinit(void);
void counterIcDutyCycleEnable(void);
void counterIcDutyCycleDisable(void);

/* TI mode functions */
void counterTiProcess(void);
void counterSetTiTimeout(uint32_t timeout);
void counterSetTiMode_Independent(void);
void counterSetTiMode_Dependent(void);

/* REF mode functions */
void counterSetRefPsc(uint16_t psc);
void counterSetRefArr(uint16_t arr);

void COUNTER_IC_TIM_Elapse(void);
void counterPeriodElapsedCallback(TIM_HandleTypeDef *htim);

extern volatile counterTypeDef counter;
extern uint32_t tim2clk, tim4clk, startTime;

extern DMA_HandleTypeDef hdma_tim2_up;
extern DMA_HandleTypeDef hdma_tim2_ch1;
extern DMA_HandleTypeDef hdma_tim2_ch2_ch4;

#endif /* COUNTER_H_ */

#endif //USE_COUNTER


