using RaftLabs.Users.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaftLabs.Users.Core.Interfaces;
 
    using RaftLabs.Users.Core.Models;
    public interface IUserService
    {

        Task<User?> GetUserByIdAsync(int id);
        Task<IEnumerable<User>> GetAllUsersAsync();
    }
 
