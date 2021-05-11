using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Text;
using System.Threading;

namespace ClientAps
{
     class ManagerActivityClient
    {
        private static List<_package> lista_procese_active;
        private static object _locker = new object();
        private Thread worker= null;
        private static bool signal_stop;// semnalul de incetare monitorizare


        public ManagerActivityClient()
        {
            lista_procese_active = new List<_package>();

            try
            {
                ThreadStart listener = new ThreadStart(startActivity);
                worker = new Thread(listener);
                worker.IsBackground = true;
                worker.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                Console.ReadLine();
                Environment.Exit(0);
            }
        }

        private static void startActivity()
        {
            Thread.CurrentThread.IsBackground = true;
            while (!signal_stop)
            {
                Process[] processes = Process.GetProcesses();
                Process.GetCurrentProcess();
                foreach (Process process in processes)
                {
                    if (process.MainWindowTitle.Length > 0)
                    {
                        set_information(process.Id);
                    }
                }
            }
        }
        
        private static void set_information(int process_Id)
        {
            Process processById = Process.GetProcessById(process_Id);
            Dictionary<string, string> processDictionary = new Dictionary<string, string>()
            {
                 {
                    "processName",
                     processById.ProcessName.ToString()
                 },
                 {
                    "processPath",
                    "De rezolvat"
                 },
                 {
                    "alreadyOpen",
                    "1"
                 },
                 {
                    "externDisp",
                    "1"
                 },
                 {
                    "aplicationType",
                     get_apps_name(process_Id)
                 },
                 {
                    "accesedTime",
                    processById.StartTime.ToString()
                 }
            };
            if(!verify_accesed_time(processById.StartTime.ToString()))
            {
                _package package_new = (_package)new processActivity();
                package_new.set_package(processDictionary);
                //Console.WriteLine(package_new.get_package_inf());
                lista_procese_active.Add(package_new);
            }
           
        }

        private static Boolean verify_accesed_time(string process_time)
        {
            lock (_locker)
            {
                foreach(_package packet_testat in lista_procese_active)
                {
                    if (process_time.Equals(packet_testat.get_accesed_time()))
                    {
                        return true;
                    }
                }
                return false;
            }; 
        }

        private static string get_apps_name(int process_Id)
        {
            Process my_proc = Process.GetProcessById(process_Id);
            if (my_proc.MainWindowTitle.Length > 0)
            {
                string[] componentsName = my_proc.MainWindowTitle.ToString().Split('-');
                int length = componentsName.Length;
                if (componentsName[length - 1][0] == ' ')
                {
                    componentsName[length - 1] = componentsName[length - 1].Substring(1);
                    return (componentsName[length - 1].ToString());
                }
                else
                {
                    return (componentsName[length - 1].ToString());
                }

            }
            return null;
        }
       
        public void sync_list(List<_package> sursa)
        {
            lock (_locker)
            {
                foreach (_package pachet_selectat in lista_procese_active)
                {
                    if (!pachet_selectat.verify_status_package())
                    {
                        pachet_selectat.set_status_package(true);
                        sursa.Add(pachet_selectat);
                    }
                }
            }
        }

    }
}
