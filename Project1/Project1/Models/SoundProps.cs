using System;
using System.Runtime.InteropServices;
using Android.Media;
using System.Timers;
using Java.Util;
using Project1;
using SQLite;
using System.Threading;
using Project1.Process;

namespace SQLiteDB.Resources.Model
{
    [Table("SoundProps")]
    public class SoundProps
    {

        [PrimaryKey, AutoIncrement, Column("Id")]
        public int Id { get; set; }
        [Column("MinFreq")]
        public double MinFreq { get; set; }
        [Column("Frequency")]
        public double Frequency { get; set; }
        [Column("MaxFreq")]
        public double MaxFreq { get; set; }
        [Column("Note")]
        public string Note { get; set; }

        public SoundProps getNote(double frequency)
        {
            SoundProps noteInfo = null;
            if(frequency > 0)
            {
                noteInfo = App.Database.GetNoteFromDbAsync(frequency).Result;
            }
            else
            {
                noteInfo = null;
            }
            
            return noteInfo;
        }

        public SoundProps getNoteFromButton(string note)
        {
            SoundProps noteInfo = null;
            if (note != string.Empty)
            {
                noteInfo = App.Database.GetNoteFromName(note).Result;
            }
            else
            {
                noteInfo = null;
            }

            return noteInfo;
        }


    }
}
