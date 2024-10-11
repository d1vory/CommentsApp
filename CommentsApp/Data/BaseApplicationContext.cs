using Bogus;
using CommentsApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CommentsApp.Data;

public abstract class BaseApplicationContext: DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Comment> Comments { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        SeedFakeData(modelBuilder);
    }
    
    private void SeedFakeData(ModelBuilder modelBuilder)
    {
        var random = new Random(100);
        Randomizer.Seed = random;
        var userIds = 20;
        var userFaker = new Faker<User>()
            .RuleFor(u => u.Id, f => userIds++)
            .RuleFor(u => u.Username, f => f.Internet.UserName())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            //.RuleFor(u => u.HomePage, f => f.Random.Int(0, 100).OrDefault<string>(f.Internet.Url(), .3f, ""));
            .RuleFor(u => u.HomePage, f => random.Next(5) == 1 ? f.Internet.Url() : "");
        var users = userFaker.Generate(50);
        modelBuilder.Entity<User>().HasData(users);

        var commentIds = 20;
        var commentFaker = new Faker<Comment>()
            .RuleFor(c => c.Id, f => commentIds++)
            .RuleFor(c => c.Text, f => f.Lorem.Text())
            .RuleFor(c => c.UserId, f => f.PickRandom(users).Id)
            .RuleFor(c => c.CreatedAt, f => f.Date.Recent(7, new DateTime(2024, 10, 11)));
        var comments = commentFaker.Generate(150);
        
        modelBuilder.Entity<Comment>().HasData(comments);

    }
}