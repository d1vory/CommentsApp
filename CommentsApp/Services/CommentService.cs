using AutoMapper;
using CommentsApp.Data;
using CommentsApp.DTO.Comment;
using CommentsApp.DTO.Common;
using CommentsApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CommentsApp.Services;


public class CommentService
{
    protected readonly BaseApplicationContext _db;
    protected readonly IMapper _mapper;


    public CommentService(BaseApplicationContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<DetailCommentDTO> CreateComment(CreateCommentDTO dto)
    {
        await ValidateComment(dto);
        var user = await GetOrCreateUser(dto);
        var comment = new Comment() { Text = dto.Text, User = user };
        await _db.Comments.AddAsync(comment);
        await _db.SaveChangesAsync();
        var createdCommentDTO = _mapper.Map<DetailCommentDTO>(comment);
        return createdCommentDTO;
    }
    
    public async Task<PaginatedList<ListCommentDTO>> GetCommentsList(int pageIndex, int pageSize, string sortOrder)
    {
        var comments = SortItems(_db.Comments, sortOrder);
        
        
        var res = await PaginatedList<ListCommentDTO>.CreateAsync(comments, pageIndex, pageSize, _mapper);
        return res;
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

    private IQueryable<Comment> SortItems(IQueryable<Comment> items, string sortOrder)
    {
        items = items.Include(c => c.User);
        switch (sortOrder)
        {
            case "username":
                items = items.OrderBy(c => c.User.Username);
                break;
            case "-username":
                items = items.OrderByDescending(c => c.User.Username);
                break;
            case "email":
                items = items.OrderBy(c => c.User.Email);
                break;
            case "-email":
                items = items.OrderByDescending(c => c.User.Email);
                break;
            case "createdAt":
                items = items.OrderBy(c => c.CreatedAt);
                break;
            case "-createdAt":
                items = items.OrderByDescending(c => c.CreatedAt);
                break;
        }

        return items;
    }

    private async Task ValidateComment(CreateCommentDTO dto)
    {
        await ValidateCommentText(dto.Text);
    }

    private async Task ValidateCommentText(string text)
    {
        
    }


}