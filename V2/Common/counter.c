/*
  *****************************************************************************
  * @file    counter.c
  * @author  Y3288231
  * @date    June 3, 2017
  * @brief   This file contains counter functions
  ***************************************************************************** 
*/ 

// Includes ===================================================================
	#ifdef USE_COUNTER
#include "cmsis_os.h"
#include "mcu_config.h"
#include "comms.h"
#include "counter.h"
#include "tim.h"


// External variables definitions =============================================
xQueueHandle counterMessageQueue;
void clearAvgBuffer(void);
void counterInitETR(void);
void counterInitIC(void);
void counter_deinit(void);

volatile counterTypeDef counter;
extern uint32_t tim2clk, tim4clk;
volatile uint8_t ic1BufferSize = 2;
volatile uint8_t ic2BufferSize = 2;

// Function definitions ========================================================
/**
  * @brief  Oscilloscope task function.
  * task is getting messages from other tasks and takes care about oscilloscope functions
  * @param  Task handler, parameters pointer
  * @retval None
  */
//portTASK_FUNCTION(vScopeTask, pvParameters){	
void CounterTask(void const *argument)
{
	counterMessageQueue = xQueueCreate(5, 20);
	char message[20];
	
	while(1){
		xQueueReceive(counterMessageQueue, message, portMAX_DELAY);
		if(message[0]=='1'){
				counterInitETR();
		}else if(message[0]=='2'){
				counterInitIC();
		}
//		else if(message[0]=='3'){ //invalidate
//			if(generator.state==GENERATOR_IDLE){
//				
//			}
//		}else if(message[0]=='4'){ //start
//			if(generator.state==GENERATOR_IDLE){
//				genInit();
//				GeneratingEnable();
//				generator.state=GENERATOR_RUN;
//			}

//		}else if(message[0]=='5'){ //stop
//			if(generator.state==GENERATOR_RUN){
//				GeneratingDisable();
//				generator.state=GENERATOR_IDLE;
//			}
//		}

	}
}

void counterSetMode(uint8_t mode){
	switch(mode){
		case ETR:
			xQueueSendToBack(messageQueue, "1SetEtrMode", portMAX_DELAY);
			break;
		case IC:
			xQueueSendToBack(messageQueue, "2SetIcMode", portMAX_DELAY);
			break;
	}
}

void counterInitETR(void){
	counter_deinit();
	counter.state = COUNTER_ETR;
	TIM_counter_etr_init();
}

void counterInitIC(void){
	counter_deinit();
	counter.state = COUNTER_IC;
	TIM_counter_ic_init();
}

void counter_deinit(void){
	switch(counter.state){
		case COUNTER_ETR:
			TIM_etr_deinit();
			break;
		case COUNTER_IC:
			TIM_ic_deinit();
			break;
		case COUNTER_IDLE:
				/* no hacer nada */
			break;		
	}
}

/**
  * @brief  This function is executed in case of DMA transfer complete event.
	*					DMA transfer complete event is triggered after TIM4 gate time elapses.
  * @param  Pointer to DMA handle structure.
  * @retval None
  */
void COUNTER_ETR_DMA_CpltCallback(DMA_HandleTypeDef *dmah)
{			
	uint32_t arr = TIM4->ARR;
	uint32_t psc = TIM4->PSC;
	uint16_t etps = ((TIM2->SMCR) & 0x3000) >> 12;			/* ETR prescaler register value */
	uint8_t etrPresc;																		/* ETR prescaler real value */
	double gateFreq;
	
	/* Save the real value of ETR prescaler for later calculations */
	switch(etps){
		case 0:
			etrPresc = 1; break;			
		case 1:
			etrPresc = 2;	break;
		case 2:
			etrPresc = 4; break;
		case 3:
			etrPresc = 8; break;
		default: 
			break;
	}
	
	gateFreq = ((double)tim4clk / (double)((arr + 1) * (psc + 1)));						/* TIM4 gating frequency */	
	counter.counterEtr.freq = (double)(counter.counterEtr.buffer * gateFreq * etrPresc);					/* Sampled frequency */
	
	ETRP_Config(counter.counterEtr.freq);
}

/**
  * @brief  This function is executed in case of DMA transfer complete event of Input Capture channel 1.
  * @param  Pointer to DMA handle structure.
  * @retval None
  */
