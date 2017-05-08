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
#include "tim.h"
#include "mcu_config.h"
/* USER CODE BEGIN 0 */

/* USER CODE END 0 */
#ifdef USE_SCOPE	
TIM_HandleTypeDef htim_scope;
#endif //USE_SCOPE
	
#ifdef USE_GEN
TIM_HandleTypeDef htim6;
TIM_HandleTypeDef htim7;
#endif //USE_GEN

#ifdef USE_COUNTER
uint32_t tim2clk, tim4clk;
unsigned int timerClockSource = 14;
//volatile uint16_t ic1bufferSize, ic2bufferSize;
TIM_HandleTypeDef htim2;
TIM_HandleTypeDef htim4;
DMA_HandleTypeDef hdma_tim2_up;
DMA_HandleTypeDef hdma_tim2_ch1;
DMA_HandleTypeDef hdma_tim2_ch2_ch4;

extern void COUNTER_ETR_DMA_CpltCallback(DMA_HandleTypeDef *dmah);	
extern void COUNTER_IC1_DMA_CpltCallback(DMA_HandleTypeDef *dmah);
extern void COUNTER_IC2_DMA_CpltCallback(DMA_HandleTypeDef *dmah);
#endif //USE_COUNTER
			
#ifdef USE_SCOPE
/* TIM3 init function */
void MX_TIM3_Init(void)
{
  TIM_ClockConfigTypeDef sClockSourceConfig;
  TIM_MasterConfigTypeDef sMasterConfig;

  htim_scope.Instance = TIM3;
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


#ifdef USE_COUNTER
/* Timer TIM4 initialization - used for time gating of Counter TIM2 */
void MX_TIM4_Init(void)
{
  TIM_ClockConfigTypeDef sClockSourceConfig;
  TIM_MasterConfigTypeDef sMasterConfig;

  htim4.Instance = TIM4;
  htim4.Init.Prescaler = TIM4_PSC;			// by default 7199
  htim4.Init.CounterMode = TIM_COUNTERMODE_UP;
  htim4.Init.Period = TIM4_ARR;					// by default 9999
  htim4.Init.ClockDivision = TIM_CLOCKDIVISION_DIV1;
  htim4.Init.AutoReloadPreload = TIM_AUTORELOAD_PRELOAD_ENABLE;
	HAL_TIM_Base_Init(&htim4);

  sClockSourceConfig.ClockSource = TIM_CLOCKSOURCE_INTERNAL;
  HAL_TIM_ConfigClockSource(&htim4, &sClockSourceConfig);

  sMasterConfig.MasterOutputTrigger = TIM_TRGO_UPDATE; // TIM_TRGO_OC1 // TIM_TRGO_RESET // TIM_TRGO_UPDATE
  sMasterConfig.MasterSlaveMode = TIM_MASTERSLAVEMODE_ENABLE;
	HAL_TIMEx_MasterConfigSynchronization(&htim4, &sMasterConfig);
}

/* TIM2 mode ETR init function */
void MX_TIM2_ETR_Init(void)
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
	timerClockSource = TIM_CLOCKSOURCE_ETRMODE2;
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

	TIM2 -> DIER |= TIM_DIER_UDE;					/* __HAL_TIM_ENABLE_DMA(&htim2, TIM_DMA_UPDATE); */			
	TIM2 -> CCMR1 |= TIM_CCMR1_CC1S_Msk;  	/* Capture/Compare 1 Selection - CC1 channel is configured as input, IC1 is mapped on TRC	*/
	TIM2 -> CCER |= TIM_CCER_CC1E;					/* CC1 channel configured as input: This bit determines if a capture of the counter value can
																					 actually be done into the input capture/compare register 1 (TIMx_CCR1) or not.  */
}

void MX_TIM2_IC_Init(void)
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
	
	TIM2 -> DIER = TIM_DIER_UDE;					/* __HAL_TIM_ENABLE_DMA(&htim2, TIM_DMA_UPDATE); */			
	TIM2 -> CCMR1 = TIM_CCMR1_CC1S_0;  		/* Capture/Compare 1 Selection - CC1 channel is configured as input, IC1 is mapped on TI1	*/
	TIM2 -> CCMR1 = TIM_CCMR1_CC2S_0;			/* IC2 is mapped on TI2 */		
	
	TIM2 -> CCER = TIM_CCER_CC1E;					/* CC1 channel configured as input: This bit determines if a capture of the counter value can
																					 actually be done into the input capture/compare register 1 (TIMx_CCR1) or not.  */
	TIM2 -> CCER = TIM_CCER_CC2E;		
	
	TIM2 -> DIER = TIM_DIER_CC1DE;				/* Capture/Compare 1 DMA request */
	TIM2 -> DIER = TIM_DIER_CC2DE;				/* Capture/Compare 1 DMA request */
}

