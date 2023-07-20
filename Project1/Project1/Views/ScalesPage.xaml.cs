using Android.Media;
using Android.Net.Wifi.P2p;
using Project1.Process;
using SQLiteDB.Resources.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Android.Resource;
using SQLite;
using Xamarin.CommunityToolkit.Extensions;
using System.Text.RegularExpressions;
using static Android.Provider.ContactsContract.CommonDataKinds;
using Java.Util;

namespace Project1.Views
{
    public partial class ScalesPage : ContentPage
    {

        private Process.Listen listen = null;
        public ScalesPage()
        {
            InitializeComponent();
            listen = new Process.Listen();
            listen.OnFrequency += Listen_OnFrequency;
            listen.Start();
        }

        private void Listen_OnFrequency(object sender, double frequency)
        {

            SoundProps noteInfo = new SoundProps();
            var noteToSave = "";

            Device.BeginInvokeOnMainThread(() => {

                var noteDetail = noteInfo.getNote(frequency);
                if (noteDetail != null)
                {
                    LabelFrequencyScales.Text = string.Format("{0}", TLLL(noteDetail.Note));
                    noteToSave = TLLL(noteDetail.Note);
                }

                if (Label1.Text == string.Empty)
                {
                        Label1.Text = noteToSave;

                }else if(Label2.Text == string.Empty && Label1.Text != string.Empty && noteToSave != Label1.Text)
                {
                    Label2.Text = noteToSave;
                }
                else if (Label3.Text == string.Empty && Label2.Text != string.Empty && noteToSave != Label1.Text && noteToSave != Label2.Text)
                {
                    Label3.Text = noteToSave;
                }
                else if (Label4.Text == string.Empty && Label3.Text != string.Empty && noteToSave != Label1.Text && noteToSave != Label2.Text && noteToSave != Label3.Text)
                {
                    Label4.Text = noteToSave;
                }
                else if (Label5.Text == string.Empty && Label4.Text != string.Empty && noteToSave != Label1.Text && noteToSave != Label2.Text && noteToSave != Label3.Text && noteToSave != Label4.Text)
                {
                    Label5.Text = noteToSave;
                }
                else if (Label6.Text == string.Empty && Label5.Text != string.Empty && noteToSave != Label1.Text && noteToSave != Label2.Text && noteToSave != Label3.Text && noteToSave != Label4.Text && noteToSave != Label5.Text)
                {
                    Label6.Text = noteToSave;
                }

            });
        }

        public static string TLLL(string str) => str.Remove(str.Length - 1);

        void ScalesList(object sender, EventArgs args)
        {
            var label1 = Label1.Text;
            var label2 = Label2.Text;
            var label3 = Label3.Text;
            var label4 = Label4.Text;
            var label5 = Label5.Text;
            var label6 = Label6.Text;

            Navigation.ShowPopupAsync(new ScalePopup(label1, label2, label3, label4, label5, label6));

        }

        public void ClearList(object sender, EventArgs args)
        {
            Label1.Text = string.Empty;
            Label2.Text = string.Empty;
            Label3.Text = string.Empty;
            Label4.Text = string.Empty;
            Label5.Text = string.Empty;
            Label6.Text = string.Empty;
        }

    }
}