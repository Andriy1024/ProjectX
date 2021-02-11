using AutoMapper;
using ProjectX.Blog.Domain;
using ProjectX.Core;

namespace ProjectX.Blog.Application
{
    public class BlogProfile : Profile
    {
        public BlogProfile()
        {
            CreateMap<ArticleEntity, ArticleDto>()
                .ForMember(d => d.CreatedAt, v => v.MapFrom(e => e.CreatedAt.ToUnixMilliseconds()))
                .ForMember(d => d.UpdatedAt, v => v.MapFrom(e => e.UpdatedAt.ToUnixMilliseconds()));

            CreateMap<CommentEntity, CommentDto>()
                .ForMember(d => d.CreatedAt, v => v.MapFrom(e => e.CreatedAt.ToUnixMilliseconds()))
                .ForMember(d => d.UpdatedAt, v => v.MapFrom(e => e.UpdatedAt.ToUnixMilliseconds()));

            CreateMap<AuthorEntity, AuthorDto>();
        }
    }
}
