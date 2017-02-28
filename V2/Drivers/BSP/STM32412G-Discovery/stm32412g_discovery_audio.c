/**
  ******************************************************************************
  * @file    stm32412g_discovery_audio.c
  * @author  MCD Application Team
  * @version V1.0.0
  * @date    04-May-2016
  * @brief   This file provides the Audio driver for the STM32412G-DISCOVERY board.
  ******************************************************************************
  * @attention
  *
  * <h2><center>&copy; COPYRIGHT(c) 2016 STMicroelectronics</center></h2>
  *
  * Redistribution and use in source and binary forms, with or without modification,
  * are permitted provided that the following conditions are met:
  *   1. Redistributions of source code must retain the above copyright notice,
  *      this list of conditions and the following disclaimer.
  *   2. Redistributions in binary form must reproduce the above copyright notice,
  *      this list of conditions and the following disclaimer in the documentation
  *      and/or other materials provided with the distribution.
  *   3. Neither the name of STMicroelectronics nor the names of its contributors
  *      may be used to endorse or promote products derived from this software
  *      without specific prior written permission.
  *
  * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
  * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
  * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
  * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
  * FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
  * DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
  * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
  * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
  * OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
  * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
  *
  ******************************************************************************
  */

/*==============================================================================
                                 User NOTES
                                 
How To use this driver:
-----------------------
   + This driver supports STM32F4xx devices on STM32412G-DISCOVERY boards.
   + Call the function BSP_AUDIO_OUT_Init(
                                    OutputDevice: physical output mode (OUTPUT_DEVICE_SPEAKER, 
                                                  OUTPUT_DEVICE_HEADPHONE or OUTPUT_DEVICE_BOTH)
                                    Volume      : Initial volume to be set (0 is min (mute), 100 is max (100%)
                                    AudioFreq   : Audio frequency in Hz (8000, 16000, 22500, 32000...)
                                                  this parameter is relative to the audio file/stream type.
                                   )
      This function configures all the hardware required for the audio application (codec, I2C, I2S, 
      GPIOs, DMA and interrupt if needed). This function returns AUDIO_OK if configuration is OK.
      If the returned value is different from AUDIO_OK or the function is stuck then the communication with
      the codec has failed (try to un-plug the power or reset device in this case).
      - OUTPUT_DEVICE_SPEAKER  : only speaker will be set as output for the audio stream.
      - OUTPUT_DEVICE_HEADPHONE: only headphones will be set as output for the audio stream.
      - OUTPUT_DEVICE_BOTH     : both Speaker and Headphone are used as outputs for the audio stream
                                 at the same time.
   + Call the function BSP_AUDIO_OUT_Play(
                                  pBuffer: pointer to the audio data file address
                                  Size   : size of the buffer to be sent in Bytes
                                 )
      to start playing (for the first time) from the audio file/stream.
   + Call the function BSP_AUDIO_OUT_Pause() to pause playing   
   + Call the function BSP_AUDIO_OUT_Resume() to resume playing.
       Note. After calling BSP_AUDIO_OUT_Pause() function for pause, only BSP_AUDIO_OUT_Resume() should be called
          for resume (it is not allowed to call BSP_AUDIO_OUT_Play() in this case).
       Note. This function should be called only when the audio file is played or paused (not stopped).
   + For each mode, you may need to implement the relative callback functions into your code.
      The Callback functions are named AUDIO_OUT_XXX_CallBack() and only their prototypes are declared in 
      the stm32412g_discovery_audio.h file. (refer to the example for more details on the callbacks implementations)
   + To Stop playing, to modify the volume level, the frequency, use the functions: BSP_AUDIO_OUT_SetVolume(), 
      AUDIO_OUT_SetFrequency(), BSP_AUDIO_OUT_SetOutputMode(), BSP_AUDIO_OUT_SetMute() and BSP_AUDIO_OUT_Stop().
   + The driver API and the callback functions are at the end of the stm32412g_discovery_audio.h file.
 

Driver architecture:
--------------------
   + This driver provides the High Audio Layer: consists of the function API exported in the stm32412g_discovery_audio.h file
     (BSP_AUDIO_OUT_Init(), BSP_AUDIO_OUT_Play() ...)
   + This driver provide also the Media Access Layer (MAL): which consists of functions allowing to access the media containing/
     providing the audio file/stream. These functions are also included as local functions into
     the stm32412g_discovery_audio_codec.c file (I2Sx_Out_Init(), I2Sx_Out_DeInit(), I2Sx_In_Init() and I2Sx_In_DeInit())

Known Limitations:
------------------
   1- If the TDM Format used to play in parallel 2 audio Stream (the first Stream is configured in codec SLOT0 and second 
      Stream in SLOT1) the Pause/Resume, volume and mute feature will control the both streams.
   2- Parsing of audio file is not implemented (in order to determine audio file properties: Mono/Stereo, Data size, 
      File size, Audio Frequency, Audio Data header size ...). The configuration is fixed for the given audio file.
   3- Supports only Stereo audio streaming.
   4- Supports only 16-bits audio data size.
==============================================================================*/

/* Includes ------------------------------------------------------------------*/
#include "stm32412g_discovery_audio.h"

/** @addtogroup BSP
  * @{
  */

/** @addtogroup STM32412G_DISCOVERY
  * @{
  */ 
  
/** @defgroup STM32412G_DISCOVERY_AUDIO STM32412G-DISCOVERY AUDIO
  * @brief This file includes the low layer driver for wm8994 Audio Codec
  *        available on STM32412G-DISCOVERY board(MB1209).
  * @{
  */ 

/** @defgroup STM32412G_DISCOVERY_AUDIO_Private_Types STM32412G Discovery Audio Private Types
  * @{
  */
typedef struct
{
  uint32_t      Frequency;      /* Record Frequency */
  uint32_t      BitResolution;  /* Record bit resolution */
  uint32_t      ChannelNbr;     /* Record Channel Number */
  uint16_t      *pRecBuf;       /* Pointer to record user buffer */
  uint32_t      RecSize;        /* Size to record in mono, double size to record in stereo */
}AUDIOIN_TypeDef;
/**
  * @}
  */ 
  
/** @defgroup STM32412G_DISCOVERY_AUDIO_Private_Defines STM32412G Discovery Audio Private Defines
  * @{
  */
/**
  * @}
  */ 

/** @defgroup STM32412G_DISCOVERY_AUDIO_Private_Macros STM32412G Discovery Audio Private macros 
  * @{
  */

#define DFSDM_OVER_SAMPLING(__FREQUENCY__) \
        (__FREQUENCY__ == AUDIO_FREQUENCY_8K)  ? 256 \
      : (__FREQUENCY__ == AUDIO_FREQUENCY_11K) ? 256 \
      : (__FREQUENCY__ == AUDIO_FREQUENCY_16K) ? 128 \
      : (__FREQUENCY__ == AUDIO_FREQUENCY_22K) ? 128 \
      : (__FREQUENCY__ == AUDIO_FREQUENCY_32K) ? 64 \
      : (__FREQUENCY__ == AUDIO_FREQUENCY_44K) ? 64  \
      : (__FREQUENCY__ == AUDIO_FREQUENCY_48K) ? 32 : 25  \

#define DFSDM_CLOCK_DIVIDER(__FREQUENCY__) \
        (__FREQUENCY__ == AUDIO_FREQUENCY_8K)  ? 24 \
      : (__FREQUENCY__ == AUDIO_FREQUENCY_11K) ? 48 \
      : (__FREQUENCY__ == AUDIO_FREQUENCY_16K) ? 24 \
      : (__FREQUENCY__ == AUDIO_FREQUENCY_22K) ? 48 \
      : (__FREQUENCY__ == AUDIO_FREQUENCY_32K) ? 24 \
      : (__FREQUENCY__ == AUDIO_FREQUENCY_44K) ? 48  \
      : (__FREQUENCY__ == AUDIO_FREQUENCY_48K) ? 32 : 72  \
        
#define DFSDM_FILTER_ORDER(__FREQUENCY__) \
        (__FREQUENCY__ == AUDIO_FREQUENCY_8K)  ? DFSDM_FILTER_SINC3_ORDER \
      : (__FREQUENCY__ == AUDIO_FREQUENCY_11K) ? DFSDM_FILTER_SINC3_ORDER \
      : (__FREQUENCY__ == AUDIO_FREQUENCY_16K) ? DFSDM_FILTER_SINC3_ORDER \
      : (__FREQUENCY__ == AUDIO_FREQUENCY_22K) ? DFSDM_FILTER_SINC3_ORDER \
      : (__FREQUENCY__ == AUDIO_FREQUENCY_32K) ? DFSDM_FILTER_SINC4_ORDER \
      : (__FREQUENCY__ == AUDIO_FREQUENCY_44K) ? DFSDM_FILTER_SINC4_ORDER  \
      : (__FREQUENCY__ == AUDIO_FREQUENCY_48K) ? DFSDM_FILTER_SINC4_ORDER : DFSDM_FILTER_SINC4_ORDER  \

#define DFSDM_RIGHT_BIT_SHIFT(__FREQUENCY__) \
        (__FREQUENCY__ == AUDIO_FREQUENCY_8K)  ? 5 \
      : (__FREQUENCY__ == AUDIO_FREQUENCY_11K) ? 4 \
      : (__FREQUENCY__ == AUDIO_FREQUENCY_16K) ? 2 \
      : (__FREQUENCY__ == AUDIO_FREQUENCY_22K) ? 2 \
      : (__FREQUENCY__ == AUDIO_FREQUENCY_32K) ? 5 \
      : (__FREQUENCY__ == AUDIO_FREQUENCY_44K) ? 6  \
      : (__FREQUENCY__ == AUDIO_FREQUENCY_48K) ? 2 : 0  \
        
/* Saturate the record PCM sample */
#define SaturaLH(N, L, H) (((N)<(L))?(L):(((N)>(H))?(H):(N)))

/**
  * @}
  */ 
  
/** @defgroup STM32412G_DISCOVERY_AUDIO_Private_Variables STM32412G Discovery Audio Private Variables
  * @{
  */
  
/*
Note: 
  these global variables are not compliant to naming rules (upper case without "_" ), 
  but we keep this naming for compatibility, in fact these variables (not static)
  could have been used by the application, example the stm32f4xx_it.c:
    void DMA2_Stream6_IRQHandler(void)
     {  HAL_DMA_IRQHandler(haudio_i2s.hdmatx);  }
*/
AUDIO_DrvTypeDef                *audio_drv;
I2S_HandleTypeDef               haudio_i2s;       /* for Audio_OUT and Audio_IN_analog mic */
I2S_HandleTypeDef               haudio_in_i2sext; /* for Analog mic with full duplex mode  */
AUDIOIN_TypeDef                 hAudioIn;

DFSDM_Channel_HandleTypeDef     haudio_in_dfsdm_leftchannel;   /* MP34DT01TR microphones on PCB top side */
DFSDM_Channel_HandleTypeDef     haudio_in_dfsdm_rightchannel;
DFSDM_Filter_HandleTypeDef      haudio_in_dfsdm_leftfilter;         /* Common filters for MP34DT01TR microphones inputs */
DFSDM_Filter_HandleTypeDef      haudio_in_dfsdm_rightfilter;
DMA_HandleTypeDef               hdma_dfsdm_left;                    /* Common DMAs for MP34DT01TR microphones inputs */
DMA_HandleTypeDef               hdma_dfsdm_right;


