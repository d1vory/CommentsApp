namespace CommentsApp.DTO.Comment;

public class RepliesCommentDTO: DetailCommentDTO
{
    public List<RepliesCommentDTO> Replies { get; set; } = [];
    public int ParentCommentId { get; set; }
}