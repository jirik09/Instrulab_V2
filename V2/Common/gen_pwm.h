/*
  *****************************************************************************
  * @file    gen_pwm.h
  * @author  HeyBirdie
  * @date    Aug 1, 2017
  * @brief   This file contains definitions and prototypes of pwm generator functions
  ***************************************************************************** 
*/ 

#ifdef USE_GEN_PWM
#ifndef GEN_PWM_H_
#define GEN_PWM_H_

/* Includes */
#include <stdint.h>
#include "stm32f3xx_hal.h"

/* Structs */
typedef struct{
	uint32_t dutyCycle;
}genPwmTypeDef;

// Defines ===========================================================
extern volatile genPwmTypeDef genPwm;

// Functions =========================================================
void GenPwmTask(void const *argument);

void genPwmSendStart(void);
void genPwmSendStop(void);
void genPwmStart(void);
void genPwmStop(void);


#endif /* GEN_PWM_H_ */

#endif //USE_GEN_PWM

