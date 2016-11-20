/*
  *****************************************************************************
  * @file    commands.h
  * @author  Y3288231
  * @date    Dec 15, 2014
  * @brief   This file contains functions for parsing commands
  ***************************************************************************** 
*/ 
#ifndef COMMANDS_H_
#define COMMANDS_H_
// Constant definitions =======================================================
// Types definitions ==========================================================
typedef  uint32_t command;


#define STR_ACK "ACK_"
#define STR_NACK "NACK"
#define STR_ERR "ERR_"

#define STR_SCOPE_OK "S_OK"
#define STR_GEN_OK "G_OK"
#define STR_GEN_NEXT "G_NX"



// Macro definitions ==========================================================
#define STRINGIFY(str) #str
#define BUILD_CMD(a) ((a[3] << 24)|(a[2] << 16)|(a[1] << 8)|(a[0]))

//#define STRINGIFY(str) #str
//#define BUILD_CMD(a) ((a[3] << 24)|(a[2] << 16)|(a[1] << 8)|(a[0]))

#define SWAP_UINT16(x) (((x & 0xff00) >> 8) | ((x & 0x00ff) << 8))
#define SWAP_UINT32(x) ( (x&0xff000000)>>24 | (x&0x00ff0000)>>8 | (x&0x0000ff00)<<8 | (x&0x000000ff)<<24 )


#define REGISTER_CMD(name,value) CMD_##name=(int)BUILD_CMD(STRINGIFY(value))

//Command definitions
//Common commands
enum{
REGISTER_CMD(IDN,IDN?),
REGISTER_CMD(VERSION,VER?),
REGISTER_CMD(GET_CONFIG,CFG?),
REGISTER_CMD(GET_REAL_FREQ,FRQ?),

REGISTER_CMD(SCOPE,OSCP),
REGISTER_CMD(GENERATOR,GEN_),
REGISTER_CMD(COMMS,COMS),
REGISTER_CMD(SYSTEM,SYST),	
	
REGISTER_CMD(ERR,ERR_),
REGISTER_CMD(ACK,ACK_),
REGISTER_CMD(NACK,NACK),
REGISTER_CMD(END,END_),

//Scope specific commands flags
REGISTER_CMD(SCOPE_TRIG_MODE,TRIG),
REGISTER_CMD(SCOPE_TRIG_EDGE,EDGE),
REGISTER_CMD(SCOPE_SAMPLING_FREQ,FREQ),
REGISTER_CMD(SCOPE_DATA_LENGTH,LENG),  //number of samples
REGISTER_CMD(SCOPE_TRIG_LEVEL,LEVL),
REGISTER_CMD(SCOPE_TRIG_CHANNEL,TRCH),
REGISTER_CMD(SCOPE_DATA_DEPTH,DATA),
REGISTER_CMD(SCOPE_CHANNELS,CHAN),
REGISTER_CMD(SCOPE_PRETRIGGER,PRET),
REGISTER_CMD(SCOPE_START,STRT),
REGISTER_CMD(SCOPE_STOP,STOP),
REGISTER_CMD(SCOPE_NEXT,NEXT),

REGISTER_CMD(SCOPE_ADC_CHANNEL_SET,A_CH),
REGISTER_CMD(SCOPE_ADC_CHANNEL_SET_DEFAULT,ADEF),
REGISTER_CMD(SCOPE_ADC_CHANNEL_SET_VREF,AREF),




REGISTER_CMD(GEN_DATA,DATA),
REGISTER_CMD(GEN_SAMPLING_FREQ,FREQ),
REGISTER_CMD(GEN_OUTBUFF_ON,B_ON),
REGISTER_CMD(GEN_OUTBUFF_OFF,B_OF),

REGISTER_CMD(GEN_DAC_VAL,DAC_),

//REGISTER_CMD(GEN_DATA_LENGTH,LENG),   //number of samples
REGISTER_CMD(GEN_DATA_LENGTH_CH1,LCH1),
REGISTER_CMD(GEN_DATA_LENGTH_CH2,LCH2),
REGISTER_CMD(GEN_CHANNELS,CHAN),
REGISTER_CMD(GEN_START,STRT),
REGISTER_CMD(GEN_STOP,STOP),


};


//Scope tigger modes
enum{
REGISTER_CMD(MODE_NORMAL,NORM),
REGISTER_CMD(MODE_AUTO,AUTO),
REGISTER_CMD(MODE_AUTO_FAST,F_A_),
REGISTER_CMD(MODE_SINGLE,SING)
};

#define isScopeTrigMode(CMD) (((CMD) == CMD_MODE_NORMAL) || \
                              ((CMD) == CMD_MODE_AUTO) || \
															((CMD) == CMD_MODE_AUTO_FAST) || \
                              ((CMD) == CMD_MODE_SINGLE))	

