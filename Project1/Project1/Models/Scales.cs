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
    [Table("Scales")]
    public class Scales
    {

        [PrimaryKey, AutoIncrement, Column("Id")]
        public int Id { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("ScaleString")]
        public string ScalesNotes { get; set; }


    }
}
