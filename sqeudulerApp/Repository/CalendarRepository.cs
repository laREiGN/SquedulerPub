using sqeudulerApp.Models;
using sqeudulerApp.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace sqeudulerApp.Repository
{
    public class CalendarRepository : ICalendar
    {
        private DB_Context db;
        public CalendarRepository(DB_Context _db)
        {
            db = _db;
        }
        //Get all rows in ScheduleFinal table in db
        public IEnumerable<Calendar> GetCalendar => db.ScheduleFinal;
        public void ScheduleUser(Calendar workshift)
        {
            db.ScheduleFinal.Add(workshift);
            db.SaveChanges();
        }


    }
}
