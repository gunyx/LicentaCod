using System;
using System.Collections.Generic;
using System.Text;

namespace ClientAps
{
    internal class processActivity : _package
    {
        private bool already_added = false;

        private string processName;
        private string aplicationType;
        private string processPath;
        private string accesedTime;
        private bool alreadyOpen;
        private bool externDisp;

        public Boolean verify_status_package()
        {
            return already_added;
        }
        public void set_status_package(bool status)
        {
            already_added = status;
        }

        // public string get_package_inf() => throw new NotImplementedException();
        public string get_accesed_time()
        {
            return accesedTime;
        }
        public string get_package_inf()
        {
            return null;
        }
        public void set_package(Dictionary<string, string> informationList)
        {
            this.processName = informationList["processName"].ToString();
            this.processPath = informationList["processPath"].ToString();
            this.aplicationType = informationList["aplicationType"].ToString();
            this.accesedTime = informationList["accesedTime"].ToString();
            if (informationList["alreadyOpen"].ToString().Equals("1"))
                this.alreadyOpen = false;
            else if (informationList["alreadyOpen"].ToString().Equals("0"))
                this.alreadyOpen = true;
            if (informationList["externDisp"].ToString().Equals("1"))
            {
                this.externDisp = false;
            }
            else
            {
                if (!informationList["externDisp"].ToString().Equals("0"))
                    return;
                this.externDisp = true;
            }
        }
    }
}
