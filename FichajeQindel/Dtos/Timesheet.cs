using System;

namespace Dtos
{
    internal class Timesheet
    {
        public int id { get; set; }
        public int? duration { get; set; }
        public string description { get; set; }
        public DateTime begin { get; set; }
        public DateTime end { get; set; }
    }
}
