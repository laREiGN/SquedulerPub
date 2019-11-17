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

        public UserTeam GetUserTeam(int id)
        {
            UserTeam dbEntity = db.UserTeam.Find(id);
            return dbEntity;
        }

        public void Remove(int id)
        {
            UserTeam dbEntity = db.UserTeam.Find(id);
            db.UserTeam.Remove(dbEntity);
            db.SaveChanges();
        }

        public bool CheckAdminOrNot(int id)
        {
            UserTeam info = GetUserTeam(id);
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
