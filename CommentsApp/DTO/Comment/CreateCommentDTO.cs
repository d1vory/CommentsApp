using System.ComponentModel.DataAnnotations;

namespace CommentsApp.DTO.Comment;

public class CreateCommentDTO
{
    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = null!;
    
    [Required]
    [DataType(DataType.EmailAddress)]
    [MaxLength(150)]
    public string Email { get; set; } = null!;

    [DataType(DataType.Url)]
    [MaxLength(250)]
    public string HomePage { get; set; } = "";

    [Required]
    public string Text { get; set; } = null!;
}