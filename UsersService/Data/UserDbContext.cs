using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using UsersService.Models;

namespace UsersService.Data
{
    public class UsersDbContext : DbContext
    {
        public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
    }
}
