using System;
using System.Collections.Generic;

namespace DAL.Entities
{
    public partial class PlaceTime
    {
        public int Id { get; set; }
        public int PlaceId { get; set; }
        public int DaysOfWeek { get; set; }
        public TimeSpan OpenTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int Status { get; set; }

        public virtual Place Place { get; set; } = null!;
    }
}
