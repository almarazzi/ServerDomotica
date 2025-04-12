using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using provaweb;
using System.Data;

namespace provaDatabase
{
    internal class Databaselogic
    {
        public record Oggect(string Role, bool V);

        public Databaselogic()
        {

        }


        public async Task AgiuntaAccount(string nomeUtente, string ruolo, string password)
        {
            using var db = new BloggingContext();
            string f = new PasswordHasher<object>().HashPassword(null!, password);
            await db.AddAsync(new Users { UserName = nomeUtente, Ruolo = ruolo, Password = f, StatoAccount = true });
            await db.SaveChangesAsync();
            
        }
        public async Task AggiornamnetoPassword(string nomeUtente, string vecchiaPassword, string nuovaPassword)
        {
            using var db = new BloggingContext();

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
            using var db = new BloggingContext();

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
            using var db = new BloggingContext();
            return db.Users.ToListAsync();
        }

        public async Task StatoAccount(string Nomeutente, bool StatoAccount)
        {
            using var db = new BloggingContext();
            var dd = await db.Users.Where(x => x.UserName == Nomeutente).FirstOrDefaultAsync();

            if (dd != null)
            {
                dd.StatoAccount = StatoAccount;
                await db.SaveChangesAsync();
            }

        }

        public async Task<bool> ConfermaStato(string Nomeutente)
        {
            using var db = new BloggingContext();
            return await db.Users.Where(x => x.UserName == Nomeutente).Select(x => x.StatoAccount).FirstOrDefaultAsync();

        }

       

    }
}
