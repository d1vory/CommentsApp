using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommentsApp.Models;

public class Comment
{
    public int Id { get; set; }
    
    public string Text { get; set; } = null!;
    
    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public User User { get; set; }  = null!;
    
    public DateTime CreatedAt { get; set; }

    [StringLength(1000)] 
    public string? File { get; set; } = null;
}