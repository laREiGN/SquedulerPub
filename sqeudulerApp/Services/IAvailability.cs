using System;
using sqeudulerApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sqeudulerApp.Services
{
    public interface IAvailability
    {
        IEnumerable<Availability> GetAvailabilities { get; }

        Availability GetAvailability(int id);
        void Add(Availability _Availability);
        void Remove(int id);
    }
}
