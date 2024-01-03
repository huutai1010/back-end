using System;
using System.Collections.Generic;

namespace DAL.Entities
{
    public partial class Journey
    {
        public Journey()
        {
            BookingPlaces = new HashSet<BookingPlace>();
        }

        public int Id { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public double TotalTime { get; set; }
        public double TotalDistance { get; set; }
        public int Status { get; set; }

        public virtual ICollection<BookingPlace> BookingPlaces { get; set; }
    }
}