void COUNTER_IC1_DMA_CpltCallback(DMA_HandleTypeDef *dmah)
{
	uint32_t psc = TIM2->PSC;	
	uint32_t ic1psc = ((TIM2->CCMR1) & TIM_CCMR1_IC1PSC_Msk) >> TIM_CCMR1_IC1PSC_Pos;	
	uint32_t capture1;
	double ic1freq;		
	
	capture1 = IC_GetCapture(counter.counterIc.ic1buffer);
	
	ic1freq = (double)(tim2clk*(psc+1)*IC_GetPrescaler(ic1psc))*((double)(ic1BufferSize-1)/(double)capture1);

	IC1PSC_Config(ic1freq);			
}


/**
  * @brief  This function is executed in case of DMA transfer complete event of Input Capture channel 2.
  * @param  Pointer to DMA handle structure.
  * @retval None
  */
void COUNTER_IC2_DMA_CpltCallback(DMA_HandleTypeDef *dmah)
{
	uint32_t psc = TIM2->PSC;	
	uint32_t ic2psc = ((TIM2->CCMR1) & TIM_CCMR1_IC2PSC_Msk) >> TIM_CCMR1_IC2PSC_Pos;
	uint32_t capture2;	
	double ic2freq;		
		
	capture2 = IC_GetCapture(counter.counterIc.ic2buffer);
	
	ic2freq = (double)(tim2clk*(psc+1)*IC_GetPrescaler(ic2psc))*((double)(ic2BufferSize-1)/(double)capture2);

	IC2PSC_Config(ic2freq);				
}

/**
  * @brief  This function is used to select the desired ETR prescaler ETPS. (TIM2 should be clocked to 144 MHz - not possible - only 72 MHz)
	* @param  freq: frequency
  * @retval none 
  */
void ETRP_Config(double freq)
{
	uint32_t smcr = TIM2 -> SMCR;	
	/* Check the range of the input frequency and set the ETR prescaler */
	if ((freq >= (tim2clk / 4)) && freq < ((tim2clk / 2))){		
			if ((smcr & 0x3000) != TIM_SMCR_ETPS_0){
				TIM2 -> SMCR &= ~TIM_SMCR_ETPS;
				TIM2 -> SMCR |= TIM_SMCR_ETPS_0;												/* Set ETR prescaler to 2 */
			}
	} else if ((freq >= (tim2clk / 2)) && (freq < (tim2clk))) {		
			if ((smcr & 0x3000) != TIM_SMCR_ETPS_1){			
				TIM2 -> SMCR &= ~TIM_SMCR_ETPS;				
				TIM2 -> SMCR |= TIM_SMCR_ETPS_1;												/* Set ETR prescaler to 4 */
			}			
	} else if ((freq >= (tim2clk)) && (freq < (tim2clk * 2))) {		
			if ((smcr & 0x3000) != TIM_SMCR_ETPS){	
				TIM2 -> SMCR &= ~TIM_SMCR_ETPS;				
				TIM2 -> SMCR |= TIM_SMCR_ETPS;													/* Set ETR prescaler to 8 */
			}					
	} else if ((smcr & 0x3000) != 0) {															
			TIM2 -> SMCR &= ~TIM_SMCR_ETPS;														/* Set ETR prescaler to 1 */										
	}
}

/**
	* @brief  This function configures ARR and PSC registers of 16bit timer if running on 72 MHz. 	
	* @param  *tim: pointer to timer structure
	* @param  ovFreq: gives the number of counter (TIMx) overflows..
  * @retval none 
  */
void ARR_PSC_Config(TIM_HandleTypeDef *tim, double ovFreq)
{	
	uint16_t arr;
	uint16_t psc;	
	
	if (ovFreq == 100) {									/* min. gate time 00.01 second */
		psc = 7199;
		arr = 99;
	} else if (ovFreq == 10) {
		psc = 7199;
		arr = 999;
	} else if (ovFreq == 1){
		psc = 7199;
		arr = 9999;
	} else if (ovFreq == 0.1) {						/* max. gate time 25.00 second */
		psc = 35999;
		arr = 19999;		
	}	
	
	tim->Instance->ARR = arr; 
	tim->Instance->PSC = psc;
}

/**
	* @brief  This function is used to select the desired prescaler of IC1 of TIM2. 
	* @param  freq: frequency
  * @retval none 
  */
