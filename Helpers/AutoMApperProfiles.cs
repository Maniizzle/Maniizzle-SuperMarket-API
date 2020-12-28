using AutoMapper;
using Supermarket.API.Domain.Model;
using Supermarket.API.Dtos;
using System.Linq;

namespace Supermarket.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDto>()
                .ForMember(dest => dest.PhotoUrl, opt =>
                  {
                      opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
                  })
                  .ForMember(dest => dest.Age, opt =>
                    {
                        opt.MapFrom((src, d) => src.DateOfBirth.CalculateAge());
                    });

            CreateMap<User, UserForDetailedDto>()
                .ForMember(dest => dest.PhotoUrl, opt =>
            {
                opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
            })
                  .ForMember(dest => dest.Age, opt =>
                  {
                      opt.MapFrom((src, d) => src.DateOfBirth.CalculateAge());
                  }); ;
            CreateMap<Photo, PhotoForDetailedDto>();
        }
    }
}