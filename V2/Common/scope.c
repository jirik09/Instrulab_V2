/*
  *****************************************************************************
  * @file    scope.c
  * @author  Y3288231
  * @date    Dec 15, 2014
  * @brief   This file contains oscilloscope functions
  ***************************************************************************** 
*/ 

// Includes ===================================================================
#ifdef USE_SCOPE

#include "cmsis_os.h"
#include "mcu_config.h"
#include "comms.h"
#include "scope.h"
#include "adc.h"
#include "tim.h"

// External variables definitions =============================================
const int16_t RANGES[8] = {RANGE_1_LOW,RANGE_1_HI,RANGE_2_LOW,RANGE_2_HI,RANGE_3_LOW,RANGE_3_HI,RANGE_4_LOW,RANGE_4_HI,};

xQueueHandle scopeMessageQueue;

uint8_t scopeBuffer[MAX_SCOPE_BUFF_SIZE+MAX_ADC_CHANNELS*SCOPE_BUFFER_MARGIN]; 
uint8_t blindBuffer[MAX_ADC_CHANNELS];
static uint32_t triggerIndex;
static uint16_t triggerLevel;
static uint32_t samplesToStop=0;
static uint32_t samplesToStart=0;

static xSemaphoreHandle scopeMutex ;
static uint32_t writingIndex=0;
static uint32_t lastWritingIndex=0;
static volatile scopeTypeDef scope;


uint32_t actualIndex = 0;
uint16_t data = 0;
uint32_t samplesTaken = 0;
uint32_t totalSmpTaken = 0;
uint32_t SmpBeforeTrig=0;
uint32_t SmpAfterTrig=0;

// Function prototypes ========================================================
uint16_t samplesPassed(uint16_t dataRemain, uint16_t lastDataRemain);
uint8_t validateBuffUsage(void);

// Function definitions =======================================================
/**
  * @brief  Oscilloscope task function.
  * task is getting messages from other tasks and takes care about oscilloscope functions
  * @param  Task handler, parameters pointer
  * @retval None
  */
//portTASK_FUNCTION(vScopeTask, pvParameters){	
void ScopeTask(void const *argument){
	
	//Build error on lines below? Lenght of Pin strings must be 4 chars long!!!
	CASSERT(sizeof(SCOPE_CH1_PIN_STR)==5);
	CASSERT(sizeof(SCOPE_CH2_PIN_STR)==5);
	CASSERT(sizeof(SCOPE_CH3_PIN_STR)==5);
	CASSERT(sizeof(SCOPE_CH4_PIN_STR)==5);
	
	scopeMessageQueue = xQueueCreate(5, 20);
	scopeMutex = xSemaphoreCreateRecursiveMutex();
	scopeSetDefault();
	char message[20];
	
	while(1){
		xQueueReceive(scopeMessageQueue, message, portMAX_DELAY);
		xSemaphoreTakeRecursive(scopeMutex, portMAX_DELAY);
		///commsSendString("SCP_Run\r\n");
		if(message[0] == '2' && scope.state != SCOPE_IDLE){ //data was send. Actualisation of scope State and/or rerun
			//if(scope.settings.triggerMode == TRIG_SINGLE){
			//	scope.state = SCOPE_DONE;
			//}else{  //TRIG_NORMAL || TRIG_AUTO (rerun)
				///commsSendString("SCP_ScopeWaitRes\r\n");
				//scopeInit();
				scope.state = SCOPE_WAIT_FOR_RESTART;
			//}
		}else if(message[0] == '3'){  //settings has been changed
			if(scope.state == SCOPE_DONE || scope.state == SCOPE_IDLE){
			}else{
				///commsSendString("SCP_ScopeReinit\r\n");
				samplingDisable();
				scopeInit();
				if(scope.state!=SCOPE_WAIT_FOR_RESTART && scope.state!=SCOPE_DATA_SENDING){
					scope.state=SCOPE_SAMPLING_WAITING;
					samplesTaken=0;
					samplingEnable();
				}
			}	
		}else if (message[0] == '4' && scope.state != SCOPE_SAMPLING_WAITING && scope.state != SCOPE_SAMPLING_TRIGGER_WAIT && scope.state != SCOPE_SAMPLING && scope.state != SCOPE_DATA_SENDING){ //enable sampling
			scopeInit();
			scope.state=SCOPE_SAMPLING_WAITING;
			samplingEnable();
			///commsSendString("SCP_ScopeStart\r\n");
			xQueueSendToBack(messageQueue, "SMPL", portMAX_DELAY); 
		}else if (message[0] == '5'){//disable sampling
			samplingDisable();
			scope.state = SCOPE_IDLE;
			///commsSendString("SCP_ScopeStop\r\n");
		}else if (message[0] == '6' && scope.state==SCOPE_WAIT_FOR_RESTART ){
			samplingEnable();
			scope.state=SCOPE_SAMPLING_WAITING;
			///commsSendString("SCP_ScopeRestart\r\n");
		}
		xSemaphoreGiveRecursive(scopeMutex);
	}
}

