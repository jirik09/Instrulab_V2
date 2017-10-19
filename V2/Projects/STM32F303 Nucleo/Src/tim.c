/**
  ******************************************************************************
  * File Name          : TIM.c
  * Date               : 18/01/2015 10:00:31
  * Description        : This file provides code for the configuration
  *                      of the TIM instances.
  ******************************************************************************
  *
  * COPYRIGHT(c) 2015 STMicroelectronics
  *
  * Redistribution and use in source and binary forms, with or without modification,
  * are permitted provided that the following conditions are met:
  *   1. Redistributions of source code must retain the above copyright notice,
  *      this list of conditions and the following disclaimer.
  *   2. Redistributions in binary form must reproduce the above copyright notice,
  *      this list of conditions and the following disclaimer in the documentation
  *      and/or other materials provided with the distribution.
  *   3. Neither the name of STMicroelectronics nor the names of its contributors
  *      may be used to endorse or promote products derived from this software
  *      without specific prior written permission.
  *
  * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
  * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
  * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
  * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
  * FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
  * DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
  * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
  * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
  * OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
  * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
  *
  ******************************************************************************
  */

/* Includes ------------------------------------------------------------------*/
#include "dac.h"
#include "tim.h"
#include "counter.h"
#include "generator.h"
#include "mcu_config.h"
#include "stdlib.h"
#include "FreeRTOS.h"
#include "task.h"
#include "semphr.h"
/* USER CODE BEGIN 0 */

/* USER CODE END 0 */
#ifdef USE_SCOPE	
TIM_HandleTypeDef htim_scope;
#endif //USE_SCOPE
	
#if defined(USE_GEN) || defined(USE_GEN_PWM)
TIM_HandleTypeDef htim6;
TIM_HandleTypeDef htim7;
	#ifdef USE_GEN_PWM
	TIM_HandleTypeDef htim1;
	TIM_HandleTypeDef htim3;
	DMA_HandleTypeDef hdma_tim6_up;
	DMA_HandleTypeDef hdma_tim7_up;
	#endif //USE_GEN_PWM
#endif // USE_GEN || USE_GEN_PWM

#ifdef USE_COUNTER
uint32_t tim2clk, tim4clk;
extern portTickType xStartTime;
extern xSemaphoreHandle counterMutex;

TIM_HandleTypeDef htim2;
TIM_HandleTypeDef htim4;
DMA_HandleTypeDef hdma_tim2_up;
DMA_HandleTypeDef hdma_tim2_ch1;
DMA_HandleTypeDef hdma_tim2_ch2_ch4;

void COUNTER_ETR_DMA_CpltCallback(DMA_HandleTypeDef *dmah);	
void COUNTER_IC1_DMA_CpltCallback(DMA_HandleTypeDef *dmah);
void COUNTER_IC2_DMA_CpltCallback(DMA_HandleTypeDef *dmah);
#endif //USE_COUNTER
			
			
#ifdef USE_SCOPE
/* TIM15 init function */
void MX_TIM15_Init(void)
{
  TIM_ClockConfigTypeDef sClockSourceConfig;
  TIM_MasterConfigTypeDef sMasterConfig;

  htim_scope.Instance = TIM15;
  htim_scope.Init.Prescaler = 0;
  htim_scope.Init.CounterMode = TIM_COUNTERMODE_UP;
  htim_scope.Init.Period = 0;
  htim_scope.Init.ClockDivision = TIM_CLOCKDIVISION_DIV1;
  HAL_TIM_Base_Init(&htim_scope);

  sClockSourceConfig.ClockSource = TIM_CLOCKSOURCE_INTERNAL;
  HAL_TIM_ConfigClockSource(&htim_scope, &sClockSourceConfig);

  sMasterConfig.MasterOutputTrigger = TIM_TRGO_UPDATE;
  sMasterConfig.MasterSlaveMode = TIM_MASTERSLAVEMODE_DISABLE;
  HAL_TIMEx_MasterConfigSynchronization(&htim_scope, &sMasterConfig);

}
#endif //USE_SCOPE
	

/**             
  * @brief  TIM6 Configuration
  * @note   TIM6 configuration is based on APB1 frequency
  * @note   TIM6 Update event occurs each TIM6CLK/256   
  * @param  None
  * @retval None
  */
	#ifdef USE_GEN
void MX_TIM6_Init(void)
{
  TIM_MasterConfigTypeDef sMasterConfig;
  
  /*##-1- Configure the TIM peripheral #######################################*/
  /* Time base configuration */
  htim6.Instance = TIM6;
  
  htim6.Init.Period = 0x7FF;          
  htim6.Init.Prescaler = 0;       
  htim6.Init.ClockDivision = 0;    
  htim6.Init.CounterMode = TIM_COUNTERMODE_UP; 
  HAL_TIM_Base_Init(&htim6);

  /* TIM6 TRGO selection */
  sMasterConfig.MasterOutputTrigger = TIM_TRGO_UPDATE;
  sMasterConfig.MasterSlaveMode = TIM_MASTERSLAVEMODE_DISABLE;
  
  HAL_TIMEx_MasterConfigSynchronization(&htim6, &sMasterConfig);
  
  /*##-2- Enable TIM peripheral counter ######################################*/
  //HAL_TIM_Base_Start(&htim6);
}
	#endif //USE_GEN


/**             
  * @brief  TIM6 Configuration
  * @note   TIM6 configuration is based on APB1 frequency
  * @note   TIM6 Update event occurs each TIM6CLK/256   
  * @param  None
  * @retval None
  */
	#ifdef USE_GEN
void MX_TIM7_Init(void)
{
  TIM_MasterConfigTypeDef sMasterConfig;
  
  /*##-1- Configure the TIM peripheral #######################################*/
  /* Time base configuration */
  htim7.Instance = TIM7;
  
  htim7.Init.Period = 0x7FF;          
  htim7.Init.Prescaler = 0;       
  htim7.Init.ClockDivision = 0;    
  htim7.Init.CounterMode = TIM_COUNTERMODE_UP; 
  HAL_TIM_Base_Init(&htim7);

  /* TIM6 TRGO selection */
  sMasterConfig.MasterOutputTrigger = TIM_TRGO_UPDATE;
  sMasterConfig.MasterSlaveMode = TIM_MASTERSLAVEMODE_DISABLE;
  
  HAL_TIMEx_MasterConfigSynchronization(&htim7, &sMasterConfig);
  
  /*##-2- Enable TIM peripheral counter ######################################*/
  //HAL_TIM_Base_Start(&htim6);
}
	#endif //USE_GEN


/* ************************************************************************************** */
/* ------------------------------- START OF PWM GENERATOR ------------------------------- */
#ifdef USE_GEN_PWM

/* TIM1 PWM init function */
static void MX_TIM1_GEN_PWM_Init(void)
{
  TIM_ClockConfigTypeDef sClockSourceConfig;
  TIM_MasterConfigTypeDef sMasterConfig;
  TIM_OC_InitTypeDef sConfigOC;
  TIM_BreakDeadTimeConfigTypeDef sBreakDeadTimeConfig;

	/* ARR = 1024 (10 bit resolution in default) */
  htim1.Instance = TIM1;
  htim1.Init.Prescaler = 0;
  htim1.Init.CounterMode = TIM_COUNTERMODE_UP;
  htim1.Init.Period = 1023;
  htim1.Init.ClockDivision = TIM_CLOCKDIVISION_DIV1;
  htim1.Init.RepetitionCounter = 0;
  htim1.Init.AutoReloadPreload = TIM_AUTORELOAD_PRELOAD_ENABLE;
  HAL_TIM_Base_Init(&htim1);

  sClockSourceConfig.ClockSource = TIM_CLOCKSOURCE_INTERNAL;
  HAL_TIM_ConfigClockSource(&htim1, &sClockSourceConfig);

  HAL_TIM_PWM_Init(&htim1);

  sMasterConfig.MasterOutputTrigger = TIM_TRGO_RESET;
  sMasterConfig.MasterOutputTrigger2 = TIM_TRGO2_RESET;
  sMasterConfig.MasterSlaveMode = TIM_MASTERSLAVEMODE_DISABLE;
  HAL_TIMEx_MasterConfigSynchronization(&htim1, &sMasterConfig);

  sConfigOC.OCMode = TIM_OCMODE_PWM1;
  sConfigOC.Pulse = 512;
  sConfigOC.OCPolarity = TIM_OCPOLARITY_HIGH;
  sConfigOC.OCNPolarity = TIM_OCNPOLARITY_HIGH;
  sConfigOC.OCFastMode = TIM_OCFAST_DISABLE;
  sConfigOC.OCIdleState = TIM_OCIDLESTATE_RESET;
  sConfigOC.OCNIdleState = TIM_OCNIDLESTATE_RESET;
  HAL_TIM_PWM_ConfigChannel(&htim1, &sConfigOC, TIM_CHANNEL_2);

  sBreakDeadTimeConfig.OffStateRunMode = TIM_OSSR_DISABLE;
  sBreakDeadTimeConfig.OffStateIDLEMode = TIM_OSSI_DISABLE;
  sBreakDeadTimeConfig.LockLevel = TIM_LOCKLEVEL_OFF;
  sBreakDeadTimeConfig.DeadTime = 0;
  sBreakDeadTimeConfig.BreakState = TIM_BREAK_DISABLE;
  sBreakDeadTimeConfig.BreakPolarity = TIM_BREAKPOLARITY_HIGH;
  sBreakDeadTimeConfig.BreakFilter = 0;
  sBreakDeadTimeConfig.Break2State = TIM_BREAK2_DISABLE;
  sBreakDeadTimeConfig.Break2Polarity = TIM_BREAK2POLARITY_HIGH;
  sBreakDeadTimeConfig.Break2Filter = 0;
  sBreakDeadTimeConfig.AutomaticOutput = TIM_AUTOMATICOUTPUT_DISABLE;
  HAL_TIMEx_ConfigBreakDeadTime(&htim1, &sBreakDeadTimeConfig);

  HAL_TIM_Base_MspInit(&htim1);
}