/* Buffers for right and left samples */
int32_t                         *pScratchBuff[DEFAULT_AUDIO_IN_CHANNEL_NBR];
int32_t                         ScratchSize;

/* Buffers status flags */
uint32_t                        DmaLeftRecHalfBuffCplt  = 0;
uint32_t                        DmaLeftRecBuffCplt      = 0;
uint32_t                        DmaRightRecHalfBuffCplt = 0;
uint32_t                        DmaRightRecBuffCplt     = 0;

/* Application Buffer Trigger */
__IO uint32_t                   AppBuffTrigger          = 0;
uint32_t __IO                   AppBuffHalf             = 0;
   
uint16_t __IO AudioInVolume = DEFAULT_AUDIO_IN_VOLUME;

/**
  * @}
  */ 

/** @defgroup STM32412G_DISCOVERY_AUDIO_Private_Function_Prototypes STM32412G Discovery Audio Private Prototypes
  * @{
  */
static void I2Sx_In_Init(uint32_t AudioFreq);
static void I2Sx_In_DeInit(void);

static void I2Sx_Out_Init(uint32_t AudioFreq);
static void I2Sx_Out_DeInit(void);

static uint8_t DFSDMx_Init(uint32_t AudioFreq);
static uint8_t DFSDMx_DeInit(void);
static void DFSDMx_ChannelMspInit(void);
static void DFSDMx_ChannelMspDeInit(void);
static void DFSDMx_FilterMspInit(void);
static void DFSDMx_FilterMspDeInit(void);

/**
  * @}
  */ 

/** @defgroup STM32412G_DISCOVERY_AUDIO_out_Private_Functions STM32412G Discovery AudioOut Private Functions
  * @{
  */ 

/**
  * @brief  Configures the audio peripherals.
  * @param  OutputDevice: OUTPUT_DEVICE_SPEAKER, OUTPUT_DEVICE_HEADPHONE,
  *                       or OUTPUT_DEVICE_BOTH.
  * @param  Volume: Initial volume level (from 0 (Mute) to 100 (Max))
  * @param  AudioFreq: Audio frequency used to play the audio stream.
  * @note   The I2S PLL input clock must be done in the user application.  
  * @retval AUDIO_OK if correct communication, else wrong communication
  */
uint8_t BSP_AUDIO_OUT_Init(uint16_t OutputDevice, uint8_t Volume, uint32_t AudioFreq)
{ 
  uint8_t  ret = AUDIO_ERROR;
  uint32_t deviceid = 0x00;
  uint16_t buffer_fake[16] = {0x00};

  I2Sx_Out_DeInit();
  AUDIO_IO_DeInit();
  
  /* PLL clock is set depending on the AudioFreq (44.1 kHz vs 48kHz groups) */
  BSP_AUDIO_OUT_ClockConfig(&haudio_i2s, AudioFreq, NULL);
  
  /* Configure the I2S peripheral */
  haudio_i2s.Instance = AUDIO_OUT_I2Sx;
  if(HAL_I2S_GetState(&haudio_i2s) == HAL_I2S_STATE_RESET)
  {
    /* Initialize the I2S Msp: this __weak function can be rewritten by the application */
    BSP_AUDIO_OUT_MspInit(&haudio_i2s, NULL);
  }
  I2Sx_Out_Init(AudioFreq);

  AUDIO_IO_Init();
  
  /* wm8994 codec initialization */
  deviceid = wm8994_drv.ReadID(AUDIO_I2C_ADDRESS);

  if(deviceid == WM8994_ID)
  {
    /* Reset the Codec Registers */
    wm8994_drv.Reset(AUDIO_I2C_ADDRESS);
    /* Initialize the audio driver structure */
    audio_drv = &wm8994_drv;
    ret = AUDIO_OK;
  }
  else
  {
    ret = AUDIO_ERROR;
  }

  if(ret == AUDIO_OK)
  {
    /* Send fake I2S data in order to generate MCLK needed by WM8994 to set its registers
     * MCLK is generated only when a data stream is sent on I2S */
    HAL_I2S_Transmit_DMA(&haudio_i2s, buffer_fake, 16);
    /* Initialize the codec internal registers */
    audio_drv->Init(AUDIO_I2C_ADDRESS, OutputDevice, Volume, AudioFreq);
    /* Stop sending fake I2S data */
    HAL_I2S_DMAStop(&haudio_i2s);
  }

  return ret;
}

/**
  * @brief  Starts playing audio stream from a data buffer for a determined size. 
  * @param  pBuffer: Pointer to the buffer 
  * @param  Size: Number of audio data BYTES.
  * @retval AUDIO_OK if correct communication, else wrong communication
  */
uint8_t BSP_AUDIO_OUT_Play(uint16_t* pBuffer, uint32_t Size)
{
  /* Call the audio Codec Play function */
  if(audio_drv->Play(AUDIO_I2C_ADDRESS, pBuffer, Size) != 0)
  {  
    return AUDIO_ERROR;
  }
  else
  {
    /* Update the Media layer and enable it for play */  
    HAL_I2S_Transmit_DMA(&haudio_i2s, pBuffer, DMA_MAX(Size / AUDIODATA_SIZE));
    
    return AUDIO_OK;
  }
}

/**
  * @brief  Sends n-Bytes on the I2S interface.
  * @param  pData: pointer on data address 
  * @param  Size: number of data to be written
  * @retval None
  */
void BSP_AUDIO_OUT_ChangeBuffer(uint16_t *pData, uint16_t Size)
{
   HAL_I2S_Transmit_DMA(&haudio_i2s, pData, Size);
}

/**
  * @brief  This function Pauses the audio file stream. In case
  *         of using DMA, the DMA Pause feature is used.
  * @WARNING When calling BSP_AUDIO_OUT_Pause() function for pause, only
  *          BSP_AUDIO_OUT_Resume() function should be called for resume (use of BSP_AUDIO_OUT_Play() 
  *          function for resume could lead to unexpected behavior).
  * @retval AUDIO_OK if correct communication, else wrong communication
  */
uint8_t BSP_AUDIO_OUT_Pause(void)
{    
  /* Call the Audio Codec Pause/Resume function */
  if(audio_drv->Pause(AUDIO_I2C_ADDRESS) != 0)
  {
    return AUDIO_ERROR;
  }
  else
  {
    /* Call the Media layer pause function */
    HAL_I2S_DMAPause(&haudio_i2s);
    
    /* Return AUDIO_OK when all operations are correctly done */
    return AUDIO_OK;
  }
}

/**
  * @brief  This function  Resumes the audio file stream.  
  * @WARNING When calling BSP_AUDIO_OUT_Pause() function for pause, only
  *          BSP_AUDIO_OUT_Resume() function should be called for resume (use of BSP_AUDIO_OUT_Play() 
  *          function for resume could lead to unexpected behavior).
  * @retval AUDIO_OK if correct communication, else wrong communication
  */
uint8_t BSP_AUDIO_OUT_Resume(void)
{    
  /* Call the Media layer pause/resume function */
  /* DMA stream resumed before accessing WM8994 register as WM8994 needs the MCLK to be generated to access its registers
   * MCLK is generated only when a data stream is sent on I2S */
  HAL_I2S_DMAResume(&haudio_i2s);

  /* Call the Audio Codec Pause/Resume function */
  if(audio_drv->Resume(AUDIO_I2C_ADDRESS) != 0)
  {
    return AUDIO_ERROR;
  }
  else
  {
    /* Return AUDIO_OK when all operations are correctly done */
    return AUDIO_OK;
  }
}

/**
  * @brief  Stops audio playing and Power down the Audio Codec. 
  * @param  Option: could be one of the following parameters 
  *           - CODEC_PDWN_SW: for software power off (by writing registers). 
  *                            Then no need to reconfigure the Codec after power on.
  *           - CODEC_PDWN_HW: completely shut down the codec (physically). 
  *                            Then need to reconfigure the Codec after power on.  
  * @retval AUDIO_OK if correct communication, else wrong communication
  */
uint8_t BSP_AUDIO_OUT_Stop(uint32_t Option)
{
  /* Call the Media layer stop function */
  HAL_I2S_DMAStop(&haudio_i2s);

  /* Call Audio Codec Stop function */
  if(audio_drv->Stop(AUDIO_I2C_ADDRESS, Option) != 0)
  {
    return AUDIO_ERROR;
  }
  else
  {
    if(Option == CODEC_PDWN_HW)
    { 
      /* Wait at least 100us */
      HAL_Delay(1);
    }
    /* Return AUDIO_OK when all operations are correctly done */
    return AUDIO_OK;
  }
}

/**
  * @brief  Controls the current audio volume level. 
  * @param  Volume: Volume level to be set in percentage from 0% to 100% (0 for 
  *         Mute and 100 for Max volume level).
  * @retval AUDIO_OK if correct communication, else wrong communication
  */
uint8_t BSP_AUDIO_OUT_SetVolume(uint8_t Volume)
{
  /* Call the codec volume control function with converted volume value */
  if(audio_drv->SetVolume(AUDIO_I2C_ADDRESS, Volume) != 0)
  {
    return AUDIO_ERROR;
  }
  else
  {
    /* Return AUDIO_OK when all operations are correctly done */
    return AUDIO_OK;
  }
}

/**
  * @brief  Enables or disables the MUTE mode by software 
  * @param  Cmd: Could be AUDIO_MUTE_ON to mute sound or AUDIO_MUTE_OFF to 
  *         unmute the codec and restore previous volume level.
  * @retval AUDIO_OK if correct communication, else wrong communication
  */
uint8_t BSP_AUDIO_OUT_SetMute(uint32_t Cmd)
{ 
  /* Call the Codec Mute function */
  if(audio_drv->SetMute(AUDIO_I2C_ADDRESS, Cmd) != 0)
  {
    return AUDIO_ERROR;
  }
  else
  {
    /* Return AUDIO_OK when all operations are correctly done */
    return AUDIO_OK;
  }
}

/**
  * @brief  Switch dynamically (while audio file is played) the output target 
  *         (speaker or headphone).
  * @param  Output: The audio output target: OUTPUT_DEVICE_SPEAKER,
  *         OUTPUT_DEVICE_HEADPHONE or OUTPUT_DEVICE_BOTH
  * @retval AUDIO_OK if correct communication, else wrong communication
  */
uint8_t BSP_AUDIO_OUT_SetOutputMode(uint8_t Output)
{
  /* Call the Codec output device function */
  if(audio_drv->SetOutputMode(AUDIO_I2C_ADDRESS, Output) != 0)
  {
    return AUDIO_ERROR;
  }
  else
  {
    /* Return AUDIO_OK when all operations are correctly done */
    return AUDIO_OK;
  }
}

/**
  * @brief  Updates the audio frequency.
  * @param  AudioFreq: Audio frequency used to play the audio stream.
  * @note   This API should be called after the BSP_AUDIO_OUT_Init() to adjust the
  *         audio frequency.
  * @retval None
  */
