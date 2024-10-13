using CommentsApp.DTO.User;

namespace CommentsApp.DTO.Comment;

public class DetailCommentDTO
{
    public int Id { get; set; }
    
    public string Text { get; set; } = null!;
    
    public DetailUserDTO User { get; set; }  = null!;
    
    public string? File { get; set; } = null;
}