/**
  * @brief  Oscilloscope trigger finding task function.
  * Task is finding trigger edge when osciloscope is running
  * @param  Task handler, parameters pointer
  * @retval None
  */
//portTASK_FUNCTION(vScopeTriggerTask, pvParameters) {
void ScopeTriggerTask(void const *argument) {
//	uint32_t actualIndex = 0;
//	uint16_t data = 0;
//	uint32_t samplesTaken = 0;
//	uint32_t totalSmpTaken = 0;
//	uint32_t SmpBeforeTrig=0;
//	uint32_t SmpAfterTrig=0;
	

	while(1){
		if(scope.state==SCOPE_SAMPLING_WAITING || scope.state==SCOPE_SAMPLING_TRIGGER_WAIT || scope.state==SCOPE_SAMPLING){
			/////commsSendString("TRIG_Run\r\n");
			xSemaphoreTakeRecursive ( scopeMutex , portMAX_DELAY );
			lastWritingIndex = writingIndex;
			writingIndex = scope.oneChanSamples - DMA_GetCurrDataCounter(scope.triggerChannel);
			actualIndex = (scope.oneChanSamples + writingIndex - 1) % scope.oneChanSamples;
			
			//wait for right level before finding trigger (lower level then trigger level for rising edge, higher level for falling edge)
			if(scope.state == SCOPE_SAMPLING_WAITING){ 
				if(scope.settings.adcRes<=8){
					data=*(scope.pChanMem[scope.triggerChannel-1]+actualIndex/2);
					data = data & 0x00ff;
				}else{
					data=*(scope.pChanMem[scope.triggerChannel-1]+actualIndex);
				}
				////data = scopeBuffer[actualIndex];
				updateTrigger();
				samplesTaken += samplesPassed(writingIndex,lastWritingIndex);	
				//start finding right level before trigger (cannot start to find it earlier because pretrigger was not taken yet)
				if (samplesTaken > samplesToStart)    
					if((scope.settings.triggerEdge == EDGE_RISING && data + NOISE_REDUCTION < triggerLevel) 
					|| (scope.settings.triggerEdge == EDGE_FALLING && data - NOISE_REDUCTION > triggerLevel)
					|| (scope.settings.triggerMode == TRIG_AUTO && samplesTaken > (scope.settings.samplesToSend * AUTO_TRIG_WAIT_NORMAL))
					|| (scope.settings.triggerMode == TRIG_AUTO_FAST && samplesTaken > (scope.settings.samplesToSend * AUTO_TRIG_WAIT_FAST))  ){ //skip waiting for trigger in case of TRIG_AUTO
						scope.state = SCOPE_SAMPLING_TRIGGER_WAIT;
						xQueueSendToBack(messageQueue, "SMPL", portMAX_DELAY);
					}
					
			//finding for trigger
			}else if(scope.state == SCOPE_SAMPLING_TRIGGER_WAIT){
				samplesTaken += samplesPassed(writingIndex,lastWritingIndex);	
				if(scope.settings.adcRes<=8){
					data=*(scope.pChanMem[scope.triggerChannel-1]+actualIndex/2);
					data = data & 0x00ff;
				}else{
					data=*(scope.pChanMem[scope.triggerChannel-1]+actualIndex);
				}
				////data = scopeBuffer[actualIndex];
				updateTrigger();
				if((scope.settings.triggerEdge == EDGE_RISING && data > triggerLevel) 
				|| (scope.settings.triggerEdge == EDGE_FALLING && data < triggerLevel)
				|| (scope.settings.triggerMode == TRIG_AUTO && samplesTaken > (scope.settings.samplesToSend * AUTO_TRIG_WAIT_NORMAL))
				|| (scope.settings.triggerMode == TRIG_AUTO_FAST && samplesTaken > (scope.settings.samplesToSend * AUTO_TRIG_WAIT_FAST))  ){
					totalSmpTaken = samplesTaken;
					samplesTaken = 0;
					scope.state = SCOPE_SAMPLING;
					triggerIndex = actualIndex;
					xQueueSendToBack(messageQueue, "TRIG", portMAX_DELAY);
				}
				
			//sampling after trigger event
			}else if(scope.state == SCOPE_SAMPLING){
				samplesTaken += samplesPassed(writingIndex, lastWritingIndex);	
			
			
				//sampling is done
				if(scope.state == SCOPE_SAMPLING && samplesTaken >= samplesToStop){
					samplingDisable();
					
					//finding exact trigger
					if (scope.settings.triggerMode != TRIG_AUTO && scope.settings.triggerMode != TRIG_AUTO_FAST){	
						if(scope.settings.adcRes>8){
							if(scope.settings.triggerEdge == EDGE_RISING){
								while(*(scope.pChanMem[scope.triggerChannel-1]+triggerIndex) > triggerLevel){
									triggerIndex--;
								}
							}else{
								while(*(scope.pChanMem[scope.triggerChannel-1]+triggerIndex) < triggerLevel){
									triggerIndex--;
								}
							}
						}else{							
							if(scope.settings.triggerEdge == EDGE_RISING){
								while(*((uint8_t *)scope.pChanMem[scope.triggerChannel-1]+triggerIndex) > triggerLevel){
									triggerIndex--;
								}
							}else{
								while(*((uint8_t *)scope.pChanMem[scope.triggerChannel-1]+triggerIndex) < triggerLevel){
									triggerIndex--;
								}
							}
						}
						triggerIndex++;
					}
					
					scope.triggerIndex = triggerIndex;
					scope.state = SCOPE_DATA_SENDING;
					SmpBeforeTrig = totalSmpTaken;
					SmpAfterTrig=samplesTaken;
					if(SmpBeforeTrig+SmpAfterTrig>0xfffff){
						samplesTaken=0;
					}
					samplesTaken = 0;
					totalSmpTaken = 0;
					//give message that data is ready
					/////commsSendString("TRIG_Sampled\r\n");
					xQueueSendToBack (messageQueue, "1DataReady__", portMAX_DELAY);
				}
			}
			xSemaphoreGiveRecursive(scopeMutex);
		}else{
			taskYIELD();
		}
	}
}

