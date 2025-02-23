using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Bewewgungssensor.Datenbank
{
    public  class HighScore
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Spielername { get; set; }

        public int Score { get; set; }
    }
}
