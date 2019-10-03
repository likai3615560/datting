using System;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;
namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;

        public AuthRepository(DataContext context)
        {
            _context = context;


        }
        public async Task<User> Login(string username, string password)
        {
            var user = await _context.users.FirstOrDefaultAsync(x => x.Username == username);

            if (user == null)
                return null;
            if (!VerifyPasswordHash(password, user.UserPwdSalt, user.UserPwd))
                return null;

            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] userPwdSalt, byte[] userPwd)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(userPwdSalt))
            {
                var passwordhash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < passwordhash.Length; i++)
                {
                    if (userPwd[i] != passwordhash[i])
                        return false;
                }
            }
            return true;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordhash, passwordsolt;
            CreatePasswordHash(password, out passwordhash, out passwordsolt);
            user.UserPwd = passwordhash;
            user.UserPwdSalt = passwordsolt;
            
            await _context.users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordhash, out byte[] passwordsolt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordsolt = hmac.Key;
                passwordhash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> UserExits(string username)
        {
            if (await _context.users.AnyAsync(x => x.Username == username))
                return true;
            return false;
        }
    }
}