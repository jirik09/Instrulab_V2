/*
  *****************************************************************************
  * @file    counter.c
  * @author  Y3288231
  * @date    June 3, 2017
  * @brief   This file contains counter functions
  ***************************************************************************** 
*/ 

// Includes ===================================================================
	#ifdef USE_COUNTER
#include "cmsis_os.h"
#include "mcu_config.h"
#include "comms.h"
#include "counter.h"
#include "tim.h"


// External variables definitions =============================================
xQueueHandle counterMessageQueue;
void clearAvgBuffer(void);
void counterInitETR(void);
void counterInitIC(void);
void counter_deinit(void);

volatile counterTypeDef counter;
extern uint32_t tim2clk, tim4clk;

// Function definitions ========================================================
/**
  * @brief  Oscilloscope task function.
  * task is getting messages from other tasks and takes care about oscilloscope functions
  * @param  Task handler, parameters pointer
  * @retval None
  */
//portTASK_FUNCTION(vScopeTask, pvParameters){	
void CounterTask(void const *argument)
{
	counterMessageQueue = xQueueCreate(5, 20);
	char message[20];
	
	while(1){
		xQueueReceive(counterMessageQueue, message, portMAX_DELAY);
		if(message[0]=='1'){
				counterInitETR();
		}else if(message[0]=='2'){

		}
//		else if(message[0]=='3'){ //invalidate
//			if(generator.state==GENERATOR_IDLE){
//				
//			}
//		}else if(message[0]=='4'){ //start
//			if(generator.state==GENERATOR_IDLE){
//				genInit();
//				GeneratingEnable();
//				generator.state=GENERATOR_RUN;
//			}

//		}else if(message[0]=='5'){ //stop
//			if(generator.state==GENERATOR_RUN){
//				GeneratingDisable();
//				generator.state=GENERATOR_IDLE;
//			}
//		}

	}
}

void counterSetMode(uint8_t mode){
	switch(mode){
		case ETR:
			xQueueSendToBack(messageQueue, "1SetEtrMode", portMAX_DELAY);
			break;
		case IC:
			xQueueSendToBack(messageQueue, "2SetIcMode", portMAX_DELAY);
			break;
	}
}

void counterInitETR(void){
	counter_deinit();
	counter.state = COUNTER_ETR;
	TIM_counter_etr_init();
}

void counterInitIC(void){
	counter_deinit();
	counter.state = COUNTER_IC;
	TIM_counter_ic_init();
}

void counter_deinit(void){
	switch(counter.state){
		case COUNTER_ETR:
			TIM_etr_deinit();
			break;
		case COUNTER_IC:
			TIM_ic_deinit();
			break;
		case COUNTER_IDLE:
				/* no hacer nada */
			break;		
	}
}

/**
  * @brief  This function is executed in case of DMA transfer complete event.
	*					DMA transfer complete event is triggered after TIM4 gate time elapses.
  * @param  Pointer to DMA handle structure.
  * @retval None
  */
void COUNTER_ETR_DMA_CpltCallback(DMA_HandleTypeDef *dmah)
{			
	uint32_t arr = TIM4->ARR;
	uint32_t psc = TIM4->PSC;
	uint16_t etps = ((TIM2->SMCR) & 0x3000) >> 12;			/* ETR prescaler register value */
	uint8_t etrPresc;																		/* ETR prescaler real value */
	double gateFreq;
	
	/* Save the real value of ETR prescaler for later calculations */
	switch(etps){
		case 0:
			etrPresc = 1; break;			
		case 1:
			etrPresc = 2;	break;
		case 2:
			etrPresc = 4; break;
		case 3:
			etrPresc = 8; break;
		default: 
			break;
	}
	
	gateFreq = ((double)tim4clk / (double)((arr + 1) * (psc + 1)));						/* TIM4 gating frequency */	
	counter.counterEtr.freq = (double)(counter.counterEtr.buffer * gateFreq * etrPresc);					/* Sampled frequency */
	
	ETRP_Config(counter.counterEtr.freq);
}

/**
  * @brief  This function is used to select the desired ETR prescaler ETPS. (TIM2 should be clocked to 144 MHz - not possible - only 72 MHz)
	* @param  freq: frequency
  * @retval none 
  */
