using Database;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.IO;
using Xamarin.Essentials;
using Xamarin.Forms;
using Environment = System.Environment;
using PermissionStatus = Plugin.Permissions.Abstractions.PermissionStatus;

namespace Project1
{
    public partial class App : Application
    {
        private static SoundDatabase database;

        public static SoundDatabase Database
        {
            get
            {
                if (database == null)
                {
                    database = new SoundDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MusicsEssential.db3"));
                }

                return database;
            }
        }

        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();

        }

        protected override async void OnStart()
        {
            var noteCount = await App.Database.CountNotesAsync();
            var tuningCount = await App.Database.CountTuningsAsync();
            var scalesCount = await App.Database.CountScalesAsync();
            try
            {
                    await App.Database.InsertAllNotes(noteCount);
                    await App.Database.InsertAllTunings(tuningCount);
                    await App.Database.InsertAllScales(scalesCount);  
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("[ERROR] Database Err", ex.Message, "No Ok");
            }

            DeviceDisplay.KeepScreenOn = true;
        }
        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
