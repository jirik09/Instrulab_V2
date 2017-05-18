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
xSemaphoreHandle counterSemaphoreBin;
xSemaphoreHandle counterMutex;

volatile counterTypeDef counter;
extern uint32_t tim2clk, tim4clk;

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
	counterMessageQueue = xQueueCreate(5, 20);  // xQueueCreate(5, sizeof(double));
	counterMutex = xSemaphoreCreateRecursiveMutex();	
	counterSemaphoreBin = xSemaphoreCreateBinary();
	
	if(counterMessageQueue == 0){
		while(1); // Queue was not created and must not be used.
	}
	char message[20];
	
	counterSetDefault();
	
	while(1){
		
		xQueueReceive(counterMessageQueue, message, portMAX_DELAY);
		
		xSemaphoreTakeRecursive(counterMutex, portMAX_DELAY);
		xSemaphoreTake(counterSemaphoreBin, portMAX_DELAY);		
		
		if(message[0]=='1'){
			counterInitETR();
		}else if(message[0]=='2'){
			counterInitIC();
		}else if(message[0]=='3'){
			counterInitREF();
		}else if(message[0]=='4'){
			counterStart();
		}else if(message[0]=='5'){
			counterStop();
		}else if(message[0]=='6'){
			counterGateConfig(counter.counterEtr.gateTime);
		}
	
		xSemaphoreGive(counterSemaphoreBin);		
		xSemaphoreGiveRecursive(counterMutex);
	}
}

/* ************************************************************************************** */
/* ------------------------------ Sending to Queue functions ---------------------------- */
/* ************************************************************************************** */
void counterSetMode(uint8_t mode){
	switch(mode){
		case ETR:
			xQueueSendToBack(counterMessageQueue, "1SetEtrMode", portMAX_DELAY);
			break;
		case IC:
			xQueueSendToBack(counterMessageQueue, "2SetIcMode", portMAX_DELAY);
			break;
		case REF:
			xQueueSendToBack(counterMessageQueue, "3SetRefMode", portMAX_DELAY);
			break;		
	}
}

void counterSendStart(void){	
	xQueueSendToBack(counterMessageQueue, "4StartCounter", portMAX_DELAY);
}

void counterSendStop(void){	
	xQueueSendToBack(counterMessageQueue, "5StopCounter", portMAX_DELAY);
}

void counterSetEtrGate(uint16_t gateTime){
	counter.counterEtr.gateTime = gateTime;
	xQueueSendToBack(counterMessageQueue, "6SetEtrGate", portMAX_DELAY);
}

/**
  * @brief  Setter for counter IC buffer size (number of edges counted)
  * @param  between 2 - 100
  * @retval None
  */
void counterSetIcSampleCount(uint16_t buffer){
	xSemaphoreTakeRecursive(counterMutex, portMAX_DELAY);
	counter.counterIc.ic1BufferSize = buffer;
	counter.counterIc.ic2BufferSize = buffer;
	xSemaphoreGiveRecursive(counterMutex);
}

/* ************************************************************************************** */
/* ---------------------------- Counter INIT DEINIT functions --------------------------- */
/* ************************************************************************************** */
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

void counterInitREF(void){
	counter_deinit();
	counter.state = COUNTER_REF;
	TIM_counter_ref_init();
}

void counter_deinit(void){
	switch(counter.state){
		case COUNTER_ETR:
			TIM_etr_deinit();
			break;
		case COUNTER_IC:
			TIM_ic_deinit();
			break;
		case COUNTER_REF:
			TIM_ref_deinit();
			break;		
		case COUNTER_IDLE:
			/* no hacer nada */
			break;		
	}
}

/* ************************************************************************************** */
/* ---------------------------- Counter START STOP functions ---------------------------- */
/* ************************************************************************************** */
void counterStart(void){
	switch(counter.state){
		case COUNTER_ETR:
		 	TIM_ETR_Start();
			break;
		case COUNTER_IC:
			TIM_IC_Start();
			break;
		case COUNTER_REF:
			TIM_REF_Start();
			break;
		case COUNTER_IDLE:
			/* no hacer nada */
			break;
	}	
}

void counterStop(void){
	switch(counter.state){
		case COUNTER_ETR:
		 	TIM_ETR_Stop();
			break;
		case COUNTER_IC:
			TIM_IC_Stop();
			break;
		case COUNTER_REF:
			TIM_REF_Stop();
			break;
		case COUNTER_IDLE:
			/* no hacer nada */
			break;
	}	
}

/* ************************************************************************************** */
/* ----------------------------- Counter callback functions ----------------------------- */
/* ************************************************************************************** */
/**
  * @brief  This function is executed in case of DMA transfer complete event.
	*					DMA transfer complete event is triggered after TIM4 gate time elapses.
  * @param  Pointer to DMA handle structure.
  * @retval None
  */
