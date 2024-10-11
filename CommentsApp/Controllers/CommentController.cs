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

    [HttpPost]
    [Route("")]
    public async Task<ActionResult> CreateComment([FromBody] CreateCommentDTO dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState.GetModelErrors());
        var comment = await _service.CreateComment(dto);
        
        return Ok(comment);
    }
    
    
}