/* TIM3 PWM init function */
static void MX_TIM3_GEN_PWM_Init(void)
{
  TIM_ClockConfigTypeDef sClockSourceConfig;
  TIM_MasterConfigTypeDef sMasterConfig;
  TIM_OC_InitTypeDef sConfigOC;

  htim3.Instance = TIM3;
  htim3.Init.Prescaler = 0;
  htim3.Init.CounterMode = TIM_COUNTERMODE_UP;
  htim3.Init.Period = 1023;
  htim3.Init.ClockDivision = TIM_CLOCKDIVISION_DIV1;
  htim3.Init.AutoReloadPreload = TIM_AUTORELOAD_PRELOAD_ENABLE;
  HAL_TIM_Base_Init(&htim3);
	
  sClockSourceConfig.ClockSource = TIM_CLOCKSOURCE_INTERNAL;
  HAL_TIM_ConfigClockSource(&htim3, &sClockSourceConfig);

  HAL_TIM_PWM_Init(&htim3);

  sMasterConfig.MasterOutputTrigger = TIM_TRGO_RESET;
  sMasterConfig.MasterSlaveMode = TIM_MASTERSLAVEMODE_DISABLE;
  HAL_TIMEx_MasterConfigSynchronization(&htim3, &sMasterConfig);

  sConfigOC.OCMode = TIM_OCMODE_PWM1;
  sConfigOC.Pulse = 512;
  sConfigOC.OCPolarity = TIM_OCPOLARITY_HIGH;
  sConfigOC.OCFastMode = TIM_OCFAST_DISABLE;
  HAL_TIM_PWM_ConfigChannel(&htim3, &sConfigOC, TIM_CHANNEL_1);

  HAL_TIM_Base_MspInit(&htim3);
}


/* TIM6 handling DMA init function */
static void MX_TIM6_GEN_PWM_Init(void)
{
  TIM_MasterConfigTypeDef sMasterConfig;

  htim6.Instance = TIM6;
  htim6.Init.Prescaler = 0;
  htim6.Init.CounterMode = TIM_COUNTERMODE_UP;
  htim6.Init.Period = 0x7FF;
  htim6.Init.AutoReloadPreload = TIM_AUTORELOAD_PRELOAD_DISABLE;
  HAL_TIM_Base_Init(&htim6);

  sMasterConfig.MasterOutputTrigger = TIM_TRGO_UPDATE;
  sMasterConfig.MasterSlaveMode = TIM_MASTERSLAVEMODE_DISABLE;
  HAL_TIMEx_MasterConfigSynchronization(&htim6, &sMasterConfig);
}


/* TIM7 handling DMA init function */
static void MX_TIM7_GEN_PWM_Init(void)
{
  TIM_MasterConfigTypeDef sMasterConfig;

  htim7.Instance = TIM7;
  htim7.Init.Prescaler = 0;
  htim7.Init.CounterMode = TIM_COUNTERMODE_UP;
  htim7.Init.Period = 0x7FF;
  htim7.Init.AutoReloadPreload = TIM_AUTORELOAD_PRELOAD_DISABLE;
  HAL_TIM_Base_Init(&htim7);
		
  sMasterConfig.MasterOutputTrigger = TIM_TRGO_UPDATE;
  sMasterConfig.MasterSlaveMode = TIM_MASTERSLAVEMODE_DISABLE;
  HAL_TIMEx_MasterConfigSynchronization(&htim7, &sMasterConfig);
}

/* -------------------------------- END OF PWM GENERATOR -------------------------------- */
/* ************************************************************************************** */
#endif //USE_GEN_PWM



/* ************************************************************************************** */
/* ---------------------------- START OF COUNTER DEFINITIONS ---------------------------- */
#ifdef USE_COUNTER
/* ************************************************************************************** */
/* ---------------------- Counter timer peripherals INIT functions ---------------------- */
/* ************************************************************************************** */

/* Timer TIM4 initialization - used for time gating of Counter TIM2 */
static void MX_TIM4_Init(void)
{
  TIM_ClockConfigTypeDef sClockSourceConfig;
  TIM_MasterConfigTypeDef sMasterConfig;

  htim4.Instance = TIM4;
	if(counter.state == COUNTER_REF){
		/* REF mode - 10M samples (10000 * 1000) */
		htim4.Init.Prescaler = 999;		
		htim4.Init.Period = 9999;								
	}else if(counter.state == COUNTER_ETR){
		/* ETR mode - 100 ms gate time by default */
		htim4.Init.Prescaler = TIM4_PSC;			// by default 7199 for ETR mode
		htim4.Init.Period = TIM4_ARR;					// by default 999 for ETR mode
	}else if((counter.state == COUNTER_IC) || counter.state == COUNTER_TI){
		/* IC mode - 100 ms interrupt event to send data */
		htim4.Init.Prescaler = TIM4_PSC;			
		htim4.Init.Period = TIM4_ARR;			
	}
  htim4.Init.CounterMode = TIM_COUNTERMODE_UP;
  htim4.Init.ClockDivision = TIM_CLOCKDIVISION_DIV1;
	htim4.Init.AutoReloadPreload = TIM_AUTORELOAD_PRELOAD_DISABLE;
	
	HAL_TIM_Base_Init(&htim4);

	if(counter.state == COUNTER_REF){
		sClockSourceConfig.ClockSource = TIM_CLOCKSOURCE_ETRMODE2;
		sClockSourceConfig.ClockPolarity = TIM_CLOCKPOLARITY_NONINVERTED;
		sClockSourceConfig.ClockPrescaler = TIM_CLOCKPRESCALER_DIV1;
		sClockSourceConfig.ClockFilter = 0;
	}else{
		sClockSourceConfig.ClockSource = TIM_CLOCKSOURCE_INTERNAL;
	}
  HAL_TIM_ConfigClockSource(&htim4, &sClockSourceConfig);

	sMasterConfig.MasterOutputTrigger = TIM_TRGO_UPDATE;
	if((counter.state == COUNTER_IC) || (counter.state == COUNTER_TI)){
		sMasterConfig.MasterSlaveMode = TIM_MASTERSLAVEMODE_DISABLE;
	}else{
		sMasterConfig.MasterSlaveMode = TIM_MASTERSLAVEMODE_ENABLE;
	}	
	HAL_TIMEx_MasterConfigSynchronization(&htim4, &sMasterConfig);
}

/* TIM2 mode ETR init function */
static void MX_TIM2_ETRorREF_Init(void)
{
  TIM_ClockConfigTypeDef sClockSourceConfig;
  TIM_SlaveConfigTypeDef sSlaveConfig;
  TIM_MasterConfigTypeDef sMasterConfig;

  htim2.Instance = TIM2;
  htim2.Init.Prescaler = 0;
  htim2.Init.CounterMode = TIM_COUNTERMODE_UP;
  htim2.Init.Period = 0xFFFFFFFF;		// full 32 bit 4 294 967 295
  htim2.Init.ClockDivision = TIM_CLOCKDIVISION_DIV1;
  htim2.Init.AutoReloadPreload = TIM_AUTORELOAD_PRELOAD_DISABLE;
  HAL_TIM_Base_Init(&htim2);

  sClockSourceConfig.ClockSource = TIM_CLOCKSOURCE_ETRMODE2;	
  sClockSourceConfig.ClockPolarity = TIM_CLOCKPOLARITY_NONINVERTED;
  sClockSourceConfig.ClockPrescaler = TIM_CLOCKPRESCALER_DIV1;
  sClockSourceConfig.ClockFilter = 0;
  HAL_TIM_ConfigClockSource(&htim2, &sClockSourceConfig);

  sSlaveConfig.SlaveMode = TIM_SLAVEMODE_COMBINED_RESETTRIGGER;
  sSlaveConfig.InputTrigger = TIM_TS_ITR3;
	sSlaveConfig.TriggerPolarity = TIM_TRIGGERPOLARITY_RISING;
	sSlaveConfig.TriggerPrescaler = TIM_TRIGGERPRESCALER_DIV1;
  HAL_TIM_SlaveConfigSynchronization(&htim2, &sSlaveConfig);
		
  sMasterConfig.MasterOutputTrigger = TIM_TRGO_RESET;
  sMasterConfig.MasterSlaveMode = TIM_MASTERSLAVEMODE_DISABLE;
  HAL_TIMEx_MasterConfigSynchronization(&htim2, &sMasterConfig);

	TIM2 -> CCMR1 &= ~TIM_CCMR1_CC1S;
	TIM2 -> CCMR1 &= ~TIM_CCMR1_CC2S;

	TIM2 -> DIER  |= TIM_DIER_UDE;					/* __HAL_TIM_ENABLE_DMA(&htim2, TIM_DMA_UPDATE); */			
	TIM2 -> CCMR1 |= TIM_CCMR1_CC1S;  			/* Capture/Compare 1 Selection - CC1 channel is configured as input, IC1 is mapped on TRC	*/																		 
}

