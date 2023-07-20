using System;
using Xamarin.Forms;
using SQLiteDB.Resources.Model;
using System.Text.RegularExpressions;
using Xamarin.CommunityToolkit.Extensions;
using Application = Xamarin.Forms.Application;
using System.Threading;
using System.Runtime.InteropServices;

namespace Project1.Views
{
    public partial class TunerPage : ContentPage
    {
        private Process.Listen listen = null;
        private string setNoteFromTuning = String.Empty;
        private int tuningId = 1;

        public TunerPage()
        {
            InitializeComponent();
            SetStandardOnStart();
            listen = new Process.Listen();
            listen.OnFrequency += Listen_OnFrequency;
            listen.Start();


        }

        private void Listen_OnFrequency(object sender, double frequency)
        {

            SoundProps noteInfo = new SoundProps();

            Device.BeginInvokeOnMainThread(() => {
                if(setNoteFromTuning == String.Empty)
                {
                    var noteDetail = noteInfo.getNote(frequency);
                    BackgroundTunerColor(noteDetail, frequency);
                    ProgressBarFreq(noteDetail, frequency);
                    if (noteDetail != null)
                    {
                        LabelFrequency.Text = string.Format("{0}", TLLL(noteDetail.Note));
                    }
                }
                else
                {
                    var noteFormButton = noteInfo.getNoteFromButton(setNoteFromTuning);
                    BackgroundTunerColor(noteFormButton, frequency);
                    ProgressBarFreq(noteFormButton, frequency);
                    if (noteFormButton != null)
                    {
                        LabelFrequency.Text = string.Format("{0}", TLLL(noteFormButton.Note));
                    }
                }
            });
        }

        public void ProgressBarFreq(SoundProps sound, double freqListen)
        {
            double greencolorchanger = 0.0;
            double redcolorchanger = 0.0;
            int GreenColorNumber = 0;
            int RedColorNumber = 0;
            if (freqListen > 0)
            {

                if (sound.Frequency > freqListen)
                {

                    greencolorchanger = (freqListen - sound.MinFreq) / (sound.Frequency - sound.MinFreq);
                    redcolorchanger = 1 - greencolorchanger;
                }
                else if (freqListen > sound.Frequency)
                {
                    greencolorchanger = (sound.MaxFreq - freqListen) / (sound.MaxFreq - sound.Frequency);
                    redcolorchanger = 1 - greencolorchanger;

                }

                if (freqListen - (2 * (sound.Frequency / sound.MinFreq)) < 
                    sound.Frequency && sound.Frequency < 
                    freqListen + 2 * (sound.MaxFreq / sound.Frequency))
                {
                    ScaleChanger.WidthRequest = 50;
                    ScaleChanger.HorizontalOptions = LayoutOptions.Center;
                    Thickness margin = ScaleChanger.Margin;
                    margin.Left = 0;
                    margin.Right = 0;
                    ScaleChanger.Margin = margin;
                    GreenColorNumber = 255;
                    RedColorNumber = 0;
                }
                else if ((freqListen - sound.MinFreq) < (sound.MaxFreq - freqListen))
                {
                    ScaleChanger.WidthRequest = ((Application.Current.MainPage.Width) * redcolorchanger) / 2;
                    ScaleChanger.HorizontalOptions = LayoutOptions.EndAndExpand;
                    Thickness margin = ScaleChanger.Margin;
                    margin.Left = 0;
                    margin.Right = Application.Current.MainPage.Width / 2;
                    ScaleChanger.Margin = margin;
                    GreenColorNumber = (int)(greencolorchanger * 255);
                    RedColorNumber = (int)(redcolorchanger * 255);
                }
                else if ((freqListen - sound.MinFreq) > (sound.MaxFreq - freqListen))
                {
                    ScaleChanger.WidthRequest = ((Application.Current.MainPage.Width - 20) * redcolorchanger) / 2;
                    ScaleChanger.HorizontalOptions = LayoutOptions.StartAndExpand;
                    Thickness margin = ScaleChanger.Margin;
                    margin.Right = 0;
                    margin.Left = Application.Current.MainPage.Width / 2;
                    ScaleChanger.Margin = margin;
                    GreenColorNumber = (int)(greencolorchanger * 255);
                    RedColorNumber = (int)(redcolorchanger * 255);
                }
            }
            else
            {
                ScaleChanger.WidthRequest = 0;
            }
            ScaleChanger.BackgroundColor = Color.FromRgb(RedColorNumber, GreenColorNumber, 0);


        }