void BSP_AUDIO_OUT_SetFrequency(uint32_t AudioFreq)
{ 
  /* PLL clock is set depending by the AudioFreq (44.1khz vs 48khz groups) */ 
  BSP_AUDIO_OUT_ClockConfig(&haudio_i2s, AudioFreq, NULL);

  /* Disable I2S peripheral to allow access to I2S internal registers */
  __HAL_I2S_DISABLE(&haudio_i2s);
    
  /* Update the I2S audio frequency configuration */
  haudio_i2s.Init.AudioFreq = AudioFreq;
  HAL_I2S_Init(&haudio_i2s);
  
  /* Enable I2S peripheral to generate MCLK */
  __HAL_I2S_ENABLE(&haudio_i2s);
}

/**
  * @brief  Deinit the audio peripherals.
  * @param  None
  * @retval None
  */
void BSP_AUDIO_OUT_DeInit(void)
{
  I2Sx_Out_DeInit();
  /* DeInit the I2S MSP : this __weak function can be rewritten by the application */
  BSP_AUDIO_OUT_MspDeInit(&haudio_i2s, NULL);
}

/**
  * @brief  Tx Transfer completed callbacks.
  * @param  hi2s: I2S handle
  * @retval None
  */
void HAL_I2S_TxCpltCallback(I2S_HandleTypeDef *hi2s)
{
  /* Manage the remaining file size and new address offset: This function 
     should be coded by user (its prototype is already declared in stm32412g_discovery_audio.h) */
  BSP_AUDIO_OUT_TransferComplete_CallBack();
}

/**
  * @brief  Tx Half Transfer completed callbacks.
  * @param  hi2s: I2S handle
  * @retval None
  */
void HAL_I2S_TxHalfCpltCallback(I2S_HandleTypeDef *hi2s)
{
  /* Manage the remaining file size and new address offset: This function 
     should be coded by user (its prototype is already declared in stm32412g_discovery_audio.h) */
  BSP_AUDIO_OUT_HalfTransfer_CallBack();
}

/**
  * @brief  I2S error callbacks.
  * @param  hi2s: I2S handle
  * @retval None
  */
void HAL_I2S_ErrorCallback(I2S_HandleTypeDef *hi2s)
{
  BSP_AUDIO_OUT_Error_CallBack();
}

/**
  * @brief  Manages the DMA full Transfer complete event.
  * @param  None
  * @retval None
  */
__weak void BSP_AUDIO_OUT_TransferComplete_CallBack(void)
{
}

/**
  * @brief  Manages the DMA Half Transfer complete event.
  * @param  None
  * @retval None
  */
__weak void BSP_AUDIO_OUT_HalfTransfer_CallBack(void)
{ 
}

/**
  * @brief  Manages the DMA FIFO error event.
  * @param  None
  * @retval None
  */
__weak void BSP_AUDIO_OUT_Error_CallBack(void)
{
}

/**
  * @brief  Initializes BSP_AUDIO_OUT MSP.
  * @param  hi2s: I2S handle
  * @retval None
  */
__weak void BSP_AUDIO_OUT_MspInit(I2S_HandleTypeDef *hi2s, void *Params)
{ 
  static DMA_HandleTypeDef hdma_i2s_tx;
  GPIO_InitTypeDef  gpio_init_structure;

  /* Enable I2S clock */
  AUDIO_OUT_I2Sx_CLK_ENABLE();

  /* Enable MCK, SCK, WS, SD and CODEC_INT GPIO clock */
  AUDIO_OUT_I2Sx_MCK_GPIO_CLK_ENABLE();
  AUDIO_OUT_I2Sx_SCK_GPIO_CLK_ENABLE();
  AUDIO_OUT_I2Sx_SD_GPIO_CLK_ENABLE();
  AUDIO_OUT_I2Sx_WS_GPIO_CLK_ENABLE();

  /* CODEC_I2S pins configuration: MCK, SCK, WS and SD pins */
  gpio_init_structure.Pin = AUDIO_OUT_I2Sx_MCK_PIN;
  gpio_init_structure.Mode = GPIO_MODE_AF_PP;
  gpio_init_structure.Pull = GPIO_NOPULL;
  gpio_init_structure.Speed = GPIO_SPEED_FAST;
  gpio_init_structure.Alternate = AUDIO_OUT_I2Sx_MCK_AF;
  HAL_GPIO_Init(AUDIO_OUT_I2Sx_MCK_GPIO_PORT, &gpio_init_structure);

  gpio_init_structure.Pin = AUDIO_OUT_I2Sx_SCK_PIN;
  gpio_init_structure.Alternate = AUDIO_OUT_I2Sx_SCK_AF;
  HAL_GPIO_Init(AUDIO_OUT_I2Sx_SCK_GPIO_PORT, &gpio_init_structure);

  gpio_init_structure.Pin = AUDIO_OUT_I2Sx_WS_PIN;
  gpio_init_structure.Alternate = AUDIO_OUT_I2Sx_WS_AF;
  HAL_GPIO_Init(AUDIO_OUT_I2Sx_WS_GPIO_PORT, &gpio_init_structure);

  gpio_init_structure.Pin = AUDIO_OUT_I2Sx_SD_PIN;
  gpio_init_structure.Alternate = AUDIO_OUT_I2Sx_SD_AF;
  HAL_GPIO_Init(AUDIO_OUT_I2Sx_SD_GPIO_PORT, &gpio_init_structure);

  /* Enable the DMA clock */
  AUDIO_OUT_I2Sx_DMAx_CLK_ENABLE();

  if(hi2s->Instance == AUDIO_OUT_I2Sx)
  {
    /* Configure the hdma_i2s_rx handle parameters */
    hdma_i2s_tx.Init.Channel             = AUDIO_OUT_I2Sx_DMAx_CHANNEL;
    hdma_i2s_tx.Init.Direction           = DMA_MEMORY_TO_PERIPH;
    hdma_i2s_tx.Init.PeriphInc           = DMA_PINC_DISABLE;
    hdma_i2s_tx.Init.MemInc              = DMA_MINC_ENABLE;
    hdma_i2s_tx.Init.PeriphDataAlignment = AUDIO_OUT_I2Sx_DMAx_PERIPH_DATA_SIZE;
    hdma_i2s_tx.Init.MemDataAlignment    = AUDIO_OUT_I2Sx_DMAx_MEM_DATA_SIZE;
    hdma_i2s_tx.Init.Mode                = DMA_CIRCULAR;
    hdma_i2s_tx.Init.Priority            = DMA_PRIORITY_HIGH;
    hdma_i2s_tx.Init.FIFOMode            = DMA_FIFOMODE_DISABLE;
    hdma_i2s_tx.Init.FIFOThreshold       = DMA_FIFO_THRESHOLD_FULL;
    hdma_i2s_tx.Init.MemBurst            = DMA_MBURST_SINGLE;
    hdma_i2s_tx.Init.PeriphBurst         = DMA_MBURST_SINGLE;

    hdma_i2s_tx.Instance = AUDIO_OUT_I2Sx_DMAx_STREAM;

    /* Associate the DMA handle */
    __HAL_LINKDMA(hi2s, hdmatx, hdma_i2s_tx);

    /* Deinitialize the Stream for new transfer */
    HAL_DMA_DeInit(&hdma_i2s_tx);

    /* Configure the DMA Stream */
    HAL_DMA_Init(&hdma_i2s_tx);
  }
  
  /* Enable and set I2Sx Interrupt to a lower priority */
  HAL_NVIC_SetPriority(SPI3_IRQn, 0x0F, 0x00);
  HAL_NVIC_EnableIRQ(SPI3_IRQn);

  /* I2S DMA IRQ Channel configuration */
  HAL_NVIC_SetPriority(AUDIO_OUT_I2Sx_DMAx_IRQ, AUDIO_OUT_IRQ_PREPRIO, 0);
  HAL_NVIC_EnableIRQ(AUDIO_OUT_I2Sx_DMAx_IRQ);
}

/**
  * @brief  Deinitializes I2S MSP.
  * @param  hi2s: I2S handle
  * @retval None
  */
__weak void BSP_AUDIO_OUT_MspDeInit(I2S_HandleTypeDef *hi2s, void *Params)
{
    GPIO_InitTypeDef  gpio_init_structure;

    /* I2S DMA IRQ Channel deactivation */
    HAL_NVIC_DisableIRQ(AUDIO_OUT_I2Sx_DMAx_IRQ);

    if(hi2s->Instance == AUDIO_OUT_I2Sx)
    {
      /* Deinitialize the DMA stream */
      HAL_DMA_DeInit(hi2s->hdmatx);
    }

    /* Disable I2S peripheral */
    __HAL_I2S_DISABLE(hi2s);

    /* Deactives CODEC_I2S pins MCK, SCK, WS and SD by putting them in input mode */
    gpio_init_structure.Pin = AUDIO_OUT_I2Sx_MCK_PIN;
    HAL_GPIO_DeInit(AUDIO_OUT_I2Sx_MCK_GPIO_PORT, gpio_init_structure.Pin);

    gpio_init_structure.Pin = AUDIO_OUT_I2Sx_SCK_PIN;
    HAL_GPIO_DeInit(AUDIO_OUT_I2Sx_SCK_GPIO_PORT, gpio_init_structure.Pin);

    gpio_init_structure.Pin = AUDIO_OUT_I2Sx_WS_PIN;
    HAL_GPIO_DeInit(AUDIO_OUT_I2Sx_WS_GPIO_PORT, gpio_init_structure.Pin);

    gpio_init_structure.Pin = AUDIO_OUT_I2Sx_SD_PIN;
    HAL_GPIO_DeInit(AUDIO_OUT_I2Sx_SD_GPIO_PORT, gpio_init_structure.Pin);

    /* Disable I2S clock */
    AUDIO_OUT_I2Sx_CLK_DISABLE();

    /* GPIO pins clock and DMA clock can be shut down in the application 
       by surcharging this __weak function */ 
}

/**
  * @brief  Clock Config.
  * @param  hi2s: might be required to set audio peripheral predivider if any.
  * @param  AudioFreq: Audio frequency used to play the audio stream.
  * @note   This API is called by BSP_AUDIO_OUT_Init() and BSP_AUDIO_OUT_SetFrequency()
  *         Being __weak it can be overwritten by the application     
  * @retval None
  */
