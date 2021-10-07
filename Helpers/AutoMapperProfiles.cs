using System.Linq;
using AutoMapper;
using MessagingApp.API.Dtos;
using MessagingApp.API.Models;

namespace MessagingApp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDto>()
            .ForMember(dest => dest.PhotoUrl, opt => {
                opt.MapFrom(src => src.Photos.FirstOrDefault(photo => photo.IsMain).Url);
            })
            .ForMember(dest => dest.Age, opt => {
                opt.ResolveUsing(d => d.DateOfBirth.CalculateAge());
            }
            );
            CreateMap<User, UserForDetailsDto>()
             .ForMember(dest => dest.PhotoUrl, opt => {
                 opt.MapFrom(src => src.Photos.FirstOrDefault(photo => photo.IsMain).Url);
             })
             .ForMember(dest => dest.Age, opt => {
                opt.ResolveUsing(d => d.DateOfBirth.CalculateAge());
            }
            );
            CreateMap<Photo,PhotosForDetailsDto>(); 
        }
    }
}