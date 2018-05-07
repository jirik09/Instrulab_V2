/*
  *****************************************************************************
  * @file    logic_analyzer.c
  * @author  HeyBirdie
  * @date    Nov 4, 2017
  * @brief   This file contains logic analyzer functions.
  ***************************************************************************** 
*/ 

// Includes ===================================================================
	#ifdef USE_LOG_ANLYS
#include "cmsis_os.h"
#include "mcu_config.h"
#include "comms.h"
#include "logic_analyzer.h"
#include "tim.h"
#include "scope.h"
#include "FreeRTOSConfig.h"
#include "portmacro.h"


// External variables definitions =============================================
xQueueHandle logAnlysMessageQueue;
xSemaphoreHandle logAnlysMutex;

volatile logAnlysTypeDef logAnlys;

// Function definitions =======================================================
/**
  * @brief  Logic analyzer task.
  * task is getting messages from other tasks and takes care about counter functions
  * @param  Task handler, parameters pointer
  * @retval None
  */
void LogAnlysTask(void const *argument)
{
	logAnlysMessageQueue = xQueueCreate(5, 20);  // xQueueCreate(5, sizeof(double)); e.g.
	logAnlysMutex = xSemaphoreCreateRecursiveMutex();	
	
	char message[20];	
	logAnlysSetDefault();
	
	while(1){
		
		xQueueReceive(logAnlysMessageQueue, message, portMAX_DELAY);		
		xSemaphoreTakeRecursive(logAnlysMutex, portMAX_DELAY);
		
		if(message[0]=='1'){
			logAnlysInit();
		}else if(message[0]=='2'){
			logAnlysDeinit();
		}else if(message[0]=='3'){
			logAnlysStart();
		}else if(message[0]=='4'){
			logAnlysStop();
		}else if(message[0]=='5'){
//			logAnlysSetTriggerRising();				
		}else if(message[0]=='6'){
//			logAnlysSetTriggerFalling();
		}else if(message[0]=='7'){						
			xQueueSendToBack(messageQueue, "LogAnlysDataSend", portMAX_DELAY); 
		}			
		
		xSemaphoreGiveRecursive(logAnlysMutex);
	}
}

/* ************************************************************************************** */
/* ---------------------- Logic analyzer basic settings via queue ----------------------- */
/* ************************************************************************************** */
void logAnlysSendInit(void){
	xQueueSendToBack(logAnlysMessageQueue, "1InitLogAnlys", portMAX_DELAY);
}

void logAnlysSendDeinit(void){
	xQueueSendToBack(logAnlysMessageQueue, "2DeinitLogAnlys", portMAX_DELAY);
}

void logAnlysSendStart(void){
	xQueueSendToBack(logAnlysMessageQueue, "3StartLogAnlys", portMAX_DELAY);
}

void logAnlysSendStop(void){
	xQueueSendToBack(logAnlysMessageQueue, "4StopLogAnlys", portMAX_DELAY);
}

//void logAnlysSendTriggerRising(void){
//	xQueueSendToBack(logAnlysMessageQueue, "5TrigRisLogAnlys", portMAX_DELAY);
//}

//void logAnlysSendTriggerFalling(void){
//	xQueueSendToBack(logAnlysMessageQueue, "6TrigFallLogAnlys", portMAX_DELAY);
//}

/* ************************************************************************************** */
/* ------------------------ Logic analyzer Interrupts/Callbacks ------------------------- */
/* ************************************************************************************** */
/* TIM4 overflow (Update Event after posttrigger) 
	 occured after trigger event that started TIM4. */
void logAnlysPeriodElapsedCallback(void){
	portBASE_TYPE xHigherPriorityTaskWoken;
//	xSemaphoreTakeFromISR(logAnlysMutex, &xHigherPriorityTaskWoken);	
	xQueueSendToBackFromISR(logAnlysMessageQueue, "7EndOfSampLogAnlys", &xHigherPriorityTaskWoken);	
//	xSemaphoreGiveFromISR(logAnlysMutex, &xHigherPriorityTaskWoken);
}

/* ************************************************************************************** */
/* --------------------------- Logic analyzer basic settings ---------------------------- */
/* ************************************************************************************** */
void logAnlysInit(void){
	/* Log. analyzer uses TIM4 as well as Universal counter. Therefore, there
		 has to be some clue for msp_init function to decide which functionality
		 to initialize - LOGA_ENABLED */
	logAnlys.state = LOGA_ENABLED;
	TIM_LogAnlys_Init();
}	

void logAnlysDeinit(void){
	TIM_LogAnlys_Deinit();
	logAnlys.state = LOGA_DISABLED;
}	

void logAnlysStart(void){
	/* Start sampling */
	TIM_LogAnlys_Start();		
	/* Wait the pretrigger time */
	vTaskDelay(logAnlys.preTriggerTime/*/portTICK_RATE_MS*/);
	
	if(logAnlys.triggerMode == LOGA_MODE_AUTO){
		/* In AUTO trigger mode the posttriger is started without event trigger. After posttrigger 
			 time elapses the data is sent to PC even if the trigger does not occur. */
		TIM_PostTrigger_SoftwareStart();
	}
	/* Enable trigger after pretrigger time elapses */
	TIM_EnableTrigger();	
}	

void logAnlysStop(void){
	TIM_LogAnlys_Stop();
}	

/* Configure TIM1 to trigger DMA data transfering with required frequency. */
void logAnlysSetSamplingFreq(uint32_t arrPsc){
	TIM_SamplingFreq_ARR_PSC_Reconfig(arrPsc);
}
	
/* Configure TIM4 to stop TIM1 */
void logAnlysSetPosttrigger(uint32_t arrPsc){
	TIM_PostTrigger_ARR_PSC_Reconfig(arrPsc);
}

void logAnlysSetPretrigger(uint32_t timeInMilliseconds){
	xSemaphoreTakeRecursive(logAnlysMutex, portMAX_DELAY);
	/* logAnlys task to sleep for defined time in milliseconds */
	logAnlys.preTriggerTime = timeInMilliseconds;
	xSemaphoreGiveRecursive(logAnlysMutex);
}

void logAnlysSetSamplesNum(uint16_t samplesNum){	
	xSemaphoreTakeRecursive(logAnlysMutex, portMAX_DELAY);
	logAnlys.samplesNumber = samplesNum;
	xSemaphoreGiveRecursive(logAnlysMutex);
}

void logAnlysSetTriggerRising(void){
	TIM_LogAnlys_RisingTrigger();
}

void logAnlysSetTriggerFalling(void){
	TIM_LogAnlys_FallingTrigger();
}

void logAnlysSetTriggerChannel(uint32_t chan){
	TIM_TriggerConfig((uint8_t)chan);
}

void logAnlysSetDefault(void){
/* By default: dataLength = 1 Ksamples, samplingFreq = 10 Ksmpls / s, trigger = 50 %
	 Therefore, 100 ms * 50 % = 50 ms. It applies that postTrigger is set with period 
	 50 ms as well as in One Pulse mode. */
	logAnlys.preTriggerTime = 50;
	logAnlys.samplesNumber = 1000;
	logAnlys.trigConfig = TRIG_CHAN1;
	logAnlys.trigEdge = TRIG_EDGE_RISING;
	logAnlys.triggerMode = LOGA_MODE_AUTO;
	logAnlys.trigOccur = TRIG_NOT_OCCURRED;
	logAnlys.bufferMemory = (uint16_t *)scopeBuffer;
}


	#endif //USE_LOG_ANLYS
		