__weak void BSP_AUDIO_OUT_ClockConfig(I2S_HandleTypeDef *hi2s, uint32_t AudioFreq, void *Params)
{ 
  RCC_PeriphCLKInitTypeDef rcc_ex_clk_init_struct;

  HAL_RCCEx_GetPeriphCLKConfig(&rcc_ex_clk_init_struct);

  /* Set the PLL configuration according to the audio frequency */
  if((AudioFreq == AUDIO_FREQUENCY_11K) || (AudioFreq == AUDIO_FREQUENCY_22K) || (AudioFreq == AUDIO_FREQUENCY_44K))
  {
    /* Configure PLLI2S prescalers */
    rcc_ex_clk_init_struct.PeriphClockSelection = (RCC_PERIPHCLK_I2S_APB1  | RCC_PERIPHCLK_PLLI2S);
    rcc_ex_clk_init_struct.I2sApb1ClockSelection = RCC_I2SAPB1CLKSOURCE_PLLI2S;
    rcc_ex_clk_init_struct.PLLI2SSelection = RCC_PLLI2SCLKSOURCE_PLLSRC;    
    rcc_ex_clk_init_struct.PLLI2S.PLLI2SM = 8;
    rcc_ex_clk_init_struct.PLLI2S.PLLI2SN = 271;
    rcc_ex_clk_init_struct.PLLI2S.PLLI2SR = 2;
    
    HAL_RCCEx_PeriphCLKConfig(&rcc_ex_clk_init_struct);
  }
  else if(AudioFreq == AUDIO_FREQUENCY_96K) /* AUDIO_FREQUENCY_96K */
  {
    /* I2S clock config */
    rcc_ex_clk_init_struct.PeriphClockSelection = (RCC_PERIPHCLK_I2S_APB1 | RCC_PERIPHCLK_PLLI2S);
    rcc_ex_clk_init_struct.I2sApb1ClockSelection = RCC_I2SAPB1CLKSOURCE_PLLI2S;
    rcc_ex_clk_init_struct.PLLI2SSelection = RCC_PLLI2SCLKSOURCE_PLLSRC;    
    rcc_ex_clk_init_struct.PLLI2S.PLLI2SM = 8;
    rcc_ex_clk_init_struct.PLLI2S.PLLI2SN = 344;
    rcc_ex_clk_init_struct.PLLI2S.PLLI2SR = 2;

    HAL_RCCEx_PeriphCLKConfig(&rcc_ex_clk_init_struct);    
  }  
  else /* AUDIO_FREQUENCY_8K, AUDIO_FREQUENCY_16K, AUDIO_FREQUENCY_48K */
  {
    /* I2S clock config
    PLLI2S_VCO: VCO_344M
    I2S_CLK(first level) = PLLI2S_VCO/PLLI2SR = 344/7 = 49.142 Mhz
    I2S_CLK_x = I2S_CLK(first level)/PLLI2SDIVR = 49.142/1 = 49.142 Mhz */
    rcc_ex_clk_init_struct.PeriphClockSelection = RCC_PERIPHCLK_I2S_APB1 | RCC_PERIPHCLK_PLLI2S;
    rcc_ex_clk_init_struct.I2sApb1ClockSelection = RCC_I2SAPB1CLKSOURCE_PLLI2S;
    rcc_ex_clk_init_struct.PLLI2SSelection = RCC_PLLI2SCLKSOURCE_PLLSRC;
    rcc_ex_clk_init_struct.PLLI2S.PLLI2SM = 8;
    rcc_ex_clk_init_struct.PLLI2S.PLLI2SN = 344;
    rcc_ex_clk_init_struct.PLLI2S.PLLI2SR = 7;
    
    HAL_RCCEx_PeriphCLKConfig(&rcc_ex_clk_init_struct);
  }
}

/*******************************************************************************
                            Static Functions
*******************************************************************************/

/**
  * @brief  Initializes the Audio Codec audio interface (I2S)
  * @note   This function assumes that the I2S input clock
  *         is already configured and ready to be used.
  * @param  AudioFreq: Audio frequency to be configured for the I2S peripheral.
  * @retval None
  */
static void I2Sx_Out_Init(uint32_t AudioFreq)
{
  /* Initialize the hAudioInI2s Instance parameter */
  haudio_i2s.Instance = AUDIO_OUT_I2Sx;

 /* Disable I2S block */
  __HAL_I2S_DISABLE(&haudio_i2s);
  
  /* I2S peripheral configuration */
  haudio_i2s.Init.AudioFreq = AudioFreq;
  haudio_i2s.Init.ClockSource = I2S_CLOCK_PLL;
  haudio_i2s.Init.CPOL = I2S_CPOL_LOW;
  haudio_i2s.Init.DataFormat = I2S_DATAFORMAT_16B;
  haudio_i2s.Init.MCLKOutput = I2S_MCLKOUTPUT_ENABLE;
  haudio_i2s.Init.Mode = I2S_MODE_MASTER_TX;
  haudio_i2s.Init.Standard =  I2S_STANDARD_PHILIPS;
  haudio_i2s.Init.FullDuplexMode = I2S_FULLDUPLEXMODE_DISABLE;
  
  /* Init the I2S */
  HAL_I2S_Init(&haudio_i2s);

 /* Enable I2S block */
  __HAL_I2S_ENABLE(&haudio_i2s);
}

/**
  * @brief  Deinitializes the Audio Codec audio interface (I2S).
  * @param  None
  * @retval None
  */
static void I2Sx_Out_DeInit(void)
{
  /* Initialize the hAudioInI2s Instance parameter */
  haudio_i2s.Instance = AUDIO_OUT_I2Sx;

 /* Disable I2S block */
  __HAL_I2S_DISABLE(&haudio_i2s);

  /* DeInit the I2S */
  HAL_I2S_DeInit(&haudio_i2s);
}
 
/**
  * @}
  */

/** @defgroup STM32412G_DISCOVERY_AUDIO_in_Private_Functions STM32412G Discovery AudioIn Private functions
  * @{
  */ 

/**
  * @brief  Initializes wave recording.
  * @note   This function assumes that the I2S input clock
  *         is already configured and ready to be used.  
  * @param  InputDevice: INPUT_DEVICE_DIGITAL_MIC or INPUT_DEVICE_ANALOG_MIC.                     
  * @param  AudioFreq: Audio frequency to be configured for the I2S peripheral.
  * @param  BitRes: Audio frequency to be configured for the I2S peripheral.
  * @param  ChnlNbr: Audio frequency to be configured for the I2S peripheral.
  * @retval AUDIO_OK if correct communication, else wrong communication
  */
uint8_t BSP_AUDIO_IN_Init(uint32_t AudioFreq, uint32_t BitRes, uint32_t ChnlNbr)
{
  return BSP_AUDIO_IN_InitEx(INPUT_DEVICE_DIGITAL_MIC, AudioFreq, BitRes, ChnlNbr);  
}

/**
  * @brief  Initializes wave recording.
  * @note   This function assumes that the I2S input clock
  *         is already configured and ready to be used.  
  * @param  InputDevice: INPUT_DEVICE_DIGITAL_MIC or INPUT_DEVICE_ANALOG_MIC.                     
  * @param  AudioFreq: Audio frequency to be configured for the I2S peripheral.
  * @param  BitRes: Audio frequency to be configured for the I2S peripheral.
  * @param  ChnlNbr: Audio frequency to be configured for the I2S peripheral.
  * @retval AUDIO_OK if correct communication, else wrong communication
  */
uint8_t BSP_AUDIO_IN_InitEx(uint16_t InputDevice, uint32_t AudioFreq, uint32_t BitRes, uint32_t ChnlNbr)
{
  uint32_t ret = AUDIO_ERROR;
  uint32_t deviceid =0;
  uint16_t buffer_fake[16] = {0x00};

  /* Store the audio record context */
  hAudioIn.Frequency     = AudioFreq;
  hAudioIn.BitResolution = BitRes;
  
  if (InputDevice == INPUT_DEVICE_DIGITAL_MIC)
  {
    /* PLL clock is set depending on the AudioFreq (44.1khz vs 48khz groups) */
    BSP_AUDIO_IN_ClockConfig(&haudio_in_dfsdm_leftchannel, AudioFreq, NULL); /* Clock config is shared between AUDIO IN and OUT for analog mic */
    
    /* MSP channels initialization */
    DFSDMx_ChannelMspInit();
    
    /* MSP filters initialization */
    DFSDMx_FilterMspInit();
    
    /* DFSDM data acquisition preparation */
    ret = DFSDMx_Init(AudioFreq);
  }
  else
  {
    /* INPUT_DEVICE_ANALOG_MIC */
    /* Disable I2S */
    I2Sx_In_DeInit();

    /* PLL clock is set depending on the AudioFreq (44.1khz vs 48khz groups) */
    BSP_AUDIO_IN_ClockConfig(&haudio_in_dfsdm_rightchannel, AudioFreq, NULL); /* Clock config is shared between AUDIO IN and OUT for analog mic */

    /* I2S data transfer preparation:
    Prepare the Media to be used for the audio transfer from I2S peripheral to memory */
    haudio_i2s.Instance = AUDIO_IN_I2Sx;
    if(HAL_I2S_GetState(&haudio_i2s) == HAL_I2S_STATE_RESET)
    {
      BSP_AUDIO_OUT_MspInit(&haudio_i2s, NULL); /* Initialize GPIOs for SPI3 Master signals */
      /* Init the I2S MSP: this __weak function can be redefined by the application*/
      BSP_AUDIO_IN_MspInit(&haudio_i2s, NULL);
    }

    /* Configure I2S */
    I2Sx_In_Init(AudioFreq);

    AUDIO_IO_Init();

    /* wm8994 codec initialization */
    deviceid = wm8994_drv.ReadID(AUDIO_I2C_ADDRESS);

    if((deviceid) == WM8994_ID)
    {
      /* Reset the Codec Registers */
      wm8994_drv.Reset(AUDIO_I2C_ADDRESS);
      /* Initialize the audio driver structure */
      audio_drv = &wm8994_drv;
      ret = AUDIO_OK;
    }
    else
    {
      ret = AUDIO_ERROR;
    }

    if(ret == AUDIO_OK)
    {
      /* Receive fake I2S data in order to generate MCLK needed by WM8994 to set its registers */
      HAL_I2S_Receive_DMA(&haudio_i2s, buffer_fake, 16);
     /* Initialize the codec internal registers */
      audio_drv->Init(AUDIO_I2C_ADDRESS, (OUTPUT_DEVICE_HEADPHONE|InputDevice), 100, AudioFreq);
      /* Stop receiving fake I2S data */
      HAL_I2S_DMAStop(&haudio_i2s);
    }
  }

  /* Return AUDIO_OK when all operations are correctly done */
  return ret;
}

/**
  * @brief  DeInitializes the audio peripheral.
  * @param  None                    
  * @retval None
  */
void BSP_AUDIO_IN_DeInit(void)
{
  BSP_AUDIO_IN_DeInitEx(INPUT_DEVICE_DIGITAL_MIC);
}

/**
  * @brief  DeInitializes the audio peripheral.
  * @param  InputDevice: INPUT_DEVICE_DIGITAL_MIC or INPUT_DEVICE_ANALOG_MIC.                    
  * @retval None
  */
void BSP_AUDIO_IN_DeInitEx(uint16_t InputDevice)
{
  if (InputDevice == INPUT_DEVICE_DIGITAL_MIC)
  {
    /* MSP channels initialization */
    DFSDMx_ChannelMspDeInit(); 
    
    /* MSP filters initialization */
    DFSDMx_FilterMspDeInit();
    
    DFSDMx_DeInit();
  }
  else
  {
    I2Sx_In_DeInit();
  }
}

/**
  * @brief  Allocate channel buffer scratch 
  * @param  pScratch : pointer to scratch tables.
  * @param  size of scratch buffer
  */
uint8_t BSP_AUDIO_IN_AllocScratch (int32_t *pScratch, uint32_t size)
{ 
  uint32_t idx;
  
  ScratchSize = size / DEFAULT_AUDIO_IN_CHANNEL_NBR;
  
  /* copy scratch pointers */
  for (idx = 0; idx < DEFAULT_AUDIO_IN_CHANNEL_NBR ; idx++)
  {
    pScratchBuff[idx] = (int32_t *)(pScratch + idx * ScratchSize);
  }
  /* Return AUDIO_OK */
  return AUDIO_OK;
}

