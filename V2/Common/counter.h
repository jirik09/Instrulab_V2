/*
  *****************************************************************************
  * @file    counter.h
  * @author  Y3288231
  * @date    June 3, 2017
  * @brief   This file contains definitions and prototypes of counter functions
  ***************************************************************************** 
*/ 
#include <stdint.h>

#ifdef USE_COUNTER
#ifndef COUNTER_H_
#define COUNTER_H_

#define IC12_BUFFER_SIZE	2

void counterSetMode(uint8_t mode);

typedef enum{
	ETR = 0,
	IC
}counterMode;

typedef enum{
	COUNTER_IDLE = 0,
	COUNTER_ETR,
	COUNTER_IC
}counterState;

typedef struct{
	uint32_t buffer;
	uint16_t arr;	
	uint16_t psc;	
	double freq;
}counterEtrTypeDef;

typedef struct{
	uint32_t ic1buffer[IC12_BUFFER_SIZE];
	uint16_t arr;	
	uint16_t psc;	
	double ic1freq;
	double freqRatio;
}counterIcTypeDef;

typedef struct{
	counterIcTypeDef counterIc;
	counterEtrTypeDef counterEtr;
//	double floatAvgBuffer[CNT_AVG_BUFF_SIZE];
//	double *pFloatAvgBuf;
//	uint8_t sampleToAvg;
	counterState state;
}counterTypeDef;



#endif /* COUNTER_H_ */

#endif //USE_COUNTER

/* End my Friend */
