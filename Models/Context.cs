using Microsoft.EntityFrameworkCore;
 
namespace brightideas.Models
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }
	    public DbSet<User> User { get; set; }
        public DbSet<Post> Post { get; set; }
        public DbSet<Like> Like { get; set; }

    }
}