/**
  * @brief  Starts audio recording.                    
  * @param  pBuf: Main buffer pointer for the recorded data storing
  * @param  size: Current size of the recorded buffer
  * @note   The Right channel is start at first with synchro on start of Left channel
  * @retval AUDIO_OK if correct communication, else wrong communication
  */
uint8_t BSP_AUDIO_IN_Record(uint16_t *pBuf, uint32_t size)
{
  return BSP_AUDIO_IN_RecordEx(INPUT_DEVICE_DIGITAL_MIC, pBuf, size);
}
/**
  * @brief  Starts audio recording.
  * @param  InputDevice: INPUT_DEVICE_DIGITAL_MIC or INPUT_DEVICE_ANALOG_MIC.                    
  * @param  pBuf: Main buffer pointer for the recorded data storing
  * @param  size: Current size of the recorded buffer
  * @note   The Right channel is start at first with synchro on start of Left channel
  * @retval AUDIO_OK if correct communication, else wrong communication
  */
uint8_t BSP_AUDIO_IN_RecordEx(uint16_t InputDevice, uint16_t *pBuf, uint32_t size)
{
  uint8_t ret = AUDIO_ERROR;
  
  hAudioIn.pRecBuf = pBuf;
  hAudioIn.RecSize = size;
  /* Reset Application Buffer Trigger */
  AppBuffTrigger = 0;
  AppBuffHalf = 0;
  
  if (InputDevice == INPUT_DEVICE_DIGITAL_MIC)
  {
    /* Call the Media layer start function for right channel */
    if(HAL_OK != HAL_DFSDM_FilterRegularStart_DMA(&haudio_in_dfsdm_rightfilter, pScratchBuff[0], ScratchSize))
    {
      return ret;
    }
    
    /* Call the Media layer start function for left channel */
    if(HAL_OK != HAL_DFSDM_FilterRegularStart_DMA(&haudio_in_dfsdm_leftfilter, pScratchBuff[1], ScratchSize))
    {
      return ret;
    }
  }
  else
  {
    /* Start the process to receive the DMA */
    if (HAL_OK != HAL_I2SEx_TransmitReceive_DMA(&haudio_i2s, pBuf, pBuf, size))
    {
      return ret;
    }
  }
  /* Return AUDIO_OK when all operations are correctly done */
  ret = AUDIO_OK;
  
  return ret;
}

/**
  * @brief  Initializes BSP_AUDIO_IN MSP.
  * @param  hi2s_in: I2S handle
  * @param  Params
  * @retval None
  */
__weak void BSP_AUDIO_IN_MspInit(I2S_HandleTypeDef *hi2s, void *Params)
{
  static DMA_HandleTypeDef hdma_i2s_rx;
  GPIO_InitTypeDef  gpio_init_structure;  

  /* Enable I2S clock */
  AUDIO_IN_I2Sx_CLK_ENABLE();

  /* Enable MCK GPIO clock, needed by the codec */
  AUDIO_OUT_I2Sx_MCK_GPIO_CLK_ENABLE();

  /* CODEC_I2S pins configuration: MCK pins */
  gpio_init_structure.Pin = AUDIO_OUT_I2Sx_MCK_PIN;
  gpio_init_structure.Mode = GPIO_MODE_AF_PP;
  gpio_init_structure.Pull = GPIO_NOPULL;
  gpio_init_structure.Speed = GPIO_SPEED_FAST;
  gpio_init_structure.Alternate = AUDIO_OUT_I2Sx_MCK_AF;
  HAL_GPIO_Init(AUDIO_OUT_I2Sx_MCK_GPIO_PORT, &gpio_init_structure);
  
  /* Enable SD GPIO clock */
  AUDIO_IN_I2Sx_EXT_SD_GPIO_CLK_ENABLE();
  /* CODEC_I2S pin configuration: SD pin */
  gpio_init_structure.Pin = AUDIO_IN_I2Sx_EXT_SD_PIN;
  gpio_init_structure.Alternate = AUDIO_IN_I2Sx_EXT_SD_AF;
  HAL_GPIO_Init(AUDIO_IN_I2Sx_EXT_SD_GPIO_PORT, &gpio_init_structure);

  /* Enable the DMA clock */
  AUDIO_IN_I2Sx_DMAx_CLK_ENABLE();
    
  if(hi2s->Instance == AUDIO_IN_I2Sx)
  {
    /* Configure the hdma_i2s_rx handle parameters */
    hdma_i2s_rx.Init.Channel             = AUDIO_IN_I2Sx_DMAx_CHANNEL;
    hdma_i2s_rx.Init.Direction           = DMA_PERIPH_TO_MEMORY;
    hdma_i2s_rx.Init.PeriphInc           = DMA_PINC_DISABLE;
    hdma_i2s_rx.Init.MemInc              = DMA_MINC_ENABLE;
    hdma_i2s_rx.Init.PeriphDataAlignment = AUDIO_IN_I2Sx_DMAx_PERIPH_DATA_SIZE;
    hdma_i2s_rx.Init.MemDataAlignment    = AUDIO_IN_I2Sx_DMAx_MEM_DATA_SIZE;
    hdma_i2s_rx.Init.Mode                = DMA_CIRCULAR;
    hdma_i2s_rx.Init.Priority            = DMA_PRIORITY_HIGH;
    hdma_i2s_rx.Init.FIFOMode            = DMA_FIFOMODE_DISABLE;
    hdma_i2s_rx.Init.FIFOThreshold       = DMA_FIFO_THRESHOLD_FULL;
    hdma_i2s_rx.Init.MemBurst            = DMA_MBURST_SINGLE;
    hdma_i2s_rx.Init.PeriphBurst         = DMA_MBURST_SINGLE;
    
    hdma_i2s_rx.Instance = AUDIO_IN_I2Sx_DMAx_STREAM;
    
    /* Associate the DMA handle */
    __HAL_LINKDMA(hi2s, hdmarx, hdma_i2s_rx);
    
    /* Deinitialize the Stream for new transfer */
    HAL_DMA_DeInit(&hdma_i2s_rx);
    
    /* Configure the DMA Stream */
    HAL_DMA_Init(&hdma_i2s_rx);
  }
  
  /* I2S DMA IRQ Channel configuration */
  HAL_NVIC_SetPriority(AUDIO_IN_I2Sx_DMAx_IRQ, AUDIO_IN_IRQ_PREPRIO, 0);
  HAL_NVIC_EnableIRQ(AUDIO_IN_I2Sx_DMAx_IRQ);	
}

/**
  * @brief  Clock Config.
  * @param  hdfsdm_channel : DFSDM channel handle, might be required to set audio peripheral predivider if any.
  * @param  AudioFreq: Audio frequency used to play the audio stream.
  * @note   This API is called by BSP_AUDIO_OUT_Init() and BSP_AUDIO_OUT_SetFrequency()
  *         Being __weak it can be overwritten by the application
  * @retval AUDIO_OK if correct communication, else wrong communication
  */
__weak uint8_t BSP_AUDIO_IN_ClockConfig(DFSDM_Channel_HandleTypeDef *hdfsdm_channel, uint32_t AudioFreq, void *Params)
{
  RCC_PeriphCLKInitTypeDef rcc_ex_clk_init_struct;

  HAL_RCCEx_GetPeriphCLKConfig(&rcc_ex_clk_init_struct);

  /* Set the PLL configuration according to the audio frequency */
  if((AudioFreq == AUDIO_FREQUENCY_11K) || (AudioFreq == AUDIO_FREQUENCY_22K) || (AudioFreq == AUDIO_FREQUENCY_44K))
  {
    /* Configure PLLI2S prescalers */
    rcc_ex_clk_init_struct.PeriphClockSelection = (RCC_PERIPHCLK_I2S_APB1  | RCC_PERIPHCLK_DFSDM);
    rcc_ex_clk_init_struct.I2sApb1ClockSelection = RCC_I2SAPB1CLKSOURCE_PLLI2S;
    rcc_ex_clk_init_struct.DfsdmClockSelection = RCC_DFSDM1CLKSOURCE_APB2;
    rcc_ex_clk_init_struct.PLLI2S.PLLI2SM = 8;
    rcc_ex_clk_init_struct.PLLI2S.PLLI2SN = 271;
    rcc_ex_clk_init_struct.PLLI2S.PLLI2SR = 2;

    HAL_RCCEx_PeriphCLKConfig(&rcc_ex_clk_init_struct);
  }
  else if(AudioFreq == AUDIO_FREQUENCY_96K)
  {
    /* I2S clock config */
    rcc_ex_clk_init_struct.PeriphClockSelection = (RCC_PERIPHCLK_I2S_APB1 | RCC_PERIPHCLK_DFSDM);
    rcc_ex_clk_init_struct.I2sApb1ClockSelection = RCC_I2SAPB1CLKSOURCE_PLLI2S;
    rcc_ex_clk_init_struct.DfsdmClockSelection = RCC_DFSDM1CLKSOURCE_APB2;
    rcc_ex_clk_init_struct.PLLI2S.PLLI2SM = 8;
    rcc_ex_clk_init_struct.PLLI2S.PLLI2SN = 344;
    rcc_ex_clk_init_struct.PLLI2S.PLLI2SR = 2;

    HAL_RCCEx_PeriphCLKConfig(&rcc_ex_clk_init_struct);    
  }
  else /* AUDIO_FREQUENCY_8K, AUDIO_FREQUENCY_16K, AUDIO_FREQUENCY_32K, AUDIO_FREQUENCY_48K */
  {
    /* I2S clock config
    PLLI2S_VCO: VCO_344M
    I2S_CLK(first level) = PLLI2S_VCO/PLLI2SR = 344/7 = 49.142 Mhz
    I2S_CLK_x = I2S_CLK(first level)/PLLI2SDIVR = 49.142/1 = 49.142 Mhz */
    rcc_ex_clk_init_struct.PeriphClockSelection = (RCC_PERIPHCLK_I2S_APB1 | RCC_PERIPHCLK_DFSDM);
    rcc_ex_clk_init_struct.I2sApb1ClockSelection = RCC_I2SAPB1CLKSOURCE_PLLI2S;
    rcc_ex_clk_init_struct.DfsdmClockSelection = RCC_DFSDM1CLKSOURCE_APB2;
    rcc_ex_clk_init_struct.PLLI2S.PLLI2SM = 8;
    rcc_ex_clk_init_struct.PLLI2S.PLLI2SN = 344;
    rcc_ex_clk_init_struct.PLLI2S.PLLI2SR = 7;

    HAL_RCCEx_PeriphCLKConfig(&rcc_ex_clk_init_struct);
  }

  if (hdfsdm_channel == &haudio_in_dfsdm_leftchannel)
  {
    /* I2S_APB1 selected as DFSDM audio clock source */
    __HAL_RCC_DFSDM1AUDIO_CONFIG(RCC_DFSDM1AUDIOCLKSOURCE_I2SAPB1);
  }

  return AUDIO_OK;
}