/**
  * @brief 	Returns number of samples between indexes.
  * @param  actual index, last index
  * @retval None
  */
uint16_t samplesPassed(uint16_t index, uint16_t lastIndex){
	uint16_t result=0;
	if(index < lastIndex){
		result= index + scope.oneChanSamples - lastIndex;
	}else{
		result= index - lastIndex;
	}	
	return result;
}

/**
  * @brief 	Checks if scope settings doesn't exceed memory
  * @param  None
  * @retval err/ok
  */
uint8_t validateBuffUsage(){
	uint8_t result=1;
	uint32_t data_len=scope.settings.samplesToSend;
	if(scope.settings.adcRes>8){
		data_len=data_len*2;
	}
	data_len=data_len*scope.numOfChannles;
	if(data_len<=MAX_SCOPE_BUFF_SIZE){
		result=0;
	}
	return result;
}

/**
  * @brief  Oscilloscope initialisation.
  * @param  None
  * @retval None
  */
void scopeInit(void){
	writingIndex = 0;
	uint32_t realfreq=0;
	
	ADC_DMA_Stop();
	
	TIM_Reconfig_scope(scope.settings.samplingFrequency,&realfreq);
	ADC_set_sampling_time(realfreq);	
	
	for(uint8_t i = 0;i<MAX_ADC_CHANNELS;i++){
		if(scope.numOfChannles>i){
			ADC_DMA_Reconfig(i,(uint32_t *)scope.pChanMem[i], scope.oneChanSamples);
		}
	}
	scope.settings.realSamplingFreq=realfreq;
}

/**
  * @brief  Update trigger level and pretriger values (can be changed on the fly)
  * @param  None
  * @retval None
  */
