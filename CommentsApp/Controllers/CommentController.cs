using CommentsApp.DTO.Comment;
using CommentsApp.Services;
using Microsoft.AspNetCore.Mvc;
using PhotoRater.Utils;

namespace CommentsApp.Controllers;

[Route("comments")]
public class CommentController: ControllerBase
{
    private readonly CommentService _service;

    public CommentController(CommentService service)
    {
        _service = service;
    }

    [HttpGet]
    [Route("")]
    public async Task<ActionResult> GetComments([FromQuery]int pageIndex=1, int pageSize=25, string sortOrder = "-createdAt")
    {
        var comments = await _service.GetCommentsList(pageIndex, pageSize, sortOrder);
        return Ok(comments);
    }
    
    [HttpGet]
    [Route("{commentId}/replies")]
    public async Task<ActionResult> GetReplyComments([FromRoute] int commentId)
    {
        var comments = await _service.GetCommentsRepliesList(commentId);
        return Ok(comments);
    }

    [HttpPost]
    [Route("")]
    public async Task<ActionResult> CreateComment([FromForm] CreateCommentDTO dto, IFormFile? file)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState.GetModelErrors());
        var comment = await _service.CreateComment(dto, file);
        
        return Ok(comment);
    }
    
    [HttpPost]
    [Route("{commentId}/reply")]
    public async Task<ActionResult> CreateReplyComment([FromRoute] int commentId, [FromForm] CreateCommentDTO dto, IFormFile? file)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState.GetModelErrors());
        var comment = await _service.CreateComment(dto, file, commentId);
        
        return Ok(comment);
    }
    
}