/*
  *****************************************************************************
  * @file    counter.c
  * @author  HeyBirdie
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
#include "portmacro.h"


// External variables definitions =============================================
xQueueHandle counterMessageQueue;
xSemaphoreHandle counterMutex;
portTickType xStartTime;

volatile counterTypeDef counter;
uint32_t startTime = 0;

/* Obsolete variables */
static uint32_t ic1PassNum = 1;
static uint32_t ic2PassNum = 1;

// Function definitions ========================================================
/**
  * @brief  Counter task function.
  * task is getting messages from other tasks and takes care about counter functions
  * @param  Task handler, parameters pointer
  * @retval None
  */
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
			counterInitTI();
		}else if(message[0]=='5'){
			counterStart();
		}else if(message[0]=='6'){
			counterStop();
		}else if(message[0]=='7'){
			counter_deinit();
		}else if(message[0]=='8'){
			counterGateConfig(counter.counterEtr.gateTime);
		}else if(message[0]=='9'){
			
		}	
		xSemaphoreGiveRecursive(counterMutex);
	}
}

/* ************************************************************************************** */
/* -------------------------------- Counter basic settings ------------------------------ */
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
		case TI:
			xQueueSendToBack(counterMessageQueue, "4SetTiMode", portMAX_DELAY);
			break;		
	}
}

void counterSendStart(void){	
	xQueueSendToBack(counterMessageQueue, "5StartCounter", portMAX_DELAY);
}

void counterSendStop(void){	
	xQueueSendToBack(counterMessageQueue, "6StopCounter", portMAX_DELAY);
}

void counterDeinit(void){
	xQueueSendToBack(counterMessageQueue, "7DeinitCounter", portMAX_DELAY);
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

void counterInitTI(void){
	counter_deinit();
	counter.state = COUNTER_TI;
	TIM_counter_ti_init();
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
		case COUNTER_TI:
			TIM_ti_deinit();
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
		case COUNTER_TI:
			TIM_TI_Start();
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
		case COUNTER_TI:
			TIM_TI_Stop();		
			break;		
		case COUNTER_IDLE:
			/* no hacer nada */
			break;
	}	
}

/**
  * @brief  Setter for counter ETR time gating
	* @param  gateTime - units [ms]: 100, 500, 1000, 5000, 10000
  * @retval None
  */
