using sqeudulerApp.Models;
using sqeudulerApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sqeudulerApp.Repository
{
    public class AvailabilityRepository : IAvailability
    {
        private DB_Context db;
        public AvailabilityRepository(DB_Context _db)
        {
            db = _db;
        }

        public IEnumerable<Availability> GetAvailabilities => db.Availability;

        public void Add(Availability _Availability)
        {
            db.Availability.Add(_Availability);
            db.SaveChanges();
        }

        public Availability GetAvailability(int id)
        {
            Availability dbEntity = db.Availability.Find(id);
            return dbEntity;
        }

        public void Remove(int id)
        {
            Availability dbEntity = db.Availability.Find(id);
            db.Availability.Remove(dbEntity);
            db.SaveChanges();
        }
    }
}
