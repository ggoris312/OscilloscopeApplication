namespace OscilloscopeConnection
{
    using System;
    using System.Runtime.InteropServices;

    internal class CWave
    {
        public string amp;
        public string Base;
        public string burstwidth;
        public string crms;
        public string fall_time;
        public string FPREshoot;
        public string frequency;
        public string h_div;
        public string h_start;
        public string h_stop;
        private int Hzchange;
        private static CWave m_wave;
        public string max;
        public string mean;
        private string[] measure_str;
        public string min;
        public string nduty;
        public string Nwidth;
        public string OVSNshoot;
        public string OVSPshoot;
        public string pduty;
        public string perid;
        public string pkpk;
        public string points;
        public string Pwidth;
        public string rise_time;
        public string rms;
        public string RPREshoot;
        private string[] timebase_str;
        private int timechange;
        public string top;
        public string v_div;
        public string v_offset;
        public string v_start;
        public string v_stop;
        private string[] vdiv_str;
        public string vmea;
        public float[] wavedata;
        public int wavelen;

        private CWave()
        {
        }

        private int convertdata(string v_div, out string unit)
        {
            string s = "";
            string[] strArray = new string[] { " " };
            int length = 0;
            length = v_div.IndexOf(strArray[0], 0);
            s = v_div.Substring(0, length);
            unit = v_div.Substring(length, v_div.Length - length);
            return int.Parse(s);
        }

        private string getdigitstr(string str)
        {
            double num = 0.0;
            string str2 = "0.0";
            string oldValue = "";
            string[] strArray = new string[] { "e", "E", "m", "M", "k", "K", "V", "S", "Hz", "%" };
            int index = 0;
            index = str.IndexOf(strArray[0], 0);
            if (index > 0)
            {
                oldValue = strArray[0];
            }
            int num3 = 0;
            num3 = str.IndexOf(strArray[1], 0);
            if (num3 > 0)
            {
                oldValue = strArray[1];
            }
            int num4 = 0;
            num4 = str.IndexOf(strArray[2], 0);
            if (num4 > 0)
            {
                oldValue = strArray[2];
            }
            int num5 = 0;
            num5 = str.IndexOf(strArray[3], 0);
            if (num5 > 0)
            {
                oldValue = strArray[3];
            }
            int num6 = 0;
            num6 = str.IndexOf(strArray[4], 0);
            if (num6 > 0)
            {
                oldValue = strArray[4];
            }
            int num7 = 0;
            num7 = str.IndexOf(strArray[5], 0);
            if (num7 > 0)
            {
                oldValue = strArray[5];
            }
            int num8 = 0;
            num8 = str.IndexOf(strArray[6], 0);
            if (num8 > 0)
            {
                oldValue = strArray[6];
            }
            int num9 = 0;
            num9 = str.IndexOf(strArray[7], 0);
            if (num9 > 0)
            {
                oldValue = strArray[7];
            }
            int num10 = 0;
            num10 = str.IndexOf(strArray[8], 0);
            if (num10 > 0)
            {
                oldValue = strArray[8];
            }
            int num11 = 0;
            num11 = str.IndexOf(strArray[9], 0);
            if (num11 > 0)
            {
                oldValue = strArray[9];
            }
            if ((((index == -1) && (num3 == -1)) && ((num4 == -1) && (num5 == -1))) && ((((num6 == -1) && (num7 == -1)) && ((num8 == -1) && (num9 == -1))) && ((num10 == -1) && (num11 == -1))))
            {
                return str2;
            }
            str = str.Replace(oldValue, "");
            if (!(str != "****"))
            {
                return str;
            }
            num = double.Parse(str);
            if (num8 != -1)
            {
                return num.ToString("F3");
            }
            if (num9 == -1)
            {
                if (num10 == -1)
                {
                    return num.ToString();
                }
                if (num >= 1000.0)
                {
                    while (num > 1000.0)
                    {
                        num /= 1000.0;
                        this.Hzchange++;
                    }
                    return num.ToString("F0");
                }
                return num.ToString("F0");
            }
            if (num <= 1.0)
            {
                while (num < 1.0)
                {
                    num *= 1000.0;
                    this.timechange++;
                }
                return num.ToString("F0");
            }
            return num.ToString("F0");
        }

        public static CWave Getwave()
        {
            m_wave = new CWave();
            return m_wave;
        }

        public void getwavedata_measure(string ch)
        {
            string cmd = ch + ":PAVA? ALL";
            string responseString = "";
            ConnectManager connectManager = ConnectManager.GetConnectManager();
            if (connectManager.isconnected())
            {
                bool flag2 = connectManager.WriteStrCmd(cmd);
                if (!flag2)
                {
                    int num = 0;
                    while (!flag2 && (num < 5))
                    {
                        num++;
                        flag2 = connectManager.WriteStrCmd(cmd);
                    }
                }
                if (flag2 && (connectManager.ReadStrFromDevice(out responseString) != -1))
                {
                    this.param_unpack(responseString);
                    this.set_measur_param();
                }
            }
        }

        private void MeasurDataPross(string resultstr)
        {
            string str = resultstr;
            int length = str.Length;
            string str2 = "";
            char[] chArray = new char[] { ' ', ',' };
            int index = 0;
            int num3 = 0;
            string str3 = "";
            index = str.IndexOf(chArray[1], 0);
            int num4 = 0;
            while (((length > 0) && (index > 0)) && (num4 < 0x17))
            {
                this.timechange = 0;
                this.Hzchange = 0;
                index = str.IndexOf(chArray[1], 0);
                if (index > 0)
                {
                    str3 = str.Substring(index + 1);
                    num3 = str3.IndexOf(chArray[1], 0);
                    if (num3 > 0)
                    {
                        str = str3.Substring(num3 + 1);
                        str3 = str3.Substring(0, num3);
                    }
                }
                else
                {
                    str3 = str;
                }
                str2 = this.getdigitstr(str3);
                if (num4 < 10)
                {
                    this.measure_str[num4] = str2 + "V";
                }
                else if (num4 < 14)
                {
                    this.measure_str[num4] = str2 + "%";
                }
                else if (num4 < 15)
                {
                    string str4 = "";
                    if (this.timechange == 0)
                    {
                        str4 = "S";
                    }
                    else if (this.timechange == 1)
                    {
                        str4 = "ms";
                    }
                    else if (this.timechange == 2)
                    {
                        str4 = "us";
                    }
                    else if (this.timechange == 3)
                    {
                        str4 = "ns";
                    }
                    else if (this.timechange == 4)
                    {
                        str4 = "ps";
                    }
                    this.measure_str[num4] = str2 + str4;
                }
                else if (num4 < 0x10)
                {
                    string str5 = "";
                    if (this.Hzchange == 0)
                    {
                        str5 = "Hz";
                    }
                    else if (this.Hzchange == 1)
                    {
                        str5 = "kHz";
                    }
                    else if (this.Hzchange == 2)
                    {
                        str5 = "MHz";
                    }
                    else if (this.Hzchange == 3)
                    {
                        str5 = "GHz";
                    }
                    else if (this.Hzchange == 4)
                    {
                        str5 = "THz";
                    }
                    this.measure_str[num4] = str2 + str5;
                }
                else if (num4 < 0x15)
                {
                    string str6 = "";
                    if (this.timechange == 0)
                    {
                        str6 = "S";
                    }
                    else if (this.timechange == 1)
                    {
                        str6 = "ms";
                    }
                    else if (this.timechange == 2)
                    {
                        str6 = "us";
                    }
                    else if (this.timechange == 3)
                    {
                        str6 = "ns";
                    }
                    else if (this.timechange == 4)
                    {
                        str6 = "ps";
                    }
                    this.measure_str[num4] = str2 + str6;
                }
                else if (num4 < 0x17)
                {
                    this.measure_str[num4] = str2 + "%";
                }
                num4++;
                length = str.Length;
            }
        }

        private void param_unpack(string resultstr)
        {
            string str = resultstr;
            this.measure_str = new string[0x19];
            this.MeasurDataPross(str);
        }

        private void set_measur_param()
        {
            this.pkpk = this.measure_str[0];
            this.max = this.measure_str[1];
            this.min = this.measure_str[2];
            this.amp = this.measure_str[3];
            this.top = this.measure_str[4];
            this.Base = this.measure_str[5];
            this.vmea = this.measure_str[6];
            this.mean = this.measure_str[7];
            this.rms = this.measure_str[8];
            this.crms = this.measure_str[9];
            this.OVSNshoot = this.measure_str[10];
            this.FPREshoot = this.measure_str[11];
            this.OVSPshoot = this.measure_str[12];
            this.RPREshoot = this.measure_str[13];
            this.perid = this.measure_str[14];
            this.frequency = this.measure_str[15];
            this.Pwidth = this.measure_str[0x10];
            this.Nwidth = this.measure_str[0x11];
            this.rise_time = this.measure_str[0x12];
            this.fall_time = this.measure_str[0x13];
            this.burstwidth = this.measure_str[20];
            this.pduty = this.measure_str[0x15];
            this.nduty = this.measure_str[0x16];
        }

        public void setproperites(WD_PARAM m_param)
        {
            this.vdiv_div_init();
            this.timebase_init();
            int index = m_param.time_base;
            this.h_div = this.timebase_str[index];
            index = m_param.fixed_vertical_gain;
            this.v_div = this.vdiv_str[index];
            this.h_start = m_param.first_point.ToString();
            this.h_stop = m_param.last_valid.ToString();
            string unit = "";
            int num2 = this.convertdata(this.v_div, out unit);
            this.v_start = "-" + (num2 * 4) + unit;
            this.v_stop = (num2 * 4) + unit;
            this.points = m_param.last_valid.ToString();
            this.v_offset = m_param.vertical_offset.ToString();
        }

        private void timebase_init()
        {
            this.timebase_str = new string[] { 
                "1 ns", "2.5 ns", "5 ns", "10 ns", "25 ns", "50 ns", "100 ns", "250 ns", "500 ns", "1 us", "2.5 us", "5 us", "10 us", "25 us", "50 us", "100 us", 
                "250 us", "500 us", "1 ms", "2.5 ms", "5 ms", "10 ms", "25 ms", "50 ms", "100 ms", "250 ms", "500 ms", "1 s", "2.5 s", " 5 s", "10 s", "25 s", 
                "50 s", "EXTERNAL"
             };
        }

        private void vdiv_div_init()
        {
            this.vdiv_str = new string[] { "2 mV", "5 mV", "10 mV", "20 mV", "50 mV", "100 mV", "200 mV", "500 mV", "1 V", "2 V", "5 V", "10 V" };
        }
    }
}

