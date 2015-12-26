/*
  *****************************************************************************
  * @file    comms_hal.c
  * @author  Y3288231
  * @date    jan 15, 2014
  * @brief   Hardware abstraction for communication
  ***************************************************************************** 
*/ 

// Includes ===================================================================
#include "cmsis_os.h"
#include "usbd_cdc_if.h"
#include "mcu_config.h"
#include "comms.h"
#include "comms_hal.h"
#include "adc.h"
#include "usb_device.h"
#include "usart.h"



// External variables definitions =============================================

// Function prototypes ========================================================


void commsSend(uint8_t chr){
	#ifdef USE_USB
	if (hUsbDeviceFS.dev_state == USBD_STATE_CONFIGURED){	
		while(CDC_Transmit_FS(&chr,1)!=USBD_OK){
			taskYIELD();
		}
	}else{
		UARTsendChar(chr);
	}
	#else
	UARTsendChar(chr);
	#endif
	
	
}

void commsSendUint32(uint32_t num){
	uint8_t buff[4];
	buff[0]=(uint8_t)(num);
	buff[1]=(uint8_t)(num>>8);
	buff[2]=(uint8_t)(num>>16);
	buff[3]=(uint8_t)(num>>24);
  commsSendBuff(buff, 4);
}

void commsSendBuff(uint8_t *buff, uint16_t len){
	#ifdef USE_USB
	if (hUsbDeviceFS.dev_state == USBD_STATE_CONFIGURED){	
		while(CDC_Transmit_FS(buff,len)!=USBD_OK){
			taskYIELD();
		}
	}else{
		UARTsendBuff((char *)buff,len);
	}
	#else
	UARTsendBuff((char *)buff,len);
	#endif
}
void commsSendString(char *chr){
	uint32_t i = 0;
	char * tmp=chr;
	while(*(tmp++)){i++;}
	#ifdef USE_USB
	if (hUsbDeviceFS.dev_state == USBD_STATE_CONFIGURED){	
		while(CDC_Transmit_FS((uint8_t*)chr,i)!=USBD_OK){
			taskYIELD();
		}
	}else{
		UARTsendBuff(chr,i);
	}
	#else
	UARTsendBuff(chr,i);
	#endif

}


#ifdef USE_USB
void commsRecieveUSB(uint8_t chr){
	if (hUsbDeviceFS.dev_state == USBD_STATE_CONFIGURED){	
		commInputByte(chr);
	}
}
#endif //USE_USB

void commsRecieveUART(uint8_t chr){
	#ifdef USE_USB
	if (hUsbDeviceFS.dev_state != USBD_STATE_CONFIGURED){	
		commInputByte(chr);
	}
	#else
	commInputByte(chr);
	#endif //USE_USB
	
}
