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

volatile counterTypeDef counter;
static uint32_t ic1PassNum = 1;
static uint32_t ic2PassNum = 1;

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
	
	if(counterMessageQueue == 0){
		while(1); // Queue was not created and must not be used.
	}
	char message[20];
	
//	counterSetDefault();
//	TIM_counter_etr_init();
	
	while(1){
		
		xQueueReceive(counterMessageQueue, message, portMAX_DELAY);
		
		xSemaphoreTakeRecursive(counterMutex, portMAX_DELAY);
		
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
			counter_deinit();
		}else if(message[0]=='7'){
			counterGateConfig(counter.counterEtr.gateTime);
		}else if(message[0]=='8'){
			
		}
	
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

void counterDeinit(void){
	xQueueSendToBack(counterMessageQueue, "6DeinitCounter", portMAX_DELAY);
}

/**
  * @brief  Setter for counter ETR time gating
	* @param  gateTime - units [ms]: 100, 500, 1000, 5000, 10000
  * @retval None
  */
void counterSetEtrGate(uint16_t gateTime){
	xSemaphoreTakeRecursive(counterMutex, portMAX_DELAY);
	counter.counterEtr.gateTime = gateTime;
	xSemaphoreGiveRecursive(counterMutex);
	xQueueSendToBack(counterMessageQueue, "7SetEtrGate", portMAX_DELAY);
}

/**
  * @brief  Setters for counters' IC buffer sizes (number of edges counted)
	* @param  buffer: range between 2 - xxx (max. value depends on free memory availability)
  * @retval None
  */
void counterSetIc1SampleCount(uint16_t buffer){
	xSemaphoreTakeRecursive(counterMutex, portMAX_DELAY);
	counter.counterIc.ic1BufferSizeTemp = buffer + 1;			// PC app sends number of samples but IC needs the number of edges, therefore "buffer + 1";		
	counter.buff1Change = BUFF1_CHANGED;
	xSemaphoreGiveRecursive(counterMutex);	
}

void counterSetIc2SampleCount(uint16_t buffer){
	xSemaphoreTakeRecursive(counterMutex, portMAX_DELAY);	
	counter.counterIc.ic2BufferSizeTemp = buffer + 1;
	counter.buff2Change = BUFF2_CHANGED;
	xSemaphoreGiveRecursive(counterMutex);		
}

/**
  * @brief  Setters for REF counter (TIM4) - PSC and REF numbers to be multiplied (number of REF clock ticks to be counted).
						Note the REF mode uses ETR struct, some ETR functions...
	* @param  buffer - psc or arr: range between 1 - 65536
  * @retval None
  */
void counterSetRefPsc(uint16_t psc){
	counter.counterEtr.psc = psc - 1;
	TIM_ARR_PSC_Config(counter.counterEtr.arr, counter.counterEtr.psc);
}

void counterSetRefArr(uint16_t arr){
	counter.counterEtr.arr = arr - 1;
	TIM_ARR_PSC_Config(counter.counterEtr.arr, counter.counterEtr.psc);
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
  * @state  USED
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

		if(counter.gateChange == GATE_CHANGED){
			counter.counterEtr.arr = counter.counterEtr.arrTemp;			
			counter.counterEtr.psc = counter.counterEtr.pscTemp;
			counter.gateChange = GATE_NOT_CHANGED;
		}
	}
	
	xSemaphoreGiveFromISR(counterMutex, &xHigherPriorityTaskWoken);
	xQueueSendToBackFromISR(messageQueue, "GEtrDataSend", &xHigherPriorityTaskWoken);
}

/**
  * @brief  This function is executed in case of DMA transfer complete event of Input Capture channel 1.
  * @param  Pointer to DMA handle structure.
  * @retval None
  * @state  NOT USED
  */
void COUNTER_IC1_DMA_CpltCallback(DMA_HandleTypeDef *dmah)
{
	portBASE_TYPE xHigherPriorityTaskWoken;
	xSemaphoreTakeFromISR(counterMutex, &xHigherPriorityTaskWoken);
	
	counter.icChannel1 = COUNTER_IRQ_IC1;
	counter.counterIc.ic1psc = TIM_IC1PSC_GetPrescaler();
	
	uint32_t capture1 = counter.counterIc.ic1buffer[counter.counterIc.ic1BufferSize-1] - counter.counterIc.ic1buffer[0];
	counter.counterIc.ic1freq = (double)(tim2clk*(counter.counterIc.psc+1)*counter.counterIc.ic1psc)*((double)(counter.counterIc.ic1BufferSize-1)/(double)capture1);
	TIM_IC1PSC_Config(counter.counterIc.ic1freq);		
	
	counterIc1BufferConfig(counter.counterIc.ic1BufferSize);
	
	xSemaphoreGiveFromISR(counterMutex, &xHigherPriorityTaskWoken);		
	
	/* The expression " > 5" adjusts the frequency of data sending (in this case 5 Hz). 
		 With this parameter also expression " / 5" has to be changed according to first exp. 
		 Implemented in order to lower a bus load (every 200 ms). */
	if ((counter.counterIc.ic1freq / counter.counterIc.ic1BufferSize > 5) && \
											 	(ic1PassNum < (counter.counterIc.ic1freq / counter.counterIc.ic1BufferSize / 5))){
		ic1PassNum++;		
	} else {
		xQueueSendToBackFromISR(messageQueue, "GIcDataSend", &xHigherPriorityTaskWoken);		
		ic1PassNum = 1;
	}
}

