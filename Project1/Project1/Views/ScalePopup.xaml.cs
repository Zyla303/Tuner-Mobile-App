using Android.App;
using Android.Net.Wifi.P2p;
using Java.Util;
using SQLiteDB.Resources.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static SQLite.TableMapping;

namespace Project1.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScalePopup : Popup
    {
        public ScalePopup(string label1, string label2, string label3, string label4, string label5, string label6)
        {
            InitializeComponent();
            GetScales(label1, label2, label3, label4, label5, label6);

        }

        void GetBackToScales(object sender, EventArgs args)
        {
            Dismiss("Search Again!");
            
        }

        async void GetScales(string label1, string label2, string label3, string label4, string label5, string label6)
        {
            List<Scales> Scales = await App.Database.GetScalesAsync();
            List<Scales> FilteredScales = new List<Scales>();
            var CopyOfScales = Scales;
            var counter = 0;
            var ok = 0;
            var numbOfLabels = 0;

            var LabelsString = String.Join(" ", label1, label2, label3, label4, label5, label6);
            var LabelsArray = LabelsString.Split(' ');
            
            foreach(string item in LabelsArray)
            {
                if(item != string.Empty)
                {
                    numbOfLabels++;
                }
            }

            foreach (Scales scale in Scales)
            {
                ok = 0;
                counter = 0;
                var x = scale.ScalesNotes.Split(' ');
                for (int i = 0; i < x.Length; i++)
                {
                    if (x[i] == label1 || x[i] == label2 || x[i] == label3 || x[i] == label4 || x[i] == label5 || x[i] == label6)
                    {
                        ok++;
                    }
                    else
                    {
                         counter++;
                    } 
                }

                if (ok == numbOfLabels)
                {
                    
                    FilteredScales.Add(scale);
                }
            }

            ScaleList.ItemsSource = FilteredScales;
        }
    }
}