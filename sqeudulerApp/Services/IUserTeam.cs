﻿using sqeudulerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sqeudulerApp.Services
{
    public interface IUserTeam
    {
        IEnumerable<UserTeam> GetUserTeams { get; }

        UserTeam GetUserTeam(int id);
        void Add(UserTeam _UserTeam);
        void Remove(int id);
    }
}
