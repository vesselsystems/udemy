using AutoMapper;
using BookstoreApp.Api.Data;
using BookstoreApp.Api.Models.Authors;

namespace BookstoreApp.Api.Configurations;

public class MapperConfig : Profile
{
    public MapperConfig()
    {
        CreateMap<AuthorCreateDto, Author>().ReverseMap();
        CreateMap<AuthorReadOnlyDto, Author>().ReverseMap();
        CreateMap<AuthorUpdateDto, Author>().ReverseMap();
    }
}
