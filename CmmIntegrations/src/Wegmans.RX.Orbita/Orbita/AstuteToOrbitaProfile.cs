using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wegmans.RX.Orbita.Orbita
{
    public class AstuteToOrbitaProfile : Profile
    {
        public AstuteToOrbitaProfile()
        {
            CreateMap<Astute.Models.Patient, PatientToOrbitaObject>()
                .ForMember(dest => dest.TransactionId, opt => opt.MapFrom(src => src.AddressId));
        }
    }
}
