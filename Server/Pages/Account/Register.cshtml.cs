using System;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CounterCulture.Services;
using CounterCulture.Models;
using CounterCulture.Utilities;

namespace CounterCulture.Pages
{
    public class RegisterModel : PageModel
    {
        public RegisterModel(
            ILogger<RegisterModel> LoggerService,
            UserManager<AppUser> UserService)
        {
            Logger = LoggerService;
            Users = UserService;
            AppUser = new AppUser();
        }

        public AppUser AppUser { get; set; }
        public ILogger<RegisterModel> Logger { get; set; }
        public UserManager<AppUser> Users { get; set; }

        public async Task<IActionResult> OnPostSubmitRegistration([FromForm] AppUser user)
        {
            // TODO: simplify with string extensions
            user.UserName = user.Email.Split('@')[0];

            var result = await Users.CreateAsync(user, user.Password);

            if(ModelState.IsValid){
                
                return LocalRedirect("/");

            } else {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
            
        }
    }
}