void counterSetEtrGate(uint16_t gateTime){
	counter.counterEtr.gateTime = gateTime;			
	xQueueSendToBack(counterMessageQueue, "8SetEtrGate", portMAX_DELAY);
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

/**
  * @brief  Setters for counters' IC buffer sizes (number of edges counted)
	* @param  buffer: range between 2 - xxx (max. value depends on free memory availability)
  * @retval None
  */
void counterSetIc1SampleCount(uint16_t buffer){
	xSemaphoreTakeRecursive(counterMutex, portMAX_DELAY);
	counter.counterIc.ic1BufferSize = buffer + 1;						 // PC app sends number of samples but IC needs the number of edges, therefore "buffer + 1";		
	DMA_Restart(&hdma_tim2_ch1);	
	xSemaphoreGiveRecursive(counterMutex);
}

void counterSetIc2SampleCount(uint16_t buffer){
	xSemaphoreTakeRecursive(counterMutex, portMAX_DELAY);	
	counter.counterIc.ic2BufferSize = buffer + 1;	
	DMA_Restart(&hdma_tim2_ch2_ch4);	
	xSemaphoreGiveRecursive(counterMutex);		
}

/**
  * @brief  Setters for counters' IC signal prescalers
	* @param  presc: 1, 2, 4, 8
  * @retval None
  */
void counterSetIc1Prescaler(uint16_t presc){	
	TIM_IC1_PSC_Config(presc);
	DMA_Restart(&hdma_tim2_ch1);
}

void counterSetIc2Prescaler(uint16_t presc){		
	TIM_IC2_PSC_Config(presc);	
	DMA_Restart(&hdma_tim2_ch2_ch4);	
}

/**
  * @brief  Enable/Disable Duty Cycle measuring under input capture (IC) mode.
						Used for decision purposes.
	* @param  None
  * @retval None
  */

/* Cahnnel 1 */
void counterIc1DutyCycleInit(void){	
	counter.icDutyCycle = DUTY_CYCLE_CH1_ENABLED;
	TIM_IC_DutyCycle_Init();	
}

void counterIc1DutyCycleDeinit(void){	
	TIM_IC_DutyCycle_Deinit();		
	counter.icDutyCycle = DUTY_CYCLE_DISABLED;
}

/* Channel 2 */
void counterIc2DutyCycleInit(void){	
	counter.icDutyCycle = DUTY_CYCLE_CH2_ENABLED;
	TIM_IC_DutyCycle_Init();	
}

void counterIc2DutyCycleDeinit(void){		
	TIM_IC_DutyCycle_Deinit();		
	counter.icDutyCycle = DUTY_CYCLE_DISABLED;
}

/* Common for both channels */
void counterIcDutyCycleEnable(void){
	TIM_IC_DutyCycle_Start();
}

void counterIcDutyCycleDisable(void){
	TIM_IC_DutyCycle_Stop();
}

/**
	* @brief  Functions used to select active adges (events) - dedicated to Duty Cycle 
						measurement configuration of IC and to events of TI mode. 						
	* @param  none
  * @retval none 
  */
void counterSetIcTi1_RisingFalling(void){	
	TIM_IC1_RisingFalling();	
	DMA_Restart(&hdma_tim2_ch1);
}	

void counterSetIcTi1_Rising(void){
	TIM_IC1_RisingOnly();	
}	

void counterSetIcTi1_Falling(void){
	TIM_IC1_FallingOnly();	
}

void counterSetIcTi2_RisingFalling(void){
	TIM_IC2_RisingFalling();
	DMA_Restart(&hdma_tim2_ch2_ch4);	
}	

void counterSetIcTi2_Rising(void){
	TIM_IC2_RisingOnly();	
}	

void counterSetIcTi2_Falling(void){
	TIM_IC2_FallingOnly();	
}

/**
  * @brief  Setter for counter TI timeout
	* @param  timeout: 500 - 28000 [ms]
  * @retval None
  */
void counterSetTiTimeout(uint16_t timeout){
	counter.counterIc.tiTimeout = timeout;				
}

/* ************************************************************************************** */
/* ----------------------------- Counter callback functions ----------------------------- */
/* ************************************************************************************** */
/**
  * @brief  This function is executed in case of DMA transfer complete event of ETR or REF mode.
	*					DMA transfer complete event is triggered after TIM4 gate time elapse.
  * @param  Pointer to DMA handle structure.
  * @retval None
  * @state  VERY USED
  */
void COUNTER_ETR_DMA_CpltCallback(DMA_HandleTypeDef *dmah)
{			
	portBASE_TYPE xHigherPriorityTaskWoken;
	xSemaphoreTakeFromISR(counterMutex, &xHigherPriorityTaskWoken);
	
	/***** Counter ETR handle *****/
	if(counter.state == COUNTER_ETR){			
		counter.counterEtr.etrp = TIM_ETPS_GetPrescaler();
		double gateFreq = ((double)tim4clk / (double)((counter.counterEtr.arr + 1) * (counter.counterEtr.psc + 1)));			/* TIM4 gating frequency */	
		counter.counterEtr.freq = ((double)counter.counterEtr.buffer * gateFreq * counter.counterEtr.etrp);								/* Sampled frequency */
		TIM_ETRP_Config(counter.counterEtr.freq);	
		
		if(counter.sampleCntChange != SAMPLE_COUNT_CHANGED){
			xQueueSendToBackFromISR(messageQueue, "GEtrDataSend", &xHigherPriorityTaskWoken);
		}else{
			counter.sampleCntChange = SAMPLE_COUNT_NOT_CHANGED;
		}		
		
	/***** Counter REF handle *****/
	}else if(counter.state == COUNTER_REF){		
		if((counter.sampleCntChange != SAMPLE_COUNT_CHANGED) && (xTaskGetTickCount() - xStartTime) < 100){
			xQueueSendToBackFromISR(messageQueue, "ORefWarning", &xHigherPriorityTaskWoken);
			TIM_REF_SecondInputDisable();				
		}else if(counter.sampleCntChange != SAMPLE_COUNT_CHANGED){	
			xQueueSendToBackFromISR(messageQueue, "GRefDataSend", &xHigherPriorityTaskWoken);			
		}else{
			counter.sampleCntChange = SAMPLE_COUNT_NOT_CHANGED;
		}				
	}
	
	xSemaphoreGiveFromISR(counterMutex, &xHigherPriorityTaskWoken);
}

/**
  * @brief  This function is executed in case of TIM4 period elapse event. This function accompanies 
						IC (freq. meas. + Duty Cycle) and TI (time interval between 2 events meas.) modes.
  * @param  Pointer to TIM handle structure.
  * @retval None
  * @state  VERY USED
  */
void HAL_TIM_PeriodElapsedCallback(TIM_HandleTypeDef *htim)
{
	if(counter.icDutyCycle == DUTY_CYCLE_DISABLED){
		if(counter.state == COUNTER_IC){
			counterIcProcess();
		}else{
			counterTiProcess();
		}						
	}else{		
		counterIcDutyCycleProcess();
	}
}

/**
  * @brief  Function colaborating with HAL_TIM_PeriodElapsedCallback to handle every 100 ms captured events if the required samples were transfered.
						Frequency of IC1 or IC2 channel is computed and sent to PC app. This approach replaces DMA data transfercomplete interrupts	of both 
						channels - the higher frequencies measured the CPU more heavy loaded, therefore replaced.
						BIN "semaphore" implemented due to communication issue when data send to PC. (too slow)
  * @param  None
  * @retval None
  * @state  VERY USED
  */
void counterIcProcess(void)
{	
	portBASE_TYPE xHigherPriorityTaskWoken;
	xSemaphoreTakeFromISR(counterMutex, &xHigherPriorityTaskWoken);	
	
	if(counter.icBin != BIN0){
		/* BINx is used to alternate data sending from IC1 and IC2. Thanks to DMA_TransferComplete function
			 if there's still no data available from one source (ICx) the second one is not stalled. Meaning,
			 IC channels don't have to necessarilly rotate/alternate if the difference of frequencies is big. */
		counter.icBin = BIN0;
		
		if(DMA_TransferComplete(&hdma_tim2_ch1)){				

			counter.counterIc.ic1psc = TIM_IC1PSC_GetPrescaler();			
			uint32_t capture1 = counter.counterIc.ic1buffer[counter.counterIc.ic1BufferSize-1] - counter.counterIc.ic1buffer[0];
			counter.counterIc.ic1freq = (double)(tim2clk*(counter.counterIc.psc+1)*counter.counterIc.ic1psc)*((double)(counter.counterIc.ic1BufferSize-1)/(double)capture1);				
			
			DMA_Restart(&hdma_tim2_ch1);
			counter.icChannel1 = COUNTER_IRQ_IC1;
			xQueueSendToBackFromISR(messageQueue, "GIcDataSend", &xHigherPriorityTaskWoken);									
		}
		
	}else if(counter.icBin != BIN1){
		
		counter.icBin = BIN1;
		
		if(DMA_TransferComplete(&hdma_tim2_ch2_ch4)){
						
			counter.counterIc.ic2psc = TIM_IC2PSC_GetPrescaler();				
			uint32_t capture2 = counter.counterIc.ic2buffer[counter.counterIc.ic2BufferSize-1] - counter.counterIc.ic2buffer[0];
			counter.counterIc.ic2freq = (double)(tim2clk*(counter.counterIc.psc+1)*counter.counterIc.ic2psc)*((double)(counter.counterIc.ic2BufferSize-1)/(double)capture2);					
						
			DMA_Restart(&hdma_tim2_ch2_ch4);		
			counter.icChannel2 = COUNTER_IRQ_IC2;		
			xQueueSendToBackFromISR(messageQueue, "GIcDataSend", &xHigherPriorityTaskWoken);	
		}
	}
	
	xSemaphoreGiveFromISR(counterMutex, &xHigherPriorityTaskWoken);			
}

/**
  * @brief  Function colaborating with HAL_TIM_PeriodElapsedCallback to handle 
						2 independent input events on 2 separate channels (Time Interval meas. 
						mode). CounterIc.ic1freq is used to send time delay between two events.
  * @param  None
  * @retval None
  * @state  VERY USED
  */
void counterTiProcess(void)
{
	portBASE_TYPE xHigherPriorityTaskWoken;
	xSemaphoreTakeFromISR(counterMutex, &xHigherPriorityTaskWoken);	
		
	/* Check timeout. */
	if((xTaskGetTickCountFromISR() - xStartTime) <= counter.counterIc.tiTimeout){
		/* Check both events occured, if not, continue. */		
		if(DMA_TransferComplete(&hdma_tim2_ch1) && DMA_TransferComplete(&hdma_tim2_ch2_ch4)){		
			
			TIM_TI_Stop();
			if(counter.counterIc.ic1buffer[0] > counter.counterIc.ic2buffer[0]){
				/* ic1freq represents time delay in this case */
				counter.counterIc.ic1freq = (counter.counterIc.ic1buffer[0] - counter.counterIc.ic2buffer[0]) / (double)tim2clk;
				counter.tiState = BIGGER_BUFF_CH1;	
			}else if(counter.counterIc.ic1buffer[0] < counter.counterIc.ic2buffer[0]){
				/* ic1freq represents time delay in this case */
				counter.counterIc.ic2freq = (counter.counterIc.ic2buffer[0] - counter.counterIc.ic1buffer[0]) / (double)tim2clk;
				counter.tiState = BIGGER_BUFF_CH2;
			}else{
				counter.tiState = EQUAL;
			}
			
			xQueueSendToBackFromISR(messageQueue, "GTiBuffersTaken", &xHigherPriorityTaskWoken);	
		}		
		
	/* If timeout occured stop TI counter and send alert to PC application. */
	}else{		
		
		counter.tiState = TIMEOUT;		
		TIM_TI_Stop();					
		xQueueSendToBackFromISR(messageQueue, "GTiTimoutOccured", &xHigherPriorityTaskWoken);	
	}
	
	xSemaphoreGiveFromISR(counterMutex, &xHigherPriorityTaskWoken);			
}

/**
  * @brief  Function colaborating with HAL_TIM_PeriodElapsedCallback to handle 
						one input (TI1 or TI2) that is feeding two IC registers to calculate pulse width
						and duty cycle. BIN implemented due to UART speed issue when sending data to PC.
  * @param  None
  * @retval None
  * @state  VERY USED
  */
void counterIcDutyCycleProcess(void)
{
	portBASE_TYPE xHigherPriorityTaskWoken;
	xSemaphoreTakeFromISR(counterMutex, &xHigherPriorityTaskWoken);	

	if(counter.icDutyCycle == DUTY_CYCLE_CH1_ENABLED){	
		if(DMA_TransferComplete(&hdma_tim2_ch1)){
			/* Calculate duty cycle and pulse width. Frequency struct variables temporarily used. */
			counter.counterIc.ic1freq = (counter.counterIc.ic2buffer[0] / (double)counter.counterIc.ic1buffer[0]) * 100;
			counter.counterIc.ic2freq = counter.counterIc.ic2buffer[0] / (double)tim2clk;
				
			TIM_IC_DutyCycleDmaRestart();		
			
			/* DMA transfers some unspecified number immediately after 
				 Duty Cycle start - getting rid of it. */
			if(counter.icBin == BIN0){
				counter.icBin = BIN1;
			}else{
				xQueueSendToBackFromISR(messageQueue, "GIc1DutyCycle", &xHigherPriorityTaskWoken);		
			}								
		}
	}else if(counter.icDutyCycle == DUTY_CYCLE_CH2_ENABLED){
		if(DMA_TransferComplete(&hdma_tim2_ch2_ch4)){			
			counter.counterIc.ic1freq = (counter.counterIc.ic1buffer[0] / (double)counter.counterIc.ic2buffer[0]) * 100;
			counter.counterIc.ic2freq = counter.counterIc.ic1buffer[0] / (double)tim2clk;
			
			TIM_IC_DutyCycleDmaRestart();			
			
			if(counter.icBin == BIN0){
				counter.icBin = BIN1;
			}else{
				xQueueSendToBackFromISR(messageQueue, "GIc2DutyCycle", &xHigherPriorityTaskWoken);		
			}					
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
			counter.counterEtr.psc = 7199;
			counter.counterEtr.arr = 999;			
			break;
		case 500: 													/* ----	gate time 00.50 second */
			counter.counterEtr.psc = 5999;
			counter.counterEtr.arr = 5999;		
			break;		
		case 1000: 													/* ----	gate time 01.00 second */
			counter.counterEtr.psc = 7199;
			counter.counterEtr.arr = 9999;		
			break;				
		case 5000: 													/* ----	gate time 05.00 second */
			counter.counterEtr.psc = 59999;
			counter.counterEtr.arr = 5999;	
			break;		
		case 10000: 												/* max. gate time 10.00 second */
			counter.counterEtr.psc = 35999;
			counter.counterEtr.arr = 19999;			
			break;
		default:
			break;			
	}
	
	TIM_ARR_PSC_Config(counter.counterEtr.arr, counter.counterEtr.psc);
}


/* ************************************************************************************** */
/* --------------------------- Obsolete functions - not used ---------------------------- */
/* ************************************************************************************** */
/**
  * @brief  This function is executed in case of DMA transfer complete event of Input Capture channel 1.
  * @param  Pointer to DMA handle structure.
  * @retval None
  * @state  NOT USED, OBSOLETE
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
  * @state  NOT USED, OBSOLETE
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
	* @brief  Function adjusting IC1 buffer size (number of edges counted) if user sends a value that cannot be served.
						For instance, if user sends number 13 (number of edges counted) and IC prescaler is 4, it is impossible to serve this value
						due to this prescaler. Therefore, instead (13/4)*4 = 3*4 + 4 = 16 will be set.
	* @param  ic1buffSize: buffer size (e.g. sent from user PC app)
  * @retval none 
  * @state  NOT USED, OBSOLETE
  */
void counterIc1BufferConfig(uint16_t ic1buffSize)
{
	if((ic1buffSize % counter.counterIc.ic1psc)!=0){	
		//counter.icFlag1 = COUNTER_BUFF_FLAG1;
		counter.counterIc.ic1BufferSize = ((ic1buffSize/counter.counterIc.ic1psc)*counter.counterIc.ic1psc+counter.counterIc.ic1psc);			
	}
}

/**
	* @brief  Function adjusting IC2 buffer size (number of edges counted)
	* @param  ic1buffSize: buffer size (e.g. sent from user PC app)
  * @retval none 
  * @state  NOT USED, OBSOLETE
  */
void counterIc2BufferConfig(uint16_t ic2buffSize)
{
	if((ic2buffSize % counter.counterIc.ic2psc)!=0){	
		//counter.icFlag2 = COUNTER_BUFF_FLAG2;
		counter.counterIc.ic2BufferSize = ((ic2buffSize/counter.counterIc.ic2psc)*counter.counterIc.ic2psc+counter.counterIc.ic2psc);
	}
}

/**
  * @brief  Counter set Default values
  * @param  None
  * @retval None
	* @state 	NOT USED, OBSOLETE
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
	counter.sampleCntChange = SAMPLE_COUNT_CHANGED;
}

	#endif //USE_COUNTER

