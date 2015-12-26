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

DAC_HandleTypeDef hdac;
DMA_HandleTypeDef hdma_dac1;
DMA_HandleTypeDef hdma_dac2;

uint32_t outputBuffEn=DAC_OUTPUTBUFFER_ENABLE;

/* DAC init function */
void MX_DAC_Init(void)
{
  DAC_ChannelConfTypeDef sConfig;

    /**DAC Initialization 
    */
  hdac.Instance = DAC;
  HAL_DAC_Init(&hdac);

    /**DAC channel OUT1 config 
    */
  sConfig.DAC_Trigger = DAC_TRIGGER_T6_TRGO;
  sConfig.DAC_OutputBuffer = outputBuffEn;
  HAL_DAC_ConfigChannel(&hdac, &sConfig, DAC_CHANNEL_1);

    /**DAC channel OUT2 config 
    */
  
	sConfig.DAC_Trigger = DAC_TRIGGER_T7_TRGO;
  sConfig.DAC_OutputBuffer = outputBuffEn;
	HAL_DAC_ConfigChannel(&hdac, &sConfig, DAC_CHANNEL_2);
}

void HAL_DAC_MspInit(DAC_HandleTypeDef* hdac)
{

  GPIO_InitTypeDef GPIO_InitStruct;
  if(hdac->Instance==DAC)
  {
  /* USER CODE BEGIN DAC_MspInit 0 */

  /* USER CODE END DAC_MspInit 0 */
    /* Peripheral clock enable */
    __DAC_CLK_ENABLE();
  
    /**DAC GPIO Configuration    
    PA4     ------> DAC_OUT1
    PA5     ------> DAC_OUT2 
    */
    GPIO_InitStruct.Pin = GPIO_PIN_4|GPIO_PIN_5;
    GPIO_InitStruct.Mode = GPIO_MODE_ANALOG;
    GPIO_InitStruct.Pull = GPIO_NOPULL;
    HAL_GPIO_Init(GPIOA, &GPIO_InitStruct);

  /* USER CODE BEGIN DAC_MspInit 1 */
				/* Set the parameters to be configured for Channel1*/
		hdma_dac1.Instance = DMA1_Stream5;
		
		hdma_dac1.Init.Channel  = DMA_CHANNEL_7;
		hdma_dac1.Init.Direction = DMA_MEMORY_TO_PERIPH;
		hdma_dac1.Init.PeriphInc = DMA_PINC_DISABLE;
		hdma_dac1.Init.MemInc = DMA_MINC_ENABLE;
		hdma_dac1.Init.PeriphDataAlignment = DMA_PDATAALIGN_HALFWORD;
		hdma_dac1.Init.MemDataAlignment = DMA_PDATAALIGN_HALFWORD;
		hdma_dac1.Init.Mode = DMA_CIRCULAR;
		hdma_dac1.Init.Priority = DMA_PRIORITY_HIGH;
		hdma_dac1.Init.FIFOMode = DMA_FIFOMODE_DISABLE;         
		hdma_dac1.Init.FIFOThreshold = DMA_FIFO_THRESHOLD_HALFFULL;
		hdma_dac1.Init.MemBurst = DMA_MBURST_SINGLE;
		hdma_dac1.Init.PeriphBurst = DMA_PBURST_SINGLE; 

		HAL_DMA_Init(&hdma_dac1);
			
		/* Associate the initialized DMA handle to the the DAC handle */
		__HAL_LINKDMA(hdac, DMA_Handle1, hdma_dac1);
		
						/* Set the parameters to be configured for Channel2*/
		hdma_dac2.Instance = DMA1_Stream6;
		
		hdma_dac2.Init.Channel  = DMA_CHANNEL_7;
		hdma_dac2.Init.Direction = DMA_MEMORY_TO_PERIPH;
		hdma_dac2.Init.PeriphInc = DMA_PINC_DISABLE;
		hdma_dac2.Init.MemInc = DMA_MINC_ENABLE;
		hdma_dac2.Init.PeriphDataAlignment = DMA_PDATAALIGN_HALFWORD;
		hdma_dac2.Init.MemDataAlignment = DMA_PDATAALIGN_HALFWORD;
		hdma_dac2.Init.Mode = DMA_CIRCULAR;
		hdma_dac2.Init.Priority = DMA_PRIORITY_HIGH;
		hdma_dac2.Init.FIFOMode = DMA_FIFOMODE_DISABLE;         
		hdma_dac2.Init.FIFOThreshold = DMA_FIFO_THRESHOLD_HALFFULL;
		hdma_dac2.Init.MemBurst = DMA_MBURST_SINGLE;
		hdma_dac2.Init.PeriphBurst = DMA_PBURST_SINGLE; 

		HAL_DMA_Init(&hdma_dac2);
			
		/* Associate the initialized DMA handle to the the DAC handle */
		__HAL_LINKDMA(hdac, DMA_Handle2, hdma_dac2);

  /* USER CODE END DAC_MspInit 1 */
  }
}

