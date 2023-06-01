using System.Security.Cryptography;
using System.Text;
using turbo_funicular.Models;

namespace turbo_funicular.Data 
{
    public class Seed 
    {
        public static void SeedData(IApplicationBuilder applicationBuilder) 
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope()) 
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                context.Database.EnsureCreated();

                if (!context.Users.Any()) 
                {
                    string passwordHash;
                    using (var sha256 = SHA256.Create())
                    {
                        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes("secret"));
                        passwordHash = Convert.ToBase64String(hashedBytes);
                    }

                    context.Users.Add(new User()
                            {
                                Username = "admin",
                                PasswordHash = passwordHash,
                                CreateDate = DateTime.Now
                            });

                    context.SaveChanges();
                }
            }
        }
    }
}