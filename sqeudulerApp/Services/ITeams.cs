using sqeudulerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sqeudulerApp.Services
{
    public interface ITeams
    {
        IEnumerable<Teams> GetTeams { get; }

        Teams GetTeam(int id);
        void Add(Teams _Teams);
        void Remove(int id);
        void Remove(string id);
    }
}
