#region

using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using NationalInstruments.VisaNS;
using Parity = System.IO.Ports.Parity;

#endregion

namespace OscilloscopeConnection
{
    internal class ConnectManager
    {
        private static string decivename = "";
        private static string decivetype = "";
        private static ConnectManager m_ConnectManager;
        private static SerialPort m_SerialPort;
        private static MessageBasedSession mbSession;
        private static string searilNo = "";
        private static string softNo = "";
        private Parity SerialParity;
        private int baudRate;
        private bool bsessionopen;
        private string comportname = "";
        private int m_connectType = -1;
        private int mdatabit;
        private string resoucename;
        public string[] resources;
        private int setupfile_len;
        private StopBits stopbit;

        private ConnectManager()
        {
        }

        public int CloseSession()
        {
            try
            {
                mbSession.Terminate();
                mbSession.Dispose();
            }
            catch (Exception)
            {
                return -1;
            }
            bsessionopen = false;
            m_ConnectManager = null;

            return 0;
        }

        public void Delay(int delaytime)
        {
            var tickCount = GetTickCount();
            var flag = true;
            while (flag)
            {
                var num2 = GetTickCount() - tickCount;
                if (num2 > delaytime)
                {
                    flag = false;
                }
            }
        }

        public int FindSrc()
        {
            try
            {
                ResourceManager localManager;
                try
                {
                    localManager = ResourceManager.GetLocalManager();
                }
                catch (Exception)
                {
                    return -5;
                }
                resources = localManager.FindResources("USB?*INSTR");
                
                var length = resources.Length;
            }
            catch (InvalidCastException)
            {
                return -4;
            }
            catch (DllNotFoundException)
            {
                return -6;
            }
            catch (NullReferenceException)
            {
                return -7;
            }
            catch (VisaException)
            {
                return -2;
            }
            catch (Exception)
            {
                return -3;
            }
            return resources.Length;
        }

        public static ConnectManager GetConnectManager()
        {
            if (m_ConnectManager == null)
            {
                m_ConnectManager = new ConnectManager();
            }
            return m_ConnectManager;
        }

        public int GetConnectType()
        {
            return m_connectType;
        }

        public string GetDeviceType()
        {
            return decivetype;
        }

        [DllImport("Kernel32.dll")]
        public static extern uint GetTickCount();

        public bool isconnected()
        {
            return bsessionopen;
        }

        public int OpenSession(string scoperesources)
        { 
                try
                {
                    mbSession = (MessageBasedSession) ResourceManager.GetLocalManager().Open(scoperesources);
                }
                catch (Exception)
                {
                    return -1;
                }
                resoucename = scoperesources;
                bsessionopen = true;
                mbSession.Timeout = 0x2710;
            
            return 0;
        }

        public int ReaddatabyNetWork(ref byte[] pdatabuff)
        {
            var length = 0;
            var sourceArray = new byte[0x200008];
            try
            {
                sourceArray = mbSession.ReadByteArray();
            }
            catch (Exception)
            {
                MessageBox.Show("Read trace data from device has problem,please check the device");
            }
            var destinationArray = new byte[9];
            Array.Copy(sourceArray, 12, destinationArray, 0, 9);
            Array.Reverse(destinationArray);
            var num2 = 0.0;
            for (var i = 0; i < 9; i++)
            {
                var num4 = 0.0;
                var num5 = (destinationArray[i] - 0x30) & 0xff;
                num4 = num5*Math.Pow(10.0, i);
                num2 += num4;
            }
            length = ((int) num2) + 2;
            Array.Copy(sourceArray, 0x15, pdatabuff, 0, length);
            return length;
        }

        public int ReadStrFromDevice(out string responseString)
        {
            var num = 0;
            responseString = "";
            try
            {
                responseString = mbSession.ReadString();
                
            }
            catch (Exception)
            {
                MessageBox.Show("Read string result from device has problem,please check the device");
                num = -1;
            }
            if (responseString == "")
            {
                MessageBox.Show("Read string result from device is null,please check the device or device setting ");
                num = -1;
            }
            return num;
        }

