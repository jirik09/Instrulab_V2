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
typedef uint32_t command;


#define STR_ACK "ACK_"
#define STR_NACK "NACK"
#define STR_ERR "ERR_"

#define STR_SCOPE_OK "S_OK"
#define STR_GEN_OK "G_OK"
#define STR_GEN_NEXT "G_NX"

#ifdef USE_COUNTER
#define STR_CNT_ETR_DATA "ETRD"		// data from ETR measurement
#define STR_CNT_ETR_BUFF "ETRB"		// buffer itself

#define STR_CNT_REF_DATA "REFD"		// data from REF measurement
#define STR_CNT_REF_WARN "WARN"		// reference counter sample count warning

#define STR_CNT_IC1_DATA "IC1D"		// data from IC1 channel meas.
#define STR_CNT_IC2_DATA "IC2D"		// data from IC2 channel meas.

#define STR_CNT_TI_DATA  "TIDA"		// data from TI
#define STR_CNT_TI_TIMEOUT "TMOT"	// Timeout occured

#define STR_CNT_DUTY_CYCLE  "DUT1"
#define STR_CNT_PULSE_WIDTH "PWD1"
#endif //USE_COUNTER

#ifdef USE_LOG_ANLYS
#define STR_LOG_ANLYS_TRIGGER_POINTER "LATP"
#define STR_LOG_ANLYS_DATA_LENGTH "LADL"
#define STR_LOG_ANLYS_DATA "LADT"
#endif //USE_LOG_ANLYS


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
//Query commands
REGISTER_CMD(IDN,IDN?),
REGISTER_CMD(VERSION,VER?),
REGISTER_CMD(GET_CONFIG,CFG?),
REGISTER_CMD(GET_REAL_FREQ,FRQ?),
REGISTER_CMD(GET_INPUTS,INP?),
REGISTER_CMD(IS_SHIELD,SH_?),	
REGISTER_CMD(RESET_DEVICE,RES!),
	
//Peripherals access commands
REGISTER_CMD(SCOPE,OSCP),
REGISTER_CMD(GENERATOR,GEN_),
REGISTER_CMD(SYNC_PWM,SYNP),	
REGISTER_CMD(LOG_ANLYS,LOGA),	
REGISTER_CMD(COUNTER,CNT_),
REGISTER_CMD(COMMS,COMS),
REGISTER_CMD(SYSTEM,SYST),	

//Communication flags	
REGISTER_CMD(ERR,ERR_),
REGISTER_CMD(ACK,ACK_),
REGISTER_CMD(NACK,NACK),
REGISTER_CMD(END,END_),

/*******************************************************/
/******************** Scope commands *******************/
/*******************************************************/
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

/*******************************************************/
/****************** Generator commands *****************/
/*******************************************************/
//Generator specific commands
REGISTER_CMD(GEN_MODE,MODE),
REGISTER_CMD(GEN_DATA,DATA),
REGISTER_CMD(GEN_SAMPLING_FREQ,FREQ),
REGISTER_CMD(GEN_OUTBUFF_ON,B_ON),
REGISTER_CMD(GEN_OUTBUFF_OFF,B_OF),

//REGISTER_CMD(GEN_DATA_LENGTH,LENG),   // number of samples
REGISTER_CMD(GEN_DATA_LENGTH_CH1,LCH1),
REGISTER_CMD(GEN_DATA_LENGTH_CH2,LCH2),
REGISTER_CMD(GEN_CHANNELS,CHAN),
REGISTER_CMD(GEN_START,STRT),
REGISTER_CMD(GEN_STOP,STOP),
REGISTER_CMD(GEN_RESET,RSET),

/*******************************************************/
/**************** PWM generator commands ***************/
/*******************************************************/
//PWM generator specific commands
REGISTER_CMD(GET_PWM_CONFIG,PCF?),

REGISTER_CMD(GEN_PWM_DEINIT,GPDI),	 	// Deinitialize PWM generator.

REGISTER_CMD(GEN_PWM_FREQ_PSC,FPWP),	// setting PWM frequency by configuration of timer's PSC register
REGISTER_CMD(GEN_PWM_FREQ_ARR,FPWA),	// setting PWM frequency by configuration of timer's ARR register

REGISTER_CMD(GEN_DAC_VAL,DAC_),

/*******************************************************/
/************* Sync PWM generator commands *************/
/*******************************************************/
REGISTER_CMD(SYNC_PWM_COMMAND,SCOM),

REGISTER_CMD(SYNC_PWM_CHAN_CONFIG,CCON),
REGISTER_CMD(SYNC_PWM_CHAN_NUM,CNUM),

REGISTER_CMD(SYNC_PWM_FREQ,SFRQ),				// Set frequency command
REGISTER_CMD(SYNC_PWM_CHAN_STATE,SSTA),	// Channel state

REGISTER_CMD(SYNC_PWM_STEP,STEP),				// Step mode command (init, deinit)

