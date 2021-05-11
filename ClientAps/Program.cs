using Caliburn.Micro;
using Nancy.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;

namespace ClientAps
{
    
    class Program
    {
        private static System.Timers.Timer aTimer;
        private static object locker = new object();
        private static List<_package> lista_pachete = new List<_package>();
        private static List<_package> lista_pachete_cerute = new List<_package>();
        private static _backpack deposit = new _backpack();
       
        private static void Main(string[] args)
        {
            // Obtinere socket pentru trimitere pachete din backpack
            ConexServer conexiune = new ConexServer();
            Socket socket_deschis = conexiune.get_socket_client();

            //Initializare mangeri pentru monitorizare procese+ ascultare si indeplinire task-uri de la sv.
            ManagerActivityClient activityClient = new ManagerActivityClient();
            ManagerRequestSv managerRequestSv = new ManagerRequestSv();

            send_from_backpack(socket_deschis);

            //Am verificat daca parserul merge
            /*
            _package ss = (detailedActivity)new detailedActivity();
            ((detailedActivity)ss).take_printscreen();
            parse_package_from_backpack(ss);*/


            //Verificare 2 sockets client-sv.
            /*--------
            ConexServer conexiune=new ConexServer();
            ConexServer conexiune2 = new ConexServer();
            Thread[] tr = new Thread[2];
            tr[0]= new Thread(new ThreadStart(conexiune.clientSocketFct));
            tr[1] = new Thread(new ThreadStart(conexiune2.clientSocketFct));
            
            tr[0].Start();
            tr[1].Start();
            foreach (Thread x in tr)
            {
                x.Start();
            }
             -------*/

        
            //Verificare sincronizare thread-uri
            /*
              activityClient.sync_list(lista_pachete);
              managerRequestSv.sync_list(lista_pachete_cerute);
              Console.WriteLine(lista_pachete.Count);
              Console.WriteLine(lista_pachete[0].verify_status_package().ToString());*/
           
        }

        //FUNCTII PENTRU COUNTERUL DESCRESCATOR PENTRU VIDEO
        /*
        private static void stopFct()
        {
            aTimer.Stop();
            aTimer.Dispose();

            Console.WriteLine("Finish");
        }

        private static void SetTimer()
        {
            
            aTimer = new System.Timers.Timer(1000);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            Console.WriteLine("Timp CURENT {0:HH:mm:ss.fff}",
                              e.SignalTime);
           // val_generala-=2;
            //Console.WriteLine(val_generala.ToString());
        }

        */


        //trimitere pachete din depozit catre server
        private static void send_from_backpack(Socket socket)
        {
            if (deposit.get_list() != null)
            {
                foreach (KeyValuePair<_package, bool> entry in deposit.get_list())
                {
                    var json_obtinut = parse_package_from_backpack(entry.Key);
                    long len_package = json_obtinut.Length;
                    byte[] buffer = Encoding.ASCII.GetBytes(json_obtinut);
                    socket.Send(buffer, 0, buffer.Length, SocketFlags.None);

                    //parcurgere dictionar si stergere pachet
                }
            }

        }

        private static string parse_package_from_backpack(_package pachet)
        {
            
            var json = new JavaScriptSerializer().Serialize(pachet);
            Console.WriteLine(json);
            return json;

            /*
            string json = JsonConvert.SerializeObject(pachet, Formatting.Indented);
            Console.WriteLine(json);*/
        }

        private static void add_to_backpack()
        {
            lock(locker)
            {
                if(!lista_pachete.Equals(null))
                {
                    foreach (_package selectie in lista_pachete)
                    {
                        if (!selectie.verify_status_package())
                        {
                            deposit.add_package(selectie);
                        }
                    }
                }

                if(!lista_pachete_cerute.Equals(null))
                {
                    foreach (_package selectie in lista_pachete_cerute)
                    {
                        if (!selectie.verify_status_package())
                        {
                            deposit.add_package(selectie);
                        }
                    }

                }
            }
        }

    }

}
