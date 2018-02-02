using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NationalInstruments.VisaNS;

namespace OscilloscopeConnection
{
    internal class ConnectionManager
    {
        private static ConnectionManager m_ConnectionManager;
        private static MessageBasedSession mbSession;
        public string[] resources;

        private ConnectionManager()
        {

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


        [DllImport("Kernel32.dll")]
        public static extern uint GetTickCount();

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
            
            m_ConnectionManager = null;

            return 0;
        }

        public static ConnectionManager getConnectionManager()
        {
            if(m_ConnectionManager == null)
            {
                m_ConnectionManager = new ConnectionManager();
            }

            return m_ConnectionManager;
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
    }
}
