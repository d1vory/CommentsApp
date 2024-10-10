using CommentsApp.Data;
using CommentsApp.DTO.Comment;
using CommentsApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CommentsApp.Services;


public class CommentService
{
    protected readonly BaseApplicationContext _db;

    public CommentService(BaseApplicationContext db)
    {
        _db = db;
    }

    public async Task<DetailCommentDTO> CreateComment(CreateCommentDTO dto)
    {
        await ValidateComment(dto);
        var user = await GetOrCreateUser(dto);
        var comment = new Comment() { Text = dto.Text, User = user };
        await _db.Comments.AddAsync(comment);
        await _db.SaveChangesAsync();
        
    }

    private async Task<User> GetOrCreateUser(CreateCommentDTO dto)
    {
        var user = await _db.Users.Where(u => u.Email == dto.Email).FirstOrDefaultAsync();
        if (user != null) return user;

        user = new User() { Email = dto.Email, Username = dto.Username, HomePage = dto.HomePage };
        await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();
        return user;
    }

    private async Task ValidateComment(CreateCommentDTO dto)
    {
        await ValidateCommentText(dto.Text);
    }

    private async Task ValidateCommentText(string text)
    {
        
    }
}