/**
  * @brief  Regular conversion complete callback.
  * @note   In interrupt mode, user has to read conversion value in this function
            using HAL_DFSDM_FilterGetRegularValue.
  * @param  hdfsdm_filter : DFSDM filter handle.
  * @retval None
  */
void HAL_DFSDM_FilterRegConvCpltCallback(DFSDM_Filter_HandleTypeDef *hdfsdm_filter)
{
  uint32_t index;
  
  if(hdfsdm_filter == &haudio_in_dfsdm_leftfilter)
  {
    DmaLeftRecBuffCplt = 1;
  }
  else
  {
    DmaRightRecBuffCplt = 1;
  }
  
  if((DmaRightRecBuffCplt == 1) && (DmaLeftRecBuffCplt == 1))
  {
    if(AppBuffTrigger >= hAudioIn.RecSize)
      AppBuffTrigger = 0;
    
    for(index = (ScratchSize/2) ; index < ScratchSize; index++)
    {
      hAudioIn.pRecBuf[AppBuffTrigger]     = (uint16_t)(SaturaLH((pScratchBuff[0][index] >> 8), -32760, 32760));
      hAudioIn.pRecBuf[AppBuffTrigger + 1] = (uint16_t)(SaturaLH((pScratchBuff[1][index] >> 8), -32760, 32760));
      AppBuffTrigger += 2;
    }
    DmaRightRecBuffCplt = DmaLeftRecBuffCplt = 0;
  }
  
  /* Update Trigger with Remaining Byte before callback if necessary */
  if(AppBuffTrigger >= hAudioIn.RecSize)
  {
    /* Reset Application Buffer Trigger */
    AppBuffTrigger = 0;
    AppBuffHalf = 0;
    
    /* Call the record update function to get the next buffer to fill and its size (size is ignored) */
    BSP_AUDIO_IN_TransferComplete_CallBack();
  }
  else if((AppBuffTrigger >= hAudioIn.RecSize/2))
  {
    if(AppBuffHalf == 0)
    {
      AppBuffHalf = 1;
      /* Manage the remaining file size and new address offset: This function
      should be coded by user (its prototype is already declared in stm32l476g_eval_audio.h) */
      BSP_AUDIO_IN_HalfTransfer_CallBack();
    }
  }
}

/**
  * @brief  Half regular conversion complete callback.
  * @param  hdfsdm_filter : DFSDM filter handle.
  * @retval None
  */
void HAL_DFSDM_FilterRegConvHalfCpltCallback(DFSDM_Filter_HandleTypeDef *hdfsdm_filter)
{
  uint32_t index;
  
  if(hdfsdm_filter == &haudio_in_dfsdm_leftfilter)
  {
    DmaLeftRecHalfBuffCplt = 1;
  }
  else
  {
    DmaRightRecHalfBuffCplt = 1;
  }
  
  if((DmaRightRecHalfBuffCplt == 1) && (DmaLeftRecHalfBuffCplt == 1))
  {
    if(AppBuffTrigger >= hAudioIn.RecSize)
      AppBuffTrigger = 0;
    
    for(index = 0; index < ScratchSize/2; index++)
    {
      hAudioIn.pRecBuf[AppBuffTrigger]     = (int16_t)(SaturaLH((pScratchBuff[0][index] >> 8), -32760, 32760));
      hAudioIn.pRecBuf[AppBuffTrigger + 1] = (int16_t)(SaturaLH((pScratchBuff[1][index] >> 8), -32760, 32760));
      AppBuffTrigger += 2;
    }
    DmaRightRecHalfBuffCplt = DmaLeftRecHalfBuffCplt = 0;
  }
  
  /* Update Trigger with Remaining Byte before callback if necessary */
  if(AppBuffTrigger >= hAudioIn.RecSize)
  {
    /* Reset Application Buffer Trigger */
    AppBuffTrigger = 0;
    AppBuffHalf = 0;
    
    /* Call the record update function to get the next buffer to fill and its size (size is ignored) */
    BSP_AUDIO_IN_TransferComplete_CallBack();
  }
  else if((AppBuffTrigger >= hAudioIn.RecSize/2))
  {
    if(AppBuffHalf == 0)
    {
      AppBuffHalf = 1;
      /* Manage the remaining file size and new address offset: This function
      should be coded by user (its prototype is already declared in stm32l476g_eval_audio.h) */
      BSP_AUDIO_IN_HalfTransfer_CallBack();
    }
  }
}

/**
  * @brief  Half reception complete callback.
  * @param  hi2s : I2S handle.
  * @retval None
  */
void HAL_I2S_RxHalfCpltCallback(I2S_HandleTypeDef *hi2s)
{
  /* Manage the remaining file size and new address offset: This function 
     should be coded by user (its prototype is already declared in stm32746g_discovery_audio.h) */
  BSP_AUDIO_IN_HalfTransfer_CallBack();
}

/**
  * @brief  Reception complete callback.
  * @param  hi2s : I2S handle.
  * @retval None
  */
void HAL_I2S_RxCpltCallback(I2S_HandleTypeDef *hi2s)
{
  /* Call the record update function to get the next buffer to fill and its size (size is ignored) */
  BSP_AUDIO_IN_TransferComplete_CallBack();
}

/**
  * @brief  Stops audio recording.                 
  * @param  None
  * @retval AUDIO_OK if correct communication, else wrong communication
  */
uint8_t BSP_AUDIO_IN_Stop(void)
{
  return (BSP_AUDIO_IN_StopEx(INPUT_DEVICE_DIGITAL_MIC)); 
}

/**
  * @brief  Stops audio recording.
  * @param  InputDevice: INPUT_DEVICE_DIGITAL_MIC or INPUT_DEVICE_ANALOG_MIC.
  * @retval AUDIO_OK if correct communication, else wrong communication
  */
uint8_t BSP_AUDIO_IN_StopEx(uint16_t InputDevice)
{
  uint32_t ret = AUDIO_ERROR;
  
  AppBuffTrigger = 0;
  AppBuffHalf    = 0;
  
  if (InputDevice == INPUT_DEVICE_DIGITAL_MIC)
  {
    /* Call the Media layer stop function for right channel */
    if(HAL_OK != HAL_DFSDM_FilterRegularStop_DMA(&haudio_in_dfsdm_rightfilter))
    {
      return ret;
    }
    
    /* Call the Media layer stop function for left channel */
    if(HAL_OK != HAL_DFSDM_FilterRegularStop_DMA(&haudio_in_dfsdm_leftfilter))
    {
      return ret;
    }
  }
  else /* InputDevice = INPUT_DEVICE_ANALOG_MIC */
  { 
    /* Call the Media layer stop function */
    if(HAL_OK != HAL_I2S_DMAStop(&haudio_i2s))
    {
      return ret;
    }
    /* Call Audio Codec Stop function */
    if(audio_drv->Stop(AUDIO_I2C_ADDRESS, CODEC_PDWN_HW) != 0)
    {
      return ret;
    }
    /* Wait at least 100us */
    HAL_Delay(1);
  }
  
  /* Return AUDIO_OK when all operations are correctly done */
  ret = AUDIO_OK;
  
  return ret;
}

/**
  * @brief  Pauses the audio file stream.
  * @param  None                  .
  * @retval AUDIO_OK if correct communication, else wrong communication
  */
uint8_t BSP_AUDIO_IN_Pause(void)
{
  /* Call the Media layer stop function */
  if(HAL_OK != HAL_DFSDM_FilterRegularStop_DMA(&haudio_in_dfsdm_rightfilter))
  {
    return AUDIO_ERROR;
  }
  
  /* Call the Media layer stop function */
  if(HAL_OK != HAL_DFSDM_FilterRegularStop_DMA(&haudio_in_dfsdm_leftfilter))
  {
    return AUDIO_ERROR;
  }
  
  /* Return AUDIO_OK when all operations are correctly done */
  return AUDIO_OK;
}

/**
  * @brief  Resumes the audio file stream.
  * @param  None                   
  * @note   The Right channel is start a first with synchro on start Left channel
  * @retval AUDIO_OK if correct communication, else wrong communication
  */
uint8_t BSP_AUDIO_IN_Resume(void)
{   
  uint8_t ret = AUDIO_ERROR;
  
  /* Call the Media layer start function for right channel */
  if(HAL_OK != HAL_DFSDM_FilterRegularStart_DMA(&haudio_in_dfsdm_rightfilter, pScratchBuff[0], ScratchSize))
  {
    return ret;
  }
  
  /* Call the Media layer start function for left channel */
  if(HAL_OK != HAL_DFSDM_FilterRegularStart_DMA(&haudio_in_dfsdm_leftfilter, pScratchBuff[1], ScratchSize))
  {
    return ret;
  }
  
  /* Return AUDIO_OK when all operations are correctly done */
  return AUDIO_OK;
}

/**
  * @brief  Controls the audio in volume level. 
  * @param  Volume: Volume level to be set in percentage from 0% to 100% (0 for 
  *         Mute and 100 for Max volume level).
  * @retval AUDIO_OK if correct communication, else wrong communication
  */
uint8_t BSP_AUDIO_IN_SetVolume(uint8_t Volume)
{
  /* Set the Global variable AudioInVolume  */
  AudioInVolume = Volume; 
  
  /* Return AUDIO_OK when all operations are correctly done */
  return AUDIO_OK;
}

/**
  * @brief  User callback when record buffer is filled.
  * @retval None
  */
__weak void BSP_AUDIO_IN_TransferComplete_CallBack(void)
{
  /* This function should be implemented by the user application.
     It is called into this driver when the current buffer is filled
     to prepare the next buffer pointer and its size. */
}

/**
  * @brief  Manages the DMA Half Transfer complete event.
  * @retval None
  */
__weak void BSP_AUDIO_IN_HalfTransfer_CallBack(void)
{
  /* This function should be implemented by the user application.
     It is called into this driver when the current buffer is filled
     to prepare the next buffer pointer and its size. */
}

/**
  * @brief  Audio IN Error callback function.
  * @retval None
  */
__weak void BSP_AUDIO_IN_Error_Callback(void)
{
  /* This function is called when an Interrupt due to transfer error on or peripheral
     error occurs. */
}

/**
  * @}
  */

/*******************************************************************************
                            Static Functions
*******************************************************************************/

/**
  * @brief  Initializes the Digital Filter for Sigma-Delta Modulators interface (DFSDM).
  * @param  AudioFreq: Audio frequency to be used to set correctly the DFSDM peripheral.
  * @note   Channel output Clock Divider and Filter Oversampling are calculated as follow: 
  *         - Clock_Divider = CLK(input DFSDM)/CLK(micro) with
  *           1MHZ < CLK(micro) < 3.2MHZ (TYP 2.4MHZ for MP34DT01TR)
  *         - Oversampling = CLK(input DFSDM)/(Clock_Divider * AudioFreq)
  * @retval AUDIO_OK if correct communication, else wrong communication
  */
