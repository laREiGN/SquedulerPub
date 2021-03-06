﻿using sqeudulerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sqeudulerApp.Services
{
    public interface ITeams
    {
        IEnumerable<Teams> GetTeams { get; }

        Teams GetTeam(string id);
        void Add(Teams _Teams);
        void Remove(string id);
    }
}
