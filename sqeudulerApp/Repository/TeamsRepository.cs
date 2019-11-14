using sqeudulerApp.Models;
using sqeudulerApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sqeudulerApp.Repository
{
    public class TeamsRepository : ITeams
    {
        private DB_Context db;
        public TeamsRepository(DB_Context _db)
        {
            db = _db;
        }

        public IEnumerable<Teams> GetTeams => db.Teams;

        public void Add(Teams _Teams)
        {
            db.Teams.Add(_Teams);
            db.SaveChanges();
        }

        public Teams GetTeam(int id)
        {
            Teams dbEntity = db.Teams.Find(id);
            return dbEntity;
        }

        public void Remove(int id)
        {
            Teams dbEntity = db.Teams.Find(id);
            db.Teams.Remove(dbEntity);
            db.SaveChanges();
        }
    }
}
