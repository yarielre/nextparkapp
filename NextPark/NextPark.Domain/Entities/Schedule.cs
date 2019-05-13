using NextPark.Domain.Core;
using NextPark.Enums.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NextPark.Domain.Entities
{
   public class Schedule: BaseEntity
    {
        public ScheduleType ScheduleType { get; set; }
        public int ScheduleId { get; set; }
        public DateTime TimeOfCreation { get; set; }
        public DateTime TimeOfExecution { get; set; }
    }
}
