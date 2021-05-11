using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ClientAps
{
    //clasa ce se va ocupa cu task-urile provenite din partea serverului
    class ManagerRequestSv
    {
        private static List<_package> tasks;
        private static object _locker = new object();
        private Thread listenerThread = null;
        bool signal;


        public ManagerRequestSv()
        {
            tasks = new List<_package>();
            
            try
            {   
                ThreadStart listener = new ThreadStart(ListenThread);
                listenerThread = new Thread(listener);
                listenerThread.IsBackground = true;
                listenerThread.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                Console.ReadLine();
                Environment.Exit(0);
            }
        }

        private void ListenThread()
        {
            try
            {
                Thread.CurrentThread.IsBackground = true;
                ConexServer conexiune2 = new ConexServer();
                conexiune2.clientSocketFct();// a 2-a conexiune doar pentru capturi video/ record
                /*
                Thread tr_sv;
                tr_sv = new Thread(new ThreadStart(conexiune2.clientSocketFct));
                tr_sv.Name = String.Format("Thread interact with sv");
                tr_sv.Start();*/
                //sa astept pentru conexiune
                while(signal)
                {
                 //thread cu TCPLisener  
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                Console.ReadLine();
                Environment.Exit(0);
            }
        }

        private void analize_request(string info, string parametrii)
        {
            if(info.Equals("Recording"))
            {
                recording_fct(tasks);
            }
            else if(info.Equals("Screenshot"))
            {
                printscreen_fct(tasks);
            }

        }
       
        private void printscreen_fct(List<_package> lista_cerinte)
        {
            _package ss = (detailedActivity)new detailedActivity();
            ((detailedActivity)ss).take_printscreen();
            lista_cerinte.Add(ss);
        }

        private void recording_fct(List<_package> lista_cerinte)
        {
            //de adaugat parametru pentru preluarea informatiilor de la sv ( timp de record)

            _package video = (detailedActivity)new detailedActivity();
            ((detailedActivity)video).start_video(5);
            lista_cerinte.Add(video);
        }

        public void sync_list(List<_package> sursa)
        {
            lock (_locker)
            {
                foreach (_package pachet_selectat in tasks)
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
