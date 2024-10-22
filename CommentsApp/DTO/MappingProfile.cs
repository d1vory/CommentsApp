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
        CreateMap<Models.Comment, ListCommentDTO>();
        CreateMap<Models.Comment, RepliesCommentDTO>().ForMember(x => x.Replies, opt => opt.Ignore());;
    }
}