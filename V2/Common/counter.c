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
#include "stdlib.h"


// External variables definitions =============================================
xQueueHandle counterMessageQueue;
xSemaphoreHandle counterMutex;
//xSemaphoreHandle counterSemaphoreBin;

volatile counterTypeDef counter;

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
	counterMessageQueue = xQueueCreate(5, 20);  // xQueueCreate(5, sizeof(double)); e.g.
	counterMutex = xSemaphoreCreateRecursiveMutex();	
//	counterSemaphoreBin = xSemaphoreCreateBinary();
	
	if(counterMessageQueue == 0){
		while(1); // Queue was not created and must not be used.
	}
	char message[20];
	
	counterSetDefault();
	TIM_counter_etr_init();
	
	while(1){
		
		xQueueReceive(counterMessageQueue, message, portMAX_DELAY);
		
		xSemaphoreTakeRecursive(counterMutex, portMAX_DELAY);
//		xSemaphoreTake(counterSemaphoreBin, portMAX_DELAY);		
		
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
		}else if(message[0]=='7'){
			/* If the user entry cannot be set due to prescaler, the recalculation 
				 is processed and the value is sent back to PC app */
			if((counter.counterIc.ic1BufferSizeTemp % (counter.counterIc.ic1psc))!=0){										
				counter.counterIc.ic1BufferSize = ((counter.counterIc.ic1BufferSizeTemp/counter.counterIc.ic1psc)*counter.counterIc.ic1psc+counter.counterIc.ic1psc);							
			}else{
				counter.counterIc.ic1BufferSize = counter.counterIc.ic1BufferSizeTemp;
			}		
			counter.icFlag = COUNTER_FLAG1;		
			vPortFree((void *)counter.counterIc.ic1buffer);			
			counter.counterIc.ic1buffer = NULL;			
			counter.counterIc.ic1buffer = (uint32_t *)pvPortMalloc(counter.counterIc.ic1BufferSize*sizeof(uint32_t));					
			xQueueSendToBack(messageQueue, "GIcBuffer1Send", portMAX_DELAY);	
			
		}else if(message[0]=='8'){
			
			if((counter.counterIc.ic2BufferSizeTemp % counter.counterIc.ic2psc)!=0){					
				counter.counterIc.ic2BufferSize = ((counter.counterIc.ic2BufferSizeTemp/counter.counterIc.ic2psc)*counter.counterIc.ic2psc+counter.counterIc.ic2psc);				
			}else{
				counter.counterIc.ic2BufferSize = counter.counterIc.ic2BufferSizeTemp;
			}
			counter.icFlag = COUNTER_FLAG2;			
			vPortFree((void *)counter.counterIc.ic2buffer);		
			counter.counterIc.ic2buffer = NULL;	
			counter.counterIc.ic2buffer = (uint32_t *)pvPortMalloc(counter.counterIc.ic2BufferSize*sizeof(uint32_t));
			xQueueSendToBack(messageQueue, "GIcBuffer2Send", portMAX_DELAY);			
		}
	
//		xSemaphoreGive(counterSemaphoreBin);		
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

/**
  * @brief  Setter for counter ETR time gating
	* @param  gateTime - units [ms]: 10, 100, 1000, 10000
  * @retval None
  */
void counterSetEtrGate(uint16_t gateTime){
	xSemaphoreTakeRecursive(counterMutex, portMAX_DELAY);
	counter.counterEtr.gateTime = gateTime;
	xSemaphoreGiveRecursive(counterMutex);
	xQueueSendToBack(counterMessageQueue, "6SetEtrGate", portMAX_DELAY);
}

/**
  * @brief  Setters for counters' IC buffer sizes (number of edges counted)
	* @param  buffer: range between 2 - xxx (max. value depends on free memory availability)
  * @retval None
  */
void counterSetIc1SampleCount(uint16_t buffer){
	counter.counterIc.ic1BufferSizeTemp = buffer;
	xQueueSendToBack(counterMessageQueue, "7SetIc1Buffer", portMAX_DELAY);	
}

void counterSetIc2SampleCount(uint16_t buffer){
	counter.counterIc.ic2BufferSizeTemp = buffer;
	xQueueSendToBack(counterMessageQueue, "8SetIc2Buffer", portMAX_DELAY);
}

