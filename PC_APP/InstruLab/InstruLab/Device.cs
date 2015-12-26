﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;

namespace InstruLab
{
    public class Device
    {
        public struct SystemConfig_def
        {
            public int CoreClock;
            public int PeriphClock;
            public string MCU;
            public string FW_Version;
            public string FREE_RTOS_Version;
            public string HAL_Version;
        }

        public struct CommsConfig_def
        {
            public int bufferLength;
            public int UartSpeed;
            public string TX_pin;
            public string RX_pin;
            public bool useUsb;
            public string DP_pin;
            public string DM_pin;
        }

        public struct ScopeConfig_def
        {
            public bool isScope;
            public int maxSamplingFrequency;
            public int realSmplFreq;
            public int maxBufferLength;
            public int maxNumChannels;
            public string[] pins;
            public int VRef;
            public int munRanges;
            public int[,] ranges;
            public byte[] buffer;
            public UInt16[,] samples;
            public double[] timeBase;
            public Scope.mode_def mode;
            public double maxTime;
            public int sampligFreq;
            public int actualChannels;
            public int actualRes;
        }

        public struct GeneratorConfig_def
        {
            public bool isGen;
            public int maxSamplingFrequency;
            public int BufferLength;
            public int dataDepth;
            public int numChannels;
            public string[] pins;
            public int VRef;
        }

        private SerialPort port;
        private string portName;
        private string name;
        private string mcu;
        private int speed;

        public SystemConfig_def systemCfg;
        public CommsConfig_def commsCfg;
        public ScopeConfig_def scopeCfg;
        public GeneratorConfig_def genCfg;


        private StreamWriter logWriter;
        private const bool writeLog = true;

        Scope Scope_form;
        Generator Gen_form;
        SynchronizationContext syncContext;

        static Semaphore commsSemaphore = new Semaphore(1,1);  // Dostupná kapacita=1; Celková=1
        static Semaphore logSemaphore = new Semaphore(1, 1);  // Dostupná kapacita=1; Celková=1


        public Device(string portName, string name, int speed)
        {
            syncContext= SynchronizationContext.Current;
            this.portName = portName;
            this.name = name;
            this.speed = speed;
        }

        public string get_processor()
        {
            return this.mcu;
        }

        public string get_name()
        {
            return this.name;
        }

        public string get_port()
        {
            return this.portName;
        }

        public void close_port() {
            if (port.IsOpen)
            {
                logTextNL("PORT zavřen: " + this.portName);
                logWriter.Close();

                port.Close();
                port.Dispose();
            }
        }

        public bool open_port()
        {
            bool result = false;
            try
            {
                this.port = new SerialPort();
                this.port.PortName = portName;
                this.port.ReadBufferSize = 128*1024;
                this.port.BaudRate = speed;
                port.Open();
                load_config();
                port.Close();
                port.ReadTimeout = 10000;
                port.WriteTimeout = 5000;
                port.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort_DataReceived);
                port.ErrorReceived += new System.IO.Ports.SerialErrorReceivedEventHandler(this.serialPort_ErrorReceived);
                port.Open();
                result= true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fatal error during connecting to device \r\n" + ex, "Fatal error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if(writeLog)
            {
                try
                {
                    bool logOpened = false;
                    int index = 1;
                    while (!logOpened)
                    {
                        try
                        {
                            logWriter = File.AppendText("logfile" + index + ".txt");
                            logOpened = true;
                        }
                        catch (Exception ex)
                        {
                            index++;
                        }
                    }
                    if (writeLog) { Log("PORT otevřen: " + portName + "  Baudrate:" + speed + "  Zařízení:" + name); }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fatal error during opening log file\r\n" + ex, "Fatal error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    
                    return false;
                }
            }
            return result;
        }