/*******************************************************/
/*************** Logic Analyzer commands ***************/
/*******************************************************/
REGISTER_CMD(LOG_ANLYS_START,STRT),
REGISTER_CMD(LOG_ANLYS_STOP,STOP),
REGISTER_CMD(LOG_ANLYS_INIT,INIT),
REGISTER_CMD(LOG_ANLYS_DEINIT,DEIN),

REGISTER_CMD(LOG_ANLYS_POSTTRIG,POST),
REGISTER_CMD(LOG_ANLYS_PRETRIG,PRET),

REGISTER_CMD(LOG_ANLYS_SAMPLING_FREQ,SMPF),
REGISTER_CMD(LOG_ANLYS_SAMPLES_NUM,SMPN),

REGISTER_CMD(LOG_ANLYS_TRIGGER_MODE,TRGM),
REGISTER_CMD(LOG_ANLYS_TRIGGER_EVENT,TRGE),
REGISTER_CMD(LOG_ANLYS_TRIGGER_CHANNEL,TRGC),

/*******************************************************/
/******************* Counter commands ******************/
/*******************************************************/
//General commands
REGISTER_CMD(CNT_MODE,MODE),						// CNT_MODE command be of three values: MODE == ETR / IC / REF
REGISTER_CMD(CNT_START,STRT),
REGISTER_CMD(CNT_STOP,STOP),
REGISTER_CMD(CNT_DEINIT,DEIN),

//Counter ETR commands
REGISTER_CMD(CNT_GATE,GATE),

//Counter IC commands
REGISTER_CMD(CNT_SAMPLE_COUNT1,BUF1),
REGISTER_CMD(CNT_SAMPLE_COUNT2,BUF2),

REGISTER_CMD(CNT_PRESC1,PRE1),
REGISTER_CMD(CNT_PRESC2,PRE2),

REGISTER_CMD(CNT_DUTY_CYCLE,DUCY),			// Command to initialize/deinitialize duty cycle measurement under input capture mode (Low Frequency measuring)

//Counter IC and TI commands
REGISTER_CMD(CNT_EVENT,EVNT),
REGISTER_CMD(CNT_TIMEOUT_TIM,TIMO),

//Counter TI commands
REGISTER_CMD(CNT_TI_MODE,TIMD),

//Counter REF commands
REGISTER_CMD(CNT_MULT_PSC,PSC_),						
REGISTER_CMD(CNT_MULT_ARR,ARR_),				// set PSC x ARR number of ticks to count from reference clock
};


//Counter modes
enum{
REGISTER_CMD(MODE_ETR,ETR_),
REGISTER_CMD(MODE_IC,IC__),
REGISTER_CMD(MODE_REF,REF_),
REGISTER_CMD(MODE_TI,TI__)
};

#define isCounterMode(CMD) (((CMD) == CMD_MODE_ETR) || \
                           ((CMD) == CMD_MODE_IC) || \
													 ((CMD) == CMD_MODE_REF) || \
													 ((CMD) == CMD_MODE_TI))

//Counter ETR sampling times
enum{
REGISTER_CMD(GATE_100m,100m),
REGISTER_CMD(GATE_500m,500m),
REGISTER_CMD(GATE_1s,1s__),
REGISTER_CMD(GATE_5s,5s__),	
REGISTER_CMD(GATE_10s,10s_)
};

#define isCounterEtrGate(CMD) (((CMD) == CMD_GATE_100m) || \
															((CMD) == CMD_GATE_500m) || \
															((CMD) == CMD_GATE_1s) || \
															((CMD) == CMD_GATE_5s) || \
															((CMD) == CMD_GATE_10s))

//Counter IC prescaler 1
enum{
REGISTER_CMD(PRESC1_1x,1x__),
REGISTER_CMD(PRESC1_2x,2x__),
REGISTER_CMD(PRESC1_4x,4x__),
REGISTER_CMD(PRESC1_8x,8x__),
};

#define isCounterIcPresc1(CMD) (((CMD) == CMD_PRESC1_1x) || \
																((CMD) == CMD_PRESC1_2x) || \
																((CMD) == CMD_PRESC1_4x) || \
																((CMD) == CMD_PRESC1_8x))		

//Counter IC prescaler 2
enum{
REGISTER_CMD(PRESC2_1x,1x__),
REGISTER_CMD(PRESC2_2x,2x__),
REGISTER_CMD(PRESC2_4x,4x__),
REGISTER_CMD(PRESC2_8x,8x__),
};

#define isCounterIcPresc2(CMD) (((CMD) == CMD_PRESC2_1x) || \
																((CMD) == CMD_PRESC2_2x) || \
																((CMD) == CMD_PRESC2_4x) || \
																((CMD) == CMD_PRESC2_8x))		