void IC1PSC_Config(double freq)
{
	uint32_t ccmr1 = (TIM2->CCMR1) & TIM_CCMR1_IC1PSC_Msk;
		
	/* PSC1 configuration */
	/* For IC_THRESHOLD = max (8), range is 9 MHz - 18 MHz */
	if ((freq >= (double)(tim2clk/IC_THRESHOLD*2)) && (freq < (double)(tim2clk/IC_THRESHOLD))){
		if (ccmr1 != TIM_CCMR1_IC1PSC_0) {
			TIM2->CCMR1 &= ~TIM_CCMR1_IC1PSC;
			TIM2->CCMR1 |= TIM_CCMR1_IC1PSC_0;				/* Set IC prescaler to 2 */
		}
	/* 18 MHz - 36 MHz */
	} else if ((freq >= (double)(tim2clk/IC_THRESHOLD)) && (freq < (double)(tim2clk*2/IC_THRESHOLD))){
		if (ccmr1 != TIM_CCMR1_IC1PSC_1){
			TIM2->CCMR1 &= ~TIM_CCMR1_IC1PSC;
			TIM2->CCMR1 |= TIM_CCMR1_IC1PSC_1;				/* Set IC prescaler to 4 */
		}
	/* 36 MHz - 72 MHz */
	} else if ((freq >= (double)(tim2clk*2/IC_THRESHOLD)) && (freq < (double)(tim2clk*4/IC_THRESHOLD))){
		if (ccmr1 != TIM_CCMR1_IC1PSC){
			TIM2->CCMR1 &= ~TIM_CCMR1_IC1PSC;
			TIM2->CCMR1 |= TIM_CCMR1_IC1PSC;					/* Set IC prescaler to 8 */
		}
	/* 0.0335276126861572265625 Hz - 9 MHz */
	} else if (ccmr1 != 0x00) {
		TIM2->CCMR1 &= ~TIM_CCMR1_IC1PSC; 					/* Set IC prescaler to 1 */
	}
}

/**
	* @brief  This function is used to select the desired prescaler of IC2 of TIM2. 
	* @param  freq: frequency
  * @retval none 
  */
void IC2PSC_Config(double freq)
{
	uint32_t ccmr2 = (TIM2->CCMR1) & TIM_CCMR1_IC2PSC_Msk;
	
	/* PSC2 configuration */
	if ((freq >= (tim2clk/IC_THRESHOLD*2)) && (freq < (tim2clk/IC_THRESHOLD))){
		if (ccmr2 != TIM_CCMR1_IC2PSC_0) {
			TIM2->CCMR1 &= ~TIM_CCMR1_IC2PSC;
			TIM2->CCMR1 |= TIM_CCMR1_IC2PSC_0;				/* Set IC prescaler to 2 */
		}
	} else if ((freq >= (tim2clk)/IC_THRESHOLD) && (freq < (tim2clk*2/IC_THRESHOLD))){
		if (ccmr2 != TIM_CCMR1_IC2PSC_1){
			TIM2->CCMR1 &= ~TIM_CCMR1_IC2PSC;
			TIM2->CCMR1 |= TIM_CCMR1_IC2PSC_1;				/* Set IC prescaler to 4 */
		}
	} else if ((freq >= (tim2clk*2)/IC_THRESHOLD) && (freq < (tim2clk*4/IC_THRESHOLD))){
		if (ccmr2 != TIM_CCMR1_IC2PSC){
			TIM2->CCMR1 &= ~TIM_CCMR1_IC2PSC;
			TIM2->CCMR1 |= TIM_CCMR1_IC2PSC;					/* Set IC prescaler to 8 */
		}
	} else if (ccmr2 != 0x00) {
		TIM2->CCMR1 &= ~TIM_CCMR1_IC2PSC; 					/* Set IC prescaler to 1 */
	}		
}

/**
  * @brief  This function returns returns the real value of prescaler.
  * @param  Prescaler ICxPSC register value.
	* @retval uint8_t presc: real value of prescaler
  */
uint8_t IC_GetPrescaler(uint32_t icxpsc)
{
	uint8_t presc;
	/* Save the real value of ICxPSC prescaler for later calculations */
	switch(icxpsc){
		case 0:
			presc = 1; break;			
		case 1:
			presc = 2;	break;
		case 2:
			presc = 4; break;
		case 3:
			presc = 8; break;
		default: 
			break;
	}	
	return presc;
}

/**
	* @brief  This function returns the difference between two trays.
	* @param  buffer: DMA buffer.
	* @retval uint32_t (capture_n - capture_0) 
  */
uint32_t IC_GetCapture(volatile uint32_t *buffer)
{
	return (buffer[ic1BufferSize-1] - buffer[0]); 
}

/**
  * @brief  Start scope sampling
  * @param  None
  * @retval None
  */
//void genStart(void){
//	xQueueSendToBack(generatorMessageQueue, "4Start", portMAX_DELAY);
//}

///**
//  * @brief  Stop scope sampling
//  * @param  None
//  * @retval None
//  */
//void genStop(void){
//	xQueueSendToBack(generatorMessageQueue, "5Stop", portMAX_DELAY);
//}

	#endif //USE_COUNTER

