using Microsoft.AspNetCore.Identity;
using Model.API.Entity;

namespace Core.API.Repository.Interface
{
    public interface IAccountRepository
    {
        Task<IdentityResult> SignUpAsync(Register register, string role);
        Task<string> LoginAsync(SignInModel signIn);
    }
}
