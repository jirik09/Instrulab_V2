/*
  *****************************************************************************
  * @file    sync_pwm.c
  * @author  HeyBirdie
  * @date    Oct 15, 2017
  * @brief   This file contains sync PWM generator functions.
  ***************************************************************************** 
*/ 

// Includes ===================================================================
	#ifdef USE_SYNC_PWM
#include "cmsis_os.h"
#include "mcu_config.h"
#include "comms.h"
#include "sync_pwm.h"
#include "tim.h"

// External variables definitions =============================================
xQueueHandle syncPwmMessageQueue;
xSemaphoreHandle syncPwmMutex;

volatile syncPwmTypeDef syncPwm;

// Function definitions =======================================================
/**
  * @brief  Generator of synchronized PWM channels task function.
  * task is getting messages from other tasks and takes care about counter functions
  * @param  Task handler, parameters pointer
  * @retval None
  */
void SyncPwmTask(void const *argument)
{
	syncPwmMessageQueue = xQueueCreate(5, 20);  // xQueueCreate(5, sizeof(double)); e.g.
	syncPwmMutex = xSemaphoreCreateRecursiveMutex();	
	
	if(syncPwmMessageQueue == 0){
		while(1); // Queue was not created and must not be used.
	}
	char message[20];
	
	syncPwmSetDefault();
	
	while(1){
		
		xQueueReceive(syncPwmMessageQueue, message, portMAX_DELAY);		
		xSemaphoreTakeRecursive(syncPwmMutex, portMAX_DELAY);
		
		if(message[0]=='1'){
			syncPwmInit();
		}else if(message[0]=='2'){
			syncPwmDeinit();
		}else if(message[0]=='3'){
			syncPwmStart();
		}else if(message[0]=='4'){
			syncPwmStop();
		}else if(message[0]=='5'){
			
		}else if(message[0]=='6'){
			
		}else if(message[0]=='7'){
			
		}else if(message[0]=='8'){
			
		}	
		
		xSemaphoreGiveRecursive(syncPwmMutex);
	}
}

/* ************************************************************************************** */
/* -------------------- Sync PWM generator basic settings via queue --------------------- */
/* ************************************************************************************** */
void syncPwmSendInit(void){
	xQueueSendToBack(syncPwmMessageQueue, "1InitSyncPwm", portMAX_DELAY);
}

void syncPwmSendDeinit(void){
	xQueueSendToBack(syncPwmMessageQueue, "2DeinitSyncPwm", portMAX_DELAY);
}

void syncPwmSendStart(void){
	xQueueSendToBack(syncPwmMessageQueue, "3StartSyncPwm", portMAX_DELAY);
}

void syncPwmSendStop(void){
	xQueueSendToBack(syncPwmMessageQueue, "4StopSyncPwm", portMAX_DELAY);
}


/* ************************************************************************************** */
/* ------------------------- Sync PWM generator basic settings -------------------------- */
/* ************************************************************************************** */
void syncPwmInit(void){
	TIM_SYNC_PWM_Init();
}	

void syncPwmDeinit(void){
	TIM_SYNC_PWM_Deinit();
}	

void syncPwmStart(void){
	TIM_SYNC_PWM_Start();
}	

void syncPwmStop(void){
	TIM_SYNC_PWM_Stop();
}	

/* The received number determines what channel needs to be configured. */
void syncPwmChannelNumber(uint8_t chanNum)
{	
	syncPwm.channelToConfig = (syncPwmChannelTypeDef)chanNum;
}

/* Set two DMA transfers to transfer the required data to CCR1 register
	 upon DMA Output Compare event. */
void syncPwmChannelConfig(uint32_t ccr1st, uint16_t ccr2nd)
{	
	TIM_SYNC_PWM_DMA_ChanConfig(ccr1st, ccr2nd);
}

/* Frequency reconfiguring. */
void syncPwmFreqReconfig(uint32_t arrPsc)
{
	TIM_ARR_PSC_Reconfig(arrPsc);
}
	
void syncPwmSetChannelState(uint8_t channel, uint8_t state)
{
	TIM_SYNC_PWM_ChannelState(channel, state);
}

void syncPwmSetStepMode(void)
{
	TIM_SYNC_PWM_StepMode_Enable();
}

void syncPwmResetStepMode(void)
{
	TIM_SYNC_PWM_StepMode_Disable();
}

void syncPwmSetDefault(void)
{
	/* Four channels to generate by default. */
	syncPwm.chan1 = CHAN_ENABLE;
	syncPwm.chan2 = CHAN_ENABLE;
	syncPwm.chan3 = CHAN_ENABLE;
	syncPwm.chan4 = CHAN_ENABLE;
	
	/* Default 4 channels equidistant 90° and 25% duty cycle settings. */
	syncPwm.dataEdgeChan1[0] = 3600;
	syncPwm.dataEdgeChan1[1] = 0;
	syncPwm.dataEdgeChan2[0] = 7200;
	syncPwm.dataEdgeChan2[1] = 3600;			
	syncPwm.dataEdgeChan3[0] = 10400;
	syncPwm.dataEdgeChan3[1] = 7200;			
	syncPwm.dataEdgeChan4[0] = 14000;
	syncPwm.dataEdgeChan4[1] = 10400;		
}

	
	#endif //USE_SYNC_PWM
		

