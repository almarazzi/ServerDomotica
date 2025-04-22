using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using provaweb;
using System.Data;

namespace provaDatabase
{

    public static class AddserviceDatabase
    {
        public static IServiceCollection addServiceDatabase(this IServiceCollection services)
        {
            services.AddDbContext<BloggingContext>();
            services.AddScoped<Databaselogic>();
            return services;
        }
    }
    public class Databaselogic(BloggingContext db)
    {
        public record Oggect(string Role, bool V);
        public async Task AgiuntaAccount(string nomeUtente, string ruolo, string password)
        {
            string f = new PasswordHasher<object>().HashPassword(null!, password);
            await db.AddAsync(new Users { UserName = nomeUtente, Ruolo = ruolo, Password = f, StatoAccount = true });
            await db.SaveChangesAsync();

        }
        public async Task AggiornamnetoPassword(string nomeUtente, string vecchiaPassword, string nuovaPassword)
        {
            var dd = await db.Users.Where(x => x.UserName == nomeUtente).FirstOrDefaultAsync();
            if (dd == null)
                return;

            var res = new PasswordHasher<object>().VerifyHashedPassword(null!, dd.Password, vecchiaPassword);
            if (res == PasswordVerificationResult.Success || res == PasswordVerificationResult.SuccessRehashNeeded)
            {
                dd.Password = new PasswordHasher<object>().HashPassword(null!, nuovaPassword);
                await db.SaveChangesAsync();
            }
        }

        public async Task<Oggect> VerificaAccount(string nomeUtente, string password)
        {
            if (nomeUtente == "root" && password == "root")
            {
                var hasUsers = await db.Users.AnyAsync();
                if (!hasUsers)
                {
                    return new Oggect("root", true);
                }
                return new Oggect("", false);
            }

            var dd = await db.Users.Where(x => x.UserName == nomeUtente).FirstOrDefaultAsync();
            if (dd != null)
            {

                var res = new PasswordHasher<object>().VerifyHashedPassword(null!, dd.Password, password);
                if (res == PasswordVerificationResult.SuccessRehashNeeded)
                {

                    dd.Password = new PasswordHasher<object>().HashPassword(null!, password);
                    await db.SaveChangesAsync();
                }

                return new Oggect(dd.Ruolo, res == PasswordVerificationResult.Success || res == PasswordVerificationResult.SuccessRehashNeeded);
            }

            return new Oggect("", false);


        }

        public Task<List<Users>> UtentiDatabase1()
        {
            return db.Users.ToListAsync();
        }

        public async Task StatoAccount(string Nomeutente, bool StatoAccount)
        {
            var dd = await db.Users.Where(x => x.UserName == Nomeutente).FirstOrDefaultAsync();

            if (dd != null)
            {
                dd.StatoAccount = StatoAccount;
                await db.SaveChangesAsync();
            }

        }

        public async Task<bool> ConfermaStato(string Nomeutente)
        {
            return await db.Users.Where(x => x.UserName == Nomeutente).Select(x => x.StatoAccount).FirstOrDefaultAsync();

        }

    }
}
