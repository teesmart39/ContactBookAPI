using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Model.API.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Data.API.ModelAuth;
using Core.API.Repository.Interface;

namespace Core.API.Repository.Implementation
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountRepository(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration ?? throw new ArgumentNullException();
            _roleManager = roleManager;
        }

        public async Task<IdentityResult> SignUpAsync(Register register, string role)
        {
            var existUser = await _userManager.FindByEmailAsync(register.Email);
            if (existUser != null)
            {
                throw new Exception("User already exist");
            }

            if (await _roleManager.RoleExistsAsync(role))
            {
                var user = new ApplicationUser()
                {
                    FirstName = register.FirstName,
                    LastName = register.LastName,
                    Email = register.Email,
                    UserName = register.Email
                };
                var result = await _userManager.CreateAsync(user, register.Password);
                if (!result.Succeeded)
                {

                    throw new Exception("Failed to create user");
                }
                await _userManager.AddToRoleAsync(user, role);
                return result;
            }
            else
            {
                throw new Exception("This role does not exist");
            }

        }
        public async Task<string> LoginAsync(SignInModel signIn)
        {
            var user = await _userManager.FindByEmailAsync(signIn.Email);
            if (user == null)
            {
                throw new Exception("Email does not exist");
            }
            if (!await _userManager.CheckPasswordAsync(user, signIn.Password))
            {
                throw new Exception("Invalid password");
            }
            var result = await _signInManager.PasswordSignInAsync(signIn.Email, signIn.Password, isPersistent: false, lockoutOnFailure: false);


            var authClaim = new List<Claim>
    {
        new Claim(ClaimTypes.Name, signIn.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };
            var authKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddDays(1),
                claims: authClaim,
                signingCredentials: new SigningCredentials(authKey, SecurityAlgorithms.HmacSha384Signature));
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}