//Counter IC pulse mode change configuration + TI (time interval)
enum{
REGISTER_CMD(EVENT_RF1,RF1_),  // Rising Falling event channel 1 (Pulse measurement channel 1 init)
REGISTER_CMD(EVENT_RF2,RF2_),  // Rising Falling event channel 2 (Pulse measurement channel 2 init) PULSE_PLI2
REGISTER_CMD(EVENT_RO1,RO1_),  // Rising only event channel 1 (Pulse measurement channel 1 deinit)
REGISTER_CMD(EVENT_RO2,RO2_),  // Rising only event channel 2 (Pulse measurement channel 2 deinit)
REGISTER_CMD(EVENT_FO1,FO1_),  // Falling only event channel 1
REGISTER_CMD(EVENT_FO2,FO2_),	 // Falling only event channel 2
REGISTER_CMD(EVENT_SEQ_AB,SQAB),
REGISTER_CMD(EVENT_SEQ_BA,SQBA),
};

#define isCounterIcTiEvent(CMD) (((CMD) == CMD_EVENT_RF1) || \
																((CMD) == CMD_EVENT_RF2) || \
																((CMD) == CMD_EVENT_RO1) || \
																((CMD) == CMD_EVENT_RO2)	|| \
																((CMD) == CMD_EVENT_FO1)	|| \
																((CMD) == CMD_EVENT_FO2)	|| \
																((CMD) == CMD_EVENT_SEQ_AB)	|| \
																((CMD) == CMD_EVENT_SEQ_BA))

//Counter IC duty cycle init/deinit
enum{
REGISTER_CMD(DUTY_CYCLE_INIT_CH1,DCI1), 
REGISTER_CMD(DUTY_CYCLE_INIT_CH2,DCI2), 
REGISTER_CMD(DUTY_CYCLE_DEINIT_CH1,DCD1), 
REGISTER_CMD(DUTY_CYCLE_DEINIT_CH2,DCD2),
REGISTER_CMD(DUTY_CYCLE_ENABLE,DCE_), 
REGISTER_CMD(DUTY_CYCLE_DISABLE,DCX_), 
};

#define isCounterIcDutyCycle(CMD) (((CMD) == CMD_DUTY_CYCLE_INIT_CH1) || \
																	((CMD) == CMD_DUTY_CYCLE_INIT_CH2) || \
																	((CMD) == CMD_DUTY_CYCLE_DEINIT_CH1) || \
																	((CMD) == CMD_DUTY_CYCLE_DEINIT_CH2) || \
																	((CMD) == CMD_DUTY_CYCLE_ENABLE) || \
																	((CMD) == CMD_DUTY_CYCLE_DISABLE))

//Counter TI mode
enum{
REGISTER_CMD(MODE_EVENT_SEQUENCE_DEP,SEQD),
REGISTER_CMD(MODE_EVENT_SEQUENCE_INDEP,SEQI),
};

#define isCounterTiMode(CMD) (((CMD) == CMD_MODE_EVENT_SEQUENCE_DEP) || \
															((CMD) == CMD_MODE_EVENT_SEQUENCE_INDEP))		


//Generator modes (NORMAL - DAC, ABNORMAL - PWM)
enum{
REGISTER_CMD(MODE_PWM,PWM_),
REGISTER_CMD(MODE_DAC,DAC_),
};

#define isGeneratorMode(CMD) (((CMD) == CMD_MODE_PWM) || \
															((CMD) == CMD_MODE_DAC))

//Sync PWM general commands
enum{
REGISTER_CMD(SYNC_PWM_INIT,INIT),
REGISTER_CMD(SYNC_PWM_DEINIT,DINI),
REGISTER_CMD(SYNC_PWM_START,STRT),
REGISTER_CMD(SYNC_PWM_STOP,STOP),
};

#define isSyncPwm(CMD) (((CMD) == CMD_SYNC_PWM_INIT) || \
												((CMD) == CMD_SYNC_PWM_DEINIT) || \
												((CMD) == CMD_SYNC_PWM_START) || \
												((CMD) == CMD_SYNC_PWM_STOP))						

//Sync PWM general commands
enum{
REGISTER_CMD(SYNC_PWM_STEP_ENABLE,STEE),
REGISTER_CMD(SYNC_PWM_STEP_DISABLE,STED),
};

#define isSyncPwmStepMode(CMD) (((CMD) == CMD_SYNC_PWM_STEP_ENABLE) || \
																((CMD) == CMD_SYNC_PWM_STEP_DISABLE))		

//Logic analyzer trigger event.
enum{
REGISTER_CMD(TRIG_MODE_AUTO,AUTO),
REGISTER_CMD(TRIG_MODE_NORMAL,NORM),
REGISTER_CMD(TRIG_MODE_SINGLE,SING),
};

#define isLogAnlysTriggerMode(CMD) (((CMD) == CMD_TRIG_MODE_AUTO) || \
																		((CMD) == CMD_TRIG_MODE_NORMAL) || \
																		((CMD) == CMD_TRIG_MODE_SINGLE))

//Logic analyzer trigger event.
enum{
REGISTER_CMD(TRIG_EDGE_RISING,RISE),
REGISTER_CMD(TRIG_EDGE_FALLING,FALL),
};

#define isLogAnlysTriggerEvent(CMD) (((CMD) == CMD_TRIG_EDGE_RISING) || \
																		((CMD) == CMD_TRIG_EDGE_FALLING))		
	

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
