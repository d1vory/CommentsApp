using System.Text.RegularExpressions;
using AutoMapper;
using CommentsApp.Data;
using CommentsApp.DTO.Comment;
using CommentsApp.DTO.Common;
using CommentsApp.Models;
using HttpExceptions.Exceptions;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace CommentsApp.Services;


public class CommentService
{
    private static readonly string[] AllowedPhotoExtensions = [".jpg", ".jpeg", ".gif", ".png"];
    private static readonly string[] AllowedFileExtensions = [".txt"];
    private static readonly string[] AllowedHtmlTags = ["a", "code", "i", "strong"];
    private readonly BaseApplicationContext _db;
    private readonly IMapper _mapper;
    private IWebHostEnvironment _webHostEnvironment;
    


    public CommentService(BaseApplicationContext db, IMapper mapper, IWebHostEnvironment webHostEnvironment)
    {
        _db = db;
        _mapper = mapper;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<DetailCommentDTO> CreateComment(CreateCommentDTO dto,  IFormFile? photo, int? parentCommentId=null)
    {
        await ValidateComment(dto);
        var user = await GetOrCreateUser(dto);
        string? imagePath = null;
        if (photo != null)
        {
            imagePath = await UploadFile(photo);
        }
        var comment = new Comment() { Text = dto.Text, User = user, File = imagePath, CreatedAt = DateTime.Now};
        if (parentCommentId != null)
        {
            var parentComment = await _db.Comments.FindAsync(parentCommentId);
            if (parentComment == null) throw new NotFoundException("Parent comment is not found");
            comment.ParentCommentId = parentCommentId;
            comment.DiscussionKey = parentComment.DiscussionKey;
        }
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
    
    public async Task<List<RepliesCommentDTO>> GetCommentsRepliesList(int parentCommentId)
    {
        var parentComment = await _db.Comments.FindAsync(parentCommentId);
        if (parentComment == null) throw new NotFoundException("Parent comment is not found");
        var comments = _db.Comments
            .Where(c => c.DiscussionKey == parentComment.DiscussionKey && c.Id != parentCommentId)
            .OrderByDescending(c => c.CreatedAt);
        var res = new List<RepliesCommentDTO>();

        var mappedComments = await _mapper.ProjectTo<RepliesCommentDTO>(comments).ToListAsync();
        var replyMap = mappedComments.ToDictionary(comment => comment.Id);

        foreach (var comment in mappedComments)
        {
            if (comment.ParentCommentId == parentCommentId)
            {
                res.Add(comment);
            }
            else
            {
                var parent = replyMap[comment.ParentCommentId];
                parent?.Replies.Add(comment);
            }
        }
        
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
        ValidateCommentTextTags(dto.Text);
    }

    private void ValidateCommentTextTags(string text)
    {
        const string regex = @"<\s*(\/*)([a-zA-Z][a-zA-Z0-9]*)([^<>])*?>";
        var matches = Regex.Matches(text, regex);
        var tagStack = new Stack<string>();
        
        foreach (Match tagMatch in matches)
        {
            var tagName = tagMatch.Groups[2].Value;
            var isSelfClosing = tagMatch.Groups[3].Value == "/";
            var isClosing = tagMatch.Groups[1].Value == "/";

            if (isSelfClosing && isClosing)
            {
                throw new BadRequestException("Html tags are not valid");
            }
            if (isSelfClosing)
            {
                continue;
            }

            if (!AllowedHtmlTags.Contains(tagName))
            {
                throw new BadRequestException("Text has unsupported html tags!");
            }
            
            switch (isClosing)
            {
                case false:
                    tagStack.Push(tagName);
                    continue;
                case true when (tagStack.Count == 0 || tagStack.Pop() != tagName):
                    throw new BadRequestException("Html tags are not valid");
            }
        }

        if (tagStack.Count != 0)
        {
            throw new BadRequestException("Html tags are not valid");
        }
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
        var fileSize = fileForm.Length;
        if (fileSize > 100000)
        {
            throw new BadRequestException("File is too large!");
        }
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