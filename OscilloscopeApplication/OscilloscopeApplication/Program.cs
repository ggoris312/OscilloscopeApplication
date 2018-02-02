using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace OscilloscopeConnection
{
    class Program
    {
        private static SerialPort m_port;
        private static ConnectManager  m_connectmanager;
        private static byte[] databuff, waveformbuff;

        public static void SetBufferSize()
        {
            databuff = new byte[0x200008];
            waveformbuff = new byte[0x200008];
        }

        private static void programStart()
        {
            Console.WriteLine("Press Q to exit the program. Please press any other key to start.");
            SetBufferSize();
            while (true)
            {
                if(Console.ReadKey().Key == ConsoleKey.Q)
                {
                    Console.WriteLine("Program exiting");
                    return;
                }
                //startDataCollection();
                getPeakVoltage();
            }

        }

        private static void getPeakVoltage()
        {
            Console.WriteLine("Will start the task");

            var connectManager = ConnectManager.GetConnectManager();

            Console.WriteLine("Getting Data.");
            string str2 = "C2:PAVA? PKPK";
            m_connectmanager.WriteStrCmd(str2);
            //var num = m_connectmanager.ReadUsbtmcData(ref databuff);
            string result;
            result = m_connectmanager.ReadCommand();
            Console.WriteLine(result);
        }

        private static  void startDataCollection()
        {
            Console.WriteLine("Will start the task");
            var cmd = "WFSU TYPE,1";
            var connectManager = ConnectManager.GetConnectManager();
            Console.WriteLine("Getting Data.");
            string str2 = "C1:WF? ALL";
            m_connectmanager.WriteStrCmd(str2);
            var num = m_connectmanager.ReadUsbtmcData(ref databuff);
            if (databuff != null)
            {
                var num4 = num;
                var num5 = 0x16f;
                var len = (num4 - num5) - 2;
                Console.Write("{0}, {1}", num4, len);
                waveformbuff = new byte[len];
                var buffer2 = new byte[] { 0x84 };
                var index = 0;
                var num8 = 0;
                for (index = num5; index < (num4 - 2); index++)
                {
                    num8 = index - num5;
                    waveformbuff[num8] = (byte)(databuff[index] + buffer2[0]);
                }
                setWaveForm(ref waveformbuff, len);
            }
        }

        public static void setWaveForm(ref byte[] buff, int len)
        {
            Console.WriteLine("Parsing the data");
            var vdiv = 1f;
            var num2 = 0f;
            var num3 = 0f;
            var num4 = 0f;
            var num5 = 0f;
            var num6 = 0f;
            var num7 = 0f;
            vdiv = getvdiv();
            num7 = getzerolin(vdiv);
            float[] wavedata = new float[len];
            for (var i = 0; i < len; i++)
            {
                wavedata[i] = (((buff[i] - ((num7 / vdiv) * 25f)) - 132f) * (vdiv / 25f)) + num7;
                Console.WriteLine("interval {0}: {1} : {2}", i, wavedata[i], buff[i]);
            }
        }

        private static float getvdiv()
        {
            var cmd = "C1:VDIV?";
            var responseString = "";
            var num = 0f;
            m_connectmanager.WriteStrCmd(cmd);
            if (m_connectmanager.ReadStrFromDevice(out responseString) == -1)
            {
                return 0f;
            }
            return Convert.ToSingle(getdatastr(responseString));
        }

        private static float getzerolin(float vdiv)
        {
            var cmd = "C1:OFST? ";
            var num = 0f;
            var responseString = "";
            m_connectmanager.WriteStrCmd(cmd);
            if (m_connectmanager.ReadStrFromDevice(out responseString) == -1)
            {
                return 0f;
            }
            return Convert.ToSingle(getdatastr(responseString));
        }

        private static string getdatastr(string str)
        {
            var chArray = new char[2];
            var str2 = "";
            chArray[0] = ' ';
            chArray[1] = ',';
            var index = 0;
            var str3 = "";
            str.TrimEnd(new[] { '\n' });
            str.TrimEnd(new[] { '\r' });
            if (str.IndexOf(chArray[1], 0) == -1)
            {
                index = str.IndexOf(chArray[0], 0);
                str3 = str.Substring(index + 1);
                str2 = getdigitstr(str3);
            }
            return str2;
        }

        private static string getdigitstr(string str)
        {
            var str2 = "";
            var chArray = new[] { 'e', 'E', 'm', 'M', 'k', 'K', 'V' };
            var index = 0;
            index = str.IndexOf(chArray[0], 0);
            var num3 = 0;
            num3 = str.IndexOf(chArray[1], 0);
            str.IndexOf(chArray[2], 0);
            str.IndexOf(chArray[3], 0);
            str.IndexOf(chArray[4], 0);
            str.IndexOf(chArray[5], 0);
            if ((index != -1) || (num3 != -1))
            {
                var startIndex = 0;
                startIndex = str.IndexOf(chArray[6], 0);
                if (startIndex != -1)
                {
                    str = str.Remove(startIndex, 1);
                    str2 = double.Parse(str).ToString();
                }
            }
            return str2;
        }

        private static bool setUpSerialPort()
        {
            string[] ports = SerialPort.GetPortNames();
            Console.WriteLine("We will first set up the Serial port");

            if(ports.Length <= 0)
            {
                Console.WriteLine("No ports found, please connect the serial device");
                Console.WriteLine("Press any key to retry after serial is plug in");

                return false;
            }
            Console.WriteLine("The following serial ports were found:");
            // Display each port name to the console.
            for (int i = 0; i < ports.Length; i++)
            {
                Console.WriteLine("{0}: {1}", i, ports[i]);
            }

            Console.WriteLine("\nPlease Select the desired port");

            while (true)
            {
                var inputString = Console.ReadLine();
                if (int.TryParse(inputString, out int number1))
                {
                    if (number1 < ports.Length)
                    {
                        m_port = new SerialPort(ports[number1], 9600, Parity.None, 8, StopBits.One);
                        break;
                    }
                    Console.WriteLine("Please Select a number that is not out of bounce");

                }
                else
                {
                    Console.WriteLine("Please Select a number that is an option");
                }
            }
            m_port.Open();
            return true;
        }

        private static bool setUpOscilloscope()
        {
            Console.WriteLine("\n The next set is to set up the oscilloscope");
            m_connectmanager = ConnectManager.GetConnectManager();

            int length = m_connectmanager.FindSrc();
            if (length == 0)
            {
                Console.WriteLine("No device is Connected to USB");
                return false;
            }
            for(int j = 0; j < m_connectmanager.resources.Length; j++)
            {
                Console.WriteLine("{0}: {1}", j, m_connectmanager.resources[j]);
            }

            Console.WriteLine("\nPlease select one of the USB devices listed");

            while (true)
            {
                var inputString = Console.ReadLine();
                if (int.TryParse(inputString, out int number1))
                {
                    if (number1 < m_connectmanager.resources.Length)
                    {
                        var i = m_connectmanager.OpenSession(m_connectmanager.resources[number1]);
                        if (i == -1)
                        {
                            Console.WriteLine("There was an error connecting to the device");
                            return false;
                        }
                        break;
                    }
                    Console.WriteLine("Please Select a number that is not out of bounce");

                }
                else
                {
                    Console.WriteLine("Please Select a number that is an option");
                }
            }
            return true;
        }

        static void Main(string[] args)
        {
            /*
            while (true)
            {
                if (setUpSerialPort())
                {
                    break;
                }
            }
            */

            while (true)
            {
                if (setUpOscilloscope())
                {
                    break;
                }
            }

            programStart();

            
            return;

        }
    }
}
