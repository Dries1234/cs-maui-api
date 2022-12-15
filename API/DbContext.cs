using Microsoft.EntityFrameworkCore;
using API.Models;
namespace API.Services
{
    public class ApiContext: DbContext
    {
        public ApiContext(DbContextOptions<ApiContext> opt) : base(opt)
        {

        }
        public DbSet<Repository> Repositories { get; set; }
        public DbSet<Language> Language { get; set; }
    }
}