/* TIM2 mode IC init function */
static void MX_TIM2_ICorTI_Init(void)
{
  TIM_ClockConfigTypeDef sClockSourceConfig;
  TIM_MasterConfigTypeDef sMasterConfig;
  TIM_IC_InitTypeDef sConfigIC;

  htim2.Instance = TIM2;
  htim2.Init.Prescaler = 0;
  htim2.Init.CounterMode = TIM_COUNTERMODE_UP;
  htim2.Init.Period = 0xFFFFFFFF;
  htim2.Init.ClockDivision = TIM_CLOCKDIVISION_DIV1;
  htim2.Init.AutoReloadPreload = TIM_AUTORELOAD_PRELOAD_ENABLE;
  HAL_TIM_Base_Init(&htim2);

  sClockSourceConfig.ClockSource = TIM_CLOCKSOURCE_INTERNAL;
  HAL_TIM_ConfigClockSource(&htim2, &sClockSourceConfig);

  HAL_TIM_IC_Init(&htim2);

  sMasterConfig.MasterOutputTrigger = TIM_TRGO_RESET;
  sMasterConfig.MasterSlaveMode = TIM_MASTERSLAVEMODE_DISABLE;
  HAL_TIMEx_MasterConfigSynchronization(&htim2, &sMasterConfig);

  sConfigIC.ICPolarity = TIM_INPUTCHANNELPOLARITY_RISING;
  sConfigIC.ICSelection = TIM_ICSELECTION_DIRECTTI;
  sConfigIC.ICPrescaler = TIM_ICPSC_DIV1;
  sConfigIC.ICFilter = 0;	
  HAL_TIM_IC_ConfigChannel(&htim2, &sConfigIC, TIM_CHANNEL_1);
	HAL_TIM_IC_ConfigChannel(&htim2, &sConfigIC, TIM_CHANNEL_2);
	
	TIM2 -> DIER |= TIM_DIER_UDE;					/* __HAL_TIM_ENABLE_DMA(&htim2, TIM_DMA_UPDATE); */			
	TIM2 -> CCMR1 |= TIM_CCMR1_CC1S_0;  	/* Capture/Compare 1 Selection - CC1 channel is configured as input, IC1 is mapped on TI1	*/
	TIM2 -> CCMR1 |= TIM_CCMR1_CC2S_0;		/* IC2 is mapped on TI2 */		
	
	TIM2 -> CCER |= TIM_CCER_CC1E;				/* CC1 channel configured as input: This bit determines if a capture of the counter value can
																					 actually be done into the input capture/compare register 1 (TIMx_CCR1) or not.  */
	TIM2 -> CCER |= TIM_CCER_CC2E;		
	
	TIM2 -> DIER |= TIM_DIER_CC1DE;				/* Capture/Compare 1 DMA request */
	TIM2 -> DIER |= TIM_DIER_CC2DE;				/* Capture/Compare 1 DMA request */
	
//	if(counter.state == COUNTER_TI){			/* If TI counter configured, initialize one-pulse timer mode */
//		TIM2->CR1 |= TIM_CR1_OPM;
//	}
}

/* ************************************************************************************** */
/* ---------------------------- Counter timer INIT functions ---------------------------- */
/* ************************************************************************************** */
void TIM_counter_etr_init(void){	
	TIM_doubleClockVal();	
	MX_TIM4_Init();	
	MX_TIM2_ETRorREF_Init();	
	tim4clk = HAL_RCCEx_GetPeriphCLKFreq(RCC_PERIPHCLK_TIM34); 	
}

void TIM_counter_ref_init(void){	
	TIM_doubleClockVal();
	MX_TIM4_Init();
	MX_TIM2_ETRorREF_Init();
}

void TIM_counter_ic_init(void){
	TIM_doubleClockVal();	
	MX_TIM4_Init();
	MX_TIM2_ICorTI_Init();
}

void TIM_counter_ti_init(void){
	TIM_doubleClockVal();	
	MX_TIM4_Init();	
	MX_TIM2_ICorTI_Init();
	TIM_TI_Init();
}

/* HAL_RCCEx_GetPeriphCLKFreq function does not count with PLL clock source for TIM2 */
void TIM_doubleClockVal(void){
	if ((RCC->CFGR3&RCC_TIM2CLK_PLLCLK) == RCC_TIM2CLK_PLLCLK){
		tim2clk = HAL_RCCEx_GetPeriphCLKFreq(RCC_PERIPHCLK_TIM2) * 2; 
	}	else {
		tim2clk = HAL_RCCEx_GetPeriphCLKFreq(RCC_PERIPHCLK_TIM2); 
	}		
}

/* ************************************************************************************** */
/* --------------------------- Counter timer DEINIT functions --------------------------- */
/* ************************************************************************************** */
void TIM_etr_deinit(void){
	HAL_TIM_Base_DeInit(&htim2);	
	HAL_TIM_Base_DeInit(&htim4);	
}

void TIM_ref_deinit(void){
	HAL_TIM_Base_DeInit(&htim2);
	HAL_TIM_Base_DeInit(&htim4);	
}

void TIM_ic_deinit(void){
	HAL_TIM_Base_DeInit(&htim2);	
	HAL_TIM_Base_DeInit(&htim4);			
}

void TIM_ti_deinit(void){
	HAL_TIM_Base_DeInit(&htim2);	
	HAL_TIM_Base_DeInit(&htim4);		
	TIM_TI_Deinit();
}

/* ************************************************************************************** */
/* ---------------------- Counter timer mode START STOP functions ----------------------- */
/* ************************************************************************************** */
void TIM_ETR_Start(void)
{		
	HAL_TIM_Base_Start(&htim2);			
	HAL_TIM_Base_Start(&htim4);	
	HAL_DMA_Start_IT(&hdma_tim2_up, (uint32_t)&(TIM2->CCR1), (uint32_t)&counter.counterEtr.buffer, 1);
	
	/* DMA requests enable */
	TIM2->DIER |= TIM_DIER_CC1DE;		
	TIM2->CCER |= TIM_CCER_CC1E;
	TIM4->EGR |= TIM_EGR_UG;	
	
	counter.sampleCntChange = SAMPLE_COUNT_CHANGED;
}

void TIM_ETR_Stop(void)
{
	HAL_DMA_Abort_IT(&hdma_tim2_up);		
	/* DMA requests disable */
	TIM2->DIER &= ~TIM_DIER_CC1DE;	
	
	HAL_TIM_Base_Stop(&htim2);	
	HAL_TIM_Base_Stop(&htim4);
}

void TIM_IC_Start(void)
{
	/* DMA requests enable */
	TIM2->DIER |= TIM_DIER_CC1DE;
	TIM2->DIER |= TIM_DIER_CC2DE;
	
	HAL_DMA_Start(&hdma_tim2_ch1, (uint32_t)&(TIM2->CCR1), (uint32_t)counter.counterIc.ic1buffer, counter.counterIc.ic1BufferSize);
	HAL_DMA_Start(&hdma_tim2_ch2_ch4, (uint32_t)&(TIM2->CCR2), (uint32_t)counter.counterIc.ic2buffer, counter.counterIc.ic2BufferSize);
	
	HAL_TIM_Base_Start(&htim2);	
	HAL_TIM_Base_Start_IT(&htim4);	
	
	/* Enable capturing */
	TIM2->CCER |= TIM_CCER_CC2E;
	TIM2->CCER |= TIM_CCER_CC1E;
}

void TIM_IC_Stop(void)
{
	/* Disable capturing */
	TIM2->CCER &= ~TIM_CCER_CC1E; 	
	TIM2->CCER &= ~TIM_CCER_CC2E; 	
	
	/* Abort DMA transfers */
	HAL_DMA_Abort(&hdma_tim2_ch1);	
	HAL_DMA_Abort(&hdma_tim2_ch2_ch4);	
	
	/* DMA requests disable */
	TIM2->DIER &= ~TIM_DIER_CC1DE;	
	TIM2->DIER &= ~TIM_DIER_CC2DE;	
	
	/* Stop timers */
	HAL_TIM_Base_Stop_IT(&htim4);		
	HAL_TIM_Base_Stop(&htim2);	
}

void TIM_TI_Start(void)
{				
	/* Get systick value to handle timeout */
	xStartTime = xTaskGetTickCount();

	/* Set DMA CNDTR buffer count */
	if(counter.abba == BIN1){
		HAL_DMA_Start(&hdma_tim2_ch1, (uint32_t)&(TIM2->CCR1), (uint32_t)counter.counterIc.ic1buffer, 1);	
	}else{
		HAL_DMA_Start(&hdma_tim2_ch2_ch4, (uint32_t)&(TIM2->CCR2), (uint32_t)counter.counterIc.ic2buffer, 1);				
	}		
	
	HAL_TIM_Base_Start(&htim2);	
	HAL_TIM_Base_Start_IT(&htim4);		

	/* DMA requests enable */
	TIM2->DIER |= TIM_DIER_CC1DE;
	TIM2->DIER |= TIM_DIER_CC2DE;			
	
	/* Enable capturing */
	TIM2->CCER |= TIM_CCER_CC2E;
	TIM2->CCER |= TIM_CCER_CC1E;	
}

