/*
  *****************************************************************************
  * @file    logic_analyzer.h
  * @author  HeyBirdie
  * @date    Nov 4, 2017
  * @brief   This file contains definitions and prototypes of logic analyzer
						 functions.
  ***************************************************************************** 
*/ 

#ifdef USE_LOG_ANLYS
#ifndef LOG_ANLYS_H_
#define LOG_ANLYS_H_

/* Includes */
#include <stdint.h>
#include "stm32f3xx_hal.h"

// Constant definitions =======================================================
//#define LOG_ANLYS_DEFAULT_SAMPLING_FREQ 	1000
//#define LOG_ANLYS_DEFAULT_PRETRIGGER 			0x8000    //[ms]


/* Enums */
typedef enum{
	LOGA_DISABLED = 0,
	LOGA_ENABLED
}stateTypeDef;

typedef enum{
	LOGA_MODE_AUTO = 0,
	LOGA_MODE_NORMAL = 1,
	LOGA_MODE_SINGLE = 2
}triggerModeTypeDef;

typedef enum{
	TRIG_CHAN1 = 0,
	TRIG_CHAN2
}trigConfigTypeDef;

typedef enum{
	TRIG_EDGE_RISING = 0,
	TRIG_EDGE_FALLING
}trigEdgeTypeDef;

typedef enum{
	TRIG_OCCURRED = 0,
	TRIG_NOT_OCCURRED
}trigOccurrenceTypeDef;

/* Structs */
typedef struct{		
	uint32_t triggerPointer;
	uint32_t preTriggerTime;
	uint16_t *bufferMemory;
	uint16_t samplesNumber;
	
	stateTypeDef state;
	triggerModeTypeDef triggerMode;
	trigConfigTypeDef trigConfig;
	trigEdgeTypeDef trigEdge;
	trigOccurrenceTypeDef trigOccur;
}logAnlysTypeDef;

// Externs ===========================================================
extern volatile logAnlysTypeDef logAnlys;

// Functions Prototypes ==============================================
void LogAnlysTask(void const *argument);

void logAnlysSendInit(void);
void logAnlysSendDeinit(void);
void logAnlysSendStart(void);
void logAnlysSendStop(void);

void logAnlysSendTriggerRising(void);
void logAnlysSendTriggerFalling(void);

void logAnlysInit(void);
void logAnlysDeinit(void);
void logAnlysStart(void);
void logAnlysStop(void);

void logAnlysSetSamplingFreq(uint32_t arrPsc);
void logAnlysSetSamplesNum(uint16_t samplesNum);

void logAnlysSetPosttrigger(uint32_t arrPsc);
void logAnlysSetPretrigger(uint32_t timeInMilliseconds);

void logAnlysSetTriggerChannel(uint32_t chan);
void logAnlysSetTriggerRising(void);
void logAnlysSetTriggerFalling(void);

void logAnlysSetDefault(void);

void logAnlysPeriodElapsedCallback(void);
void triggerEventOccuredCallback(void);	

#endif /* LOG_ANLYS__H_ */

#endif //USE_LOG_ANLYS

