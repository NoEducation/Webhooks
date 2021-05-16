using AirlineWeb.Dtos;
using AirlineWeb.Models;
using AutoMapper;

namespace AirlineWeb.Profiles
{
    public class FlightDetailsProfile : Profile
    {
        public FlightDetailsProfile()
        {
            this.CreateMap<FlightDetailsCreateDto, FlightDetails>();
            this.CreateMap<FlightDetailsUpdateDto, FlightDetails>();
            this.CreateMap<FlightDetails,FlightDetailsReadDto>();         
        }
    }
}
