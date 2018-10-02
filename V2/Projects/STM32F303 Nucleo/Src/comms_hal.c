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
extern commBuffer commTX;
extern UART_HandleTypeDef huart2;
uint8_t insertCharToBuff(commBuffer *buff, uint8_t chr);

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

void commsSendInt32(int32_t num){
	commsSendUint32(num);
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




void commsSendDMA(uint8_t chr){
	#ifdef USE_USB
	if (hUsbDeviceFS.dev_state == USBD_STATE_CONFIGURED){	
		while(CDC_Transmit_FS(&chr,1)!=USBD_OK){
			taskYIELD();
		}
	}else{
		UARTsendChar(chr);
	}
	#else
	while (!isXferComplete());
	if (insertCharToBuff(&commTX, chr)){
			//send DMA
			HAL_UART_Transmit_DMA(&huart2,commTX.memory,COMM_TX_BUFFER_SIZE);
			commTX.writePointer=0;
			commTX.state = BUFF_EMPTY;
			//while(huart2.gState != HAL_UART_STATE_READY){
			//}
		}
	#endif
	
	
}

void commsSendInt32DMA(int32_t num){
	commsSendUint32DMA(num);
}

void commsSendUint32DMA(uint32_t num){
	uint8_t buff[4];
	buff[0]=(uint8_t)(num);
	buff[1]=(uint8_t)(num>>8);
	buff[2]=(uint8_t)(num>>16);
	buff[3]=(uint8_t)(num>>24);
  commsSendBuffDMA(buff, 4);
}

void commsSendBuffDMA(uint8_t *buff, uint16_t len){
	#ifdef USE_USB
//	if (hUsbDeviceFS.dev_state == USBD_STATE_CONFIGURED){	
//		while(CDC_Transmit_FS(buff,len)!=USBD_OK){
//			taskYIELD();
//		}
//	}else{
//		UARTsendBuff((char *)buff,len);
//	}
	#else
	// if there is enough space in buffer dont care about check and insert data
	if(len < commTX.bufferSize - commTX.writePointer && isXferComplete()){
		while (len>0){
			*(commTX.memory + commTX.writePointer) = *buff++;
			commTX.writePointer = (commTX.writePointer + 1) % COMM_BUFFER_SIZE;
			len--;
		}
	}else{
		while(len>0){
			while (!isXferComplete());
			if (insertCharToBuff(&commTX, *(buff++))){
				//send DMA
				HAL_UART_Transmit_DMA(&huart2,commTX.memory,COMM_TX_BUFFER_SIZE);
				commTX.writePointer=0;
				commTX.state = BUFF_EMPTY;
			}
			len--;
		}
	}
	#endif
}

void commsSendStringDMA(char *chr){
	uint32_t i = 0;
	char * tmp=chr;
	while(*(tmp++)){i++;}
	#ifdef USE_USB
//	if (hUsbDeviceFS.dev_state == USBD_STATE_CONFIGURED){	
//		while(CDC_Transmit_FS((uint8_t*)chr,i)!=USBD_OK){
//			taskYIELD();
//		}
//	}else{
//		UARTsendBuff(chr,i);
//	}
	#else
		// if there is enough space in buffer dont care about check and insert data
	if(i < commTX.bufferSize - commTX.writePointer && isXferComplete()){
		while (i>0){
			*(commTX.memory + commTX.writePointer) = *chr++;
			commTX.writePointer = (commTX.writePointer + 1) % COMM_BUFFER_SIZE;
			i--;
		}
	}else{
		while(i>0){
			while (!isXferComplete());
			if (insertCharToBuff(&commTX, *(chr++))){
				//send DMA
				HAL_UART_Transmit_DMA(&huart2,commTX.memory,COMM_TX_BUFFER_SIZE);
				commTX.writePointer=0;
				commTX.state = BUFF_EMPTY;
			}
			i--;
		}
	}
	#endif

}

uint8_t insertCharToBuff(commBuffer *buff, uint8_t chr){
	if(buff->state == BUFF_FULL){
		return 1;
	}else{
		*(buff->memory + buff->writePointer) = chr;
		buff->writePointer = (buff->writePointer + 1) % COMM_BUFFER_SIZE;
		if(buff->state == BUFF_EMPTY){
			buff->state = BUFF_DATA;
		}else if(buff->state == BUFF_DATA && buff->writePointer == buff->readPointer){
			buff->state = BUFF_FULL;
		}
		
		if(buff->state == BUFF_FULL){
			return 1;
		}else{
			return 0;
		}
	}
}

void flushBuff(uint16_t threshold){
	while (!isXferComplete());
	if(commTX.writePointer>threshold){
		HAL_UART_Transmit_DMA(&huart2,commTX.memory,commTX.writePointer);
		commTX.writePointer=0;
		commTX.state = BUFF_EMPTY;
	}
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
