/*
  *****************************************************************************
  * @file    gen_pwm.c
  * @author  HeyBirdie
  * @date    Aug 1, 2017
  * @brief   This file contains pwm generator functions
  ***************************************************************************** 
*/ 

// Includes ===================================================================
	#ifdef USE_GEN_PWM
#include "cmsis_os.h"
#include "mcu_config.h"
#include "comms.h"
#include "gen_pwm.h"
#include "tim.h"

// External variables definitions =============================================
xQueueHandle genPwmMessageQueue;
xSemaphoreHandle genPwmMutex;

volatile genPwmTypeDef genPwm;

// Function definitions =======================================================
/**
  * @brief  Counter task function.
  * task is getting messages from other tasks and takes care about counter functions
  * @param  Task handler, parameters pointer
  * @retval None
  */
void GenPwmTask(void const *argument)
{
	genPwmMessageQueue = xQueueCreate(5, 20);  // xQueueCreate(5, sizeof(double)); e.g.
	genPwmMutex = xSemaphoreCreateRecursiveMutex();	
	
	if(genPwmMessageQueue == 0){
		while(1); // Queue was not created and must not be used.
	}
	char message[20];
	
	while(1){
		
		xQueueReceive(genPwmMessageQueue, message, portMAX_DELAY);		
		xSemaphoreTakeRecursive(genPwmMutex, portMAX_DELAY);
		
		if(message[0]=='1'){
			//genPwmStart();
		}else if(message[0]=='2'){
			//genPwmStop();
		}else if(message[0]=='3'){
			
		}else if(message[0]=='4'){
			
		}else if(message[0]=='5'){
			
		}else if(message[0]=='6'){
			
		}else if(message[0]=='7'){
			
		}else if(message[0]=='8'){
			
		}	
		
		xSemaphoreGiveRecursive(genPwmMutex);
	}
}

/* ************************************************************************************** */
/* ---------------------------- PWM generator basic settings ---------------------------- */
/* ************************************************************************************** */
void genPwmSendStart(void){
	xQueueSendToBack(genPwmMessageQueue, "1StartGenPwm", portMAX_DELAY);
}

void genPwmSendStop(void){
	xQueueSendToBack(genPwmMessageQueue, "2StopGenPwm", portMAX_DELAY);
}
	
	
	#endif //USE_GEN_PWM
		

