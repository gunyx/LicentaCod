using System;
using System.Collections.Generic;
using System.Text;

namespace ClientAps
{
    class peripheralsActivity:_package
    {
        private bool already_added = false;

        public Boolean verify_status_package()
        {
            return already_added;
        }
        public void set_status_package(bool status)
        {
            already_added = status;
        }

        public string get_package_inf() => throw new NotImplementedException();

        public string get_accesed_time() => throw new NotImplementedException();

        public void set_package(Dictionary<string, string> informationList) => throw new NotImplementedException();

    }
}
