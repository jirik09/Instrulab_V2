/**
  ******************************************************************************
  * File Name          : DAC.c
  * Date               : 18/01/2015 10:00:30
  * Description        : This file provides code for the configuration
  *                      of the DAC instances.
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

#ifdef USE_GEN
#include "dac.h"

#include "gpio.h"
#include "tim.h"

/* USER CODE BEGIN 0 */

/* USER CODE END 0 */

DAC_HandleTypeDef hdac1;
DAC_HandleTypeDef hdac2;
DMA_HandleTypeDef hdma_dac1_ch2;
DMA_HandleTypeDef hdma_dac2_ch1;

uint32_t outputBuffEn=DAC_OUTPUTBUFFER_DISABLE;

void MX_DAC1_Init(void)
{

  DAC_ChannelConfTypeDef sConfig;

    /**DAC Initialization 
    */
  hdac1.Instance = DAC1;
  HAL_DAC_Init(&hdac1);

    /**DAC channel OUT2 config 
    */
  sConfig.DAC_Trigger = DAC_TRIGGER_T7_TRGO;
  sConfig.DAC_OutputBuffer = outputBuffEn;

  HAL_DAC_ConfigChannel(&hdac1, &sConfig, DAC_CHANNEL_2);
	hdac1.Instance->CR |= DAC_CR_BOFF2;
}

/* DAC2 init function */
void MX_DAC2_Init(void)
{

  DAC_ChannelConfTypeDef sConfig;

    /**DAC Initialization 
    */
  hdac2.Instance = DAC2;
  HAL_DAC_Init(&hdac2);

    /**DAC channel OUT1 config 
    */
  sConfig.DAC_Trigger = DAC_TRIGGER_T6_TRGO;
  sConfig.DAC_OutputBuffer = outputBuffEn;

  HAL_DAC_ConfigChannel(&hdac2, &sConfig, DAC_CHANNEL_1);
	hdac2.Instance->CR |= DAC_CR_BOFF1;

}

void HAL_DAC_MspInit(DAC_HandleTypeDef* hdac)
{

  GPIO_InitTypeDef GPIO_InitStruct;
  if(hdac->Instance==DAC1)
  {
  /* USER CODE BEGIN DAC1_MspInit 0 */

  /* USER CODE END DAC1_MspInit 0 */
    /* Peripheral clock enable */
    __DAC1_CLK_ENABLE();
  
    /**DAC1 GPIO Configuration    
    PA5     ------> DAC1_OUT2 
    */
    GPIO_InitStruct.Pin = GPIO_PIN_5;
    GPIO_InitStruct.Mode = GPIO_MODE_ANALOG;
    GPIO_InitStruct.Pull = GPIO_NOPULL;
    HAL_GPIO_Init(GPIOA, &GPIO_InitStruct);

    /* Peripheral DMA init*/
  
    hdma_dac1_ch2.Instance = DMA1_Channel4;
    hdma_dac1_ch2.Init.Direction = DMA_MEMORY_TO_PERIPH;
    hdma_dac1_ch2.Init.PeriphInc = DMA_PINC_DISABLE;
    hdma_dac1_ch2.Init.MemInc = DMA_MINC_ENABLE;
    hdma_dac1_ch2.Init.PeriphDataAlignment = DMA_PDATAALIGN_HALFWORD;
    hdma_dac1_ch2.Init.MemDataAlignment = DMA_MDATAALIGN_HALFWORD;
    hdma_dac1_ch2.Init.Mode = DMA_CIRCULAR;
    hdma_dac1_ch2.Init.Priority = DMA_PRIORITY_HIGH;
    HAL_DMA_Init(&hdma_dac1_ch2);

		__HAL_REMAPDMA_CHANNEL_ENABLE(HAL_REMAPDMA_TIM7_DAC1_CH2_DMA1_CH4);

		__HAL_LINKDMA(hdac,DMA_Handle2,hdma_dac1_ch2);		

  /* USER CODE BEGIN DAC1_MspInit 1 */

  /* USER CODE END DAC1_MspInit 1 */
  }
  else if(hdac->Instance==DAC2)
  {
  /* USER CODE BEGIN DAC2_MspInit 0 */

  /* USER CODE END DAC2_MspInit 0 */
    /* Peripheral clock enable */
    __DAC2_CLK_ENABLE();
  
    /**DAC2 GPIO Configuration    
    PA6     ------> DAC2_OUT1 
    */
    GPIO_InitStruct.Pin = GPIO_PIN_6;
    GPIO_InitStruct.Mode = GPIO_MODE_ANALOG;
    GPIO_InitStruct.Pull = GPIO_NOPULL;
    HAL_GPIO_Init(GPIOA, &GPIO_InitStruct);

    /* Peripheral DMA init*/
  
    hdma_dac2_ch1.Instance = DMA1_Channel5;
    hdma_dac2_ch1.Init.Direction = DMA_MEMORY_TO_PERIPH;
    hdma_dac2_ch1.Init.PeriphInc = DMA_PINC_DISABLE;
    hdma_dac2_ch1.Init.MemInc = DMA_MINC_ENABLE;
    hdma_dac2_ch1.Init.PeriphDataAlignment = DMA_PDATAALIGN_HALFWORD;
    hdma_dac2_ch1.Init.MemDataAlignment = DMA_MDATAALIGN_HALFWORD;
    hdma_dac2_ch1.Init.Mode = DMA_CIRCULAR;
    hdma_dac2_ch1.Init.Priority = DMA_PRIORITY_HIGH;
    HAL_DMA_Init(&hdma_dac2_ch1);

		__HAL_REMAPDMA_CHANNEL_ENABLE(HAL_REMAPDMA_DAC2_CH1_DMA1_CH5);

		__HAL_LINKDMA(hdac,DMA_Handle1,hdma_dac2_ch1);

  /* USER CODE BEGIN DAC2_MspInit 1 */

  /* USER CODE END DAC2_MspInit 1 */
  }

}

