using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Management;
using System.Text;

namespace ClientAps
{
    public class detailedActivity: _package
    {
        [JsonProperty]
        private bool already_added = false;
        [JsonProperty]
        private string type;
        [JsonProperty]
        private string location;

        public Boolean verify_status_package()
        {
            return already_added;
        }
        public void set_status_package(bool status)
        {
           already_added=status;
        }

        public string get_accesed_time() => throw new NotImplementedException();
        public string get_package_inf()
        {
            return type + location + already_added.ToString();
        }

        public void set_package(Dictionary<string, string> informationList)
        {
            this.type = informationList["Type monitoring"].ToString();
            this.location = informationList["pathProof"].ToString();
        }

        private static Video startVideo(string video_name)
        {
            var recorder = new Video(new VideoParameters(video_name, 20, SharpAvi.KnownFourCCs.Codecs.MicrosoftMpeg4V3));

            return recorder;
            //Console.WriteLine("Press any key to Stop...");
            //Console.ReadKey();
            //recorder.Dispose();
        }

        public void start_video(int secunde)
        {
            var intrerupator = startVideo("out.avi");

            //dupa x secunde se da disponse -> implementare counter descrescator
            intrerupator.Dispose();
            set_information_video("out.avi");
        }

        public void take_printscreen()
        {
            int step = 0, width = 0, height = 0;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2",
                                                                           "SELECT * FROM Win32_VideoController");
           
            foreach (ManagementObject queryObj in searcher.Get())
            {
                if (step == 0)
                {
                    width = Int32.Parse(queryObj["CurrentHorizontalResolution"].ToString());
                    height = Int32.Parse(queryObj["CurrentVerticalResolution"].ToString());
                }
                else
                {
                    if (width > Int32.Parse(queryObj["CurrentHorizontalResolution"].ToString()))
                    {
                        width = Int32.Parse(queryObj["CurrentHorizontalResolution"].ToString());
                    }
                    if (height > Int32.Parse(queryObj["CurrentVerticalResolution"].ToString()))
                    {
                        height = Int32.Parse(queryObj["CurrentVerticalResolution"].ToString());
                    }
                }

                if(searcher.Get().Count<=2)
                {
                    break;
                }

                step++;
            }

            var bitmap = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(0, 0, 0, 0,
                bitmap.Size, CopyPixelOperation.SourceCopy);
            }
            if (check_screenshot_directory())
            {
                string finalName = Directory.GetCurrentDirectory().ToString() + @"\Screenshot\" + get_time() + ".jpg";
                bitmap.Save(finalName, ImageFormat.Jpeg);
                Console.WriteLine(finalName);
                set_information_ss(finalName);
            }
            else
            {
                Console.WriteLine("Eroare cu folderul pentru screenshot-uri");
            }
        }

        private void set_information_video(string source)
        {
            this.type = "Record";
            this.location = source;
        }

        private void set_information_ss(string source)
        {
            this.type = "Screenshot";
            this.location = source;
        }

        private string get_time()
        {
            DateTime localDate = DateTime.Now;
            DateTime utcDate = DateTime.UtcNow;
            String[] cultureNames = { "ro-Ro" };
            string namePrintScreen = null;
            foreach (var cultureName in cultureNames)
            {
                var culture = new CultureInfo(cultureName);
                //Console.WriteLine(localDate.ToString(culture));
                namePrintScreen = localDate.ToString(culture).Replace(":", "-");
                //Console.WriteLine(namePrintScreen);
            }

            return namePrintScreen;
        }
    
        private bool check_screenshot_directory()
        {
            string path = @"Screenshot";

            try
            {
                if (Directory.Exists(path))
                {
                    return true;
                }

                DirectoryInfo dir = Directory.CreateDirectory(path);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
            return false;
        }
    }
}