/**
  * @brief  Setters for REF counter (TIM4) - PSC and REF numbers to be multiplied (number of REF clock ticks to be counted).
						Note the REF mode uses ETR struct, some ETR functions...
	* @param  buffer - psc or arr: range between 1 - 65536
  * @retval None
  */
void counterSetRefPsc(uint16_t psc){
	xSemaphoreTakeRecursive(counterMutex, portMAX_DELAY);
	counter.counterEtr.psc = psc - 1;
	TIM_ARR_PSC_Config(counter.counterEtr.arr, counter.counterEtr.psc);
	xSemaphoreGiveRecursive(counterMutex);
//	xQueueSendToBack(counterMessageQueue, "", portMAX_DELAY);		
}

void counterSetRefArr(uint16_t arr){
	xSemaphoreTakeRecursive(counterMutex, portMAX_DELAY);
	counter.counterEtr.arr = arr - 1;
	TIM_ARR_PSC_Config(counter.counterEtr.arr, counter.counterEtr.psc);
	xSemaphoreGiveRecursive(counterMutex);
//	xQueueSendToBack(counterMessageQueue, "", portMAX_DELAY);		
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
			TIM_ETR_Start();
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
			TIM_ETR_Stop();
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
	xSemaphoreTakeFromISR(counterMutex, &xHigherPriorityTaskWoken);
	
	if(counter.state==COUNTER_ETR){
		counter.counterEtr.etrp = TIM_ETPS_GetPrescaler();
		double gateFreq = ((double)tim4clk / (double)((counter.counterEtr.arr + 1) * (counter.counterEtr.psc + 1)));			/* TIM4 gating frequency */	
		counter.counterEtr.freq = ((double)counter.counterEtr.buffer * gateFreq * counter.counterEtr.etrp);								/* Sampled frequency */
		TIM_ETRP_Config(counter.counterEtr.freq);	
	}
	
	xSemaphoreGiveFromISR(counterMutex, &xHigherPriorityTaskWoken);
	xQueueSendToBackFromISR(messageQueue, "GEtrDataSend", &xHigherPriorityTaskWoken);
}

/**
  * @brief  This function is executed in case of DMA transfer complete event of Input Capture channel 1.
  * @param  Pointer to DMA handle structure.
  * @retval None
  */
void COUNTER_IC1_DMA_CpltCallback(DMA_HandleTypeDef *dmah)
{
	portBASE_TYPE xHigherPriorityTaskWoken;
	xSemaphoreTakeFromISR(counterMutex, &xHigherPriorityTaskWoken);
	
	counter.icChannel = COUNTER_IRQ_IC1;
	counter.counterIc.ic1psc = TIM_IC1PSC_GetPrescaler();
	
	uint32_t capture1 = counter.counterIc.ic1buffer[counter.counterIc.ic1BufferSize-1] - counter.counterIc.ic1buffer[0];
	counter.counterIc.ic1freq = (double)(tim2clk*(counter.counterIc.psc+1)*counter.counterIc.ic1psc)*((double)(counter.counterIc.ic1BufferSize-1)/(double)capture1);
	TIM_IC1PSC_Config(counter.counterIc.ic1freq);		
	
	counterIc1BufferConfig(counter.counterIc.ic1BufferSize);
	
	xSemaphoreGiveFromISR(counterMutex, &xHigherPriorityTaskWoken);		
	xQueueSendToBackFromISR(messageQueue, "GIcDataSend", &xHigherPriorityTaskWoken);		
}

/**
  * @brief  This function is executed in case of DMA transfer complete event of Input Capture channel 2.
  * @param  Pointer to DMA handle structure.
  * @retval None
  */
