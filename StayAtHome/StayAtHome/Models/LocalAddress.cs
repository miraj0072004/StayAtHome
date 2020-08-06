using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace StayAtHome.Models
{
    public class LocalAddress
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
}