void updateTrigger(void){
	triggerLevel = (scope.settings.triggerLevel * scope.settings.adcLevels) >> 16;
	samplesToStop = ((scope.settings.samplesToSend * (UINT16_MAX - scope.settings.pretrigger)) >> 16)+1;
	samplesToStart = (scope.settings.samplesToSend * (scope.settings.pretrigger)) >> 16;
}

/**
  * @brief  Oscilloscope set Default values
  * @param  None
  * @retval None
  */
void scopeSetDefault(void){
	scope.bufferMemory = scopeBuffer;
	scope.settings.samplingFrequency = SCOPE_DEFAULT_SAMPLING_FREQ;
	scope.settings.triggerEdge = SCOPE_DEFAULT_TRIG_EDGE;
	scope.settings.triggerMode = SCOPE_DEFAULT_TRIGGER;
	scope.settings.triggerLevel = SCOPE_DEFAULT_TRIGGER_LEVEL;
	scope.settings.pretrigger = SCOPE_DEFAULT_PRETRIGGER;
	scope.settings.adcRes = SCOPE_DEFAULT_ADC_RES;
	scope.settings.adcLevels=pow(2,SCOPE_DEFAULT_ADC_RES);
	scope.settings.samplesToSend = SCOPE_DEFAULT_DATA_LEN;
	scope.pChanMem[0] = (uint16_t*)scopeBuffer;
	scope.oneChanMemSize = MAX_SCOPE_BUFF_SIZE+SCOPE_BUFFER_MARGIN;
	if(scope.settings.adcRes>8){
		scope.oneChanSamples = scope.oneChanMemSize/2;
	}else{
		scope.oneChanSamples = scope.oneChanMemSize;
	}
	scope.numOfChannles = 1;
	scope.triggerChannel = 1;
}

/**
  * @brief  Getter function of pointer for data buffer.
  * @param  None
  * @retval pointer to buffer
  */
uint8_t GetNumOfChannels (void){
	return scope.numOfChannles;
}

/**
  * @brief  Getter function of pointer for data buffer.
  * @param  None
  * @retval pointer to buffer
  */
uint16_t *getDataPointer(uint8_t chan){
	return scope.pChanMem[chan];
}

/**
  * @brief  Getter function of one channel memory size.
  * @param  None
  * @retval mem size
  */
uint32_t getOneChanMemSize(){
	return scope.oneChanMemSize;
}

/**
  * @brief  Getter function of one channel samples.
  * @param  None
  * @retval mem size
  */
uint32_t getOneChanMemSamples(){
	return scope.oneChanSamples;
}

/**
  * @brief  Getter function of trigger index.
  * @param  None
  * @retval pointer to sample where trigger occured
  */
uint32_t getTriggerIndex(void){
	return triggerIndex;
}

/**
  * @brief  Getter function of data length.
  * @param  None
  * @retval data length
  */
uint32_t getSamples(void){
	return scope.settings.samplesToSend;
}

/**
  * @brief  Getter function of ADC resolution.
  * @param  None
  * @retval ADC resolution
  */
uint16_t getADCRes(void){
	return scope.settings.adcRes;
}

/**
  * @brief  Getter function of pretrigger.
  * @param  None
  * @retval pretrigger value
  */
uint16_t getPretrigger(void){
	return scope.settings.pretrigger;
}

/**
  * @brief  Getter for oscilloscope state.
  * @param  None
  * @retval scope state
  */
scopeState getScopeState(void){
	return scope.state;
}

/**
  * @brief  Setter for trigger mode
  * @param  Scope Trigger mode
  * @retval None
  */
void scopeSetTriggerMode(scopeTriggerMode mode){
	xSemaphoreTakeRecursive(scopeMutex, portMAX_DELAY);
	scope.settings.triggerMode = mode;
	xSemaphoreGiveRecursive(scopeMutex);
}

/**
  * @brief  Setter for trigger edge
  * @param  Scope Trigger edge
  * @retval None
  */
void scopeSetTriggerEdge(scopeTriggerEdge edge){
	xSemaphoreTakeRecursive(scopeMutex, portMAX_DELAY);
	scope.settings.triggerEdge = edge;
	xSemaphoreGiveRecursive(scopeMutex);
	xQueueSendToBack(scopeMessageQueue, "3Invalidate", portMAX_DELAY); //cannot change this property on the on the fly (scope must re-init)
}

/**
  * @brief  Setter for ADC resolution
  * @param  ADC resolution 2^N where N is number of bits
  * @retval success/error
  */
