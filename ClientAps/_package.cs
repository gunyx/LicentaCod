using System;
using System.Collections.Generic;
using System.Text;

namespace ClientAps
{
    interface _package
    {
        Boolean verify_status_package();
        
        string get_package_inf();

        string get_accesed_time();

        void set_package(Dictionary<string, string> informationList);

        void set_status_package(bool status);
    }
}
