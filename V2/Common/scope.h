/*
  *****************************************************************************
  * @file    scope.h
  * @author  Y3288231
  * @date    Dec 15, 2014
  * @brief   This file contains definitions and prototypes of oscilloscope functions
  ***************************************************************************** 
*/ 
#ifdef USE_SCOPE

#ifndef SCOPE_H_
#define SCOPE_H_

// Constant definitions =======================================================
#define AUTO_TRIG_MAX_WAIT 4
#define NOISE_REDUCTION 16

#define SCOPE_DEFAULT_SAMPLING_FREQ 1000
#define SCOPE_DEFAULT_TRIGGER_LEVEL 0x8000
#define SCOPE_DEFAULT_PRETRIGGER 0x8000
#define SCOPE_DEFAULT_DATA_LEN 100
#define SCOPE_DEFAULT_TRIGGER TRIG_AUTO
#define SCOPE_DEFAULT_TRIG_EDGE EDGE_RISING
#define SCOPE_DEFAULT_ADC_RES 12

// Types definitions ==========================================================

typedef enum{
	TRIG_NORMAL = 0,
	TRIG_AUTO,
	TRIG_SINGLE
} scopeTriggerMode;

typedef enum{
	EDGE_RISING = 0,
	EDGE_FALLING
} scopeTriggerEdge;

typedef enum{
	SCOPE_IDLE = 0,
	SCOPE_SAMPLING_WAITING,
	SCOPE_SAMPLING_TRIGGER_WAIT,
	SCOPE_SAMPLING,
	SCOPE_DATA_SENDING,	
	SCOPE_DONE,
	SCOPE_WAIT_FOR_RESTART,
	SCOPE_ERR
}scopeState;

typedef struct{
	uint32_t samplingFrequency; 
	uint32_t realSamplingFreq;
	uint32_t samplesToSend;
	scopeTriggerEdge triggerEdge;	
	scopeTriggerMode triggerMode;	
	uint16_t triggerLevel;					//65535 is 100%
	uint16_t pretrigger;						//65535 is 100%
	uint16_t adcRes;
	uint16_t adcLevels;
}scopeSettings;

typedef struct{
	uint8_t *bufferMemory;		
	uint32_t triggerIndex;		
	scopeSettings settings;
	scopeState state;	
	uint8_t numOfChannles;
	uint16_t *pChanMem[MAX_ADC_CHANNELS];
	uint8_t adcChannel[MAX_ADC_CHANNELS];
	uint32_t oneChanMemSize;
	uint32_t oneChanSamples;
	uint8_t triggerChannel;
} scopeTypeDef;

// Exported variables =========================================================
extern xQueueHandle scopeMessageQueue;
//extern uint16_t scopeBuffer[MAX_SCOPE_BUFF_SIZE/2]; 

// Exported functions =========================================================
void ScopeTask(void const *argument);
void ScopeTriggerTask(void const *argument) ;

void scopeInit(void); 
void scopeSetDefault(void); 
void updateTrigger(void);
void scopeStartMeasure(void);
void scopeSetSettings(scopeSettings *psSettings);

void scopeSetSamplingFrequency(uint32_t sSamplingFrequency);
void scopeSetTriggerEdge(scopeTriggerEdge sTrigEdge);
void scopeSetTriggerMode(scopeTriggerMode sTrigMode);
void scopeSetTriggerLevel(uint16_t sTrigLevel);
void scopeSetPretrigger(uint16_t sPretrig);

void sendData(void);
uint8_t GetNumOfChannels (void);
uint16_t *getDataPointer (uint8_t chan);
uint32_t getOneChanMemSize(void);
uint32_t getOneChanMemSamples(void);
uint32_t getTriggerIndex(void);
uint32_t getSamples(void);
uint16_t getADCRes(void);
uint16_t getPretrigger(void);
scopeState getScopeState(void);

void scopeSetTriggerMode(scopeTriggerMode mode);
void scopeSetTriggerEdge(scopeTriggerEdge edge);
uint8_t scopeSetDataDepth(uint16_t depth);
uint8_t scopeSetSamplingFreq(uint32_t freq);
void scopeSetTrigLevel(uint16_t level);
void scopeSetPretrigger(uint16_t pretrig);
uint8_t scopeSetNumOfSamples(uint32_t len);
uint8_t scopeSetTrigChannel(uint8_t chan);
uint32_t scopeGetRealSmplFreq(void);
uint8_t scopeSetNumOfChannels(uint8_t chan);
uint8_t scopeSetTrigChannel(uint8_t chan);
uint8_t scopeSetADCInputChannel(uint8_t adc, uint8_t chann);

const int16_t* scopeGetRanges(uint8_t * len);
void scopeRestart(void);
void scopeStart(void);
void scopeStop(void);

#endif /* SCOPE_H_ */
#endif //USE_SCOPE

