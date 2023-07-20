using Android.Media;
using Android.Net.Wifi.P2p;
using Java.Lang.Ref;
using SQLiteDB.Resources.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using static Android.Icu.Util.Calendar;

namespace Project1.Process
{
    public class Listen
    {
        private Thread recordingThread = null;
        private bool threadRun = false;
        private System.Timers.Timer timer = null;
        private AudioRecord audioRecorder = null;
        int BUFFER_SIZE = 16384;                                                                                     //wjększy bufor = wieksza dokładność, wg. 16384 wg mnie jest akuratne
        int SAMPLE_RATE = 44100;
        private byte[] bufferData;
        public event EventHandler<double> OnFrequency = null;

        public void Start()
        {
            bufferData = new byte[BUFFER_SIZE];

            ReadStream();
            Calculate();
        }

        public void Stop()
        {
            threadRun = false;

            timer?.Stop();
            timer?.Dispose();

            audioRecorder?.Stop();
            audioRecorder?.Dispose();
        }
        /*
        TODO: Może być inny timer, chodzi o to żeby odczyt z bufora był w odzielnym wątku
        a co jakiś czas następowało przeliczenie i wyświetlenie danych
        */
        //var idk = Transformate.GetPowerSpectrum(fftComplex);


        private void Calculate()
        {

            timer = new System.Timers.Timer();
            timer.Interval = 100;
            timer.Elapsed += (object sender, ElapsedEventArgs e) =>
            {
                var fftComplex = Transformate.ByteToComplex(bufferData);
                Transformate.FFT(fftComplex);
                var freq = Transformate.GetMaxFrequencyVector(fftComplex, SAMPLE_RATE);

                OnFrequency?.Invoke(this, freq);
                
            };
            timer.Start();
        }


        private void ReadStream()
        {
            audioRecorder = new AudioRecord(AudioSource.Mic,
                           sampleRateInHz: SAMPLE_RATE,
                           ChannelIn.Mono,
                           Android.Media.Encoding.Pcm8bit,
                           bufferSizeInBytes: BUFFER_SIZE);
            audioRecorder.StartRecording();

            threadRun = true;
            recordingThread = new Thread(() =>
            {
                byte[] buffer = new byte[BUFFER_SIZE];
                while (threadRun)
                {
                    audioRecorder.Read(buffer, 0, buffer.Length);
                    buffer.CopyTo(bufferData, 0);
                    Thread.Sleep(100);                                                          //aby sie telefon nie zagotował
                }
            });
            recordingThread.IsBackground = true;
            recordingThread.Start();
        }

    }
}
