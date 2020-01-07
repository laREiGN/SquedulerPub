using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sqeudulerApp.Models;
using sqeudulerApp.Services;

namespace sqeudulerApp.Repository
{
    public class RequestsRepository : IRequests
    {
        private DB_Context db;
        public RequestsRepository(DB_Context _db)
        {
            db = _db;
        }

        public IEnumerable<Requests> GetAllRequests => db.Requests;

        public void Add(Requests _Requests)
        {
            db.Requests.Add(_Requests);
            db.SaveChanges();
        }

        public Requests GetRequests(int id)
        {
            Requests dbEntity = db.Requests.Find(id);
            return dbEntity;
        }

        public void Remove(int id)
        {
            Requests dbEntity = db.Requests.Find(id);
            db.Requests.Remove(dbEntity);
            db.SaveChanges();
        }
    }
}
