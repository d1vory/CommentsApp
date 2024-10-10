using CommentsApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CommentsApp.Data;

public abstract class BaseApplicationContext: DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Comment> Comments { get; set; }
}