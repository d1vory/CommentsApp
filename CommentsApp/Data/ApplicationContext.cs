using Bogus;
using CommentsApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CommentsApp.Data;

public class ApplicationContext: BaseApplicationContext
{
    protected readonly IConfiguration _configuration;

    public ApplicationContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        optionsBuilder.UseSqlServer(connectionString).LogTo(Console.WriteLine, LogLevel.Information);
    }
}