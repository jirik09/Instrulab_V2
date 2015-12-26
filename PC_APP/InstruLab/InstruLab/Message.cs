﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InstruLab
{
    public class Message
    {
        public enum MsgRequest { FIND_DEVICES, CONNECT_DEVICE, DISCONNECT,SCOPE_NEW_DATA, SCOPE_TRIGGERED, SCOPE_WAIT,SCOPE_FREQ,GEN_OK,GEN_NEXT,GEN_FRQ}
        private MsgRequest type;
        private int num;
        private string msg;

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


        public MsgRequest GetRequest() {
            return this.type;
        }

        public string GetMessage() {
            return this.msg;
        }

        public int GetNum() {
            return this.num;
        }
    }
}