void HAL_DAC_MspDeInit(DAC_HandleTypeDef* hdac)
{

  if(hdac->Instance==DAC)
  {
  /* USER CODE BEGIN DAC_MspDeInit 0 */

  /* USER CODE END DAC_MspDeInit 0 */
    /* Peripheral clock disable */
    __DAC_CLK_DISABLE();
  
    /**DAC GPIO Configuration    
    PA4     ------> DAC_OUT1
    PA5     ------> DAC_OUT2 
    */
    HAL_GPIO_DeInit(GPIOA, GPIO_PIN_4|GPIO_PIN_5);

  /* USER CODE BEGIN DAC_MspDeInit 1 */

  /* USER CODE END DAC_MspDeInit 1 */
  }
} 

/* USER CODE BEGIN 1 */
void DAC_DMA_Reconfig(uint8_t chan, uint32_t *buff, uint32_t len){
	uint32_t dacChannel=0;
	switch(chan){
		case 0:
			dacChannel=DAC_CHANNEL_1;
		break;
		case 1:
			dacChannel=DAC_CHANNEL_2;
		break;
	}
	HAL_DAC_Stop_DMA(&hdac,dacChannel);
	HAL_DAC_Start_DMA(&hdac, dacChannel, buff, len, DAC_ALIGN_12B_R);
}


void DACDisableOutput(void){
	GPIO_InitTypeDef GPIO_InitStruct;
  GPIO_InitStruct.Pin = GPIO_PIN_4|GPIO_PIN_5;
  GPIO_InitStruct.Mode = GPIO_MODE_INPUT;
  GPIO_InitStruct.Pull = GPIO_PULLDOWN;
  HAL_GPIO_Init(GPIOA, &GPIO_InitStruct);
}

void DACEnableOutput(void){
	GPIO_InitTypeDef GPIO_InitStruct;
  GPIO_InitStruct.Pin = GPIO_PIN_4|GPIO_PIN_5;
  GPIO_InitStruct.Mode = GPIO_MODE_ANALOG;
  GPIO_InitStruct.Pull = GPIO_NOPULL;
  HAL_GPIO_Init(GPIOA, &GPIO_InitStruct);
}

void DACSetOutputBuffer(void){
	outputBuffEn=DAC_OUTPUTBUFFER_ENABLE;
}

void DACUnsetOutputBuffer(void){
	outputBuffEn=DAC_OUTPUTBUFFER_DISABLE;
}

/**
  * @brief  Enable sampling
  * @param  None
  * @retval None
  */
void GeneratingEnable (void){
	MX_DAC_Init();
	DACEnableOutput();
	TIMGenEnable();
}

/**
  * @brief  Disable sampling
  * @param  None
  * @retval None
  */
void GeneratingDisable (void){
	DACDisableOutput();
	TIMGenDisable();	
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
