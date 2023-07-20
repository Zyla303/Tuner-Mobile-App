using Project1.Models;
using SQLiteDB.Resources.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Project1.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SaveNotePopUp : Popup
    {
        public int noteId;
        public string noteLen;
        public int position;
        public string _note;

        public SaveNotePopUp(string note)
        {
            InitializeComponent();
            _note = note;
            YourNote.Text = TLLL(note);
            TakeNoteIdOnLinesAsync(note);
        }

        async Task TakeNoteIdOnLinesAsync(string note)
        {
            var noteFromDatabase = await App.Database.GetNoteFromName(note);
            noteId = noteFromDatabase.Id; //---
        }

        void ChooseNoteLen(System.Object sender, System.EventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            noteLen = radioButton.Content.ToString();
        }

        void GetBackToRecorder(System.Object sender, System.EventArgs e)
        {
            List<NoteToLines> noteDetails = null;
            Dismiss(noteDetails);
        }

        void AddNoteToNext(System.Object sender, System.EventArgs e)
        {

            List<NoteToLines> noteDetails = new List<NoteToLines>
            { new NoteToLines { Id = noteId, Len = noteLen, Position = 1, Note = _note} };
            Dismiss(noteDetails);
        }

        void AddNoteToThis(System.Object sender, System.EventArgs e)
        {
            List<NoteToLines> noteDetails = new List<NoteToLines>
            { new NoteToLines { Id = noteId, Len = noteLen, Position = 0, Note = _note} };
            Dismiss(noteDetails);
        }

        public static string TLLL(string str)
        {
            return Regex.Replace(str,
            @"[0-9]+", match => ((char)(match.Value[0] - '0' + '₀')).ToString());
        }

        public static string ReverseTLLL(string str)
        {
            string split1 = str.Substring(0, str.Length - 1);
            string split2 = str.Substring(str.Length - 1, 1);
            string numb = Regex.Replace(split2,
            ".", match => ((char)(match.Value[0] - '₀' + '0')).ToString());

            return string.Concat(split1, numb);
        }

    }
}