//Scope trigger edges
enum{
REGISTER_CMD(EDGE_RISING,RISE),
REGISTER_CMD(EDGE_FALLING,FALL)
};

#define isScopeTrigEdge(CMD) (((CMD) == CMD_EDGE_RISING) || \
                              ((CMD) == CMD_EDGE_FALLING))	

//Scope sampling frequencies
enum{
REGISTER_CMD(FREQ_1K,1K__),
REGISTER_CMD(FREQ_2K,2K__),
REGISTER_CMD(FREQ_5K,5K__),
REGISTER_CMD(FREQ_10K,10K_),
REGISTER_CMD(FREQ_20K,20K_),
REGISTER_CMD(FREQ_50K,50K_),
REGISTER_CMD(FREQ_100K,100K),
REGISTER_CMD(FREQ_200K,200K),
REGISTER_CMD(FREQ_500K,500K),
REGISTER_CMD(FREQ_1M,1M__),
REGISTER_CMD(FREQ_2M,2M__),
REGISTER_CMD(FREQ_5M,5M__),
REGISTER_CMD(FREQ_10M,10M_),
REGISTER_CMD(FREQ_MAX,MAX_),
};

#define isScopeFreq(CMD) (((CMD) == CMD_FREQ_1K) || \
                          ((CMD) == CMD_FREQ_2K) || \
                          ((CMD) == CMD_FREQ_5K) || \
                          ((CMD) == CMD_FREQ_10K) || \
                          ((CMD) == CMD_FREQ_20K) || \
                          ((CMD) == CMD_FREQ_50K) || \
                          ((CMD) == CMD_FREQ_100K) || \
                          ((CMD) == CMD_FREQ_200K) || \
                          ((CMD) == CMD_FREQ_500K) || \
                          ((CMD) == CMD_FREQ_1M) || \
                          ((CMD) == CMD_FREQ_2M) || \
                          ((CMD) == CMD_FREQ_5M) || \
													((CMD) == CMD_FREQ_10M) || \
                          ((CMD) == CMD_FREQ_MAX))	
													
//Scope data lengths
enum{
REGISTER_CMD(SAMPLES_100,100_),	
REGISTER_CMD(SAMPLES_200,200_),	
REGISTER_CMD(SAMPLES_500,500_),	
REGISTER_CMD(SAMPLES_1K,1K__),	
REGISTER_CMD(SAMPLES_2K,2K__),
REGISTER_CMD(SAMPLES_5K,5K__),
REGISTER_CMD(SAMPLES_10K,10K_),
REGISTER_CMD(SAMPLES_20K,20K_),
REGISTER_CMD(SAMPLES_50K,50K_),
REGISTER_CMD(SAMPLES_100K,100K)
};
#define isScopeNumOfSamples(CMD) (((CMD) == CMD_SAMPLES_100) || \
																((CMD) == CMD_SAMPLES_200) || \
                                ((CMD) == CMD_SAMPLES_500) || \
                                ((CMD) == CMD_SAMPLES_1K) || \
                                ((CMD) == CMD_SAMPLES_2K) || \
                                ((CMD) == CMD_SAMPLES_5K) || \
                                ((CMD) == CMD_SAMPLES_10K) || \
                                ((CMD) == CMD_SAMPLES_20K) || \
																((CMD) == CMD_SAMPLES_50K) || \
																((CMD) == CMD_SAMPLES_100K))	


//Scope Data depths
enum{
REGISTER_CMD(DATA_DEPTH_12B,12B_),	
REGISTER_CMD(DATA_DEPTH_10B,10B_),
REGISTER_CMD(DATA_DEPTH_8B,8B__),
REGISTER_CMD(DATA_DEPTH_6B,6B__)
};
#define isScopeDataDepth(CMD) (((CMD) == CMD_DATA_DEPTH_12B) || \
                               ((CMD) == CMD_DATA_DEPTH_10B) || \
                               ((CMD) == CMD_DATA_DEPTH_8B) || \
                               ((CMD) == CMD_DATA_DEPTH_6B))	
															 
//Number of channels
enum{
REGISTER_CMD(CHANNELS_1,1CH_),
REGISTER_CMD(CHANNELS_2,2CH_),
REGISTER_CMD(CHANNELS_3,3CH_),
REGISTER_CMD(CHANNELS_4,4CH_)
};
#define isChannel(CMD) (((CMD) == CMD_CHANNELS_1) || \
                              ((CMD) == CMD_CHANNELS_2) || \
															((CMD) == CMD_CHANNELS_3) || \
                              ((CMD) == CMD_CHANNELS_4))		

// Exported variables =========================================================
// Exported functions =========================================================



#endif /* COMMANDS_H_ */