void TIM_TI_Stop(void)
{
	/* Abort DMA transfers */
	HAL_DMA_Abort(&hdma_tim2_ch1);	
	HAL_DMA_Abort(&hdma_tim2_ch2_ch4);	
	
	HAL_TIM_Base_Stop_IT(&htim4);	
	HAL_TIM_Base_Stop(&htim2);	
	
	/* Disable capturing */
	TIM2->CCER &= ~TIM_CCER_CC1E; 	
	TIM2->CCER &= ~TIM_CCER_CC2E; 	
	
	/* DMA requests disable */
	TIM2->DIER &= ~TIM_DIER_CC1DE;
	TIM2->DIER &= ~TIM_DIER_CC2DE;	
}

void TIM_TI_Init(void)
{		
	/* Do not run timer after initialization, wait for start command */
	TIM2->CR1 &= ~TIM_CR1_CEN;
	/* Disable time elapse interrupt */
	HAL_TIM_Base_Stop_IT(&htim4);	
	/* Disable capturing */
	TIM2->CCER &= ~TIM_CCER_CC1E; 	
	TIM2->CCER &= ~TIM_CCER_CC2E; 		
	/* Set IC1 prescaler to 1 */
	TIM2->CCMR1 &= ~TIM_CCMR1_IC1PSC;		
	/* Set IC2 prescaler to 1 */
	TIM2->CCMR1 &= ~TIM_CCMR1_IC2PSC;		
	/* Select the valid trigger input TI1FP1 */
	TIM2->SMCR &= ~TIM_SMCR_TS;
	TIM2->SMCR |= TIM_SMCR_TS_0 | TIM_SMCR_TS_2;			
	/* Configure the slave mode controller in reset mode */
	TIM2->SMCR &= ~TIM_SMCR_SMS;
	TIM2->SMCR |= TIM_SMCR_SMS_2;
	
	/* The very first number transfered by DMA on first event (timer triggered)
		 is random number (who knows why) -> throw away */
	counter.bin = BIN0;	
	/* AB event sequence first */
	counter.abba = BIN0;	
}

void TIM_TI_Deinit(void)
{
	/* Disable capturing*/
	TIM2->CCER &= ~TIM_CCER_CC1E; 	
	TIM2->CCER &= ~TIM_CCER_CC2E; 		
	/* Select the active polarity for TI1FP1 (rising edge) */
	TIM2->CCER &= ~(uint16_t)(TIM_CCER_CC1P | TIM_CCER_CC1NP);		
	/* Select the active polarity for TI1FP2 (rising edge) */
	TIM2->CCER &= ~(uint16_t)(TIM_CCER_CC2P | TIM_CCER_CC2NP);	
	/* Unselect the trigger input */
	TIM2->SMCR &= ~TIM_SMCR_TS;		
	/* Disable the slave mode controller */
	TIM2->SMCR &= ~TIM_SMCR_SMS;	
}


/* ************************************************************************************** */
/* ------------------------------ IC duty cycle functions ------------------------------- */
/* ************************************************************************************** */

void TIM_IC_DutyCycleDmaRestart(void)
{	
	HAL_DMA_Abort(&hdma_tim2_ch1);
	HAL_DMA_Abort(&hdma_tim2_ch2_ch4);
	
	/* Set DMA CNDTR buffer count */
	HAL_DMA_Start(&hdma_tim2_ch1, (uint32_t)&(TIM2->CCR1), (uint32_t)counter.counterIc.ic1buffer, 1);  
	HAL_DMA_Start(&hdma_tim2_ch2_ch4, (uint32_t)&(TIM2->CCR2), (uint32_t)counter.counterIc.ic2buffer, 1);				
}

void TIM_IC_DutyCycle_Init(void)
{	
	/* Stop timer elapse event interrupt first */
	HAL_TIM_Base_Stop_IT(&htim4);	
	
	/* Disable capturing to configure CCxS */
	TIM2->CCER &= ~TIM_CCER_CC1E; 	
	TIM2->CCER &= ~TIM_CCER_CC2E; 

	if(counter.icDutyCycle == DUTY_CYCLE_CH1_ENABLED){	
		/* Set IC1 prescaler to 1 */
		TIM2->CCMR1 &= ~TIM_CCMR1_IC1PSC;		
		/* Select the active input for CCR1 */
		TIM2->CCMR1 &= ~TIM_CCMR1_CC1S;
		TIM2->CCMR1 |= TIM_CCMR1_CC1S_0;	
		/* Select the active polarity for TI1FP1 (rising edge) */
		TIM2->CCER &= ~(uint16_t)(TIM_CCER_CC1P | TIM_CCER_CC1NP);
		/* Select the active input for CCR2 */
		TIM2->CCMR1 &= ~TIM_CCMR1_CC2S;
		TIM2->CCMR1 |= TIM_CCMR1_CC2S_1;	
		/* Select the active polarity for TI1FP2 (falling edge) */
		TIM2->CCER &= ~(uint16_t)(TIM_CCER_CC2NP);	
		TIM2->CCER |= (uint16_t)(TIM_CCER_CC2P);		
		/* Select the valid trigger input */
		TIM2->SMCR &= ~TIM_SMCR_TS;
		TIM2->SMCR |= TIM_SMCR_TS_0 | TIM_SMCR_TS_2;
	}else{
		/* Set IC2 prescaler to 1 */
		TIM2->CCMR1 &= ~TIM_CCMR1_IC2PSC;
		/* Select the active input for CCR1 */
		TIM2->CCMR1 &= ~TIM_CCMR1_CC1S;
		TIM2->CCMR1 |= TIM_CCMR1_CC1S_1;	
		/* Select the active polarity for TI1FP1 (falling edge) */
		TIM2->CCER &= ~(uint16_t)(TIM_CCER_CC1NP);	
		TIM2->CCER |= (uint16_t)(TIM_CCER_CC1P);
		/* Select the active input for CCR2 */
		TIM2->CCMR1 &= ~TIM_CCMR1_CC2S;
		TIM2->CCMR1 |= TIM_CCMR1_CC2S_0;	
		/* Select the active polarity for TI1FP2 (rising edge) */			
		TIM2->CCER &= ~(uint16_t)(TIM_CCER_CC2P | TIM_CCER_CC2NP);
		/* Select the valid trigger input */
		TIM2->SMCR &= ~TIM_SMCR_TS;
		TIM2->SMCR |= TIM_SMCR_TS_1 | TIM_SMCR_TS_2;		
	}
	
	/* Configure the slave mode controller in reset mode */
	TIM2->SMCR &= ~TIM_SMCR_SMS;
	TIM2->SMCR |= TIM_SMCR_SMS_2;
}

void TIM_IC_DutyCycle_Deinit(void)
{			
	/* Select the active input for CCR1 */
	TIM2->CCMR1 &= ~TIM_CCMR1_CC1S;
	TIM2->CCMR1 |= TIM_CCMR1_CC1S_0;			
	/* Select the active polarity for TI1FP1 (rising edge) */
	TIM2->CCER &= ~(uint16_t)(TIM_CCER_CC1P | TIM_CCER_CC1NP);		
	/* Select the active input for CCR2 */
	TIM2->CCMR1 &= ~TIM_CCMR1_CC2S;
	TIM2->CCMR1 |= TIM_CCMR1_CC2S_0;	
	/* Select the active polarity for TI1FP2 (rising edge) */
	TIM2->CCER &= ~(uint16_t)(TIM_CCER_CC2P | TIM_CCER_CC2NP);	
	/* Unselect the trigger input */
	TIM2->SMCR &= ~TIM_SMCR_TS;		
	/* Disable the slave mode controller */
	TIM2->SMCR &= ~TIM_SMCR_SMS;
	/* Start DMAs */
	HAL_DMA_Start(&hdma_tim2_ch1, (uint32_t)&(TIM2->CCR1), (uint32_t)counter.counterIc.ic1buffer, counter.counterIc.ic1BufferSize);
	HAL_DMA_Start(&hdma_tim2_ch2_ch4, (uint32_t)&(TIM2->CCR2), (uint32_t)counter.counterIc.ic2buffer, counter.counterIc.ic2BufferSize);	
	/* DMA requests enable */
	TIM2->DIER |= TIM_DIER_CC1DE;
	TIM2->DIER |= TIM_DIER_CC2DE;	
	HAL_TIM_Base_Start_IT(&htim4);
	/* Enable capturing for IC mode */
	TIM2->CCER |= TIM_CCER_CC1E;
	TIM2->CCER |= TIM_CCER_CC2E;		
}