void HAL_DAC_MspDeInit(DAC_HandleTypeDef* hdac)
{

  if(hdac->Instance==DAC1)
  {
  /* USER CODE BEGIN DAC1_MspDeInit 0 */

  /* USER CODE END DAC1_MspDeInit 0 */
    /* Peripheral clock disable */
    __DAC1_CLK_DISABLE();
  
    /**DAC1 GPIO Configuration    
    PA5     ------> DAC1_OUT2 
    */
    HAL_GPIO_DeInit(GPIOA, GPIO_PIN_5);

    /* Peripheral DMA DeInit*/
    HAL_DMA_DeInit(hdac->DMA_Handle2);
  /* USER CODE BEGIN DAC1_MspDeInit 1 */

  /* USER CODE END DAC1_MspDeInit 1 */
  }
  else if(hdac->Instance==DAC2)
  {
  /* USER CODE BEGIN DAC2_MspDeInit 0 */

  /* USER CODE END DAC2_MspDeInit 0 */
    /* Peripheral clock disable */
    __DAC2_CLK_DISABLE();
  
    /**DAC2 GPIO Configuration    
    PA6     ------> DAC2_OUT1 
    */
    HAL_GPIO_DeInit(GPIOA, GPIO_PIN_6);

    /* Peripheral DMA DeInit*/
    HAL_DMA_DeInit(hdac->DMA_Handle1);
  /* USER CODE BEGIN DAC2_MspDeInit 1 */

  /* USER CODE END DAC2_MspDeInit 1 */
  }

}

/* USER CODE BEGIN 1 */
void DAC_DMA_Reconfig(uint8_t chan, uint32_t *buff, uint32_t len){
	uint32_t dacChannel=0;
	switch(chan){
		case 0:
			dacChannel=DAC_CHANNEL_1;
			HAL_DAC_Stop_DMA(&hdac2,dacChannel);
			HAL_DAC_Start_DMA(&hdac2, dacChannel, buff, len, DAC_ALIGN_12B_R);
		break;
		case 1:
			dacChannel=DAC_CHANNEL_2;
			HAL_DAC_Stop_DMA(&hdac1,dacChannel);
			HAL_DAC_Start_DMA(&hdac1, dacChannel, buff, len, DAC_ALIGN_12B_R);
		break;
	}
	
}

void DACDisableOutput(void){
	GPIO_InitTypeDef GPIO_InitStruct;
  GPIO_InitStruct.Pin = GPIO_PIN_6|GPIO_PIN_5;
  GPIO_InitStruct.Mode = GPIO_MODE_INPUT;
  GPIO_InitStruct.Pull = GPIO_NOPULL;
  HAL_GPIO_Init(GPIOA, &GPIO_InitStruct);
}

void DACEnableOutput(void){
	GPIO_InitTypeDef GPIO_InitStruct;
  GPIO_InitStruct.Pin = GPIO_PIN_6|GPIO_PIN_5;
  GPIO_InitStruct.Mode = GPIO_MODE_ANALOG;
  GPIO_InitStruct.Pull = GPIO_NOPULL;
  HAL_GPIO_Init(GPIOA, &GPIO_InitStruct);
}

void DACSetOutputBuffer(void){
	outputBuffEn=DAC_OUTPUTBUFFER_DISABLE; //Swaped in F303K8 why?
}

void DACUnsetOutputBuffer(void){
	outputBuffEn=DAC_OUTPUTBUFFER_ENABLE; //swaped in F303K8 why?
}



/**
  * @brief  Enable sampling
  * @param  None
  * @retval None
  */
void GeneratingEnable (void){
	MX_DAC1_Init();
	MX_DAC2_Init();
	DACEnableOutput();
	TIMGenEnable();
}

/**
  * @brief  Disable sampling
  * @param  None
  * @retval None
  */
void GeneratingDisable (void){
	TIMGenDisable();
	HAL_DAC_Stop(&hdac1,DAC_CHANNEL_2);
	HAL_DAC_Stop(&hdac2,DAC_CHANNEL_1);
	DACDisableOutput();	
}

/* USER CODE END 1 */

/**
  * @}
  */

/**
  * @}
  */
#endif //USE_GEN
/************************ (C) COPYRIGHT STMicroelectronics *****END OF FILE****/
