using AutoMapper;
using CommentsApp.Data;
using CommentsApp.DTO.Comment;
using CommentsApp.DTO.Common;
using CommentsApp.Models;
using HttpExceptions.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace CommentsApp.Services;


public class CommentService
{
    private static readonly string[] AllowedPhotoExtensions = [".jpg", ".jpeg", ".gif", ".png"];
    private static readonly string[] AllowedFileExtensions = [".txt"];
    private readonly BaseApplicationContext _db;
    private readonly IMapper _mapper;
    private IWebHostEnvironment _webHostEnvironment;
    


    public CommentService(BaseApplicationContext db, IMapper mapper, IWebHostEnvironment webHostEnvironment)
    {
        _db = db;
        _mapper = mapper;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<DetailCommentDTO> CreateComment(CreateCommentDTO dto,  IFormFile? photo)
    {
        await ValidateComment(dto);
        var user = await GetOrCreateUser(dto);
        string? imagePath = null;
        if (photo != null)
        {
            imagePath = await UploadFile(photo);
        }
        var comment = new Comment() { Text = dto.Text, User = user, File = imagePath, CreatedAt = DateTime.Now};
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

    private async Task<string> UploadFile(IFormFile file)
    {
        var fileName = $"{Guid.NewGuid()}-{Path.GetFileName(file.FileName)}";
        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);
        
        var extension = Path.GetExtension(filePath).ToLower();

        if (AllowedPhotoExtensions.Contains(extension))
        {
            await SaveImage(file, filePath);
        }
        else if (AllowedFileExtensions.Contains(extension))
        {
            await SaveFile(file, filePath);
        }
        else
        {
            throw new BadRequestException("This file type is not supported");
        }
        
        var imagePath = $"images/{fileName}";
        return imagePath;
    }

    private async Task SaveFile(IFormFile fileForm, string path)
    {
        using (var stream = new FileStream(path, FileMode.Create))
        {
            await fileForm.CopyToAsync(stream);
        }
    }
    
    private async Task SaveImage(IFormFile imageForm, string imagePath)
    {

        try
        {
            using (var stream = imageForm.OpenReadStream())
            {
                var image = await Image.LoadAsync(stream);
                if (image == null) throw new BadRequestException("Provided file is not image!");
                var width = image.Width;
                var height = image.Height;
                if (width > 320 || height > 240)
                {
                    image.Mutate(x => x.Resize(320, 240));
                }

                await image.SaveAsync(imagePath);
            }
        }
        catch (Exception e) when (e is InvalidImageContentException or UnknownImageFormatException)
        {
            throw new BadRequestException("Provided file is not image!");
        }
    }

}