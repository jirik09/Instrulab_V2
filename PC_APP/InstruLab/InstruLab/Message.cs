using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LEO
{
    public class Message
    {
        public enum MsgRequest { FIND_DEVICES, CONNECT_DEVICE, DISCONNECT, SCOPE_NEW_DATA, SCOPE_TRIGGERED,
            SCOPE_WAIT, SCOPE_FREQ, GEN_OK, GEN_NEXT, GEN_FRQ, GEN_ERR, VOLT_NEW_DATA, BODE_NEW_DATA,
            BODE_START_MEAS, COUNTER_ETR_DATA, COUNTER_IC1_DATA, COUNTER_IC2_DATA, COUNTER_REF_DATA,
            COUNTER_ETR_BUFFER, COUNTER_IC1_BUFF_WAIT, COUNTER_IC2_BUFF_WAIT, COUNTER_REF_WAIT,
            COUNTER_REF_WARN, COUNTER_TI_TIMEOUT, COUNTER_TI_EQUAL, COUNTER_TI_BIGGER_BUF1,
            COUNTER_TI_BIGGER_BUF2, COUNTER_IC1_DUTY_CYCLE, COUNTER_IC1_PULSE_WIDTH,
            NULL_MESSAGE
        }
        private MsgRequest type;
        private int num;
        private string msg;
        private double flt;

        public Message(MsgRequest type, string msg)
        {
            this.type = type;
            this.msg = msg;
        }

        public Message(MsgRequest type)
        {
            this.type = type;
        }

        public Message(MsgRequest type, int num)
        {
            this.type = type;
            this.num = num;
        }

        public Message(MsgRequest type, string msg, int num)
        {
            this.msg = msg;
            this.type = type;
            this.num = num;
        }

        public Message(MsgRequest type, string msg, double flt)
        {
            this.msg = msg;
            this.type = type;
            this.flt = flt;
        }


        public MsgRequest GetRequest() {
            return this.type;
        }

        public string GetMessage() {
            return this.msg;
        }

        public int GetNum() {
            return this.num;
        }

        public double GetFlt()
        {
            return this.flt;
        }
    }
}
