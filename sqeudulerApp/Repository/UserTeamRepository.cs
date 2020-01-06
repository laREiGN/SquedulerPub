using Microsoft.EntityFrameworkCore;
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

        // Tom: De parameter TeamId is eigenlijk de team code
        public UserTeam GetUserTeam(int id, string TeamId)
        {
            UserTeam dbEntity = db.UserTeam.Where(x => x.UserID == id && x.Team == TeamId).Single();
            var result = dbEntity;
/*            var dbEntity = from ut in db.UserTeam where ut.UserID == id && ut.Team == TeamId select ut;
            
            UserTeam result = dbEntity.Skip(1).FirstOrDefault();*/
            return result;
        }

        public void PromoteUserInTeam(int UserId, string TeamCode) 
        {
            UserTeam dbEntity = GetUserTeam(UserId, TeamCode);
            dbEntity.Role = "admin";
            bool saveFailed;
            do
            {
                saveFailed = false;
                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;

                    // Update original values from the database
                    var entry = ex.Entries.Single();
                    entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                }

            } while (saveFailed);
        }
        
        public void Remove(int id, string TeamId)
        {
            UserTeam dbEntity = GetUserTeam(id, TeamId);

            db.UserTeam.Remove(dbEntity);

            bool saveFailed;
            do
            {
                saveFailed = false;
                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;

                    // Update original values from the database
                    var entry = ex.Entries.Single();
                    entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                }

            } while (saveFailed);
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
