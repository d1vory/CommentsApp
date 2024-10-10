using AutoMapper;
using CommentsApp.DTO.Comment;
using CommentsApp.DTO.User;

namespace CommentsApp.DTO;

public class MappingProfile: Profile
{
    public MappingProfile()
    {
        CreateMap<Models.User, DetailUserDTO>();
        CreateMap<Models.Comment, DetailCommentDTO>();

    }
}