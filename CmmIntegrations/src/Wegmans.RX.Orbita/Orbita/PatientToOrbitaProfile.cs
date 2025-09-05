using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wegmans.RX.Orbita.Orbita
{
    public class PatientToOrbitaProfile : Profile
    {
        public PatientToOrbitaProfile()
        {
            CreateMap<PatientToOrbitaObject, OrbitaPatient>()
                .ForMember(dest => dest.DOB, opt => opt.MapFrom(src => src.DateOfBirth))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.EmailAddress))
                .ForMember(dest => dest.TransactionID, opt => opt.Ignore());
        }
    }
}