void ETRP_Config(double freq)
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
	* @brief  This function automatically configures ARR and PSC registers of given timer. 
						The configurable overflow period range depends on the "psc" value calculated as:

				psc = mean(sqrt(timclk / ovFreq_max), sqrt(timclk / ovFreq_min))

	* @param  timclk: peripheral clock
	* @param  *tim: pointer to timer structure
	* @param  ovFreq: gives the number of counter (TIMx) overflows..
  * @retval none 
  */
void ARR_PSC_AutoConfig(TIM_HandleTypeDef *tim, uint32_t timclk, double ovFreq)
{	
	uint32_t arr;
	double psc;
	
	if (ovFreq >= 10 && ovFreq <= 100) {									/* min. gate time 00.01 second */
		psc = 3532;
	} else if (ovFreq >= 1 && ovFreq < 10) {
		psc = 5584;
	} else if (ovFreq >= 0.1 && ovFreq < 1){
		psc = 17659;   
	} else if (ovFreq > 0.04 && ovFreq < 0.1) {						/* max. gate time 25.00 second */
		psc = 65536;	
	}
	
	arr = ((double)timclk / ovFreq) / psc;	
	
	tim->Instance->ARR = (uint32_t)(arr - 1); 
	tim->Instance->PSC = (uint32_t)(psc - 1);
}

///**
//  * @brief  Oscilloscope set Default values
//  * @param  None
//  * @retval None
//  */
//void generatorSetDefault(void){
//	generator.bufferMemory=generatorBuffer;
//	for(uint8_t i = 0;i<MAX_DAC_CHANNELS;i++){
//		generator.generatingFrequency[i]=DEFAULT_GENERATING_FREQ;
//		generator.realGenFrequency[i]=DEFAULT_GENERATING_FREQ;
//	}
//	
//	generator.numOfChannles=1;
//	generator.maxOneChanSamples=MAX_GENERATOR_BUFF_SIZE/2;
//	generator.oneChanSamples[0]=MAX_GENERATOR_BUFF_SIZE/2;
//	generator.pChanMem[0]=generatorBuffer;
//	generator.state=GENERATOR_IDLE;
//	generator.DAC_res=DAC_DATA_DEPTH;
//}

//void genInit(void){
//	for(uint8_t i = 0;i<MAX_DAC_CHANNELS;i++){
//		TIM_Reconfig_gen(generator.generatingFrequency[i],i,0);
//		if(generator.numOfChannles>i){
//			DAC_DMA_Reconfig(i,(uint32_t *)generator.pChanMem[i], generator.oneChanSamples[i]);
//		}else{
//			DAC_DMA_Reconfig(i,NULL,0);
//		}
//	}
//}


//uint8_t genSetData(uint16_t index,uint8_t length,uint8_t chan){
//	uint8_t result = GEN_INVALID_STATE;
//	if(generator.state==GENERATOR_IDLE ){
//		if ((index*2+length)/2<=generator.oneChanSamples[chan-1] && generator.numOfChannles>=chan){
//			if(commBufferReadNBytes((uint8_t *)generator.pChanMem[chan-1]+index*2,length)==length && commBufferReadByte(&result)==0 && result==';'){
//				result = 0;
//				xQueueSendToBack(generatorMessageQueue, "3Invalidate", portMAX_DELAY);
//			}else{
//			result = GEN_INVALID_DATA;
//			}
//		}else{
//			result = GEN_OUT_OF_MEMORY;
//		}
//	}
//	return result;
//}

//uint8_t genSetFrequency(uint32_t freq,uint8_t chan){
//	uint8_t result = GEN_TO_HIGH_FREQ;
//	uint32_t realFreq;
//	if(freq<=MAX_GENERATING_FREQ){
//		generator.generatingFrequency[chan-1] = freq;
//		result = TIM_Reconfig_gen(generator.generatingFrequency[chan-1],chan-1,&realFreq);
//		generator.realGenFrequency[chan-1] = realFreq;
//	}
//	return result;
//}

//void genSendRealSamplingFreq(void){
//	xQueueSendToBack(messageQueue, "2SendGenFreq", portMAX_DELAY);
//}

//void genDataOKSendNext(void){
//	xQueueSendToBack(messageQueue, "7GenNext", portMAX_DELAY);
//}

