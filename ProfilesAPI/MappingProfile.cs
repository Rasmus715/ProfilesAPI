using AutoMapper;
using ProfilesAPI.Models;
using ProfilesAPI.ViewModels;

namespace ProfilesAPI;

public class MappingProfile : Profile {
    public MappingProfile() 
    {
        CreateMap<DoctorViewModel, Doctor>();
        CreateMap<Doctor, DoctorViewModel>();
    }
}