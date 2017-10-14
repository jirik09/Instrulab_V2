/**
  ******************************************************************************
  * File Name          : TIM.h
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
/* Define to prevent recursive inclusion -------------------------------------*/
#ifndef __tim_H
#define __tim_H
#ifdef __cplusplus
 extern "C" {
#endif

/* Includes ------------------------------------------------------------------*/
#include "stm32f3xx_hal.h"
	 
uint8_t TIM_Reconfig(uint32_t samplingFreq,TIM_HandleTypeDef* htim_base,uint32_t* realFreq);
	 
#ifdef USE_SCOPE
uint8_t TIM_Reconfig_scope(uint32_t samplingFreq,uint32_t* realFreq);
#endif //USE_SCOPE
	 
#if defined(USE_GEN) || defined(USE_GEN_PWM)
uint8_t TIM_Reconfig_gen(uint32_t samplingFreq,uint8_t chan,uint32_t* realFreq);
#endif //USE_GEN || USE_GEN_PWM

#ifdef USE_SCOPE
void TIMScopeEnable(void);
void TIMScopeDisable(void);
void MX_TIM15_Init(void);
uint32_t getMaxScopeSamplingFreq(uint8_t ADCRes);
#endif //USE_SCOPE

#if defined(USE_GEN) || defined(USE_GEN_PWM)
void TIMGenEnable(void);
void TIMGenDisable(void);
void MX_TIM6_Init(void);
void MX_TIM7_Init(void);

void TIMGenInit(void);
void TIMGenPwmDeinit(void);
void TIMGenDacDeinit(void);
#endif //USE_GEN || USE_GEN_PWM

#ifdef USE_GEN_PWM
/* PWM generatin timers */
static void MX_TIM1_GEN_PWM_Init(void);
static void MX_TIM3_GEN_PWM_Init(void);
/* DMA update timers */
static void MX_TIM6_GEN_PWM_Init(void);
static void MX_TIM7_GEN_PWM_Init(void);

void TIM_DMA_Reconfig(uint8_t chan);
void TIM_GEN_PWM_PSC_Config(uint16_t pscVal, uint8_t chan);
void TIM_GEN_PWM_ARR_Config(uint16_t arrVal, uint8_t chan);
void TIMGenPwmInit(void);

/* TIM1 and TIM3 enable/disable */
void TIMGenPWMEnable(void);
void TIMGenPWMDisable(void);
void PWMGeneratingEnable(void);
void PWMGeneratingDisable(void);
#endif //USE_GEN_PWM

#ifdef USE_COUNTER
typedef enum{
	false = 0,
	true = 1
} bool;

extern TIM_HandleTypeDef htim2;
extern TIM_HandleTypeDef htim4;
extern DMA_HandleTypeDef hdma_tim2_up;
extern DMA_HandleTypeDef hdma_tim2_ch1;
extern DMA_HandleTypeDef hdma_tim2_ch2_ch4;

void TIM_doubleClockVal(void);

static void MX_TIM4_Init(void);
static void MX_TIM2_ETRorREF_Init(void);
static void MX_TIM2_ICorTI_Init(void);

/* Modes initialization functions */
void TIM_counter_etr_init(void);
void TIM_counter_ref_init(void);
void TIM_counter_ic_init(void);
void TIM_counter_ti_init(void);

void TIM_etr_deinit(void);
void TIM_ref_deinit(void);
void TIM_ic_deinit(void);
void TIM_ti_deinit(void);

void TIM_ETR_Start(void);
void TIM_REF_Start(void);
void TIM_IC_Start(void);
void TIM_TI_Start(void);

void TIM_ETR_Stop(void);
void TIM_REF_Stop(void);
void TIM_IC_Stop(void);
void TIM_TI_Stop(void);

/* counter specific */
void TIM_ETRP_Config(double freq);
void TIM_IC1PSC_Config(double freq);
void TIM_IC2PSC_Config(double freq);
void TIM_IC1_PSC_Config(uint8_t prescVal);
void TIM_IC2_PSC_Config(uint8_t prescVal);
void TIM_ARR_PSC_Config(uint16_t arr, uint16_t psc);

void TIM_IC1_RisingFalling(void);
void TIM_IC2_RisingFalling(void);
void TIM_IC1_RisingOnly(void);
void TIM_IC2_RisingOnly(void);
void TIM_IC1_FallingOnly(void);
void TIM_IC2_FallingOnly(void);

void TIM_IC_DutyCycle_Start(void);
void TIM_IC_DutyCycle_Stop(void);
void TIM_IC_DutyCycle_Init(void);
void TIM_IC_DutyCycle_Deinit(void);
void TIM_IC_DutyCycleDmaRestart(void);

void TIM_REF_SecondInputDisable(void);
void TIM_TI_Clear(void);

uint8_t TIM_ETPS_GetPrescaler(void);
uint8_t TIM_IC1PSC_GetPrescaler(void);
uint8_t TIM_IC2PSC_GetPrescaler(void);
uint8_t TIM_GetPrescaler(uint32_t regPrescValue);

bool DMA_TransferComplete(DMA_HandleTypeDef *dmah);
void DMA_Restart(DMA_HandleTypeDef *dmah);

extern void COUNTER_ETR_DMA_CpltCallback(DMA_HandleTypeDef *dmah);	
extern void COUNTER_IC1_DMA_CpltCallback(DMA_HandleTypeDef *dmah);
extern void COUNTER_IC2_DMA_CpltCallback(DMA_HandleTypeDef *dmah);
#endif // USE_COUNTER

	 
#ifdef __cplusplus
}
#endif
#endif /*__ tim_H */

/**
  * @}
  */

/**
  * @}
  */

/************************ (C) COPYRIGHT STMicroelectronics *****END OF FILE****/