void COUNTER_ETR_DMA_CpltCallback(DMA_HandleTypeDef *dmah)
{			
	portBASE_TYPE xHigherPriorityTaskWoken;
	xSemaphoreTakeFromISR(counterSemaphoreBin, &xHigherPriorityTaskWoken);
	
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
	
	gateFreq = ((double)tim4clk / (double)((counter.counterEtr.arr + 1) * (counter.counterEtr.psc + 1)));			/* TIM4 gating frequency */	
	counter.counterEtr.freq = (double)(counter.counterEtr.buffer * gateFreq * etrPresc);											/* Sampled frequency */
	
	ETRP_Config(counter.counterEtr.freq);
	
	xSemaphoreGiveFromISR(counterSemaphoreBin, &xHigherPriorityTaskWoken);			
}

/**
  * @brief  This function is executed in case of DMA transfer complete event of Input Capture channel 1.
  * @param  Pointer to DMA handle structure.
  * @retval None
  */
void COUNTER_IC1_DMA_CpltCallback(DMA_HandleTypeDef *dmah)
{
	portBASE_TYPE xHigherPriorityTaskWoken;
	xSemaphoreTakeFromISR(counterSemaphoreBin, &xHigherPriorityTaskWoken);	
		
	uint32_t ic1psc = ((TIM2->CCMR1) & TIM_CCMR1_IC1PSC_Msk) >> TIM_CCMR1_IC1PSC_Pos;
	
	uint32_t capture1 = IC1_GetCapture(counter.counterIc.ic1buffer);
	double ic1freq = (double)(tim2clk*(counter.counterIc.psc+1)*IC_GetPrescaler(ic1psc))*((double)(counter.counterIc.ic1BufferSize-1)/(double)capture1);

	IC1PSC_Config(ic1freq);		

	xSemaphoreGiveFromISR(counterSemaphoreBin, &xHigherPriorityTaskWoken);	
}


/**
  * @brief  This function is executed in case of DMA transfer complete event of Input Capture channel 2.
  * @param  Pointer to DMA handle structure.
  * @retval None
  */
void COUNTER_IC2_DMA_CpltCallback(DMA_HandleTypeDef *dmah)
{
	portBASE_TYPE xHigherPriorityTaskWoken;
	xSemaphoreTakeFromISR(counterSemaphoreBin, &xHigherPriorityTaskWoken);	
	
	uint32_t ic2psc = ((TIM2->CCMR1) & TIM_CCMR1_IC2PSC_Msk) >> TIM_CCMR1_IC2PSC_Pos;
		
	uint32_t capture2 = IC2_GetCapture(counter.counterIc.ic2buffer);
	double ic2freq = (double)(tim2clk*(counter.counterIc.psc+1)*IC_GetPrescaler(ic2psc))*((double)(counter.counterIc.ic2BufferSize-1)/(double)capture2);

	IC2PSC_Config(ic2freq);		

	xSemaphoreGiveFromISR(counterSemaphoreBin, &xHigherPriorityTaskWoken);			
}

/* ************************************************************************************** */
/* ----------------------------- Counter specific functions ----------------------------- */
/* ************************************************************************************** */
/**
	* @brief  This function configures ARR and PSC registers of 16bit timer if running on 72 MHz. 	
	* @param  *tim: pointer to timer structure
	* @param  ovFreq: gives the number of counter (TIMx) overflows..
  * @retval none 
  */
void counterGateConfig(uint16_t gateTime)
{		
	if (gateTime == 10) {									/* min. gate time 00.01 second */
		counter.counterEtr.psc = 7199;
		counter.counterEtr.arr = 99;
	} else if (gateTime == 100) {					/* ----	gate time 00.10 second */
		counter.counterEtr.psc = 7199;
		counter.counterEtr.arr = 999;
	} else if (gateTime == 1000){					/* ----	gate time 01.00 second */
		counter.counterEtr.psc = 7199;
		counter.counterEtr.arr = 9999;
	} else if (gateTime == 10000) {				/* max. gate time 10.00 second */
		counter.counterEtr.psc = 35999;
		counter.counterEtr.arr = 19999;		
	}	
	
	TIM_ARR_PSC_Config(counter.counterEtr.arr, counter.counterEtr.psc);
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
uint32_t IC1_GetCapture(uint32_t *buffer)
{
	return (buffer[counter.counterIc.ic1BufferSize-1] - buffer[0]); 
}

uint32_t IC2_GetCapture(uint32_t *buffer)
{
	return (buffer[counter.counterIc.ic2BufferSize-1] - buffer[0]); 
}

/**
  * @brief  Counter set Default values
  * @param  None
  * @retval None
  */
void counterSetDefault(void)
{
	/* ETR counter default values */
	counter.counterEtr.psc = TIM4_PSC;	
	counter.counterEtr.arr = TIM4_ARR;
	counter.counterEtr.gateTime = 1000;				/* 1000 ms = 1 s */
	counter.counterEtr.buffer = 0;

	/* IC counter default values */
	counter.counterIc.psc = 0;		
	counter.counterIc.arr = 0xFFFFFFFF;
	counter.counterIc.ic1BufferSize = 2;
	counter.counterIc.ic2BufferSize = 2;
}

	#endif //USE_COUNTER

