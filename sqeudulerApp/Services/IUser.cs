using sqeudulerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sqeudulerApp.Services
{
    public interface IUser
    {
        //get all users
        IEnumerable<User> GetUsers { get; }

        User GetUser(int id);
        void Add(User _User);
        void Remove(int id);
        int EmailToID(string email);
    }
}
