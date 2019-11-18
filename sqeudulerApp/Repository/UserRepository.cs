using sqeudulerApp.Models;
using sqeudulerApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sqeudulerApp.Repository
{
    public class UserRepository : IUser
    {
        private DB_Context db;
        public UserRepository(DB_Context _db)
        {
            db = _db;
        }

        public IEnumerable<User> GetUsers => db.User;

        public void Add(User _User)
        {
            db.User.Add(_User);
            db.SaveChanges();
        }

        public User GetUser(int id)
        {
            User dbEntity = db.User.Find(id);
            return dbEntity;
        }

        public void Remove(int id)
        {
            User dbEntity = db.User.Find(id);
            db.User.Remove(dbEntity);
            db.SaveChanges();
        }

        public int EmailToID(string email)
        {
            int Result = db.User.Where(x => x.Email == email).Select(x => x.UserId).SingleOrDefault();
            return Result;
        }

    }
}