void TIM_IC_DutyCycle_Start(void)
{	
	/* Set DMA CNDTR buffer count */
	HAL_DMA_Start(&hdma_tim2_ch1, (uint32_t)&(TIM2->CCR1), (uint32_t)counter.counterIc.ic1buffer, 1);
	HAL_DMA_Start(&hdma_tim2_ch2_ch4, (uint32_t)&(TIM2->CCR2), (uint32_t)counter.counterIc.ic2buffer, 1);				
	
	HAL_TIM_Base_Start(&htim2);	
	HAL_TIM_Base_Start_IT(&htim4);		
	
	/* DMA requests enable */
	TIM2->DIER |= TIM_DIER_CC1DE;
	TIM2->DIER |= TIM_DIER_CC2DE;

	/* Enable capturing */
	TIM2->CCER |= TIM_CCER_CC2E;
	TIM2->CCER |= TIM_CCER_CC1E;	

	/* The very first number transfered by DMA on first event (timer triggered)
		 is random number (who knows why) -> throw away */
	counter.bin = BIN0;	
}  

void TIM_IC_DutyCycle_Stop(void)
{
	/* Abort DMA transfers */
	HAL_DMA_Abort(&hdma_tim2_ch1);	
	HAL_DMA_Abort(&hdma_tim2_ch2_ch4);	
	
	/* Disable capture to configure CCxS */
	TIM2->CCER &= ~TIM_CCER_CC1E; 	
	TIM2->CCER &= ~TIM_CCER_CC2E; 	
	
	/* DMA requests disable */
	TIM2->DIER &= ~TIM_DIER_CC1DE;
	TIM2->DIER &= ~TIM_DIER_CC2DE;
}

/* ************************************************************************************** */
/* ----------------------------- Specific counter functions ----------------------------- */
/* ************************************************************************************** */
/**
  * @brief  This function is used to select the desired ETR prescaler ETPS. (TIM2 should be clocked to 144 MHz)
	* @param  freq: frequency
  * @retval none 
  */
void TIM_ETRP_Config(double freq)
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
	* @brief  This function is used to select the desired prescaler of IC1 of TIM2. 
						Automatic change due to frequency.
	* @param  freq: frequency
  * @retval none 
  */
void TIM_IC1PSC_Config(double freq)
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
						Automatic change due to frequency.
	* @param  freq: frequency
  * @retval none 
  */
void TIM_IC2PSC_Config(double freq)
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
	* @brief  This function is used to select the desired prescaler of IC1 of TIM2. 
						Direct change of prescaler due to value given to function as parameter.
	* @param  prescVal: value of prescaler (1, 2, 4, 8)
  * @retval none 
  */
void TIM_IC1_PSC_Config(uint8_t prescVal)
{	
	TIM2->CCMR1 &= ~TIM_CCMR1_IC1PSC;
	/* Save the real value of ICxPSC prescaler for later calculations */
	switch(prescVal){	
		case 2:
			TIM2->CCMR1 |= TIM_CCMR1_IC1PSC_0; break;
		case 4:
			TIM2->CCMR1 |= TIM_CCMR1_IC1PSC_1; break;
		case 8:
			TIM2->CCMR1 |= TIM_CCMR1_IC1PSC; break;
		default:
			TIM2->CCMR1 &= ~TIM_CCMR1_IC1PSC; break;
	}		
}

/**
	* @brief  This function is used to select the desired prescaler of IC2 of TIM2. 
						Direct change of prescaler due to value given to function as parameter.
	* @param  prescVal: value of prescaler (1, 2, 4, 8)
  * @retval none 
  */
void TIM_IC2_PSC_Config(uint8_t prescVal)
{	
	TIM2->CCMR1 &= ~TIM_CCMR1_IC2PSC;
	/* Save the real value of ICxPSC prescaler for later calculations */
	switch(prescVal){		
		case 2:
			TIM2->CCMR1 |= TIM_CCMR1_IC2PSC_0; break;
		case 4:
			TIM2->CCMR1 |= TIM_CCMR1_IC2PSC_1; break;
		case 8:
			TIM2->CCMR1 |= TIM_CCMR1_IC2PSC; break;
		default:
			TIM2->CCMR1 &= ~TIM_CCMR1_IC2PSC; break;
	}				
}

/**
	* @brief  Functions used to select active adges for events capturing (IC + Duty, TI modes)						
	* @param  none
  * @retval none 
  */
void TIM_IC1_RisingFalling(void)
{
	TIM2->CCER |= (TIM_CCER_CC1P | TIM_CCER_CC1NP); 
}

void TIM_IC1_RisingOnly(void)
{
	TIM2->CCER &= ~(uint16_t)(TIM_CCER_CC1P | TIM_CCER_CC1NP);	
}

void TIM_IC1_FallingOnly(void)
{
	TIM2->CCER &= ~(uint16_t)(TIM_CCER_CC1NP);	
	TIM2->CCER |= (uint16_t)(TIM_CCER_CC1P);	
}

void TIM_IC2_RisingFalling(void)
{
	TIM2->CCER |= (TIM_CCER_CC2P | TIM_CCER_CC2NP);
}

void TIM_IC2_RisingOnly(void)
{
	TIM2->CCER &= ~(uint16_t)(TIM_CCER_CC2P | TIM_CCER_CC2NP);	
}

void TIM_IC2_FallingOnly(void)
{
	TIM2->CCER &= ~(uint16_t)(TIM_CCER_CC2NP);	
	TIM2->CCER |= (uint16_t)(TIM_CCER_CC2P);	
}

void TIM_TI_Sequence_AB(void){
	/* Select the valid trigger input TI1FP1 */
	TIM2->SMCR &= ~TIM_SMCR_TS;
	TIM2->SMCR |= TIM_SMCR_TS_0 | TIM_SMCR_TS_2;	
	/* ABBA used for calculation decision in counterTiProcess() function.
		 Time t_AB - time delay between AB events measured. */
	counter.abba = BIN0;
}

void TIM_TI_Sequence_BA(void){
	/* Select the valid trigger input TI2FP2 */
	TIM2->SMCR &= ~TIM_SMCR_TS;
	TIM2->SMCR |= TIM_SMCR_TS_1 | TIM_SMCR_TS_2;	
	/* ABBA used for calculation decision in counterTiProcess() function.
		 Time t_BA - time delay between BA events measured. */
	counter.abba = BIN1;
}

/**
	* @brief  Function settings ARR and PSC values of TIM4 (gate time) - ETR, REF
	* @params arr, psc
  * @retval none 
  */
void TIM_ARR_PSC_Config(uint16_t arr, uint16_t psc)
{					
	TIM4->ARR = arr;
	TIM4->PSC = psc;
	
	if(counter.state!=COUNTER_IC){							
		xStartTime = xTaskGetTickCount();		
		TIM4->CR1 |= TIM_CR1_CEN;				
		counter.sampleCntChange = SAMPLE_COUNT_CHANGED;
	}		
	
	/* Generate an update event to reload the Prescaler and the repetition counter immediately */		
	TIM4->EGR |= TIM_EGR_UG;
}

void TIM_REF_SecondInputDisable(void){
	TIM4->CR1 &= ~TIM_CR1_CEN;
}

/**
	* @brief  Function getting ETRP (external trigger source prescaler) value of TIM2. 
	* @params none
	* @retval etps: ETRP prescaler register value
  */
uint8_t TIM_ETPS_GetPrescaler(void)
{	
	uint16_t etpsRegVal = ((TIM2->SMCR) & 0x3000) >> 12;			/* ETR prescaler register value */		
	return TIM_GetPrescaler(etpsRegVal);
}

uint8_t TIM_IC1PSC_GetPrescaler(void)
{
	uint32_t ic1psc = ((TIM2->CCMR1) & TIM_CCMR1_IC1PSC_Msk) >> TIM_CCMR1_IC1PSC_Pos;	
	return TIM_GetPrescaler(ic1psc);
}

uint8_t TIM_IC2PSC_GetPrescaler(void)
{
	uint32_t ic2psc = ((TIM2->CCMR1) & TIM_CCMR1_IC2PSC_Msk) >> TIM_CCMR1_IC2PSC_Pos;	
	return TIM_GetPrescaler(ic2psc);
}

/**
	* @brief  This function returns a real value of given register value prescaler. 
	* @params regPrescValue: ETRP prescaler register value
	* @retval presc: real prescaler value used for later calculations
  */