        public void load_config()
        {
            if (port.IsOpen)
            {
                int wait = 50;
                port.Write(Commands.SYSTEM + ":" + Commands.CONFIGRequest + ";");
                char[] msg_char = new char[256];
                byte[] msg_byte = new byte[256];
                Thread.Sleep(wait);                
                int toRead = port.BytesToRead;
                port.Read(msg_byte, 0, toRead);
                msg_char = System.Text.Encoding.ASCII.GetString(msg_byte).ToCharArray();

                if (new string(msg_char, 0, 4).Equals("SYST"))
                {
                    this.systemCfg.CoreClock = BitConverter.ToInt32(msg_byte, 4);
                    this.systemCfg.PeriphClock = BitConverter.ToInt32(msg_byte, 8);
                    this.systemCfg.MCU = new string(msg_char, 12, toRead-16);
                }

                port.Write(Commands.VersionRequest + ";");
                Thread.Sleep(wait);
                toRead = port.BytesToRead;
                port.Read(msg_byte, 0, toRead);
                msg_char = System.Text.Encoding.ASCII.GetString(msg_byte).ToCharArray();

                if (new string(msg_char, 0, 4).Equals("VER_"))
                {
                    this.systemCfg.FW_Version = new string(msg_char, 16,8);
                    this.systemCfg.FREE_RTOS_Version = new string(msg_char, 32,6);
                    this.systemCfg.HAL_Version = new string(msg_char, 44, 6);
                }


                port.Write(Commands.COMMS + ":" + Commands.CONFIGRequest + ";");
                Thread.Sleep(wait);
                toRead = port.BytesToRead;
                port.Read(msg_byte, 0, toRead);
                msg_char = System.Text.Encoding.ASCII.GetString(msg_byte).ToCharArray();

                if (new string(msg_char, 0, 4).Equals("COMM"))
                {
                    this.commsCfg.bufferLength = BitConverter.ToInt32(msg_byte, 4);
                    this.commsCfg.UartSpeed = BitConverter.ToInt32(msg_byte, 8);
                    this.commsCfg.TX_pin = new string(msg_char, 12,4);
                    this.commsCfg.RX_pin = new string(msg_char, 16,4);
                    if (toRead > 24)
                    {
                        this.commsCfg.DP_pin = new string(msg_char, 24, 4);
                        this.commsCfg.DM_pin = new string(msg_char, 28, 4);
                        this.commsCfg.useUsb = true;
                    }else{
                        this.commsCfg.useUsb = false;
                    }
                }

                port.Write(Commands.SCOPE + ":" + Commands.CONFIGRequest + ";");
                Thread.Sleep(wait);
                toRead = port.BytesToRead;
                port.Read(msg_byte, 0, toRead);
                msg_char = System.Text.Encoding.ASCII.GetString(msg_byte).ToCharArray();

                if (new string(msg_char, 0, 4).Equals("OSCP"))
                {
                    scopeCfg.isScope=true;
                    scopeCfg.maxSamplingFrequency = BitConverter.ToInt32(msg_byte, 4);
                    scopeCfg.maxBufferLength = BitConverter.ToInt32(msg_byte, 8);
                    scopeCfg.maxNumChannels = BitConverter.ToInt32(msg_byte, 12);
                    scopeCfg.pins=new string[scopeCfg.maxNumChannels];
                    for (int i = 0; i < this.scopeCfg.maxNumChannels; i++) {
                        scopeCfg.pins[i] = new string(msg_char, 16+4*i, 4);
                    }
                    scopeCfg.VRef = BitConverter.ToInt32(msg_byte, 16 + 4 * scopeCfg.maxNumChannels);

                    scopeCfg.munRanges = (toRead - 24 - 4 * scopeCfg.maxNumChannels) / 4;
                    scopeCfg.ranges= new int[2,scopeCfg.munRanges];
                    for (int i = 0; i < this.scopeCfg.munRanges; i++)
                    {
                        scopeCfg.ranges[0, i] = BitConverter.ToInt16(msg_byte, 20 + 4 * scopeCfg.maxNumChannels + 4 * i);
                        scopeCfg.ranges[1, i] = BitConverter.ToInt16(msg_byte, 22 + 4 * scopeCfg.maxNumChannels + 4 * i);
                    }
                    

                }else{
                    scopeCfg.isScope=false;
                }

                port.Write(Commands.GENERATOR + ":" + Commands.CONFIGRequest + ";");
                Thread.Sleep(wait);
                toRead = port.BytesToRead;
                port.Read(msg_byte, 0, toRead);
                msg_char = System.Text.Encoding.ASCII.GetString(msg_byte).ToCharArray();

                if (new string(msg_char, 0, 4).Equals("GEN_"))
                {
                    genCfg.isGen=true;
                    genCfg.maxSamplingFrequency = BitConverter.ToInt32(msg_byte, 4);
                    genCfg.BufferLength = BitConverter.ToInt32(msg_byte, 8);
                    genCfg.dataDepth = BitConverter.ToInt32(msg_byte, 12);
                    genCfg.numChannels = BitConverter.ToInt32(msg_byte, 16);
                    genCfg.pins = new string[genCfg.numChannels];
                    for (int i = 0; i < this.genCfg.numChannels; i++)
                    {
                        genCfg.pins[i] = new string(msg_char, 20 + 4 * i, 4);
                    }
                    genCfg.VRef = BitConverter.ToInt32(msg_byte, 20 + 4 * genCfg.numChannels);
                }else{
                    genCfg.isGen=false;
                }
            }
        }