uint8_t scopeSetDataDepth(uint16_t res){
	uint8_t result=BUFFER_SIZE_ERR;
	uint8_t resTmp=res;
	xSemaphoreTakeRecursive(scopeMutex, portMAX_DELAY);
	scope.settings.adcRes = res;
	if(validateBuffUsage()){
		scope.settings.adcRes = resTmp;
	}else{
		scope.settings.adcLevels=pow(2,scope.settings.adcRes);
		if(scope.settings.adcRes>8){
			scope.oneChanSamples=scope.oneChanMemSize/2;
		}else{
			scope.oneChanSamples=scope.oneChanMemSize;
		}
		adcSetResolution(res);
		result=0;
	}
	xSemaphoreGiveRecursive(scopeMutex);
	xQueueSendToBack(scopeMessageQueue, "3Invalidate", portMAX_DELAY);
	
	return result;
}

/**
  * @brief  Setter for sampling frequency
  * @param  Samples per second
  * @retval success/error
  */
uint8_t scopeSetSamplingFreq(uint32_t freq){
	uint8_t result=SCOPE_INVALID_SAMPLING_FREQ;
	xSemaphoreTakeRecursive(scopeMutex, portMAX_DELAY);
	
	if (freq<=MAX_SAMPLING_FREQ){
		scope.settings.samplingFrequency = freq;
	
	}else{
		scope.settings.samplingFrequency=getMaxScopeSamplingFreq(scope.settings.adcRes);
	}
	result=0;
	xSemaphoreGiveRecursive(scopeMutex);
	xQueueSendToBack(scopeMessageQueue, "3Invalidate", portMAX_DELAY);
	
	return result;
}

/**
  * @brief  Setter for trigger level
  * @param  signal level to trigger (0xFFFF is 100%)
  * @retval None
  */
void scopeSetTrigLevel(uint16_t level){
	xSemaphoreTakeRecursive(scopeMutex, portMAX_DELAY);
	scope.settings.triggerLevel = level;
	xSemaphoreGiveRecursive(scopeMutex);
}

/**
  * @brief  Setter for pretrigger
  * @param  fraction of buffer before trigger event (0xFFFF is 100%)
  * @retval None
  */
void scopeSetPretrigger(uint16_t pretrig){
	xSemaphoreTakeRecursive(scopeMutex, portMAX_DELAY);
	scope.settings.pretrigger = pretrig;
	xSemaphoreGiveRecursive(scopeMutex);
}

/**
  * @brief  Setter for data length
  * @param  flength of data which will be send
  * @retval None
  */
uint8_t scopeSetNumOfSamples(uint32_t smp){
	uint8_t result=BUFFER_SIZE_ERR;
	uint32_t smpTmp=scope.settings.samplesToSend;
	xSemaphoreTakeRecursive(scopeMutex, portMAX_DELAY);
	if(smp<scope.oneChanSamples){
		scope.settings.samplesToSend = smp;
		result=0;
	}
	xSemaphoreGiveRecursive(scopeMutex);
	xQueueSendToBack(scopeMessageQueue, "3Invalidate", portMAX_DELAY);
	return result;
}

/**
  * @brief  Setter for number of channels
  * @param  number of channels
  * @retval success/error
  */
uint8_t scopeSetNumOfChannels(uint8_t chan){
	uint8_t result=BUFFER_SIZE_ERR;
	uint8_t chanTmp=scope.numOfChannles;
	xSemaphoreTakeRecursive(scopeMutex, portMAX_DELAY);
	if(chan<=MAX_ADC_CHANNELS){
		scope.numOfChannles=chan;
		if(validateBuffUsage()){
			scope.numOfChannles = chanTmp;
		}else{
				scope.oneChanMemSize=MAX_SCOPE_BUFF_SIZE/chan+SCOPE_BUFFER_MARGIN-(MAX_SCOPE_BUFF_SIZE/chan+SCOPE_BUFFER_MARGIN)%2;
				if(scope.settings.adcRes>8){
					scope.oneChanSamples=scope.oneChanMemSize/2;
				}else{
					scope.oneChanSamples=scope.oneChanMemSize;
				}
				for(uint8_t i=0;i<chan;i++){
				scope.pChanMem[i]=(uint16_t *)(&scopeBuffer[i*scope.oneChanMemSize]+(uint32_t)(&scopeBuffer[i*scope.oneChanMemSize])%2);
			}
			result=0;
		}
		xSemaphoreGiveRecursive(scopeMutex);
		xQueueSendToBack(scopeMessageQueue, "3Invalidate", portMAX_DELAY);
	}
	return result;
}

