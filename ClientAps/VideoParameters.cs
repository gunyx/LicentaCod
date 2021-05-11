using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using SharpAvi;
using SharpAvi.Codecs;
using SharpAvi.Output;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Management;


namespace ClientAps
{
    public class VideoParameters
    {
        public int Height;
        public int Width;
        string file;
        int fps;
        FourCC codec;
       
        public VideoParameters(string fisier, int fps_param,FourCC Encoder)
        {
            this.file = fisier;
            this.fps = fps_param;
            codec = Encoder;
            set_size();
        }

        void set_size()
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
            this.Width = width;
            this.Height = height;
            //this.Width = 1366;
            //this.Height = 768;
        }

        public AviWriter CreateAviWriter()
        {
            return new AviWriter(this.file);
        }
   
    }

    public class Video: IDisposable
    {
        AviWriter writer;
        VideoParameters parametrii;
        IAviVideoStream videoStream;
        Thread writerThread;
        ManualResetEvent stopThread = new ManualResetEvent(false);

        public Video(VideoParameters parametrii)
        {
            this.parametrii = parametrii;

            writer = parametrii.CreateAviWriter();
            videoStream=(IAviVideoStream)writer.AddUncompressedVideoStream(parametrii.Width,parametrii.Height);
            
            writerThread = new Thread(RecordFunction);
            writerThread.Start();
        }

        public void Dispose()
        {
            stopThread.Set();
            writerThread.Join();
           
            writer.Close();

            stopThread.Dispose();
        }

        void RecordFunction()
        {
            var frameInterval = TimeSpan.FromSeconds(1 / (double)writer.FramesPerSecond);
            var buffer = new byte[parametrii.Width * parametrii.Height * 4];
            Task videoWriteTask = null;
            var timeTillNextFrame = TimeSpan.Zero;

            while (!stopThread.WaitOne(timeTillNextFrame))
            {
                var timestamp = DateTime.Now;

                Screenshot(buffer);

                videoWriteTask?.Wait();

               
                videoWriteTask = videoStream.WriteFrameAsync(true, buffer, 0, buffer.Length);

                timeTillNextFrame = timestamp + frameInterval - DateTime.Now;
                if (timeTillNextFrame < TimeSpan.Zero)
                    timeTillNextFrame = TimeSpan.Zero;
            }

           
            videoWriteTask?.Wait();
        }

        public void Screenshot(byte[] Buffer)
        {
            using (var BMP = new Bitmap(parametrii.Width, parametrii.Height))
            {
                using (var g = Graphics.FromImage(BMP))
                {
                    g.CopyFromScreen(Point.Empty, Point.Empty, new Size(parametrii.Width, parametrii.Height), CopyPixelOperation.SourceCopy);

                    g.Flush();

                    var bits = BMP.LockBits(new Rectangle(0, 0, parametrii.Width, parametrii.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
                    Marshal.Copy(bits.Scan0, Buffer, 0, Buffer.Length);
                    BMP.UnlockBits(bits);
                }
            }
        }
    }
}
