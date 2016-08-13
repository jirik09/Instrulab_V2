using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;


namespace InstruLab
{
    class Comms_thread
    {
        //promenne pro pripojeni a spravu devices
        private bool connected = false;
        public int progress = 0;

        private int numberOfPorts = 0;
        private List<Device> devices = new List<Device>();
        private Queue<Message> comms_q = new Queue<Message>();
        Message messg;
        private Device connectedDevice;
        private bool newDevices = false;
        private bool run_th = true;
        SerialPort serialPort;
        public enum CommsStates {IDLE,FINDING,FOUND_DEVS,NO_DEV_FOUND,DEVICES_READ,CONNECTING,CONNECTED,ERROR,DISCONNECTED}
        private CommsStates commState = CommsStates.IDLE;


        public void run()
        {
            while (run_th)
            {
                if (comms_q.Count > 0)
                {
                    messg = comms_q.Dequeue();
                    switch (messg.GetRequest())
                    {
                        case Message.MsgRequest.FIND_DEVICES:
                            clearListOfDevices();
                            find_devices(115200, 1, 0);
                            break;
                        case Message.MsgRequest.CONNECT_DEVICE:
                            connect_device(messg.GetMessage());
                            break;
                    }
                }
                if (connectedDevice!=null && connectedDevice.isPortError())
                {
                    commState = CommsStates.ERROR;
                }
                Thread.Sleep(10);
            }
        }

        public void stop()
        {
            this.run_th = false;
        }



        // nalezne vsechna pripojena zarizeni a da je do listu
        public void find_devices(int speed, double mull, double offset)
        {
            this.commState = CommsStates.FINDING;
           
            numberOfPorts = 0;

            if (!connected)
            {
                serialPort = new SerialPort();
                serialPort.ReadBufferSize = 128 * 1024;
                serialPort.BaudRate = speed;

                string[] devList = SerialPort.GetPortNames();
                string[] uniqueDevList = devList.Distinct().ToArray();

                foreach (string s in uniqueDevList)
                {
                    numberOfPorts++;
                }

                int counter = 0;
                foreach (string serial in uniqueDevList)
                {
                    counter++;
                    progress = (int)((double)counter  * 100 / numberOfPorts*mull + offset*100);
                    try
                    {
                        Thread.Yield();
                        serialPort.PortName = serial;
                        serialPort.ReadTimeout = 5000;
                        serialPort.WriteTimeout = 1000;
                        serialPort.Open();
                        serialPort.Write(Commands.IDNRequest+";");
                        Thread.Sleep(250);

                        char[] msg = new char[256];
                        int toRead = serialPort.BytesToRead;

                        serialPort.Read(msg, 0, toRead);
                        string msgInput = new string(msg, 0, 4);
                        string deviceName = new string(msg, 4, toRead - 4);

                        Thread.Yield();
                        if (msgInput.Equals(Commands.ACKNOWLEDGE))
                        {
                            bool inList=false;
                            Device newDevice = new Device(serialPort.PortName, deviceName, serialPort.BaudRate);
                            foreach (var item in devices)
                            {
                                if (item.Equals(newDevice)) {
                                    inList = true;
                                }
                            }
                            if (!inList)
                            {
                                devices.Add(new Device(serialPort.PortName, deviceName, serialPort.BaudRate));
                            }
                        }
                        serialPort.Close();
                        serialPort.Dispose();
                    }
                    catch (Exception ex)
                    {
                        if (serialPort.IsOpen)
                        {
                            serialPort.Close();
                        }
                        Console.WriteLine(ex);
                    }
                }
                if (devices.Count > 0)
                {
                    newDevices = true;
                    this.commState = CommsStates.FOUND_DEVS;
                }
                else {
                    this.commState = CommsStates.NO_DEV_FOUND;
                }
                
            }
        }

        public void clearListOfDevices()
        {
            devices.Clear();
        }

        public CommsStates get_comms_state() {
            return this.commState;
        }

        public int get_progress(){
            return this.progress;
        }
        public int get_num_of_devices()
        {
            return this.devices.Count;
        }



        public string[] get_dev_names()
        {
            string[] result = new string[devices.Count()];
            newDevices = false;
            int i = 0;
            foreach (Device d in devices)
            {
                result[i] = d.get_port()+": "+d.get_name();
                i++;
            }
            this.commState = CommsStates.DEVICES_READ; //access from different thread (can cause some issue:( )
            return result;
        }

        public string get_connected_device_port() {
            return this.connectedDevice.get_port();
        }

        public Device get_connected_device()
        {
            return this.connectedDevice;
        }

        public void connect_device(string port)
        {
            this.commState = CommsStates.CONNECTING;
            foreach (Device d in devices)
            {
                if (port.Equals(d.get_port()))
                {
                    this.connectedDevice = d;
                    break;
                }
            }

            if (connectedDevice.open_port())
            {
                this.commState = CommsStates.CONNECTED;
            }
            else {
                this.commState = CommsStates.ERROR;
            }

        }

        public void disconnect_device() {

            connectedDevice.close_gen();
            connectedDevice.close_scope();
            connectedDevice.close_volt();
            connectedDevice.close_port();
            this.commState = CommsStates.DISCONNECTED;
        }

        public void add_message(Message msg) {
            this.comms_q.Enqueue(msg);
        }


        



    }
}