/**
  * @brief  This function is executed in case of DMA transfer complete event of Input Capture channel 2.
  * @param  Pointer to DMA handle structure.
  * @retval None
  * @state  NOT USED
  */
void COUNTER_IC2_DMA_CpltCallback(DMA_HandleTypeDef *dmah)
{
	portBASE_TYPE xHigherPriorityTaskWoken;
	xSemaphoreTakeFromISR(counterMutex, &xHigherPriorityTaskWoken);
	
	counter.icChannel2 = COUNTER_IRQ_IC2;	
	counter.counterIc.ic2psc = TIM_IC2PSC_GetPrescaler();
		
	uint32_t capture2 = counter.counterIc.ic2buffer[counter.counterIc.ic2BufferSize-1] - counter.counterIc.ic2buffer[0];
	counter.counterIc.ic2freq = (double)(tim2clk*(counter.counterIc.psc+1)*counter.counterIc.ic2psc)*((double)(counter.counterIc.ic2BufferSize-1)/(double)capture2);
	TIM_IC2PSC_Config(counter.counterIc.ic2freq);		
	
	counterIc2BufferConfig(counter.counterIc.ic2BufferSize);
	
	xSemaphoreGiveFromISR(counterMutex, &xHigherPriorityTaskWoken);	
	
	/* The expression " > 5" adjusts the frequency of data sending (in this case 5 Hz). 
		 With this parameter also expression " / 5" has to be changed according to first exp. 
		 Implemented in order to lower a bus load (every 200 ms). */
	if ((counter.counterIc.ic2freq / counter.counterIc.ic2BufferSize > 5) && \
											 	(ic2PassNum < (counter.counterIc.ic2freq / counter.counterIc.ic2BufferSize / 5))){
		ic2PassNum++;		
	} else {
		xQueueSendToBackFromISR(messageQueue, "LIcDataSend", &xHigherPriorityTaskWoken);		
		ic2PassNum = 1;
	}
}

/**
  * @brief  This function is executed in case of TIM4 period elapse event. Frequencies of IC1 and IC2 channels
						are computed and sent to PC app. This approach replaces DMA data transfer	complete interrupts	
						of both channels - the higher frequencies measured the CPU more heavy loaded.
  * @param  Pointer to TIM handle structure.
  * @retval None
  * @state  USED
  */