        public void BackgroundTunerColor(SoundProps soundProps, double frequency)
        {
            if (frequency > 0)
            {
                var min = soundProps.MinFreq;
                var freq = soundProps.Frequency;
                var max = soundProps.MaxFreq;
                var myFrequency = frequency;
                double greencolorchanger = 0.0;
                double redcolorchanger = 0.0;

                if (freq > myFrequency)
                {

                    greencolorchanger = (myFrequency - min) / (freq - min);
                    redcolorchanger = 1 - greencolorchanger;
                }
                else if (myFrequency > freq)
                {
                    greencolorchanger = (max - myFrequency) / (max - freq);
                    redcolorchanger = 1 - greencolorchanger;

                }

                if (greencolorchanger < 0.0)
                {
                    BackgroundTuner.BackgroundColor = Color.FromRgb(255, 0, 0);
                }

                else if (redcolorchanger < 0.0)
                {
                    BackgroundTuner.BackgroundColor = Color.FromRgb(0, 255, 0);
                }
                else if (greencolorchanger > 1.0)
                {
                    BackgroundTuner.BackgroundColor = Color.FromRgb(0, 255, 0);
                }
                else if (redcolorchanger > 1.0)
                {
                    BackgroundTuner.BackgroundColor = Color.FromRgb(255, 0, 0);
                }
                else
                {
                    int GreenColorNumber = (int)(greencolorchanger * 255);
                    int RedColorNumber = (int)(redcolorchanger * 255);

                    BackgroundTuner.BackgroundColor = Color.FromRgb(RedColorNumber, GreenColorNumber, 0);
                }

            }
            else
            {
                BackgroundTuner.BackgroundColor = Color.Black;
            }

        }

        public static string TLLL(string str)
        { 
                return Regex.Replace(str,
                @"[0-9]+", match => ((char)(match.Value[0] - '0' + '₀')).ToString());
        }

        public static string ReverseTLLL(string str)
        {
            string split1 = str.Substring(0, str.Length - 1);
            string split2 = str.Substring(str.Length-1, 1);
            string numb = Regex.Replace(split2,
            ".", match => ((char)(match.Value[0] - '₀' + '0')).ToString());

            return string.Concat(split1, numb);
        }

        public void StopListen(object sender, EventArgs e)
        => listen?.Stop();

        async void TuningChange(System.Object sender, System.EventArgs e)
        {
            BackgroundTuner.BackgroundColor = Color.Black;

            var result = await Navigation.ShowPopupAsync(new ChooseTuningPopUp(tuningId));

            var myTuning = App.Database.GetTuningWithId((int)result).Result;

            tuningId = myTuning.Id;

            FirstButton.Text = TLLL(myTuning.FirstNote);
            SecondButton.Text = TLLL(myTuning.SecondNote);
            ThirdButton.Text = TLLL(myTuning.ThirdNote);
            FourthButton.Text = TLLL(myTuning.FourthNote);
            FifthButton.Text = TLLL(myTuning.FifthNote);
            SixthButton.Text = TLLL(myTuning.SixthNote);
            SecondButton.BackgroundColor = Color.Black;
            SixthButton.BackgroundColor = Color.Black;
            ThirdButton.BackgroundColor = Color.Black;
            FourthButton.BackgroundColor = Color.Black;
            FifthButton.BackgroundColor = Color.Black;
            FirstButton.BackgroundColor = Color.Black;
            setNoteFromTuning = String.Empty;
        }
        
        public void FirstNoteChange(object sender, EventArgs e)
        {
            if (ReverseTLLL(FirstButton.Text) != setNoteFromTuning)
            {
                setNoteFromTuning = ReverseTLLL(FirstButton.Text);
                SecondButton.BackgroundColor = Color.Black;
                SixthButton.BackgroundColor = Color.Black;
                ThirdButton.BackgroundColor = Color.Black;
                FourthButton.BackgroundColor = Color.Black;
                FifthButton.BackgroundColor = Color.Black;
                FirstButton.BackgroundColor = Color.White;
            }
            else
            {
                setNoteFromTuning = String.Empty;
                FirstButton.BackgroundColor = Color.Black;
            }

        }

