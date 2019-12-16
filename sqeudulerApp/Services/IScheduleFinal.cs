using sqeudulerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sqeudulerApp.Services
{
    public interface IScheduleFinal
    {
        IEnumerable<ScheduleFinal> GetSchedules { get; }

        ScheduleFinal GetSchedule(int id);
        void Add(ScheduleFinal _ScheduleFinal);
        void Remove(int id);
    }
}
