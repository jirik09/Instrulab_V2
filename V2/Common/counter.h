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
	REF	
}counterMode;

typedef enum{
	COUNTER_IDLE = 0,
	COUNTER_ETR,
	COUNTER_IC,
	COUNTER_REF,	
}counterState;

typedef enum{
	COUNTER_IRQ_IC1 = 0,
	COUNTER_IRQ_IC1_PASS
}counterIcChannel1;

typedef enum{
	COUNTER_IRQ_IC2 = 0,
	COUNTER_IRQ_IC2_PASS
}counterIcChannel2;

//typedef enum{
//	COUNTER_PRESC1_CHANGED = 0,
//	COUNTER_PRESC1_NOT_CHANGED
//}counterIcPresc1;

//typedef enum{
//	COUNTER_PRESC2_CHANGED = 0,
//	COUNTER_PRESC2_NOT_CHANGED
//}counterIcPresc2;

//typedef enum{
//	BUFF1_CHANGED = 0,
//	BUFF1_NOT_CHANGED
//}counterIc1BuffChange;

//typedef enum{
//	BUFF2_CHANGED = 0,
//	BUFF2_NOT_CHANGED
//}counterIc2BuffChange;

typedef enum{
	SAMPLE_COUNT_CHANGED = 0,
	SAMPLE_COUNT_NOT_CHANGED
}counterRefSmplCntChange;

typedef enum{
	BIN0 = 0,
	BIN1
}counterIcBin;

/* ETR struct is also used for REF mode as only the difference 
	 is the clock feeding of timer 4 and the data sent to PC app */
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
}counterIcTypeDef;

typedef struct{
	counterIcTypeDef counterIc;
	counterEtrTypeDef counterEtr;	
//  counterRefTypeDef counterRef;	-> ETR structure used for REF	
	counterState state;
	
	counterIcChannel1 icChannel1;
	counterIcChannel2 icChannel2;
//	counterIc1BuffChange buff1Change;
//	counterIc2BuffChange buff2Change;
//	counterIcPresc1 icPresc1;
//	counterIcPresc2 icPresc2;
	counterIcBin icBin;
	counterRefSmplCntChange sampleCntChange;

}counterTypeDef;

// Exported functions =========================================================
void CounterTask(void const *argument);

/* Counter general functions */
void counterInitETR(void);
void counterInitIC(void);
void counterInitREF(void);
void counter_deinit(void);
void counterSendStart(void);
void counterSendStop(void);
void counterStart(void);
void counterStop(void);
void counterDeinit(void);
void counterSetDefault(void);
void counterSetMode(uint8_t mode);

/* ETR mode functions */
void counterSetEtrGate(uint16_t gateTime);
void counterGateConfig(uint16_t gateTime);

/* IC mode functions */
void counterSetIc1SampleCount(uint16_t buffer);
void counterSetIc2SampleCount(uint16_t buffer);
void counterSetIc1Prescaler(uint16_t presc);
void counterSetIc2Prescaler(uint16_t presc);
void counterSetIc1PulseMeas(void);
void counterSetIc2PulseMeas(void);
void counterResetIc1PulseMeas(void);
void counterResetIc2PulseMeas(void);
void counterIc1BufferConfig(uint16_t ic1buffSize);
void counterIc2BufferConfig(uint16_t ic2buffSize);

/* REF mode functions */
void counterSetRefPsc(uint16_t psc);
void counterSetRefArr(uint16_t arr);

void COUNTER_IC_TIM_Elapse(void);
void HAL_TIM_PeriodElapsedCallback(TIM_HandleTypeDef *htim);

extern volatile counterTypeDef counter;
extern uint32_t tim2clk, tim4clk, startTime;

extern DMA_HandleTypeDef hdma_tim2_up;
extern DMA_HandleTypeDef hdma_tim2_ch1;
extern DMA_HandleTypeDef hdma_tim2_ch2_ch4;

#endif /* COUNTER_H_ */

#endif //USE_COUNTER