uint8_t TIM_GetPrescaler(uint32_t regPrescValue)
{
	uint8_t presc;
	/* Save the real value of ICxPSC prescaler for later calculations */
	switch(regPrescValue){
		case 0:
			presc = 1; break;			
		case 1:
			presc = 2; break;
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
	* @brief  Function testing if DMA transfer complete bit is set. 
	* @params DMA_HandleTypeDef *dmah: pointer to a DMA_HandleTypeDef structure that contains
  *         the configuration information for the specified DMA Channel.  
	* @retval bool: true, false
  */
bool DMA_TransferComplete(DMA_HandleTypeDef *dmah)
{
	uint32_t dmaIsrReg = dmah->DmaBaseAddress->ISR;	
		
	if(dmaIsrReg & (uint32_t)(DMA_FLAG_TC1 << dmah->ChannelIndex)){		
		if(counter.state == COUNTER_IC){
			/* Clear the transfer complete flag */
			dmah->DmaBaseAddress->IFCR = DMA_FLAG_TC1 << dmah->ChannelIndex;						
		}		
		return true;		
	} else {		
		return false;
	}	
}

/* The value of DMA buffer size can be changed only if aborted first */
void DMA_Restart(DMA_HandleTypeDef *dmah)
{
	if(dmah == &hdma_tim2_ch1){
		HAL_DMA_Abort(&hdma_tim2_ch1);				
		HAL_DMA_Start(&hdma_tim2_ch1, (uint32_t)&(TIM2->CCR1), (uint32_t)counter.counterIc.ic1buffer, counter.counterIc.ic1BufferSize);	
	}else{
		HAL_DMA_Abort(&hdma_tim2_ch2_ch4);				
		HAL_DMA_Start(&hdma_tim2_ch2_ch4, (uint32_t)&(TIM2->CCR2), (uint32_t)counter.counterIc.ic2buffer, counter.counterIc.ic2BufferSize);	
	}
}

/* ---------------------------- END OF COUNTER DEFINITIONS ------------------------------ */
/* ************************************************************************************** */
#endif //USE_COUNTER



void HAL_TIM_Base_MspInit(TIM_HandleTypeDef* htim_base)
{
	GPIO_InitTypeDef GPIO_InitStruct;
	
	#ifdef USE_SCOPE
  if(htim_base->Instance==TIM15)
  {
    /* Peripheral clock enable */
    __TIM15_CLK_ENABLE();
  }
	#endif //USE_SCOPE
	
	/* Note: PC app must send the mode first even if only one 
		 generator is implemented in device */
	#if defined(USE_GEN) || defined(USE_GEN_PWM)
		#ifdef USE_GEN
		/* DAC generator mode TIM decision */
		if(generator.modeState==GENERATOR_DAC){
			if(htim_base->Instance==TIM6){
				__TIM6_CLK_ENABLE();
			}
			if(htim_base->Instance==TIM7){
				__TIM7_CLK_ENABLE();
			}	
		}
		#endif //USE_GEN
	
		#ifdef USE_GEN_PWM
		/* PWM generator mode TIM decision */
		if(generator.modeState==GENERATOR_PWM){
			if(htim_base->Instance==TIM1){
				__TIM1_CLK_ENABLE();
				
				/**TIM1 GPIO Configuration    
				PA9     ------> TIM1_CH2 
				*/
				GPIO_InitStruct.Pin = GPIO_PIN_9;
				GPIO_InitStruct.Mode = GPIO_MODE_AF_PP;
				GPIO_InitStruct.Pull = GPIO_NOPULL;
				GPIO_InitStruct.Speed = GPIO_SPEED_FREQ_HIGH;
				GPIO_InitStruct.Alternate = GPIO_AF6_TIM1;
				HAL_GPIO_Init(GPIOA, &GPIO_InitStruct);			
			}	
			if(htim_base->Instance==TIM3){
				__TIM3_CLK_ENABLE();
				
				/**TIM3 GPIO Configuration    
				PB4     ------> TIM3_CH1 
				*/
				GPIO_InitStruct.Pin = GPIO_PIN_4;
				GPIO_InitStruct.Mode = GPIO_MODE_AF_PP;
				GPIO_InitStruct.Pull = GPIO_NOPULL;
				GPIO_InitStruct.Speed = GPIO_SPEED_FREQ_HIGH;
				GPIO_InitStruct.Alternate = GPIO_AF2_TIM3;
				HAL_GPIO_Init(GPIOB, &GPIO_InitStruct);			
			}				
			if(htim_base->Instance==TIM6){
				__TIM6_CLK_ENABLE();
				
				/* Peripheral DMA init*/  
				hdma_tim6_up.Instance = DMA1_Channel3;
				hdma_tim6_up.Init.Direction = DMA_MEMORY_TO_PERIPH;
				hdma_tim6_up.Init.PeriphInc = DMA_PINC_DISABLE;
				hdma_tim6_up.Init.MemInc = DMA_MINC_ENABLE;
				hdma_tim6_up.Init.PeriphDataAlignment = DMA_PDATAALIGN_HALFWORD;
				hdma_tim6_up.Init.MemDataAlignment = DMA_MDATAALIGN_HALFWORD;
				hdma_tim6_up.Init.Mode = DMA_CIRCULAR;
				hdma_tim6_up.Init.Priority = DMA_PRIORITY_HIGH;
				HAL_DMA_Init(&hdma_tim6_up);
				TIM6->DIER |= TIM_DIER_UDE;

				__HAL_DMA_REMAP_CHANNEL_ENABLE(HAL_REMAPDMA_TIM6_DAC1_CH1_DMA1_CH3);
				__HAL_LINKDMA(htim_base,hdma[TIM_DMA_ID_UPDATE],hdma_tim6_up);
			}
			if(htim_base->Instance==TIM7){
				__TIM7_CLK_ENABLE();
				
				/* Peripheral DMA init*/
				hdma_tim7_up.Instance = DMA1_Channel4;
				hdma_tim7_up.Init.Direction = DMA_MEMORY_TO_PERIPH;
				hdma_tim7_up.Init.PeriphInc = DMA_PINC_DISABLE;
				hdma_tim7_up.Init.MemInc = DMA_MINC_ENABLE;
				hdma_tim7_up.Init.PeriphDataAlignment = DMA_PDATAALIGN_HALFWORD;
				hdma_tim7_up.Init.MemDataAlignment = DMA_MDATAALIGN_HALFWORD;
				hdma_tim7_up.Init.Mode = DMA_CIRCULAR;
				hdma_tim7_up.Init.Priority = DMA_PRIORITY_HIGH;
				HAL_DMA_Init(&hdma_tim7_up);
				TIM7->DIER |= TIM_DIER_UDE;
				
				__HAL_LINKDMA(htim_base,hdma[TIM_DMA_ID_UPDATE],hdma_tim7_up);
			}			
		}
		#endif //USE_GEN_PWM
	#endif //USE_GEN || USE_GEN_PWM
	
	#ifdef USE_COUNTER    	
	if(htim_base->Instance==TIM2){
		
		if(counter.state==COUNTER_ETR||counter.state==COUNTER_REF){			
			
			__TIM2_CLK_ENABLE();
				
			/**TIM2 GPIO Configuration    
			PA0     ------> TIM2_ETR 
			*/
			GPIO_InitStruct.Pin = GPIO_PIN_0;
			GPIO_InitStruct.Mode = GPIO_MODE_AF_PP;
			GPIO_InitStruct.Pull = GPIO_NOPULL;
			GPIO_InitStruct.Speed = GPIO_SPEED_FREQ_HIGH;
			GPIO_InitStruct.Alternate = GPIO_AF1_TIM2;
			HAL_GPIO_Init(GPIOA, &GPIO_InitStruct);

			/* Peripheral DMA init*/

			hdma_tim2_up.Instance = DMA1_Channel2;		
			hdma_tim2_up.Init.Direction = DMA_PERIPH_TO_MEMORY;
			hdma_tim2_up.Init.PeriphInc = DMA_PINC_DISABLE;
			hdma_tim2_up.Init.MemInc = DMA_MINC_DISABLE;
			hdma_tim2_up.Init.PeriphDataAlignment = DMA_PDATAALIGN_WORD;
			hdma_tim2_up.Init.MemDataAlignment = DMA_MDATAALIGN_WORD;
			hdma_tim2_up.Init.Mode = DMA_CIRCULAR;
			hdma_tim2_up.Init.Priority = DMA_PRIORITY_HIGH;
			HAL_DMA_Init(&hdma_tim2_up);
			
			__HAL_LINKDMA(htim_base,hdma[TIM_DMA_ID_UPDATE],hdma_tim2_up);		
			HAL_DMA_RegisterCallback(&hdma_tim2_up, HAL_DMA_XFER_CPLT_CB_ID, COUNTER_ETR_DMA_CpltCallback);		
			
			/* DMA1_Channel2_IRQn interrupt configuration */
			HAL_NVIC_SetPriority(DMA1_Channel2_IRQn, 9, 0);
			HAL_NVIC_EnableIRQ(DMA1_Channel2_IRQn);		
					
			if(counter.state==COUNTER_ETR){
				counter.counterEtr.psc = TIM4_PSC;	
				counter.counterEtr.arr = TIM4_ARR;
				counter.counterEtr.gateTime = 100;				/* 100 ms */												
			}else{
				counter.counterEtr.psc = 999;	
				counter.counterEtr.arr = 9999;				
			}
			counter.counterEtr.etrp = 1;
			counter.counterEtr.buffer = 0;
			counter.sampleCntChange = SAMPLE_COUNT_CHANGED;					
			
		}else if(counter.state==COUNTER_IC||counter.state==COUNTER_TI){
		
			__HAL_RCC_TIM2_CLK_ENABLE();
		
			/**TIM2 GPIO Configuration    
			PA0     ------> TIM2_CH1
			PA1     ------> TIM2_CH2 
			*/
			GPIO_InitStruct.Pin = GPIO_PIN_0|GPIO_PIN_1;
			GPIO_InitStruct.Mode = GPIO_MODE_AF_PP;
			GPIO_InitStruct.Pull = GPIO_NOPULL;
			GPIO_InitStruct.Speed = GPIO_SPEED_FREQ_HIGH;
			GPIO_InitStruct.Alternate = GPIO_AF1_TIM2;
			HAL_GPIO_Init(GPIOA, &GPIO_InitStruct);

			/* Peripheral DMA init*/
			
			hdma_tim2_ch2_ch4.Instance = DMA1_Channel7;
			hdma_tim2_ch2_ch4.Init.Direction = DMA_PERIPH_TO_MEMORY;
			hdma_tim2_ch2_ch4.Init.PeriphInc = DMA_PINC_DISABLE;
			if(counter.state==COUNTER_IC){
				hdma_tim2_ch2_ch4.Init.MemInc = DMA_MINC_ENABLE;
			}else{
				hdma_tim2_ch2_ch4.Init.MemInc = DMA_MINC_DISABLE;
			}
			hdma_tim2_ch2_ch4.Init.PeriphDataAlignment = DMA_PDATAALIGN_WORD;
			hdma_tim2_ch2_ch4.Init.MemDataAlignment = DMA_MDATAALIGN_WORD;
			hdma_tim2_ch2_ch4.Init.Mode = DMA_NORMAL;
			hdma_tim2_ch2_ch4.Init.Priority = DMA_PRIORITY_HIGH;
			HAL_DMA_Init(&hdma_tim2_ch2_ch4);
			
			/* Several peripheral DMA handle pointers point to the same DMA handle.
			 Be aware that there is only one channel to perform all the requested DMAs. */
			__HAL_LINKDMA(htim_base,hdma[TIM_DMA_ID_CC2],hdma_tim2_ch2_ch4);
			__HAL_LINKDMA(htim_base,hdma[TIM_DMA_ID_CC4],hdma_tim2_ch2_ch4);		

			hdma_tim2_ch1.Instance = DMA1_Channel5;
			hdma_tim2_ch1.Init.Direction = DMA_PERIPH_TO_MEMORY;
			hdma_tim2_ch1.Init.PeriphInc = DMA_PINC_DISABLE;
			if(counter.state==COUNTER_IC){
				hdma_tim2_ch1.Init.MemInc = DMA_MINC_ENABLE;
			}else{
				hdma_tim2_ch1.Init.MemInc = DMA_MINC_DISABLE;
			}
			hdma_tim2_ch1.Init.PeriphDataAlignment = DMA_PDATAALIGN_WORD;
			hdma_tim2_ch1.Init.MemDataAlignment = DMA_MDATAALIGN_WORD;
			hdma_tim2_ch1.Init.Mode = DMA_NORMAL;
			hdma_tim2_ch1.Init.Priority = DMA_PRIORITY_HIGH;
			HAL_DMA_Init(&hdma_tim2_ch1);

			__HAL_LINKDMA(htim_base,hdma[TIM_DMA_ID_CC1],hdma_tim2_ch1);

//			HAL_DMA_RegisterCallback(&hdma_tim2_ch1, HAL_DMA_XFER_CPLT_CB_ID, COUNTER_IC1_DMA_CpltCallback);
//			HAL_DMA_RegisterCallback(&hdma_tim2_ch2_ch4, HAL_DMA_XFER_CPLT_CB_ID, COUNTER_IC2_DMA_CpltCallback);		
			
			/* DMA1_Channel5_IRQn interrupt configuration */
//			HAL_NVIC_SetPriority(DMA1_Channel5_IRQn, 9, 0);
//			HAL_NVIC_EnableIRQ(DMA1_Channel5_IRQn);		
			
			/* DMA1_Channel7_IRQn interrupt configuration */
//			HAL_NVIC_SetPriority(DMA1_Channel7_IRQn, 9, 0);
//			HAL_NVIC_EnableIRQ(DMA1_Channel7_IRQn);								
			
			if(counter.state == COUNTER_IC){
				counter.counterIc.ic1BufferSize = 2;			/* the lowest value of icxBufferSize is 2! - 1 sample for IC frequency measuring */
				counter.counterIc.ic2BufferSize = 2;
				counter.icChannel1 = COUNTER_IRQ_IC_PASS;
				counter.icChannel2 = COUNTER_IRQ_IC_PASS;				
			}else{
				counter.counterIc.ic1BufferSize = 1;			/* only 1 sample for one event that occurs on one single channel */
				counter.counterIc.ic2BufferSize = 1;
				counter.counterIc.tiTimeout = 4000;
			}
			counter.counterIc.ic1psc = 1;
			counter.counterIc.ic2psc = 1;
			TIM_IC1_PSC_Config(1);
			TIM_IC2_PSC_Config(1);	
			counter.counterIc.psc = 0;		
			counter.counterIc.arr = 0xFFFFFFFF;
			
//			counter.counterIc.ic1buffer = (uint32_t *)pvPortMalloc(counter.counterIc.ic1BufferSize*sizeof(uint32_t));
//			counter.counterIc.ic2buffer = (uint32_t *)pvPortMalloc(counter.counterIc.ic2BufferSize*sizeof(uint32_t));	
		}
	}
	
	if(htim_base->Instance==TIM4){
		__TIM4_CLK_ENABLE();
		
		if(counter.state==COUNTER_REF){
			
		 /**TIM4 GPIO Configuration    
			PA8     ------> TIM4_ETR_REF (as reference) 
			*/
			GPIO_InitStruct.Pin = GPIO_PIN_8;
			GPIO_InitStruct.Mode = GPIO_MODE_AF_PP;
			GPIO_InitStruct.Pull = GPIO_NOPULL;
			GPIO_InitStruct.Speed = GPIO_SPEED_FREQ_HIGH;
			GPIO_InitStruct.Alternate = GPIO_AF10_TIM4;
			HAL_GPIO_Init(GPIOA, &GPIO_InitStruct);	
	
		}else if(counter.state==COUNTER_IC||counter.state==COUNTER_TI){
			
			HAL_NVIC_SetPriority(TIM4_IRQn, 9, 0);
			HAL_NVIC_EnableIRQ(TIM4_IRQn);			
		}
	}
	#endif //USE_COUNTER
}



void HAL_TIM_Base_MspDeInit(TIM_HandleTypeDef* htim_base)
{
	#ifdef USE_SCOPE
  if(htim_base->Instance==TIM15)
  {
  /* USER CODE BEGIN TIM15_MspDeInit 0 */

  /* USER CODE END TIM15_MspDeInit 0 */
    /* Peripheral clock disable */
    __TIM15_CLK_DISABLE();
  /* USER CODE BEGIN TIM15_MspDeInit 1 */

  /* USER CODE END TIM15_MspDeInit 1 */
  }
	#endif //USE_SCOPE
	
	#if defined(USE_GEN) || defined(USE_GEN_PWM)
		#ifdef USE_GEN
		if(generator.modeState==GENERATOR_DAC){
			if(htim_base->Instance==TIM6){
				__TIM6_CLK_DISABLE();				
			}
			if(htim_base->Instance==TIM7){
				__TIM7_CLK_DISABLE();				
			}
		}
		#endif //USE_GEN
		
		#ifdef USE_GEN_PWM
		if(generator.modeState==GENERATOR_PWM){
			if(htim_base->Instance==TIM1){
				__TIM1_CLK_DISABLE();
			}
			if(htim_base->Instance==TIM3){
				__TIM3_CLK_DISABLE();
			}
			if(htim_base->Instance==TIM6){
				__TIM6_CLK_DISABLE();
				
				/* Peripheral DMA DeInit*/
				HAL_DMA_DeInit(htim_base->hdma[TIM_DMA_ID_UPDATE]);				
			}
			if(htim_base->Instance==TIM7){
				__TIM7_CLK_DISABLE();
				
				/* Peripheral DMA DeInit*/
				HAL_DMA_DeInit(htim_base->hdma[TIM_DMA_ID_UPDATE]);				
			}
		}
		#endif //USE_GEN_PWM
		
	#endif //USE_GEN || USE_GEN_PWM
	
	#ifdef USE_COUNTER
	if(htim_base->Instance==TIM2){
		
		if(counter.state==COUNTER_ETR||counter.state==COUNTER_REF){   
			
			HAL_GPIO_DeInit(GPIOA, GPIO_PIN_0);		/* TIM2 GPIO Configuration PA0 -> TIM2_ETR */		
			HAL_NVIC_DisableIRQ(DMA1_Channel2_IRQn);
			HAL_DMA_UnRegisterCallback(&hdma_tim2_up, HAL_DMA_XFER_CPLT_CB_ID);		
			HAL_DMA_DeInit(htim_base->hdma[TIM_DMA_ID_UPDATE]);		
			
			TIM2 -> DIER &= ~TIM_DIER_UDE;					
			TIM2 -> CCMR1 &= ~TIM_CCMR1_CC1S_Msk;
			TIM2 -> CCER &= ~TIM_CCER_CC1E;
			
		}else if(counter.state==COUNTER_IC||counter.state == COUNTER_TI){		
			
			HAL_GPIO_DeInit(GPIOA, GPIO_PIN_0|GPIO_PIN_1);
//			HAL_NVIC_DisableIRQ(DMA1_Channel5_IRQn);	
//			HAL_NVIC_DisableIRQ(DMA1_Channel7_IRQn);			
//			HAL_DMA_UnRegisterCallback(&hdma_tim2_ch1, HAL_DMA_XFER_CPLT_CB_ID);		
//			HAL_DMA_UnRegisterCallback(&hdma_tim2_ch2_ch4, HAL_DMA_XFER_CPLT_CB_ID);				
			HAL_DMA_DeInit(htim_base->hdma[TIM_DMA_ID_CC2]);
//			HAL_DMA_DeInit(htim_base->hdma[TIM_DMA_ID_CC4]);
			HAL_DMA_DeInit(htim_base->hdma[TIM_DMA_ID_CC1]);	

			TIM2 -> CCMR1 &= ~TIM_CCMR1_CC1S_0;  		
			TIM2 -> CCMR1 &= ~TIM_CCMR1_CC2S_0;			
			TIM2 -> CCER &= ~TIM_CCER_CC1E;					
			TIM2 -> CCER &= ~TIM_CCER_CC2E;			
			TIM2 -> DIER &= ~TIM_DIER_CC1DE;				/* Capture/Compare 1 DMA request deinit */
			TIM2 -> DIER &= ~TIM_DIER_CC2DE;				/* Capture/Compare 1 DMA request deinit */			
			TIM2->CR1 &= ~TIM_CR1_OPM;							/* Deactivate one pulse mode */
			
	//		vPortFree(counter.counterIc.ic1buffer);
	//		vPortFree(counter.counterIc.ic2buffer);
	//		counter.counterIc.ic1buffer = NULL;
	//		counter.counterIc.ic2buffer = NULL;	
	
	//		HAL_TIM_Base_DeInit(&htim4);
		}		
		__TIM2_CLK_DISABLE(); 		
	}
	
	if(htim_base->Instance==TIM4){
		
		if(counter.state==COUNTER_REF){
			HAL_GPIO_DeInit(GPIOA, GPIO_PIN_8);
			
		} else if(counter.state==COUNTER_IC||counter.state==COUNTER_TI){
			HAL_NVIC_DisableIRQ(TIM4_IRQn);
			
		}	else if(counter.state==COUNTER_ETR){
			HAL_GPIO_DeInit(GPIOA, GPIO_PIN_0);
			
		}
		__TIM4_CLK_DISABLE();
	}
	#endif //USE_COUNTER
} 




/* USER CODE BEGIN 1 */
#ifdef USE_SCOPE
uint8_t TIM_Reconfig_scope(uint32_t samplingFreq,uint32_t* realFreq){
	return TIM_Reconfig(samplingFreq,&htim_scope,realFreq);
}
#endif //USE_SCOPE

#ifdef USE_SCOPE
void TIMScopeEnable(){
	HAL_TIM_Base_Start(&htim_scope);
}
void TIMScopeDisable(){
	HAL_TIM_Base_Stop(&htim_scope);
}	
uint32_t getMaxScopeSamplingFreq(uint8_t ADCRes){
	if(ADCRes==12){
		return 4800000;
	}else if(ADCRes==8){
		return 6000000;
	}
	return HAL_RCC_GetPCLK2Freq()/(ADCRes+2);
}
#endif //USE_SCOPE

#if defined(USE_GEN) || defined(USE_GEN_PWM)
uint8_t TIM_Reconfig_gen(uint32_t samplingFreq,uint8_t chan,uint32_t* realFreq){
	if(chan==0){
		return TIM_Reconfig(samplingFreq,&htim6,realFreq);
	}else if(chan==1){
		return TIM_Reconfig(samplingFreq,&htim7,realFreq);
	}else{
		return 0;
	}
}

void TIMGenEnable(void){
  HAL_TIM_Base_Start(&htim6);
	HAL_TIM_Base_Start(&htim7);
}

void TIMGenDisable(void){
  HAL_TIM_Base_Stop(&htim6);
	HAL_TIM_Base_Stop(&htim7);
}

void TIMGenInit(void){
	MX_DAC_Init();
	MX_TIM6_Init();
	MX_TIM7_Init();
}
#endif //USE_GEN || USE_GEN_PWM


#ifdef USE_GEN_PWM
void TIM_DMA_Reconfig(uint8_t chan){	
	if(chan==0){
		HAL_DMA_Abort(&hdma_tim6_up);
		HAL_DMA_Start(&hdma_tim6_up, (uint32_t)generator.pChanMem[0], (uint32_t)&(TIM1->CCR2), generator.oneChanSamples[0]);
	}else if(chan==1){
		HAL_DMA_Abort(&hdma_tim7_up);
		HAL_DMA_Start(&hdma_tim7_up, (uint32_t)generator.pChanMem[1], (uint32_t)&(TIM3->CCR1), generator.oneChanSamples[1]);
	}
}

void PWMGeneratingEnable(void){
	if(generator.numOfChannles==1){				
		HAL_TIM_PWM_Start(&htim1, TIM_CHANNEL_2);
		HAL_TIM_Base_Start(&htim6);		
	}else if(generator.numOfChannles>1){				
		HAL_TIM_PWM_Start(&htim1, TIM_CHANNEL_2);
		HAL_TIM_Base_Start(&htim6);						
		HAL_TIM_PWM_Start(&htim3, TIM_CHANNEL_1);		
		HAL_TIM_Base_Start(&htim7);			
	}
}

void PWMGeneratingDisable(void){
	if(generator.numOfChannles==1){				
		HAL_TIM_PWM_Stop(&htim1, TIM_CHANNEL_2);
		HAL_TIM_Base_Stop(&htim6);		
	}else if(generator.numOfChannles>1){				
		HAL_TIM_PWM_Stop(&htim1, TIM_CHANNEL_2);
		HAL_TIM_Base_Stop(&htim6);						
		HAL_TIM_PWM_Stop(&htim3, TIM_CHANNEL_1);		
		HAL_TIM_Base_Stop(&htim7);			
	}
}

void TIMGenPwmInit(void){
		MX_TIM1_GEN_PWM_Init();	
		MX_TIM6_GEN_PWM_Init();
		MX_TIM3_GEN_PWM_Init();			// PWM generation
		MX_TIM7_GEN_PWM_Init();			// DMA transaction timing
}

void TIMGenPwmDeinit(void){
		HAL_TIM_Base_DeInit(&htim1);
		HAL_TIM_Base_DeInit(&htim6);			
		HAL_TIM_Base_DeInit(&htim3);		
		HAL_TIM_Base_DeInit(&htim7);
}

void TIMGenPWMEnable(void){
  HAL_TIM_PWM_Start(&htim1, TIM_CHANNEL_2);
	HAL_TIM_PWM_Start(&htim3, TIM_CHANNEL_1);
}

void TIMGenPWMDisable(void){
  HAL_TIM_PWM_Stop(&htim1, TIM_CHANNEL_2);
	HAL_TIM_PWM_Stop(&htim3, TIM_CHANNEL_1);
}

void TIMGenDacDeinit(void){
	HAL_TIM_Base_DeInit(&htim6);
	HAL_TIM_Base_DeInit(&htim7);
}

void TIM_GEN_PWM_PSC_Config(uint16_t pscVal, uint8_t chan){
	if(chan == 1){
		TIM1->PSC = pscVal;
	}else{
		TIM3->PSC = pscVal;
	}
}

void TIM_GEN_PWM_ARR_Config(uint16_t arrVal, uint8_t chan){
	if(chan == 1){
		TIM1->ARR = arrVal;
	}else{
		TIM3->ARR = arrVal;
	}	
}

#endif //USE_GEN_PWM


uint8_t TIM_Reconfig(uint32_t samplingFreq,TIM_HandleTypeDef* htim_base,uint32_t* realFreq){
	
	int32_t clkDiv;
	uint16_t prescaler;
	uint16_t autoReloadReg;
	uint32_t errMinRatio = 0;
	uint8_t result = UNKNOW_ERROR;
	
	clkDiv = ((2*HAL_RCC_GetPCLK2Freq() / samplingFreq)+1)/2; //to minimize rounding error
	
	if(clkDiv == 0){ //error
		result = GEN_FREQ_MISMATCH;
	}else if(clkDiv <= 0x0FFFF){ //Sampling frequency is high enough so no prescaler needed
		prescaler = 0;
		autoReloadReg = clkDiv - 1;
		result = 0;
	}else{	// finding prescaler and autoReload value
		uint32_t errVal = 0xFFFFFFFF;
		uint32_t errMin = 0xFFFFFFFF;
		uint16_t ratio = clkDiv>>16; 
		uint16_t div;
		
		while(errVal != 0){
			ratio++;
			div = clkDiv/ratio;
			errVal = clkDiv - (div*ratio);

			if(errVal < errMin){
				errMin = errVal;
				errMinRatio = ratio;
			}
		
			if(ratio == 0xFFFF){ //exact combination wasnt found. we use best found
				div = clkDiv/errMinRatio;
				ratio = errMinRatio;
				break;
			}			
		}

		if(ratio > div){
			prescaler = div - 1;
			autoReloadReg = ratio - 1;		
		}else{
			prescaler = ratio - 1;
			autoReloadReg = div - 1;
		}	

		if(errVal){
			result = GEN_FREQ_IS_INACCURATE;
		}else{
			result = 0;
		}
	}
	if(realFreq!=0){
		*realFreq=HAL_RCC_GetPCLK2Freq()/((prescaler+1)*(autoReloadReg+1));
//		if(*realFreq>MAX_SAMPLING_FREQ && autoReloadReg<0xffff){
//			autoReloadReg++;
//			*realFreq=HAL_RCC_GetPCLK2Freq()/((prescaler+1)*(autoReloadReg+1));
//		}
	}
	htim_base->Init.Period = autoReloadReg;
  htim_base->Init.Prescaler = prescaler;
  HAL_TIM_Base_Init(htim_base);
	return result;

}

/* USER CODE END 1 */

/**
  * @}
  */

/**
  * @}
  */

/************************ (C) COPYRIGHT STMicroelectronics *****END OF FILE****/