        public void SecondNoteChange(object sender, EventArgs e)
        {
            if (ReverseTLLL(SecondButton.Text) != setNoteFromTuning)
            {
                setNoteFromTuning = ReverseTLLL(SecondButton.Text);
                FirstButton.BackgroundColor = Color.Black;
                SixthButton.BackgroundColor = Color.Black;
                ThirdButton.BackgroundColor = Color.Black;
                FourthButton.BackgroundColor = Color.Black;
                FifthButton.BackgroundColor = Color.Black;
                SecondButton.BackgroundColor = Color.White;
            }
            else
            {
                setNoteFromTuning = String.Empty;
                SecondButton.BackgroundColor = Color.Black;
            }
        }

        public void ThirdNoteChange(object sender, EventArgs e)
        {
            if (ReverseTLLL(ThirdButton.Text) != setNoteFromTuning)
            {
                setNoteFromTuning = ReverseTLLL(ThirdButton.Text);
                FirstButton.BackgroundColor = Color.Black;
                SecondButton.BackgroundColor = Color.Black;
                SixthButton.BackgroundColor = Color.Black;
                FourthButton.BackgroundColor = Color.Black;
                FifthButton.BackgroundColor = Color.Black;
                ThirdButton.BackgroundColor = Color.White;
            }
            else
            {
                setNoteFromTuning = String.Empty;
                ThirdButton.BackgroundColor = Color.Black;
            }
        }

        public void FourthNoteChange(object sender, EventArgs e)
        {
            if (ReverseTLLL(FourthButton.Text) != setNoteFromTuning)
            {
                setNoteFromTuning = ReverseTLLL(FourthButton.Text);
                FirstButton.BackgroundColor = Color.Black;
                SecondButton.BackgroundColor = Color.Black;
                ThirdButton.BackgroundColor = Color.Black;
                SixthButton.BackgroundColor = Color.Black;
                FifthButton.BackgroundColor = Color.Black;
                FourthButton.BackgroundColor = Color.White;
            }
            else
            {
                setNoteFromTuning = String.Empty;
                FourthButton.BackgroundColor = Color.Black;
            }
        }

        public void FifthNoteChange(object sender, EventArgs e)
        {
            if (ReverseTLLL(FifthButton.Text) != setNoteFromTuning)
            {
                setNoteFromTuning = ReverseTLLL(FifthButton.Text);
                FirstButton.BackgroundColor = Color.Black;
                SecondButton.BackgroundColor = Color.Black;
                ThirdButton.BackgroundColor = Color.Black;
                FourthButton.BackgroundColor = Color.Black;
                SixthButton.BackgroundColor = Color.Black;
                FifthButton.BackgroundColor = Color.White;
            }
            else
            {
                setNoteFromTuning = String.Empty;
                FifthButton.BackgroundColor = Color.Black;
            }
        }

        public void SixthNoteChange(object sender, EventArgs e)
        {
            if (ReverseTLLL(SixthButton.Text) != setNoteFromTuning)
            {
                setNoteFromTuning = ReverseTLLL(SixthButton.Text);
                FirstButton.BackgroundColor = Color.Black;
                SecondButton.BackgroundColor = Color.Black;
                ThirdButton.BackgroundColor = Color.Black;
                FourthButton.BackgroundColor = Color.Black;
                FifthButton.BackgroundColor = Color.Black;
                SixthButton.BackgroundColor = Color.White;
            }
            else
            {
                setNoteFromTuning = String.Empty;
                SixthButton.BackgroundColor = Color.Black;
            }

        }

        public void SetStandardOnStart()
        {
            var tuning = App.Database.GetTuningWithId(1).Result;
            FirstButton.Text = TLLL(tuning.FirstNote);
            SecondButton.Text = TLLL(tuning.SecondNote);
            ThirdButton.Text = TLLL(tuning.ThirdNote);
            FourthButton.Text = TLLL(tuning.FourthNote);
            FifthButton.Text = TLLL(tuning.FifthNote);
            SixthButton.Text = TLLL(tuning.SixthNote);
        }

    }
}