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
	
	if(logAnlysMessageQueue == 0){
		while(1); // Queue was not created and must not be used.
	}
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
			
		}else if(message[0]=='6'){
			
		}else if(message[0]=='7'){
			
		}else if(message[0]=='8'){
			
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


/* ************************************************************************************** */
/* --------------------------- Logic analyzer basic settings ---------------------------- */
/* ************************************************************************************** */
void logAnlysInit(void){
	
}	

void logAnlysDeinit(void){

}	

void logAnlysStart(void){
	
}	

void logAnlysStop(void){
	
}	

void logAnlysSetDefault(void){
	
}



	
	#endif //USE_LOG_ANLYS
		