/**
  * @brief  Setter for trigger channel
  * @param  trigger channel
  * @retval None
  */
uint8_t scopeSetTrigChannel(uint8_t chan){
	uint8_t result=SCOPE_INVALID_TRIGGER_CHANNEL;
	if(chan<=MAX_ADC_CHANNELS){
		xSemaphoreTakeRecursive(scopeMutex, portMAX_DELAY);
		scope.triggerChannel=chan;
		result=0;
		xSemaphoreGiveRecursive(scopeMutex);
		xQueueSendToBack(scopeMessageQueue, "3Invalidate", portMAX_DELAY);
	}
	return result;
}

uint32_t scopeGetRealSmplFreq(){
	return scope.settings.realSamplingFreq;
}


/**
  * @brief  sets ADC channel to sample
  * @param  ADC number, Channel number
  * @retval error
  */
uint8_t scopeSetADCInputChannel(uint8_t adc, uint8_t chann){
	uint8_t result = SCOPE_INVALID_ADC_CHANNEL;
	if(adc < MAX_ADC_CHANNELS && chann < NUM_OF_ANALOG_INPUTS[adc]){
		xSemaphoreTakeRecursive(scopeMutex, portMAX_DELAY);
		scope.adcChannel[adc] = chann;
		adcSetInputChannel(adc, chann);
		result = 0;
		xSemaphoreGiveRecursive(scopeMutex);
		xQueueSendToBack(scopeMessageQueue, "3Invalidate", portMAX_DELAY);
	}
	return result;
}

/**
  * @brief  sets ADC channel to default
  * @param  ADC number, Channel number
  * @retval error
  */
uint8_t scopeSetADCInputChannelDefault(){
	uint8_t result = SCOPE_INVALID_ADC_CHANNEL;
	xSemaphoreTakeRecursive(scopeMutex, portMAX_DELAY);
	for(uint8_t i = 0;i<MAX_ADC_CHANNELS;i++){
		scope.adcChannel[i] = ANALOG_DEFAULT_INPUTS[i];
		adcSetInputChannel(i, ANALOG_DEFAULT_INPUTS[i]);
		result = 0;
	}
	xSemaphoreGiveRecursive(scopeMutex);
	xQueueSendToBack(scopeMessageQueue, "3Invalidate", portMAX_DELAY);
	return result;
}

/**
  * @brief  sets ADC channel to default
  * @param  ADC number, Channel number
  * @retval error
  */
uint8_t scopeSetADCInputChannelVref(){
	uint8_t result = SCOPE_INVALID_ADC_CHANNEL;
	xSemaphoreTakeRecursive(scopeMutex, portMAX_DELAY);
	for(uint8_t i = 0;i<MAX_ADC_CHANNELS;i++){
		scope.adcChannel[i] = ANALOG_VREF_INPUTS[i];
		adcSetInputChannel(i, ANALOG_VREF_INPUTS[i]);
		result = 0;
	}
	xSemaphoreGiveRecursive(scopeMutex);
	xQueueSendToBack(scopeMessageQueue, "3Invalidate", portMAX_DELAY);
	return result;
}


/**
  * @brief  return pointer to dafinition of ranges
  * @param  int16 pointer
  * @retval None
  */
const int16_t* scopeGetRanges(uint8_t * len){
	*len=sizeof(RANGES);
	return RANGES;
	}

/**
  * @brief  Restart scope sampling
  * @param  None
  * @retval None
  */
void scopeRestart(void){
	xQueueSendToBack(scopeMessageQueue, "6Restart", portMAX_DELAY);
}

/**
  * @brief  Start scope sampling
  * @param  None
  * @retval None
  */
void scopeStart(void){
	xQueueSendToBack(scopeMessageQueue, "4Start", portMAX_DELAY);
}

/**
  * @brief  Stop scope sampling
  * @param  None
  * @retval None
  */
void scopeStop(void){
	xQueueSendToBack(scopeMessageQueue, "5Stop", portMAX_DELAY);
}


#endif //USE_SCOPE