        public void wait_for_data(int watch) 
        {
            Thread.Sleep(1);
            if (watch <= 0) { 
               // throw (new Exception("Reading timeout")); 
            } 
        } 

        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            char[] inputMsg = new char[4];
            byte[] inputData = new byte[4];
            int watchDog = 250;
            if (writeLog) { logTextNL("Příjem dat: " + port.BytesToRead.ToString()); }
            
            while (port.IsOpen && port.BytesToRead > 0)
            {
                
              /* inputMsg[0] = inputMsg[1]; //safe for un aligned messages
                inputMsg[1] = inputMsg[2];
                inputMsg[2] = inputMsg[3];
                inputMsg[3] = (char)port.ReadChar();*/

                try
                {
                    while (port.BytesToRead < 4)
                    {
                        wait_for_data(watchDog--);
                        if (watchDog < 0)
                        {
                            port.Read(inputMsg, 0, 2);
                        }
                    }
                    port.Read(inputMsg, 0, 4);
                    switch (new string(inputMsg, 0, 4))
                    {
                        case Commands.SCOPE_INCOME:
                            int res;
                            int leng;
                            int currChan;
                            int numChan;
                            int i = 0;
                            int partsLen = 4096;
                            while (port.BytesToRead < 12)
                            {
                                wait_for_data(watchDog--);
                            }
                            port.Read(inputData, 0, 4);
                            scopeCfg.realSmplFreq=inputData[0] * 256 * 256 * 256 + inputData[1] * 256 * 256 + inputData[2] * 256 + inputData[3];
                            res = port.ReadByte();
                            leng = port.ReadByte() * 65536 + port.ReadByte() * 256 + port.ReadByte();
                            port.Read(inputData, 0, 2);
                            currChan = port.ReadByte();
                            numChan = port.ReadByte();
                            scopeCfg.buffer = new Byte[leng];
                            int wasRead = 0;
                            int toRead = (leng - wasRead) > partsLen ? partsLen : (leng - wasRead);

                            while (toRead > 0)
                            {
                               ///// if (port.BytesToRead <= leng + wasRead)
                               /// {
                                    wasRead += port.Read(scopeCfg.buffer, wasRead, Math.Min(port.BytesToRead,toRead));
                                ///}
                                ///else {
                                 ///   wasRead += port.Read(scopeCfg.buffer, wasRead, toRead);
                               /// }
                                
                                toRead = (leng - wasRead) > partsLen ? partsLen : (leng - wasRead);
                            }


                            if (res > 8) //resolution >8 bits
                            {
                                if (currChan == 1)
                                {
                                    scopeCfg.samples = new UInt16[numChan, leng / 2];
                                }
                                for (i = 0; i < leng / 2; i++)
                                {
                                    scopeCfg.samples[currChan - 1, i] = BitConverter.ToUInt16(scopeCfg.buffer, i * 2);
                                }
                            }
                            else  //resolution <=8 bits
                            {
                                if (currChan == 1)
                                {
                                    scopeCfg.samples = new UInt16[numChan, leng];
                                }
                                for (i = 0; i < leng; i++)
                                {
                                    scopeCfg.samples[currChan - 1, i] = scopeCfg.buffer[i];
                                }
                            }


                            if (currChan == numChan)
                            {
                                if (res > 8)
                                {
                                    scopeCfg.timeBase = new double[leng / 2];
                                    generate_time_base(scopeCfg.realSmplFreq, leng / 2);
                                }
                                else
                                {
                                    scopeCfg.timeBase = new double[leng];
                                    generate_time_base(scopeCfg.realSmplFreq, leng);
                                }
                                scopeCfg.actualChannels = numChan;
                                scopeCfg.actualRes = res;
                                if (writeLog) { logRecieved("SCOPE DATA RECIEVED: Leng " + leng + ", Res " + res + ", Chan " + currChan + " of " + numChan); }
                                Scope_form.add_message(new Message(Message.MsgRequest.SCOPE_NEW_DATA));
                            }
                            //Console.WriteLine("SCOPE DATA RECIEVED: Leng "+leng+", Res "+res+", Chan "+currChan+" of "+numChan);
                            break;

                        case Commands.ACKNOWLEDGE:
                            if (writeLog) { logRecieved("ACK"); }
                            //Console.WriteLine(Commands.ACKNOWLEDGE);
                            break;
                        case Commands.SCOPE_OK:
                            if (writeLog) { logRecieved("S_OK"); }
                            //Console.WriteLine(Commands.ACKNOWLEDGE);
                            break;
                        case Commands.SAMPLING:
                            if (writeLog) { logRecieved("SMPL"); }
                            //Console.WriteLine(Commands.ACKNOWLEDGE);
                            break;
                        case Commands.TRIGGERED:
                            //Console.WriteLine(Commands.TRIGGERED);
                            if (writeLog) { logRecieved("TRIG"); }
                            Scope_form.add_message(new Message(Message.MsgRequest.SCOPE_TRIGGERED));
                            break;
                        case Commands.GEN_OK:
                            //Console.WriteLine(Commands.TRIGGERED);
                            if (writeLog) { logRecieved("OK"); }
                            Gen_form.add_message(new Message(Message.MsgRequest.GEN_OK));
                            break;
                        case Commands.GEN_NEXT:
                            //Console.WriteLine(Commands.TRIGGERED);
                            if (writeLog) { logRecieved("NEXT"); }
                            Gen_form.add_message(new Message(Message.MsgRequest.GEN_NEXT));
                            break;
                        case Commands.GENERATOR:
                            while (port.BytesToRead < 8)
                            {
                                wait_for_data(watchDog--);
                            }
                            port.Read(inputMsg, 0, 4);
                            port.Read(inputData, 0, 4);
                            Gen_form.add_message(new Message(Message.MsgRequest.GEN_FRQ,new string(inputMsg,0,4),inputData[1]*256*256+inputData[2]*256+inputData[3]));
                            //Console.WriteLine(Commands.TRIGGERED);
                            if (writeLog) { logRecieved("GEN_FRQ?" + new string(inputMsg, 1, 3) + " CH" + inputData[3].ToString()); }
                            break;
                        default:
                            if (inputMsg[0] == Commands.ERROR)
                            {
                                try
                                {
                                    int err = int.Parse(new string(inputMsg, 1, 3));
                                    if (writeLog) { logRecieved("ERROR " + err); }
                                    MessageBox.Show("Error recieved \r\n" + new string(inputMsg, 0, 4) + "\r\n" + getErrReason(err), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    //scopeCfg.mode = Scope.mode_def.IDLE;
                                }
                                catch (Exception ex)
                                {
                                    if (writeLog) { logRecieved("Unknown message " + new string(inputMsg, 0, 4)); }
                                    MessageBox.Show("Unknow Error recieved \r\n" + new string(inputMsg, 0, 4), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    //Console.WriteLine(new string(inputMsg, 0, 4));
                                }
                            }
                            else {
                                if (writeLog) { logRecieved("Unknown message " + new string(inputMsg, 0, 4)); }
                                    MessageBox.Show("Unknow message recieved \r\n" + new string(inputMsg, 0, 4), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    //Console.WriteLine(new string(inputMsg, 0, 4));
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    if (port.IsOpen)
                    {
                        port.DiscardInBuffer();
                        MessageBox.Show("Error recieved \r\n" + ex, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                Thread.Yield();
            }
        }

        private void serialPort_ErrorReceived(object sender, System.IO.Ports.SerialErrorReceivedEventArgs e)
        {
            if (writeLog) { logTextNL("Serial port error recieved:\r\n"); }
        }



        private void generate_time_base(int sampling, int lenght)
        {
            for (int i = 0; i < lenght; i++)
            {
                scopeCfg.timeBase[i] = (double)(i) / sampling;
            }
            scopeCfg.maxTime = (double)(lenght) / sampling;
        }

        public void open_scope() {
            if (Scope_form == null || Scope_form.IsDisposed)
            {
                Scope_form = new Scope(this);
                Scope_form.Show();
            }
            else
            {
                Scope_form.BringToFront();
            }
        }

        public void close_scope() {
            if (Scope_form != null)
            {
                Scope_form.Close();
            }
        }

        public void open_gen()
        {
            if (Gen_form == null || Gen_form.IsDisposed)
            {
                Gen_form = new Generator(this);
                Gen_form.Show();
            }
            else
            {
                Gen_form.BringToFront();
            }
        }

        public void close_gen()
        {
            if (Gen_form != null)
            {
                Gen_form.Close();
            }
        }
        
        public SystemConfig_def getSystemCfg() {
            return this.systemCfg;
        }
        public CommsConfig_def getCommsCfg()
        {
            return this.commsCfg;
        }


        public void set_scope_mode(Scope.mode_def mod) {
            this.scopeCfg.mode = mod;
        }

        public Scope.mode_def get_scope_mode() {
            return scopeCfg.mode;
        }

        
        public bool takeCommsSemaphore(int ms){
            bool result = false;
            result = commsSemaphore.WaitOne(ms);
            if (!result) {
                throw new Exception("Unable to take semaphore");
            }
            return result;
        }

        public void giveCommsSemaphore(){
            commsSemaphore.Release();
        }


        public void send(string s)
        {
            try
            {
                port.Write(s);

                //   if (!s.Equals("OSCP:SRAT")) {
                logSend(s);
                // }
                //  Console.WriteLine(s);
            }
            catch (Exception ex)
            {
                if (writeLog) { logTextNL("Data se nepodařilo odeslat:\r\n" + ex); }
                Console.WriteLine(ex);
            }
        }

        public void send_short_2byte(int l)
        {
            byte[] bt = BitConverter.GetBytes(l);
            byte[] se = new byte[2];
            se[0] = bt[0];
            se[1] = bt[1];
            logText(l.ToString("D4") + "(0x" + BitConverter.ToString(se, 0).Replace("-", "") + ")");
            port.Write(bt, 0, 2);
            // Console.WriteLine(l.ToString());
        }


        public void send_short(int l)
        {
            byte[] bt = BitConverter.GetBytes(l);
            byte[] se = new byte[4];
            se[0] = 0;
            se[1] = 0;
            se[2] = bt[0];
            se[3] = bt[1];
            logTextNL(l.ToString() + "(0x" + BitConverter.ToString(se, 0).Replace("-", "") + ")");
            port.Write(bt, 0, 4);
           // Console.WriteLine(l.ToString());
        }

        public void send_int(int l)
        {
            byte[] bt = BitConverter.GetBytes(l);
            byte[] se = new byte[4];

            se[0] = bt[0];
            se[1] = bt[1];
            se[2] = bt[2];
            se[3] = bt[3];
            logTextNL(l.ToString() + "(0x" + BitConverter.ToString(se, 0).Replace("-", "") + ")");
            port.Write(se, 0, 4);
           // Console.WriteLine(l.ToString());
        }

        public void Log(string logMessage)
        {
            logSemaphore.WaitOne(1000);
            logWriter.Write("\r\nLog Entry : ");
            logWriter.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
            DateTime.Now.ToLongDateString());
            logWriter.WriteLine("  :{0}", logMessage);
            logWriter.WriteLine("-------------------------------");
            logSemaphore.Release();
        }

        public void LogEnd()
        {
            logSemaphore.WaitOne(1000);
            logWriter.WriteLine("Konec logu");
            logWriter.WriteLine("-------------------------------");
            logWriter.WriteLine("\r\n\r\n\r\n");
            logSemaphore.Release();
        }

        public void logSend(string s)
        {
            logSemaphore.WaitOne(1000);
            logWriter.Write("Send (" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "." + DateTime.Now.Millisecond + "): " + s + "\r\n");
            logSemaphore.Release();
        }
        public void logRecieved(string s)
        {
            logSemaphore.WaitOne(1000);
            logWriter.Write("Recieved (" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "." + DateTime.Now.Millisecond + "): " + s + "\r\n");
            logSemaphore.Release();
        }

        public void logTextNL(string s)
        {
            logSemaphore.WaitOne(1000);
            logWriter.Write(s+"\r\n");
            logSemaphore.Release();
        }

        public void logText(string s)
        {
            logSemaphore.WaitOne(1000);
            logWriter.Write(s + ", ");
            logSemaphore.Release();
        }

        public string getErrReason(int err)
        {
            string result = null;
            switch (err)
            {
                case 1:
                    result = "Reading configuration error";
                    break;
                case 50:
                    result = "Scope - Invalid feature";
                    break;
                case 54:
                    result = "Scope - Invalid param";
                    break;
                case 55:
                    result = "Scope - Unsuported resolution";
                    break;
                case 56:
                    result = "Scope - Invalid trigger channel";
                    break;
                case 57:
                    result = "Scope - Invalid sampling frequency";
                    break;
                case 58:
                    result = "Scope - Buffer size error";
                    break;
                case 100:
                    result = "Gen - Invalid feature";
                    break;
                case 101:
                    result = "Gen - Invalid state";
                    break;
                case 102:
                    result = "Gen - Writing data out of memory";
                    break;
                case 103:
                    result = "Gen - Buffer size error";
                    break;
                case 104:
                    result = "Gen - Missing data";
                    break;
                case 105:
                    result = "Gen - To high samling frequency";
                    break;
                case 107:
                    result = "Gen - Frequency is inaccurate";
                    break;
                case 108:
                    result = "Gen - Frequency mismatch";
                    break;
                case 109:
                    result = "Gen - Invalid data";
                    break;
                case 255:
                    result = "Unknown error";
                    break;
                case 533:
                    result = "Communication error";
                    break;
                case 999:
                    result = "Unsuported function";
                    break;
            }
            return result;
        }


        //end class
    }
}