void TIM_counter_etr_init(void){	
	
	MX_TIM4_Init();
	MX_TIM2_ETR_Init();
	
	tim4clk = HAL_RCCEx_GetPeriphCLKFreq(RCC_PERIPHCLK_TIM34); 	
	if ((RCC->CFGR3&RCC_TIM2CLK_PLLCLK) == RCC_TIM2CLK_PLLCLK){
		tim2clk = HAL_RCCEx_GetPeriphCLKFreq(RCC_PERIPHCLK_TIM2) * 2; 
	}	else {
		tim2clk = HAL_RCCEx_GetPeriphCLKFreq(RCC_PERIPHCLK_TIM2); 
	}		
}

void TIM_counter_ic_init(void){
	MX_TIM2_IC_Init();
	
	if ((RCC->CFGR3&RCC_TIM2CLK_PLLCLK) == RCC_TIM2CLK_PLLCLK){
		tim2clk = HAL_RCCEx_GetPeriphCLKFreq(RCC_PERIPHCLK_TIM2) * 2; 
	}	else {
		tim2clk = HAL_RCCEx_GetPeriphCLKFreq(RCC_PERIPHCLK_TIM2); 
	}		
}

void TIM_etr_deinit(void){
	HAL_TIM_Base_DeInit(&htim2);
	HAL_TIM_Base_DeInit(&htim4);
}

void TIM_ic_deinit(void){
	HAL_TIM_Base_DeInit(&htim2);	
}

#endif // USE_COUNTER


void HAL_TIM_Base_MspInit(TIM_HandleTypeDef* htim_base)
{
	#ifdef USE_SCOPE
  if(htim_base->Instance==TIM3)
  {
    /* Peripheral clock enable */
    __TIM3_CLK_ENABLE();
  }
	#endif //USE_SCOPE

	#ifdef USE_GEN
	if(htim_base->Instance==TIM6){
		__TIM6_CLK_ENABLE();
	}
	if(htim_base->Instance==TIM7){
		__TIM7_CLK_ENABLE();
	}
	#endif //USE_GEN
	
	#ifdef USE_COUNTER
  GPIO_InitTypeDef GPIO_InitStruct;	  
	
	if(htim_base->Instance==TIM2 && timerClockSource==TIM_CLOCKSOURCE_ETRMODE2){

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
		HAL_NVIC_SetPriority(DMA1_Channel2_IRQn, 3, 0);
		HAL_NVIC_EnableIRQ(DMA1_Channel2_IRQn);		
		
	} else if (htim_base->Instance==TIM2 && timerClockSource==TIM_CLOCKSOURCE_INTERNAL){
		
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
    hdma_tim2_ch2_ch4.Init.MemInc = DMA_MINC_ENABLE;
    hdma_tim2_ch2_ch4.Init.PeriphDataAlignment = DMA_PDATAALIGN_WORD;
    hdma_tim2_ch2_ch4.Init.MemDataAlignment = DMA_MDATAALIGN_WORD;
    hdma_tim2_ch2_ch4.Init.Mode = DMA_CIRCULAR;
    hdma_tim2_ch2_ch4.Init.Priority = DMA_PRIORITY_HIGH;
    HAL_DMA_Init(&hdma_tim2_ch2_ch4);
		
    /* Several peripheral DMA handle pointers point to the same DMA handle.
     Be aware that there is only one channel to perform all the requested DMAs. */
    __HAL_LINKDMA(htim_base,hdma[TIM_DMA_ID_CC2],hdma_tim2_ch2_ch4);
    __HAL_LINKDMA(htim_base,hdma[TIM_DMA_ID_CC4],hdma_tim2_ch2_ch4);		

    hdma_tim2_ch1.Instance = DMA1_Channel5;
    hdma_tim2_ch1.Init.Direction = DMA_PERIPH_TO_MEMORY;
    hdma_tim2_ch1.Init.PeriphInc = DMA_PINC_DISABLE;
    hdma_tim2_ch1.Init.MemInc = DMA_MINC_ENABLE;
    hdma_tim2_ch1.Init.PeriphDataAlignment = DMA_PDATAALIGN_WORD;
    hdma_tim2_ch1.Init.MemDataAlignment = DMA_MDATAALIGN_WORD;
    hdma_tim2_ch1.Init.Mode = DMA_CIRCULAR;
    hdma_tim2_ch1.Init.Priority = DMA_PRIORITY_HIGH;
    HAL_DMA_Init(&hdma_tim2_ch1);

    __HAL_LINKDMA(htim_base,hdma[TIM_DMA_ID_CC1],hdma_tim2_ch1);

		HAL_DMA_RegisterCallback(&hdma_tim2_ch1, HAL_DMA_XFER_CPLT_CB_ID, COUNTER_IC1_DMA_CpltCallback);
		HAL_DMA_RegisterCallback(&hdma_tim2_ch2_ch4, HAL_DMA_XFER_CPLT_CB_ID, COUNTER_IC2_DMA_CpltCallback);		
		
		/* DMA1_Channel5_IRQn interrupt configuration */
		HAL_NVIC_SetPriority(DMA1_Channel5_IRQn, 3, 0);
		HAL_NVIC_EnableIRQ(DMA1_Channel5_IRQn);		
	}
	
	if(htim_base->Instance==TIM4){
		__TIM4_CLK_ENABLE();
	}
	#endif //USE_COUNTER
}


