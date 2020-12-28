using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Supermarket.API.Domain.Model;
using Supermarket.API.Domain.Repositories;
using Supermarket.API.Persistence.Context;

namespace Supermarket.API.Persistence.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _dataContext;

        public AuthRepository(AppDbContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<User> Login(string username, string password)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(x => x.Username == username);
            if (user == null) return null;
            if (!VerifyPassword(password, user.PasswordHash, user.PasswordSalt)) return null;
            return user;
        }

        private bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i]) return false;
                }
            }
            return true;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _dataContext.Users.AddAsync(user);
            await _dataContext.SaveChangesAsync();
            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            //using using so 2
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> UserExists(string username)
        {
            if (await _dataContext.Users.AnyAsync(x => x.Username == username)) return true;
            return false;
        }
    }
}