void COUNTER_IC2_DMA_CpltCallback(DMA_HandleTypeDef *dmah)
{
	portBASE_TYPE xHigherPriorityTaskWoken;
	xSemaphoreTakeFromISR(counterMutex, &xHigherPriorityTaskWoken);
	
	counter.icChannel = COUNTER_IRQ_IC2;	
	counter.counterIc.ic2psc = TIM_IC2PSC_GetPrescaler();
		
	uint32_t capture2 = counter.counterIc.ic2buffer[counter.counterIc.ic2BufferSize-1] - counter.counterIc.ic2buffer[0];
	counter.counterIc.ic2freq = (double)(tim2clk*(counter.counterIc.psc+1)*counter.counterIc.ic2psc)*((double)(counter.counterIc.ic2BufferSize-1)/(double)capture2);
	TIM_IC2PSC_Config(counter.counterIc.ic2freq);		
	
	counterIc2BufferConfig(counter.counterIc.ic2BufferSize);
	
	xSemaphoreGiveFromISR(counterMutex, &xHigherPriorityTaskWoken);	
	xQueueSendToBackFromISR(messageQueue, "GIcDataSend", &xHigherPriorityTaskWoken);
}

/* ************************************************************************************** */
/* ----------------------------- Counter specific functions ----------------------------- */
/* ************************************************************************************** */
/**
	* @brief  This function configures ARR and PSC registers of 16bit timer if running on 72 MHz. 	
	* @param  gateTime: gate time in [ms] 	
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
	* @brief  Function adjusting IC1 buffer size (number of edges counted) if user sends a value that cannot be served.
						For instance, if user sends number 13 (number of edges counted) and IC prescaler is 4, it is impossible to serve this value
						due to this prescaler. Therefore, instead (13/4)*4 = 3*4 + 4 = 16 will be set.
	* @param  ic1buffSize: buffer size (e.g. sent from user PC app)
  * @retval none 
  */
void counterIc1BufferConfig(uint16_t ic1buffSize)
{
	if((ic1buffSize % counter.counterIc.ic1psc)!=0){	
		counter.icFlag = COUNTER_FLAG1;
		counter.counterIc.ic1BufferSize = ((ic1buffSize/counter.counterIc.ic1psc)*counter.counterIc.ic1psc+counter.counterIc.ic1psc);
		
		vPortFree((void *)counter.counterIc.ic1buffer);
		counter.counterIc.ic1buffer = NULL;				
		counter.counterIc.ic1buffer = (uint32_t *)pvPortMalloc(counter.counterIc.ic1BufferSize*sizeof(uint32_t));				
	}
}

/**
	* @brief  Function adjusting IC2 buffer size (number of edges counted)
	* @param  ic1buffSize: buffer size (e.g. sent from user PC app)
  * @retval none 
  */
void counterIc2BufferConfig(uint16_t ic2buffSize)
{
	if((ic2buffSize % counter.counterIc.ic2psc)!=0){	
		counter.icFlag = COUNTER_FLAG2;
		counter.counterIc.ic2BufferSize = ((ic2buffSize/counter.counterIc.ic2psc)*counter.counterIc.ic2psc+counter.counterIc.ic2psc);
		
		vPortFree((void *)counter.counterIc.ic2buffer);
		counter.counterIc.ic2buffer = NULL;				
		counter.counterIc.ic2buffer = (uint32_t *)pvPortMalloc(counter.counterIc.ic2BufferSize*sizeof(uint32_t));	
	}
}

/**
  * @brief  Counter set Default values
  * @param  None
  * @retval None
  */
void counterSetDefault(void)
{
	counter.state = COUNTER_ETR;
	
	/* ETR counter default values */
	counter.counterEtr.psc = TIM4_PSC;	
	counter.counterEtr.arr = TIM4_ARR;
	counter.counterEtr.gateTime = 1000;				/* 1000 ms = 1 s */
	counter.counterEtr.buffer = 0;
	counter.counterEtr.etrp = 1;

	/* IC counter default values */
	counter.counterIc.psc = 0;		
	counter.counterIc.arr = 0xFFFFFFFF;
	counter.counterIc.ic1BufferSize = 4;			/* the lowest value of icxBufferSize is 2! */
	counter.counterIc.ic2BufferSize = 4;
	counter.counterIc.ic1psc = 1;
	counter.counterIc.ic2psc = 1;
	counter.icChannel = COUNTER_IRQ_IC_PASS;
	counter.icFlag = COUNTER_FLAG_PASS;
}

	#endif //USE_COUNTER

