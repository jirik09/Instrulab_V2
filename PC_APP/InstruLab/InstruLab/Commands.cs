using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LEO
{
    class Commands
    {
        public const string IDN = "IDN_";

        /* COMMUNICATION */
        // General
        public const string IDNRequest = "IDN?";
        public const string VersionRequest = "VER?";
        public const string CONFIGRequest = "CFG?";
        public const string GET_REAL_SMP_FREQ = "FRQ?";
        public const string GET_SCOPE_INPUTS = "INP?";
        public const string IS_SHIELD_CONNECTED = "SH_?";

        public const string RESET_DEVICE = "RES!";

        public const string GENERATOR = "GEN_";        
        public const string COUNTER = "CNT_";
        public const string SCOPE = "OSCP";
        public const string SCOPE_INCOME = "OSC_";
        public const string COMMS = "COMS";
        public const string SYSTEM = "SYST";

        // Counter modes
        public const string CNT_MODE = "MODE";
        public const string CNT_ETR = "ETR_";
        public const string CNT_REF = "REF_";
        public const string CNT_IC = "IC__";

        // Counter complete deinit
        public const string DEINIT = "DEIN";

        // Counter ETR GATE times
        public const string CNT_GATE = "GATE";
        public const string CNT_GATE_100m = "100m";
        public const string CNT_GATE_500m = "500m";
        public const string CNT_GATE_1s = "1s__";
        public const string CNT_GATE_5s = "5s__";
        public const string CNT_GATE_10s = "10s_";

        // Counter IC SAMPLE counts
        public const string CNT_SAMPLE_COUNT1 = "BUF1";
        public const string CNT_SAMPLE_COUNT2 = "BUF2";

        // Counter REF SAMPLE count
        public const string CNT_SAMPLES_PSC = "PSC_";
        public const string CNT_SAMPLES_ARR = "ARR_";
        public const string COUNTER_REF_WARN = "WARN";

        // Counter messages received
        public const string COUNTER_ETR_DATA = "ETRD";
        public const string COUNTER_REF_DATA = "REFD";
        public const string COUNTER_IC1_DATA = "IC1D";
        public const string COUNTER_IC2_DATA = "IC2D";
        public const string COUNTER_IC1_BUFF = "IC1B";
        public const string COUNTER_IC2_BUFF = "IC2B";

        //general
        public const string ACKNOWLEDGE = "ACK_";
        public const char ERROR = 'E';

        public const string START = "STRT";
        public const string STOP = "STOP";        

        //scope
        public const string CHANNELS = "CHAN";
        public const string DATA_LENGTH = "LENG";  //number of samples

        public const string DATA_LENGTH_CH1 = "LCH1"; 
        public const string DATA_LENGTH_CH2 = "LCH2";

        public const string SAMPLING_FREQ = "FREQ";

        public const string TRIGGERED = "TRIG";
        public const string SAMPLING = "SMPL";
        public const string SCOPE_OK = "S_OK";
        //Scope specific commands flags
        public const string SCOPE_TRIG_MODE = "TRIG";
        public const string SCOPE_TRIG_EDGE = "EDGE";
        public const string SCOPE_TRIG_LEVEL = "LEVL";
        public const string SCOPE_TRIG_CHANNEL = "TRCH";
        public const string SCOPE_DATA_DEPTH = "DATA";
        public const string SCOPE_PRETRIGGER = "PRET";
        public const string SCOPE_NEXT = "NEXT";
        public const string SCOPE_FREQ = "OSCF";
        public const string SCOPE_ADC_CHANNEL = "A_CH";
        public const string SCOPE_ADC_CHANNEL_DEAFULT = "ADEF";
        public const string SCOPE_ADC_CHANNEL_VREF = "AREF";
        
        //Generator
        public const string GEN_DATA = "DATA";
        public const string GEN_PWM_FREQ_PSC = "FPWP";
        public const string GEN_PWM_FREQ_ARR = "FPWA";

        public const string GEN_OK = "G_OK";
        public const string GEN_NEXT = "G_NX";

        public const string GEN_BUFF_ON = "B_ON";
        public const string GEN_BUFF_OFF = "B_OF";

        public const string GEN_DAC_VAL = "DAC_";

        //Generator modes        
        public const string GEN_MODE = "MODE";
        public const string GEN_MODE_PWM = "PWM_";
        public const string GEN_MODE_DAC = "DAC_";        

        //scope modes
        public const string MODE_NORMAL = "NORM";
        public const string MODE_AUTO = "AUTO";
        public const string MODE_SINGLE = "SING";
        public const string MODE_AUTO_FAST = "F_A_";

        //Scope trigger edges

        public const string EDGE_RISING = "RISE";
        public const string EDGE_FALLING = "FALL";




        //Scope sampling frequencies

        public const string FREQ_1K = "1K__";
        public const string FREQ_2K = "2K__";
        public const string FREQ_5K = "5K__";
        public const string FREQ_10K = "10K_";
        public const string FREQ_20K = "20K_";
        public const string FREQ_50K = "50K_";
        public const string FREQ_100K = "100K";
        public const string FREQ_200K = "200K";
        public const string FREQ_500K = "500K";
        public const string FREQ_1M = "1M__";
        public const string FREQ_2M = "2M__";
        public const string FREQ_5M = "5M__";
        public const string FREQ_10M = "10M_";
        public const string FREQ_MAX = "MAX_";




        //Scope data lengths

        public const string SAMPLES_100 = "100_";
        public const string SAMPLES_200 = "200_";
        public const string SAMPLES_500 = "500_";
        public const string SAMPLES_1K = "1K__";
        public const string SAMPLES_2K = "2K__";
        public const string SAMPLES_5K = "5K__";
        public const string SAMPLES_10K = "10K_";
        public const string SAMPLES_20K = "20K_";
        public const string SAMPLES_50K = "50K_";
        public const string SAMPLES_100K = "100K";



        //Scope Data depths

        public const string DATA_DEPTH_12B = "12B_";
        public const string DATA_DEPTH_10B = "10B_";
        public const string DATA_DEPTH_8B = "8B__";
        public const string DATA_DEPTH_6B = "6B__";



        //Number of channels

        public const string CHANNELS_1 = "1CH_";
        public const string CHANNELS_2 = "2CH_";
        public const string CHANNELS_3 = "3CH_";
        public const string CHANNELS_4 = "4CH_";





/*
          REGISTER_CMD(IDN,IDN?),
REGISTER_CMD(GET_CONFIG,CFG?),

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

REGISTER_CMD(GEN_DATA,DATA),
REGISTER_CMD(GEN_SAMPLING_FREQ,FREQ),
REGISTER_CMD(GEN_GET_REAL_SMP_FREQ,FRQ?),
REGISTER_CMD(GEN_DATA_LENGTH,LENG),   //number of samples
REGISTER_CMD(GEN_CHANNELS,CHAN),
REGISTER_CMD(GEN_START,STRT),
REGISTER_CMD(GEN_STOP,STOP)
         REGISTER_CMD(MODE_NORMAL,NORM),
        REGISTER_CMD(MODE_AUTO,AUTO),
        REGISTER_CMD(MODE_SINGLE,SING)
        };

        #define isScopeTrigMode(CMD) (((CMD) == CMD_MODE_NORMAL) || \
                                      ((CMD) == CMD_MODE_AUTO) || \
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
        REGISTER_CMD(FREQ_10M,10M_)
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
                                  ((CMD) == CMD_FREQ_10M))	
													
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
 
         */

    }
}
