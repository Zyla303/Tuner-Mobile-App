using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteDB.Resources.Model
{
    [Table("Tunings")]
    public class Tuning
    {
        [PrimaryKey, AutoIncrement, Column("Id")]
        public int Id { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("First Note")]
        public string FirstNote { get; set; }

        [Column("Second Note")]
        public string SecondNote { get; set; }

        [Column("ThirdNote")]
        public string ThirdNote { get; set; }

        [Column("Fourth")]
        public string FourthNote { get; set; }

        [Column("Fifth Note")]
        public string FifthNote { get; set; }

        [Column("Sixth Note")]
        public string SixthNote { get; set; }

    }
}
