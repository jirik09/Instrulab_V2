/*
  *****************************************************************************
  * @file    generator.h
  * @author  Y3288231
  * @date    Dec 15, 2014
  * @brief   This file contains definitions and prototypes of oscilloscope functions
  ***************************************************************************** 
*/ 
#if defined(USE_GEN) || defined(USE_GEN_PWM)
#ifndef GENERATOR_H_
#define GENERATOR_H_

#include "stdint.h"

#define DEFAULT_GENERATING_FREQ 1000
#define MAX_DAC_CHANNELS 2

typedef enum{
	GEN_PWM = 0,
	GEN_DAC
}generatorMode;

typedef enum{
	GENERATOR_PWM = 0,
	GENERATOR_DAC
}generatorModeState;

typedef enum{
	GENERATOR_IDLE = 0,
	GENERATOR_RUN
}generatorState;

typedef struct{
	uint16_t *bufferMemory;		
	uint32_t generatingFrequency[MAX_DAC_CHANNELS];
	uint32_t realGenFrequency[MAX_DAC_CHANNELS];
	generatorState state;	
	generatorModeState modeState;
	uint8_t numOfChannles;
	uint16_t *pChanMem[MAX_DAC_CHANNELS];					// buffer itself
	uint16_t oneChanSamples[MAX_DAC_CHANNELS];		// buffer size
	uint32_t maxOneChanSamples;
	uint16_t DAC_res;
	/* PWM generator part of struct */
//	uint16_t genPwmArr[MAX_DAC_CHANNELS];	
//	uint16_t genPwmPsc[MAX_DAC_CHANNELS];	
}generatorTypeDef;


void GeneratorTask(void const *argument);
void generatorSetDefault(void);
void genInit(void);
void genPwmInit(void);
uint8_t genSetData(uint16_t index,uint8_t length,uint8_t chan);
uint8_t genSetFrequency(uint32_t freq,uint8_t chan);
void genSendRealSamplingFreq(void);
void genDataOKSendNext(void);
void genStatusOK(void);
uint32_t genGetRealSmplFreq(uint8_t chan);
uint8_t genSetLength(uint32_t length,uint8_t chan);
uint8_t genSetNumOfChannels(uint8_t chan);
uint8_t genSetDAC(uint16_t chann1,uint16_t chann2);
void genSetOutputBuffer(void);
void genUnsetOutputBuffer(void);
void genStart(void);
void genStop(void);

/* PWM generator function prototypes */
void genSetMode(uint8_t mode);
void generatorSetModePWM(void);
void generatorSetModeDAC(void);
void generator_deinit(void);
void genSetPwmFrequencyPSC(uint32_t pscVal, uint8_t chan);
void genSetPwmFrequencyARR(uint32_t arrVal, uint8_t chan);

extern volatile generatorTypeDef generator;

#endif /* GENERATOR_H_ */

#endif // USE_GEN || USE_GEN_PWM