//void genStatusOK(void){
//	xQueueSendToBack(messageQueue, "8GenOK", portMAX_DELAY);
//}


//uint32_t genGetRealSmplFreq(uint8_t chan){
//	return generator.realGenFrequency[chan-1];
//}

//uint8_t genSetLength(uint32_t length,uint8_t chan){
//	uint8_t result=GEN_INVALID_STATE;
//	if(generator.state==GENERATOR_IDLE){
//		uint32_t smpTmp=generator.maxOneChanSamples;
//		if(length<=generator.maxOneChanSamples){
//			generator.oneChanSamples[chan-1]=length;
//			clearGenBuffer();
//			result=0;
//		}else{
//			result = GEN_BUFFER_SIZE_ERR;
//		}
//		xQueueSendToBack(generatorMessageQueue, "3Invalidate", portMAX_DELAY);
//	}
//	return result;
//}



//uint8_t genSetNumOfChannels(uint8_t chan){
//	uint8_t result=GEN_INVALID_STATE;
//	uint8_t chanTmp=generator.numOfChannles;
//	if(generator.state==GENERATOR_IDLE){
//		if(chan<=MAX_DAC_CHANNELS){
//			while(chanTmp>0){
//				if(generator.oneChanSamples[--chanTmp]>MAX_GENERATOR_BUFF_SIZE/2/chan){
//					return GEN_BUFFER_SIZE_ERR;
//				}
//			}
//			generator.numOfChannles=chan;
//			generator.maxOneChanSamples=MAX_GENERATOR_BUFF_SIZE/2/chan;
//			for(uint8_t i=0;i<chan;i++){
//				generator.pChanMem[i]=(uint16_t *)&generatorBuffer[i*generator.maxOneChanSamples];
//			}
//			result=0;
//			xQueueSendToBack(generatorMessageQueue, "3Invalidate", portMAX_DELAY);
//		}
//	}
//	return result;
//}


///**
//  * @brief 	Checks if scope settings doesn't exceed memory
//  * @param  None
//  * @retval err/ok
//  */
////uint8_t validateGenBuffUsage(){
////	uint8_t result=1;
////	uint32_t data_len=generator.maxOneChanSamples;
////	if(generator.DAC_res>8){
////		data_len=data_len*2;
////	}
////	data_len=data_len*generator.numOfChannles;
////	if(data_len<=MAX_GENERATOR_BUFF_SIZE){
////		result=0;
////	}
////	return result;
////}

///**
//  * @brief 	Clears generator buffer
//  * @param  None
//  * @retval None
//  */
//void clearGenBuffer(void){
//	for(uint32_t i=0;i<MAX_GENERATOR_BUFF_SIZE/2;i++){
//		generatorBuffer[i]=0;
//	}
//}


//void genSetOutputBuffer(void){
//	DACSetOutputBuffer();
//}

//void genUnsetOutputBuffer(void){
//	DACUnsetOutputBuffer();
//}

//uint8_t genSetDAC(uint16_t chann1,uint16_t chann2){
//	uint8_t result=0;
//	if(generator.state==GENERATOR_IDLE){
//		for(uint8_t i = 0;i<MAX_DAC_CHANNELS;i++){
//			result+=genSetLength(1,i+1);
//		}
//		result+=genSetNumOfChannels(MAX_DAC_CHANNELS);
//	}
//	if(MAX_DAC_CHANNELS>0){
//		*generator.pChanMem[0]=chann1;
//		result+=genSetFrequency(100,1);
//	}
//	if(MAX_DAC_CHANNELS>1){
//		*generator.pChanMem[1]=chann2;
//		result+=genSetFrequency(100,2);
//	}
//	genStart();	

//	
//	return result;
//}
/**
  * @brief  Start scope sampling
  * @param  None
  * @retval None
  */
//void genStart(void){
//	xQueueSendToBack(generatorMessageQueue, "4Start", portMAX_DELAY);
//}

///**
//  * @brief  Stop scope sampling
//  * @param  None
//  * @retval None
//  */
//void genStop(void){
//	xQueueSendToBack(generatorMessageQueue, "5Stop", portMAX_DELAY);
//}

	#endif //USE_COUNTER