static uint8_t DFSDMx_Init(uint32_t AudioFreq)
{  
  /*####LEFT CHANNEL ####*/
  __HAL_DFSDM_CHANNEL_RESET_HANDLE_STATE(&haudio_in_dfsdm_leftchannel);   
  haudio_in_dfsdm_leftchannel.Init.OutputClock.Activation   = ENABLE;
  haudio_in_dfsdm_leftchannel.Instance                      = AUDIO_DFSDMx_LEFT_CHANNEL;  
  haudio_in_dfsdm_leftchannel.Init.OutputClock.Selection    = DFSDM_CHANNEL_OUTPUT_CLOCK_AUDIO;
  /* Set the DFSDM clock OUT audio frequency configuration */
  haudio_in_dfsdm_leftchannel.Init.OutputClock.Divider      = DFSDM_CLOCK_DIVIDER(AudioFreq);
  haudio_in_dfsdm_leftchannel.Init.Input.Multiplexer        = DFSDM_CHANNEL_EXTERNAL_INPUTS;
  haudio_in_dfsdm_leftchannel.Init.Input.DataPacking        = DFSDM_CHANNEL_STANDARD_MODE;
  haudio_in_dfsdm_leftchannel.Init.Input.Pins               = DFSDM_CHANNEL_SAME_CHANNEL_PINS;
  /* Request to sample stable data for LEFT micro on Rising edge */
  haudio_in_dfsdm_leftchannel.Init.SerialInterface.Type     = DFSDM_CHANNEL_SPI_RISING;
  haudio_in_dfsdm_leftchannel.Init.SerialInterface.SpiClock = DFSDM_CHANNEL_SPI_CLOCK_INTERNAL;
  haudio_in_dfsdm_leftchannel.Init.Awd.FilterOrder          = DFSDM_CHANNEL_SINC1_ORDER;
  haudio_in_dfsdm_leftchannel.Init.Awd.Oversampling         = 10;
  haudio_in_dfsdm_leftchannel.Init.Offset                   = 0;
  haudio_in_dfsdm_leftchannel.Init.RightBitShift            = DFSDM_RIGHT_BIT_SHIFT(AudioFreq);
  haudio_in_dfsdm_leftchannel.State                         = HAL_DFSDM_CHANNEL_STATE_RESET;
  if(HAL_OK != HAL_DFSDM_ChannelInit(&haudio_in_dfsdm_leftchannel))
  {
    return AUDIO_ERROR;
  }
  
  /*####RIGHT CHANNEL ####*/
  __HAL_DFSDM_CHANNEL_RESET_HANDLE_STATE(&haudio_in_dfsdm_rightchannel);    
  haudio_in_dfsdm_rightchannel.Init.OutputClock.Activation   = ENABLE;
  haudio_in_dfsdm_rightchannel.Instance                       = AUDIO_DFSDMx_RIGHT_CHANNEL;
  haudio_in_dfsdm_rightchannel.Init.OutputClock.Selection    = DFSDM_CHANNEL_OUTPUT_CLOCK_AUDIO;
  /* Set the DFSDM clock OUT audio frequency configuration */
  haudio_in_dfsdm_rightchannel.Init.OutputClock.Divider      = DFSDM_CLOCK_DIVIDER(AudioFreq);
  haudio_in_dfsdm_rightchannel.Init.Input.Multiplexer        = DFSDM_CHANNEL_EXTERNAL_INPUTS;
  haudio_in_dfsdm_rightchannel.Init.Input.DataPacking        = DFSDM_CHANNEL_STANDARD_MODE;
  haudio_in_dfsdm_rightchannel.Init.Input.Pins               = DFSDM_CHANNEL_FOLLOWING_CHANNEL_PINS;
  /* Request to sample stable data for RIGHT micro on Falling edge */
  haudio_in_dfsdm_rightchannel.Init.SerialInterface.Type     = DFSDM_CHANNEL_SPI_FALLING;
  haudio_in_dfsdm_rightchannel.Init.SerialInterface.SpiClock = DFSDM_CHANNEL_SPI_CLOCK_INTERNAL;
  haudio_in_dfsdm_rightchannel.Init.Awd.FilterOrder          = DFSDM_CHANNEL_SINC1_ORDER;
  haudio_in_dfsdm_rightchannel.Init.Awd.Oversampling         = 10;
  haudio_in_dfsdm_rightchannel.Init.Offset                   = 0;
  haudio_in_dfsdm_rightchannel.Init.RightBitShift            = DFSDM_RIGHT_BIT_SHIFT(AudioFreq);
  haudio_in_dfsdm_rightchannel.State                         = HAL_DFSDM_CHANNEL_STATE_RESET;
  if(HAL_OK != HAL_DFSDM_ChannelInit(&haudio_in_dfsdm_rightchannel))
  {
    return AUDIO_ERROR;
  }
  
  /*####FILTER 0####*/
  __HAL_DFSDM_FILTER_RESET_HANDLE_STATE(&haudio_in_dfsdm_leftfilter);  
  haudio_in_dfsdm_leftfilter.Init.RegularParam.Trigger         = DFSDM_FILTER_SW_TRIGGER;
  haudio_in_dfsdm_leftfilter.Init.RegularParam.FastMode        = ENABLE;
  haudio_in_dfsdm_leftfilter.Init.RegularParam.DmaMode         = ENABLE;
  haudio_in_dfsdm_leftfilter.Init.InjectedParam.Trigger        = DFSDM_FILTER_SW_TRIGGER;
  haudio_in_dfsdm_leftfilter.Init.InjectedParam.ScanMode       = DISABLE;
  haudio_in_dfsdm_leftfilter.Init.InjectedParam.DmaMode        = DISABLE;
  haudio_in_dfsdm_leftfilter.Init.InjectedParam.ExtTrigger     = DFSDM_FILTER_EXT_TRIG_TIM8_TRGO;
  haudio_in_dfsdm_leftfilter.Init.InjectedParam.ExtTriggerEdge = DFSDM_FILTER_EXT_TRIG_BOTH_EDGES;
  haudio_in_dfsdm_leftfilter.Init.FilterParam.SincOrder        = DFSDM_FILTER_ORDER(AudioFreq);
  /* Set the DFSDM Filters Oversampling to have correct sample rate */
  haudio_in_dfsdm_leftfilter.Init.FilterParam.Oversampling     = DFSDM_OVER_SAMPLING(AudioFreq);
  haudio_in_dfsdm_leftfilter.Init.FilterParam.IntOversampling  = 1;
  haudio_in_dfsdm_leftfilter.State                             = HAL_DFSDM_FILTER_STATE_RESET;
  haudio_in_dfsdm_leftfilter.Instance                          = AUDIO_DFSDMx_LEFT_FILTER;
  if(HAL_OK != HAL_DFSDM_FilterInit(&haudio_in_dfsdm_leftfilter))
  {
    return AUDIO_ERROR;
  }
  
  /* Configure injected channel */
  if(HAL_OK != HAL_DFSDM_FilterConfigRegChannel(&haudio_in_dfsdm_leftfilter, AUDIO_DFSDMx_LEFT_CHANNEL_FOR_FILTER, DFSDM_CONTINUOUS_CONV_ON))
  {
    return AUDIO_ERROR;
  }
  
  /*####FILTER 1####*/
  __HAL_DFSDM_CHANNEL_RESET_HANDLE_STATE(&haudio_in_dfsdm_rightchannel);   
  haudio_in_dfsdm_rightfilter.Init.RegularParam.Trigger         = DFSDM_FILTER_SYNC_TRIGGER;
  haudio_in_dfsdm_rightfilter.Init.RegularParam.FastMode        = ENABLE;
  haudio_in_dfsdm_rightfilter.Init.RegularParam.DmaMode         = ENABLE;
  haudio_in_dfsdm_rightfilter.Init.InjectedParam.Trigger        = DFSDM_FILTER_SW_TRIGGER;
  haudio_in_dfsdm_rightfilter.Init.InjectedParam.ScanMode       = DISABLE;
  haudio_in_dfsdm_rightfilter.Init.InjectedParam.DmaMode        = DISABLE;
  haudio_in_dfsdm_rightfilter.Init.InjectedParam.ExtTrigger     = DFSDM_FILTER_EXT_TRIG_TIM8_TRGO;
  haudio_in_dfsdm_rightfilter.Init.InjectedParam.ExtTriggerEdge = DFSDM_FILTER_EXT_TRIG_BOTH_EDGES;
  haudio_in_dfsdm_rightfilter.Init.FilterParam.SincOrder        = DFSDM_FILTER_ORDER(AudioFreq);
  /* Set the DFSDM Filters Oversampling to have correct sample rate */
  haudio_in_dfsdm_rightfilter.Init.FilterParam.Oversampling     = DFSDM_OVER_SAMPLING(AudioFreq);
  haudio_in_dfsdm_rightfilter.Init.FilterParam.IntOversampling  = 1;
  haudio_in_dfsdm_rightfilter.State                             = HAL_DFSDM_FILTER_STATE_RESET;
  haudio_in_dfsdm_rightfilter.Instance                          = AUDIO_DFSDMx_RIGHT_FILTER;
  if(HAL_OK != HAL_DFSDM_FilterInit(&haudio_in_dfsdm_rightfilter))
  {
    return AUDIO_ERROR;
  }
  
  /* Configure injected channel */
  if(HAL_OK != HAL_DFSDM_FilterConfigRegChannel(&haudio_in_dfsdm_rightfilter, AUDIO_DFSDMx_RIGHT_CHANNEL_FOR_FILTER, DFSDM_CONTINUOUS_CONV_ON))
  {
    return AUDIO_ERROR;
  }
  
  return AUDIO_OK;
}

/**
  * @brief  De-initializes the Digital Filter for Sigma-Delta Modulators interface (DFSDM).
  * @param  None
  * @retval AUDIO_OK if correct communication, else wrong communication
  */
static uint8_t DFSDMx_DeInit(void)
{
  haudio_in_dfsdm_leftfilter.Instance = AUDIO_DFSDMx_LEFT_FILTER;
  haudio_in_dfsdm_rightfilter.Instance = AUDIO_DFSDMx_RIGHT_FILTER;
  haudio_in_dfsdm_leftchannel.Instance = AUDIO_DFSDMx_LEFT_CHANNEL;
  haudio_in_dfsdm_rightchannel.Instance = AUDIO_DFSDMx_RIGHT_CHANNEL;
  /* De-initializes the DFSDM filters to allow access to DFSDM internal registers */
  if(HAL_OK != HAL_DFSDM_FilterDeInit(&haudio_in_dfsdm_leftfilter))
  {
    return AUDIO_ERROR;
  }

  if(HAL_OK != HAL_DFSDM_FilterDeInit(&haudio_in_dfsdm_rightfilter))
  {
    return AUDIO_ERROR;
  }

  /* De-initializes the DFSDM channels to allow access to DFSDM internal registers */
  if(HAL_OK != HAL_DFSDM_ChannelDeInit(&haudio_in_dfsdm_leftchannel))
  {
    return AUDIO_ERROR;
  }

  if(HAL_OK != HAL_DFSDM_ChannelDeInit(&haudio_in_dfsdm_rightchannel))
  {
    return AUDIO_ERROR;
  }

  return AUDIO_OK;
}

/**
  * @brief  Initializes the DFSDM channel MSP.
  * @param  None
  * @retval None
  */
