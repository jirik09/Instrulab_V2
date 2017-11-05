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

/* Enums */
typedef enum{
	LOGA_DISABLED = 0,
	LOGA_ENABLED
}stateTypeDef;

/* Structs */
typedef struct{		
	stateTypeDef state;
}logAnlysTypeDef;

// Externs ===========================================================
extern volatile logAnlysTypeDef logAnlys;

// Functions Prototypes ==============================================
void LogAnlysTask(void const *argument);

void logAnlysSendInit(void);
void logAnlysSendDeinit(void);
void logAnlysSendStart(void);
void logAnlysSendStop(void);

void logAnlysInit(void);
void logAnlysDeinit(void);
void logAnlysStart(void);
void logAnlysStop(void);

void logAnlysSetDefault(void);


#endif /* LOG_ANLYS__H_ */

#endif //USE_LOG_ANLYS

