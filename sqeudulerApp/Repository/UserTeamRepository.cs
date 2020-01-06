using sqeudulerApp.Models;
using sqeudulerApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sqeudulerApp.Repository
{
    public class UserTeamRepository : IUserTeam
    {
        private DB_Context db;
        public UserTeamRepository(DB_Context _db)
        {
            db = _db;
        }

        public IEnumerable<UserTeam> GetUserTeams => db.UserTeam;

        public void Add(UserTeam _User)
        {
            db.UserTeam.Add(_User);
            db.SaveChanges();
        }

        public UserTeam GetUserTeam(int id, string TeamId)
        {
            UserTeam dbEntity = db.UserTeam.Where(x => x.UserID == id && x.Team == TeamId).SingleOrDefault();
            return dbEntity;
        }

        public void Remove(int id, string TeamId)
        {
            UserTeam dbEntity = GetUserTeam(id, TeamId);
            db.UserTeam.Remove(dbEntity);
            db.SaveChanges();
        }

        public bool CheckAdminOrNot(int id, string TeamId)
        {
            UserTeam info = GetUserTeam(id, TeamId);
            if (info.Role == "admin")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
