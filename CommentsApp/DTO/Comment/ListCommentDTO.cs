using CommentsApp.DTO.User;

namespace CommentsApp.DTO.Comment;

public class ListCommentDTO
{
    public int Id { get; set; }
    
    public string Text { get; set; } = null!;
    
    public DetailUserDTO User { get; set; }  = null!;
    
    public DateTime CreatedAt { get; set; }
    
    public string? Photo { get; set; } = null;
}