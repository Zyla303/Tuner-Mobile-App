using Project1.Views;
using SQLiteDB.Resources.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Project1.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChooseTuningPopUp : Popup
    {
        private int _tuningId;

        public ChooseTuningPopUp(int tuningId)
        {
            InitializeComponent();
            GetListOfTunings();
            _tuningId = tuningId;
        }
        
        void GetBackToTuner(object sender, EventArgs args)
        {
            var id = _tuningId;
            Dismiss(id);
            
        }

        async void GetListOfTunings()
        {
            List<Tuning> tunings = await App.Database.GetTuningsAsync();
            foreach (Tuning tuning in tunings)
            {
                tuning.FirstNote = TLLL(tuning.FirstNote);
                tuning.SecondNote = TLLL(tuning.SecondNote);
                tuning.ThirdNote = TLLL(tuning.ThirdNote);
                tuning.FourthNote = TLLL(tuning.FourthNote);
                tuning.FifthNote = TLLL(tuning.FifthNote);
                tuning.SixthNote = TLLL(tuning.SixthNote);
            }
            TuningList.ItemsSource = tunings.ToList();
        }

        void ChooseScale(Object Sender, ItemTappedEventArgs e)
        {
            var _result = e.Item as Tuning;
            Dismiss(_result.Id);
        }

        public static string TLLL(string str)
        {
            return Regex.Replace(str,
            @"[0-9]+", match => ((char)(match.Value[0] - '0' + '₀')).ToString());
        }


    }
}