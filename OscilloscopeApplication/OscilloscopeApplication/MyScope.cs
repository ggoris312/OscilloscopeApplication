namespace OscilloscopeConnection
{
    internal class MyScope
    {
        private static MyScope myscope;
        private string address;
        private string bus;
        private string device;
        private string sn;
        private string status;

        private MyScope()
        {
        }

        public string Getaddress()
        {
            return address;
        }

        public string Getbus()
        {
            return bus;
        }

        public string GetDevice()
        {
            return device;
        }

        public string GetNO()
        {
            return sn;
        }

        public static MyScope Getscpoe()
        {
            if (myscope == null)
            {
                myscope = new MyScope();
            }
            return myscope;
        }

        public string Getstatus()
        {
            return status;
        }

        public void Setaddress(string str)
        {
            address = str;
        }

        public void Setbus(string str)
        {
            bus = str;
        }

        public void SetDevice(string str)
        {
            device = str;
        }

        public void SetNO(string str)
        {
            sn = str;
        }

        public void Setstatus(string str)
        {
            status = str;
        }
    }
}