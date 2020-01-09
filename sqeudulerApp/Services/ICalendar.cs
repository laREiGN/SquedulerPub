using sqeudulerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sqeudulerApp.Services
{
    public interface ICalendar
    {
        IEnumerable<Calendar> GetCalendar { get; }
        void ScheduleUser(Calendar workshift) { }
    }
}
