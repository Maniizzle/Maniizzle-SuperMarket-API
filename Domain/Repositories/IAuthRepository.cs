using Supermarket.API.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Supermarket.API.Domain.Repositories
{
    public interface IAuthRepository
    {
        Task<User> Register(User user, string password);

        Task<User> Login(string username, string password);

        Task<bool> UserExists(string username);
    }
}