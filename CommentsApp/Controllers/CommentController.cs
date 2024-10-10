using CommentsApp.DTO.Comment;
using CommentsApp.Services;
using Microsoft.AspNetCore.Mvc;
using PhotoRater.Utils;

namespace CommentsApp.Controllers;

[Route("comments")]
public class CommentController: ControllerBase
{
    private readonly CommentService _service;

    [HttpGet]
    [Route("")]
    public async Task<ActionResult> GetComments()
    {
        return Ok("Hello world");
    }

    [HttpPost]
    [Route("")]
    public async Task<ActionResult> CreateComment([FromBody] CreateCommentDTO dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState.StringifyModelErrors());
        var comment = await _service.CreateComment(dto);
 
        
        return Ok("Created");
    }
    
    
}