static void DFSDMx_ChannelMspInit(void)
{
  GPIO_InitTypeDef  GPIO_InitStruct;
  
  /* Enable DFSDM clock */
  AUDIO_DFSDMx_CLK_ENABLE();
  
  /* Enable GPIO clock */
  AUDIO_DFSDMx_CKOUT_DMIC_GPIO_CLK_ENABLE();
  /* DFSDM pins configuration: DFSDM_CKOUT, DMIC_DATIN pins ------------------*/
  GPIO_InitStruct.Pin = AUDIO_DFSDMx_CKOUT_PIN;
  GPIO_InitStruct.Mode = GPIO_MODE_AF_PP;
  GPIO_InitStruct.Pull = GPIO_NOPULL;
  GPIO_InitStruct.Speed = GPIO_SPEED_HIGH;
  GPIO_InitStruct.Alternate = AUDIO_DFSDMx_CKOUT_DMIC_AF;
  HAL_GPIO_Init(AUDIO_DFSDMx_CKOUT_DMIC_GPIO_PORT, &GPIO_InitStruct);
  
  /* MP34DT01TR microphones uses DFSDM_DATIN0 input pin */
  AUDIO_DFSDMx_DMIC_GPIO_CLK_ENABLE();
  GPIO_InitStruct.Pin = AUDIO_DFSDMx_DMIC_PIN;
  GPIO_InitStruct.Mode = GPIO_MODE_AF_PP;
  GPIO_InitStruct.Pull = GPIO_NOPULL;
  GPIO_InitStruct.Speed = GPIO_SPEED_HIGH;
  GPIO_InitStruct.Alternate = AUDIO_DFSDMx_DMIC_AF;
  HAL_GPIO_Init(AUDIO_DFSDMx_DMIC_GPIO_PORT, &GPIO_InitStruct);
}

/**
  * @brief  DeInitializes the DFSDM channel MSP.
  * @param  None
  * @retval None
  */
static void DFSDMx_ChannelMspDeInit(void)
{
  GPIO_InitTypeDef  GPIO_InitStruct;
  
  /* DFSDM pins configuration: DFSDM_CKOUT, DMIC_DATIN pins ------------------*/
  GPIO_InitStruct.Pin = AUDIO_DFSDMx_CKOUT_PIN;
  HAL_GPIO_DeInit(AUDIO_DFSDMx_CKOUT_DMIC_GPIO_PORT, GPIO_InitStruct.Pin);
  
  /* MP34DT01TR microphones uses DFSDM_DATIN0 input pin */
  GPIO_InitStruct.Pin = AUDIO_DFSDMx_DMIC_PIN;
  HAL_GPIO_DeInit(AUDIO_DFSDMx_DMIC_GPIO_PORT, GPIO_InitStruct.Pin);
}

/**
  * @brief  Initializes the DFSDM filter MSP.
  * @param  None
  * @retval None
  */
static void DFSDMx_FilterMspInit(void)
{
  /* Enable DFSDM clock */
  AUDIO_DFSDMx_CLK_ENABLE();
  
  /* Enable the DMA clock */
  AUDIO_DFSDMx_DMAx_CLK_ENABLE();
  
  /* AUDIO_DFSDMx_LEFT FILTER */
  /* Configure the hdma_dfsdm_left handle parameters */
  hdma_dfsdm_left.Init.Direction           = DMA_PERIPH_TO_MEMORY;
  hdma_dfsdm_left.Init.PeriphInc           = DMA_PINC_DISABLE;
  hdma_dfsdm_left.Init.MemInc              = DMA_MINC_ENABLE;
  hdma_dfsdm_left.Init.PeriphDataAlignment = AUDIO_DFSDMx_DMAx_PERIPH_DATA_SIZE;
  hdma_dfsdm_left.Init.MemDataAlignment    = AUDIO_DFSDMx_DMAx_MEM_DATA_SIZE;
  hdma_dfsdm_left.Init.Mode                = DMA_CIRCULAR;
  hdma_dfsdm_left.Init.Priority            = DMA_PRIORITY_HIGH;
  hdma_dfsdm_left.Init.FIFOMode            = DMA_FIFOMODE_DISABLE;
  hdma_dfsdm_left.Init.MemBurst            = DMA_MBURST_SINGLE;
  hdma_dfsdm_left.Init.PeriphBurst         = DMA_PBURST_SINGLE;
  hdma_dfsdm_left.State                    = HAL_DMA_STATE_RESET;  
  hdma_dfsdm_left.Init.Channel             = AUDIO_DFSDMx_DMAx_LEFT_CHANNEL; 
  hdma_dfsdm_left.Instance                 = AUDIO_DFSDMx_DMAx_LEFT_STREAM; 
  
  /* Associate the DMA handle */
  __HAL_LINKDMA(&haudio_in_dfsdm_leftfilter, hdmaReg, hdma_dfsdm_left);
  
  /* Reset DMA handle state */
  __HAL_DMA_RESET_HANDLE_STATE(&hdma_dfsdm_left);
  
  /* Configure the DMA Channel */
  HAL_DMA_Init(&hdma_dfsdm_left);
  
  /* DMA IRQ Channel configuration */
  HAL_NVIC_SetPriority(AUDIO_DFSDMx_DMAx_LEFT_IRQ, AUDIO_IN_IRQ_PREPRIO, 0);
  HAL_NVIC_EnableIRQ(AUDIO_DFSDMx_DMAx_LEFT_IRQ);
  
  
  /* AUDIO_DFSDMx_RIGHT_FILTER */
  /* Configure the hdma_dfsdm_right handle parameters */
  hdma_dfsdm_right.Init.Direction           = DMA_PERIPH_TO_MEMORY;
  hdma_dfsdm_right.Init.PeriphInc           = DMA_PINC_DISABLE;
  hdma_dfsdm_right.Init.MemInc              = DMA_MINC_ENABLE;
  hdma_dfsdm_right.Init.PeriphDataAlignment = AUDIO_DFSDMx_DMAx_PERIPH_DATA_SIZE;
  hdma_dfsdm_right.Init.MemDataAlignment    = AUDIO_DFSDMx_DMAx_MEM_DATA_SIZE;
  hdma_dfsdm_right.Init.Mode                = DMA_CIRCULAR;
  hdma_dfsdm_right.Init.Priority            = DMA_PRIORITY_HIGH;
  hdma_dfsdm_right.Init.FIFOMode            = DMA_FIFOMODE_DISABLE;
  hdma_dfsdm_right.Init.MemBurst            = DMA_MBURST_SINGLE;
  hdma_dfsdm_right.Init.PeriphBurst         = DMA_PBURST_SINGLE;
  hdma_dfsdm_right.State                    = HAL_DMA_STATE_RESET;  
  hdma_dfsdm_right.Init.Channel             = AUDIO_DFSDMx_DMAx_RIGHT_CHANNEL;
  hdma_dfsdm_right.Instance                 = AUDIO_DFSDMx_DMAx_RIGHT_STREAM;  
  
  /* Associate the DMA handle */
  __HAL_LINKDMA(&haudio_in_dfsdm_rightfilter, hdmaReg, hdma_dfsdm_right);
  
  /* Reset DMA handle state */
  __HAL_DMA_RESET_HANDLE_STATE(&hdma_dfsdm_right);
  
  /* Configure the DMA Channel */
  HAL_DMA_Init(&hdma_dfsdm_right);
  
  /* DMA IRQ Channel configuration */
  HAL_NVIC_SetPriority(AUDIO_DFSDMx_DMAx_RIGHT_IRQ, AUDIO_IN_IRQ_PREPRIO, 0);
  HAL_NVIC_EnableIRQ(AUDIO_DFSDMx_DMAx_RIGHT_IRQ);
}

/**
  * @brief  DeInitializes the DFSDM filter MSP.
  * @param  None
  * @retval None
  */
static void DFSDMx_FilterMspDeInit(void)
{
  /* Configure the DMA Channel */
  HAL_DMA_DeInit(&hdma_dfsdm_left);
  HAL_DMA_DeInit(&hdma_dfsdm_right);
}

/**
  * @brief  Initializes the Audio Codec audio interface (I2S)
  * @note   This function assumes that the I2S input clock
  *         is already configured and ready to be used.
  * @param  AudioFreq: Audio frequency to be configured for the I2S peripheral.
  * @retval None
  */
static void I2Sx_In_Init(uint32_t AudioFreq)
{
  /* Initialize the hAudioInI2s and haudio_in_i2sext Instance parameters */
  haudio_i2s.Instance = AUDIO_IN_I2Sx;
  haudio_in_i2sext.Instance = I2S3ext;

 /* Disable I2S block */
  __HAL_I2S_DISABLE(&haudio_i2s);
  __HAL_I2S_DISABLE(&haudio_in_i2sext);

  /* I2S peripheral configuration */
  haudio_i2s.Init.AudioFreq = AudioFreq;
  haudio_i2s.Init.ClockSource = I2S_CLOCK_PLL;
  haudio_i2s.Init.CPOL = I2S_CPOL_LOW;
  haudio_i2s.Init.DataFormat = I2S_DATAFORMAT_16B;
  haudio_i2s.Init.MCLKOutput = I2S_MCLKOUTPUT_ENABLE;
  haudio_i2s.Init.Mode = I2S_MODE_MASTER_TX;
  haudio_i2s.Init.Standard = I2S_STANDARD_PHILIPS;
  haudio_i2s.Init.FullDuplexMode = I2S_FULLDUPLEXMODE_ENABLE;
  /* Init the I2S */
  HAL_I2S_Init(&haudio_i2s);

  /* I2Sext peripheral configuration */
  haudio_in_i2sext.Init.AudioFreq = AudioFreq;
  haudio_in_i2sext.Init.ClockSource = I2S_CLOCK_PLL;
  haudio_in_i2sext.Init.CPOL = I2S_CPOL_HIGH;
  haudio_in_i2sext.Init.DataFormat = I2S_DATAFORMAT_16B;
  haudio_in_i2sext.Init.MCLKOutput = I2S_MCLKOUTPUT_ENABLE;
  haudio_in_i2sext.Init.Mode = I2S_MODE_SLAVE_RX;
  haudio_in_i2sext.Init.Standard = I2S_STANDARD_PHILIPS;

  /* Init the I2Sext */
  HAL_I2S_Init(&haudio_in_i2sext);

 /* Enable I2S block */
  __HAL_I2S_ENABLE(&haudio_i2s);
  __HAL_I2S_ENABLE(&haudio_in_i2sext);
}

/**
  * @brief  Deinitializes the Audio Codec audio interface (I2S).
  * @param  None
  * @retval None
  */
static void I2Sx_In_DeInit(void)
{
  /* Initialize the hAudioInI2s Instance parameter */
  haudio_i2s.Instance = AUDIO_IN_I2Sx;

 /* Disable I2S block */
  __HAL_I2S_DISABLE(&haudio_i2s);

  /* DeInit the I2S */
  HAL_I2S_DeInit(&haudio_i2s);

  /* Initialize the hAudioInI2s Instance parameter */
  haudio_in_i2sext.Instance = I2S3ext;

 /* Disable I2S block */
  __HAL_I2S_DISABLE(&haudio_in_i2sext);

  /* DeInit the I2S */
  HAL_I2S_DeInit(&haudio_in_i2sext);
}

/**
  * @}
  */ 
  
/**
  * @}
  */

/**
  * @}
  */

/**
  * @}
  */ 

/************************ (C) COPYRIGHT STMicroelectronics *****END OF FILE****/