        public void ReadUsbtmcBmpData(ref byte[] databuff, int len)
        {
            if ((m_connectType == 1) || (m_connectType == 2))
            {
                try
                {
                    // Reading until the buffer is exausted
                    var buff = new List<Byte>();
                    do
                    {
                        buff.AddRange(mbSession.ReadByteArray());
                    } while (mbSession.LastStatus == VisaStatusCode.SuccessMaxCountRead);
                    databuff = buff.ToArray();
                }
                catch (Exception)
                {
                    MessageBox.Show("Read BMP data from device has problem,please check the device");
                }
            }
            else if (m_connectType == 0)
            {
                try
                {
                    if (len > 0x2800)
                    {
                        var num = len/0x2800;
                        var buffer = new byte[0x2800];
                        for (var i = 0; i < num; i++)
                        {
                            m_SerialPort.Read(buffer, 0, 0x2800);
                            buffer.CopyTo(databuff, (i*0x2800));
                            Delay(0xbb8);
                        }
                        var count = len - (num*0x2800);
                        m_SerialPort.Read(databuff, num*0x2800, count);
                        Delay(0xbb8);
                    }
                    else
                    {
                        m_SerialPort.Read(databuff, 0, len);
                        Delay(0xbb8);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Read BMP data from device has problem,please check the device");
                }
            }
        }

        public int ReadUsbtmcData(ref byte[] databuff)
        {
            var countToRead = 0;
           
            var buffer = new byte[0x40];
            var buffer2 = new byte[9];
            try
            {
                buffer = mbSession.ReadByteArray(12);
                buffer2 = mbSession.ReadByteArray(9);        
            }
            catch (Exception)
            {
                    Console.WriteLine("Read trace data from device has problem,please check the device");
            }
            Array.Reverse(buffer2);
            var num2 = 0.0;
            for (var i = 0; i < 9; i++)
            {
                var num4 = 0.0;
                var num5 = (buffer2[i] - 0x30) & 0xff;
                num4 = num5*Math.Pow(10.0, i);
                num2 += num4;
            }
              
            countToRead = ((int) num2) + 2;
               
            if (countToRead <= 0)
            {
                return countToRead;
            }
            try
            {
                   
                databuff = mbSession.ReadByteArray(countToRead);
                return countToRead;
            }
            catch (Exception)
            {
                Console.WriteLine("Read trace data from device has problem,please check the device");
                return countToRead;
            }
        }

        public int ReadUsbtmcSetup(ref byte[] databuff)
        {
            var num = 0;
            if ((m_connectType == 1) || (m_connectType == 2))
            {
                try
                {
                    databuff = mbSession.ReadByteArray();
                }
                catch (Exception)
                {
                    MessageBox.Show("Read setup file data from device has problem,please check the device");
                }
                return databuff.Length;
            }
            if (m_connectType != 0)
            {
                return num;
            }
            try
            {
                m_SerialPort.Read(databuff, 0, 0x2800);
            }
            catch (Exception)
            {
                MessageBox.Show("Read setup file data from device has problem,please check the device");
            }
            Delay(0x3e8);
            var destinationArray = new byte[9];
            Array.Copy(databuff, 7, destinationArray, 0, 9);
            Array.Reverse(destinationArray);
            var num2 = 0.0;
            for (var i = 0; i < 9; i++)
            {
                var num4 = 0.0;
                var num5 = (destinationArray[i] - 0x30) & 0xff;
                num4 = num5*Math.Pow(10.0, i);
                num2 += num4;
            }
            setupfile_len = ((int) num2) + 0x11;
            return setupfile_len;
        }

        public void RS232_Device_Setting(string comname, int baudrate, int databit, int Paritytype, int stopbittype)
        {
            comportname = comname;
            baudRate = baudrate;
            mdatabit = databit;
            if (Paritytype == 0)
            {
                SerialParity = Parity.None;
            }
            if (stopbittype == 0)
            {
                stopbit = StopBits.One;
            }
        }

        public void SetConnectDeviceName(string str)
        {
            decivename = str;
        }

        public void SetConnectDeviceType(string str)
        {
            decivetype = str;
        }

        public void SetConnecType(int type)
        {
            m_connectType = type;
        }

        public void SetsearilNo(string str)
        {
            searilNo = str;
        }

        public void SetSetupfileLen(int Length)
        {
            setupfile_len = Length;
        }

        public void SetsoftNo(string str)
        {
            softNo = str;
        }

        private uint Verify_Databuff_Get_VerifyNum(char[] pBuff, int buff_len)
        {
            uint num2 = 0;
            for (uint i = 0; i < buff_len; i++)
            {
                num2 += pBuff[i];
            }
            num2 = ~num2;
            return (num2 + 1);
        }

        public void WriteBufferData(ref byte[] databuff)
        {
            if (decivetype == "ATTEN")
            {
                var s = "PNSU";
                var str2 = searilNo + softNo + s;
                var pBuff = str2.ToCharArray();
                var length = str2.Length;
                var str3 = Verify_Databuff_Get_VerifyNum(pBuff, length).ToString("X");
                s = (str3.Substring(6, 2) + str3.Substring(4, 2)) + str3.Substring(2, 2) + str3.Substring(0, 2);
                var bytes = Encoding.Default.GetBytes(s);
                if ((m_connectType == 1) || (m_connectType == 2))
                {
                    var num3 = databuff.Length + 8;
                    var destinationArray = new byte[num3];
                    Array.Copy(bytes, destinationArray, bytes.Length);
                    Array.Copy(databuff, 0, destinationArray, bytes.Length, databuff.Length);
                    try
                    {
                        mbSession.Write(destinationArray);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Write setup file data from device has problem,please check the device");
                    }
                }
                else if (m_connectType == 0)
                {
                    var count = setupfile_len + 8;
                    var buffer3 = new byte[count];
                    Array.Copy(bytes, buffer3, bytes.Length);
                    Array.Copy(databuff, 0, buffer3, bytes.Length, setupfile_len);
                    try
                    {
                        m_SerialPort.Write(buffer3, 0, count);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Write setup file data from device has problem,please check the device");
                    }
                }
            }
            else if ((m_connectType == 1) || (m_connectType == 2))
            {
                try
                {
                    mbSession.Write(databuff);
                }
                catch (Exception)
                {
                    MessageBox.Show("Write setup file data from device has problem,please check the device");
                }
            }
            else if (m_connectType == 0)
            {
                try
                {
                    m_SerialPort.Write(databuff, 0, setupfile_len);
                }
                catch (Exception)
                {
                    MessageBox.Show("Write setup file data from device has problem,please check the device");
                }
            }
        }

        public string ReadCommand()
        {
            string result = mbSession.ReadString();
            return result;
        }

        public bool WriteStrCmd(string cmd)
        {
            var flag = true;
            cmd = cmd.ToUpper();
            if ((decivetype == "ATTEN") && (cmd != "*IDN?"))
            {
                var str = searilNo + softNo + cmd;
                var pBuff = str.ToCharArray();
                var length = str.Length;
                var str2 = Verify_Databuff_Get_VerifyNum(pBuff, length).ToString("X");
                cmd = (str2.Substring(6, 2) + str2.Substring(4, 2) + str2.Substring(2, 2)) + str2.Substring(0, 2) + cmd;
            }
            try
            {
                cmd = cmd + "\n";
               
                    mbSession.Write(cmd);
                    return flag;
                
            }
            catch (Exception)
            {
                MessageBox.Show("Write cmd has problem,please check the cmd string or the device");
                flag = false;
            }
            return flag;
        }
    }
}