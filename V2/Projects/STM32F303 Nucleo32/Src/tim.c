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
#include "dac.h"
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

void TIMGenInit(void){
	MX_TIM6_Init();
	MX_TIM7_Init();
	MX_DAC1_Init();
	MX_DAC2_Init();
}

void TIMGenDacDeinit(void){
//	HAL_TIM_Base_DeInit(&htim6);
//	HAL_TIM_Base_DeInit(&htim7);
	
	/* Reset TIM peripherals */
	
	RCC->APB1RSTR |= RCC_APB1RSTR_TIM6RST;
	RCC->APB1RSTR &= ~RCC_APB1RSTR_TIM6RST;	
	
	RCC->APB1RSTR |= RCC_APB1RSTR_TIM7RST;
	RCC->APB1RSTR &= ~RCC_APB1RSTR_TIM7RST;	
	
	/* Reset DAC peripheral */
	RCC->APB1RSTR |= RCC_APB1RSTR_DAC1RST;
	RCC->APB1RSTR &= ~RCC_APB1RSTR_DAC1RST;		
}
	
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

void HAL_TIM_Base_MspInit(TIM_HandleTypeDef* htim_base)
{
	#ifdef USE_SCOPE
  if(htim_base->Instance==TIM3)
  {
  /* USER CODE BEGIN TIM3_MspInit 0 */

  /* USER CODE END TIM3_MspInit 0 */
    /* Peripheral clock enable */
    __TIM3_CLK_ENABLE();
  /* USER CODE BEGIN TIM3_MspInit 1 */

  /* USER CODE END TIM3_MspInit 1 */
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



/* USER CODE END 1 */

/**
  * @}
  */

/**
  * @}
  */

/************************ (C) COPYRIGHT STMicroelectronics *****END OF FILE****/
