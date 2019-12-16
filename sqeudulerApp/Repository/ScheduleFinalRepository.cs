using sqeudulerApp.Models;
using sqeudulerApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sqeudulerApp.Repository
{
    public class ScheduleFinalRepository : IScheduleFinal
    {
        private DB_Context db;
        public ScheduleFinalRepository(DB_Context _db)
        {
            db = _db;
        }

        public IEnumerable<ScheduleFinal> GetSchedules => db.ScheduleFinal;

        public void Add(ScheduleFinal _ScheduleFinal)
        {
            db.ScheduleFinal.Add(_ScheduleFinal);
            db.SaveChanges();
        }

        public ScheduleFinal GetSchedule(int id)
        {
            ScheduleFinal dbEntity = db.ScheduleFinal.Find(id);
            return dbEntity;
        }

        public void Remove(int id)
        {
            ScheduleFinal dbEntity = db.ScheduleFinal.Find(id);
            db.ScheduleFinal.Remove(dbEntity);
            db.SaveChanges();
        }
    }
}
