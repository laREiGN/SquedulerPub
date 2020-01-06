using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sqeudulerApp.Models;

namespace sqeudulerApp.Services
{
    public interface IRequests
    {
        //get all users
        IEnumerable<Requests> GetAllRequests{ get; }
        Requests GetRequests(int id);
        void Add(Requests _Requests);
        void Remove(int id);

    }
}