void HAL_TIM_Base_MspDeInit(TIM_HandleTypeDef* htim_base)
{
	#ifdef USE_SCOPE
  if(htim_base->Instance==TIM3)
  {
  /* USER CODE BEGIN TIM3_MspDeInit 0 */

  /* USER CODE END TIM3_MspDeInit 0 */
    /* Peripheral clock disable */
    __TIM3_CLK_DISABLE();
  /* USER CODE BEGIN TIM3_MspDeInit 1 */

  /* USER CODE END TIM3_MspDeInit 1 */
  }
	#endif //USE_SCOPE
	
	#ifdef USE_GEN
	if(htim_base->Instance==TIM6){
		__TIM6_CLK_DISABLE();
	}
	if(htim_base->Instance==TIM7){
		__TIM7_CLK_DISABLE();
	}
	#endif //USE_GEN
	
	#ifdef USE_COUNTER
	if(htim_base->Instance==TIM2 && timerClockSource==TIM_CLOCKSOURCE_ETRMODE2){
		__TIM2_CLK_DISABLE();    
    HAL_GPIO_DeInit(GPIOA, GPIO_PIN_0);		/* TIM2 GPIO Configuration PA0 -> TIM2_ETR */
		HAL_DMA_DeInit(htim_base->hdma[TIM_DMA_ID_UPDATE]);
	} else if(htim_base->Instance==TIM2 && timerClockSource==TIM_CLOCKSOURCE_INTERNAL){
		__TIM2_CLK_DISABLE(); 		
    HAL_GPIO_DeInit(GPIOA, GPIO_PIN_0|GPIO_PIN_1);
    HAL_DMA_DeInit(htim_base->hdma[TIM_DMA_ID_CC2]);
    HAL_DMA_DeInit(htim_base->hdma[TIM_DMA_ID_CC4]);
    HAL_DMA_DeInit(htim_base->hdma[TIM_DMA_ID_CC1]);
	}
	if(htim_base->Instance==TIM4){
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

#ifdef USE_GEN
uint8_t TIM_Reconfig_gen(uint32_t samplingFreq,uint8_t chan,uint32_t* realFreq){
	if(chan==0){
		return TIM_Reconfig(samplingFreq,&htim6,realFreq);
	}else if(chan==1){
		return TIM_Reconfig(samplingFreq,&htim7,realFreq);
	}else{
		return 0;
	}
}
#endif //USE_GEN


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


	#ifdef USE_GEN
void TIMGenEnable(void){
  HAL_TIM_Base_Start(&htim6);
	HAL_TIM_Base_Start(&htim7);
}
void TIMGenDisable(void){
  HAL_TIM_Base_Stop(&htim6);
	HAL_TIM_Base_Stop(&htim7);
}

	#endif //USE_GEN


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