void HAL_TIM_PeriodElapsedCallback(TIM_HandleTypeDef *htim)
{
	portBASE_TYPE xHigherPriorityTaskWoken;
	xSemaphoreTakeFromISR(counterMutex, &xHigherPriorityTaskWoken);	
		
	if(counter.icBin != BIN0){
		/* BINx is used to alternate data sending from IC1 and IC2. However, thanks to DMA_TransferComplete function
			 if there's still no data available from one source (ICx) the second one is not stalled. Meaning,
			 IC channels don't have to necessarilly rotate/alternate. */
		counter.icBin = BIN0;
		
		if(DMA_TransferComplete(&hdma_tim2_ch1)){
			
			counter.icChannel1 = COUNTER_IRQ_IC1;	
			counter.counterIc.ic1psc = TIM_IC1PSC_GetPrescaler();
			
			uint32_t capture1 = counter.counterIc.ic1buffer[counter.counterIc.ic1BufferSize-1] - counter.counterIc.ic1buffer[0];
			counter.counterIc.ic1freq = (double)(tim2clk*(counter.counterIc.psc+1)*counter.counterIc.ic1psc)*((double)(counter.counterIc.ic1BufferSize-1)/(double)capture1);
			
			TIM_IC1PSC_Config(counter.counterIc.ic1freq);		
			
			if(counter.buff1Change == BUFF1_CHANGED){
				counter.counterIc.ic1BufferSize = counter.counterIc.ic1BufferSizeTemp;
				counter.icFlag1 = COUNTER_BUFF_FLAG1;	
				counter.buff1Change = BUFF1_NOT_CHANGED;									
			} 
			
			counterIc1BufferConfig(counter.counterIc.ic1BufferSize);	
			
			HAL_DMA_Abort(&hdma_tim2_ch1);
			HAL_DMA_Start(&hdma_tim2_ch1, (uint32_t)&(TIM2->CCR1), (uint32_t)counter.counterIc.ic1buffer, counter.counterIc.ic1BufferSize);	
			xQueueSendToBackFromISR(messageQueue, "GIcDataSend", &xHigherPriorityTaskWoken);					
		}
		
	} else if(counter.icBin != BIN1){
		
		counter.icBin = BIN1;
		
		if(DMA_TransferComplete(&hdma_tim2_ch2_ch4)){
			
			counter.icBin = BIN1;		
			counter.icChannel2 = COUNTER_IRQ_IC2;		
			counter.counterIc.ic2psc = TIM_IC2PSC_GetPrescaler();
				
			uint32_t capture2 = counter.counterIc.ic2buffer[counter.counterIc.ic2BufferSize-1] - counter.counterIc.ic2buffer[0];
			counter.counterIc.ic2freq = (double)(tim2clk*(counter.counterIc.psc+1)*counter.counterIc.ic2psc)*((double)(counter.counterIc.ic2BufferSize-1)/(double)capture2);
			
			TIM_IC2PSC_Config(counter.counterIc.ic2freq);		

			if(counter.buff2Change == BUFF2_CHANGED){
				counter.counterIc.ic2BufferSize = counter.counterIc.ic2BufferSizeTemp;
				counter.buff2Change = BUFF2_NOT_CHANGED;
				counter.icFlag2 = COUNTER_BUFF_FLAG2;						
			}
			
			counterIc2BufferConfig(counter.counterIc.ic2BufferSize);
			
			HAL_DMA_Abort(&hdma_tim2_ch2_ch4);
			HAL_DMA_Start(&hdma_tim2_ch2_ch4, (uint32_t)&(TIM2->CCR2), (uint32_t)counter.counterIc.ic2buffer, counter.counterIc.ic2BufferSize);			
			xQueueSendToBackFromISR(messageQueue, "LIcDataSend", &xHigherPriorityTaskWoken);	
		}
	}

	xSemaphoreGiveFromISR(counterMutex, &xHigherPriorityTaskWoken);				
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
	switch(gateTime){
		case 100:														/* min.	gate time 00.10 second */
			counter.counterEtr.pscTemp = 7199;
			counter.counterEtr.arrTemp = 999;			
			break;
		case 500: 													/* ----	gate time 00.50 second */
			counter.counterEtr.pscTemp = 5999;
			counter.counterEtr.arrTemp = 5999;		
			break;		
		case 1000: 													/* ----	gate time 01.00 second */
			counter.counterEtr.pscTemp = 7199;
			counter.counterEtr.arrTemp = 9999;		
			break;				
		case 5000: 													/* ----	gate time 05.00 second */
			counter.counterEtr.pscTemp = 59999;
			counter.counterEtr.arrTemp = 5999;	
			break;		
		case 10000: 												/* max. gate time 10.00 second */
			counter.counterEtr.pscTemp = 35999;
			counter.counterEtr.arrTemp = 19999;			
			break;
		default:
			break;			
	}
	
	TIM_ARR_PSC_Config(counter.counterEtr.arrTemp, counter.counterEtr.pscTemp);
	counter.gateChange = GATE_CHANGED;
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
		counter.icFlag1 = COUNTER_BUFF_FLAG1;
		counter.counterIc.ic1BufferSize = ((ic1buffSize/counter.counterIc.ic1psc)*counter.counterIc.ic1psc+counter.counterIc.ic1psc);			
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
		counter.icFlag2 = COUNTER_BUFF_FLAG2;
		counter.counterIc.ic2BufferSize = ((ic2buffSize/counter.counterIc.ic2psc)*counter.counterIc.ic2psc+counter.counterIc.ic2psc);
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
	counter.counterEtr.gateTime = 100;				/* 1000 ms = 1 s */
	counter.counterEtr.buffer = 0;
	counter.counterEtr.etrp = 1;

	/* IC counter default values */
	counter.counterIc.psc = 0;		
	counter.counterIc.arr = 0xFFFFFFFF;
	/* BUFFER SIZE REPRESENTS THE NUMBER OF EDGES TAKEN ON INPUT,
		 NUMBER OF SAMPLES = buffer size + 1 */
	counter.counterIc.ic1BufferSize = 2;			/* the lowest value of icxBufferSize is 2! */
	counter.counterIc.ic2BufferSize = 2;			/* 1 sample by default */
	counter.counterIc.ic1psc = 1;
	counter.counterIc.ic2psc = 1;
	counter.icChannel1 = COUNTER_IRQ_IC1_PASS;
	counter.icChannel2 = COUNTER_IRQ_IC2_PASS;
	counter.icFlag1 = COUNTER_BUFF_FLAG1_PASS;
	counter.icFlag2 = COUNTER_BUFF_FLAG2_PASS;
	counter.buff1Change = BUFF1_NOT_CHANGED;
	counter.buff2Change = BUFF2_NOT_CHANGED;
	counter.gateChange = GATE_NOT_CHANGED;
}

	#endif //USE_COUNTER

