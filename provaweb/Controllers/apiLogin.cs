using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using provaDatabase;
using System.Data;
using System.Security.Claims;

namespace provaweb.Controllers
{

    [Route("[controller]")]
    [ApiController]

    public class Login : ControllerBase
    {
        private readonly ILogger<Login> m_logger;
        private readonly Databaselogic dblogic;

        public Login(ILogger<Login> logger, Databaselogic databaselogic)
        {
            m_logger = logger;
            dblogic = databaselogic;
        }

        public record UserCredentials(string username, string password);
        public record NewUser(string username, string password, bool am);
        public record UserChangePassword(string Username, string PasswordAtt, string PasswordNuv);
        public record UserDatabase(string UserName, string Role, bool IsOnline, bool StatoAccount);
        public record StatoAccount1(string Username, bool StatoAccount);
        public record getRuolo(string Username, string ruolo);

        [HttpGet("Autenticazione")]
        [Authorize]
        public async Task<IActionResult> Autenticazione()
        {
            if (User.Identity!.Name! == "root") return Ok();
            var f = await dblogic.ConfermaStato(User.Identity!.Name!);
            if (f == true) return Ok();
            await HttpContext.SignOutAsync(scheme: CookieAuthenticationDefaults.AuthenticationScheme);
            return StatusCode(404);
        }

        [HttpPost("cookie")]
        public async Task<IActionResult> CreazioneCookie(UserCredentials UserCredentials)
        {
            var username = UserCredentials.username;
            var password = UserCredentials.password;
            var f = false;
            var r = await dblogic.VerificaAccount(username, password);
            if (r.Role == "root")
            {
                f = true;
            }
            else
            {
                f = await dblogic.ConfermaStato(username);
            }
            if (f == true)
            {
                if (r.V == true)
                {

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, username),
                        new Claim(ClaimTypes.Role,r.Role),

                    };
                    var claimsIdentity = new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddYears(1),
                        AllowRefresh = true,
                    };
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                    return Ok();
                }
            }
            return BadRequest();
        }

        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(scheme: CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }

        [HttpPost("AggiuntaAccount")]
        [Authorize(Roles = "Admin,root")]

        public async Task<IActionResult> AggiuntaAccount(NewUser NewUser)
        {
            var password = NewUser.password;
            var username = NewUser.username;
            var am = NewUser.am;
            if (am == true)
            {
                try
                {
                    await dblogic.AgiuntaAccount(username, "Admin", password);
                }
                catch (DbUpdateException)
                {
                    return StatusCode(404);
                }
            }
            else
            {
                try
                {
                    await dblogic.AgiuntaAccount(username, "Basic", password);
                }
                catch (DbUpdateException)
                {
                    return StatusCode(404);
                }
            }
            var isroot = User.Claims.Where(x => x.Type == ClaimTypes.Name).Select(x => x.Value == "root").First();
            var u = await dblogic.UtentiDatabase1();
            if (isroot && u.Any())
            {
                await HttpContext.SignOutAsync(scheme: CookieAuthenticationDefaults.AuthenticationScheme);
                return Ok();
            }
            return Ok();
        }
        [HttpGet("GetRuolo")]
        [Authorize(Roles = "Admin,Basic,root")]
        public ActionResult<getRuolo> GetRuolo()
        {
            var NomeUtente = User.Identity!.Name;
            var ruolo = User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).First();
            return Ok(new getRuolo(NomeUtente!, ruolo));
        }


        [HttpGet("Getlistuser")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDatabase>>> Getlistuser([FromServices] ActiveUsersService activeUsers)
        {
            var u = await dblogic.UtentiDatabase1();
            return Ok(u.Select(x => new UserDatabase(x.UserName, x.Ruolo, activeUsers.IsActive(x.UserName), x.StatoAccount)));
        }

        [HttpPut("StatoAccount")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> StatoAccount(StatoAccount1 statoAccount1)
        {
            var username = statoAccount1.Username;
            var StatoAccount = statoAccount1.StatoAccount;
            await dblogic.StatoAccount(username, StatoAccount);
            return Ok();

        }

        [HttpPut("cambiaPassword")]
        [Authorize(Roles = "Admin,Basic")]
        public async Task<IActionResult> CambiaPassword(UserChangePassword UserChangePassword)
        {
            var nomeutente = UserChangePassword.Username;
            var passwordvecchia = UserChangePassword.PasswordAtt;
            var passwordnuova = UserChangePassword.PasswordNuv;
            await dblogic.AggiornamnetoPassword(nomeutente, passwordvecchia, passwordnuova);
            return Ok();
        }


    }
}