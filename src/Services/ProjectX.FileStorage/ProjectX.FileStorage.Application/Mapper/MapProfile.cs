using AutoMapper;
using ProjectX.FileStorage.Domain;

namespace ProjectX.FileStorage.Application.Mapper
{
    public class MapProfile : Profile
    {
        public MapProfile()
        {
            CreateMap<FileEntity, FileDto>();
        }
    }
}
