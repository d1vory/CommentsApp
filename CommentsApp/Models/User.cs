using System.ComponentModel.DataAnnotations;

namespace CommentsApp.Models;

public class User
{
    public int Id { get; set; }
    
    [StringLength(50)]
    public string Username { get; set; } = null!;
    
    [StringLength(150)]
    public string Email { get; set; } = null!;
    
    [StringLength(250)]
    public string HomePage { get; set; } = null!;
    
}