using AirlineWeb.Data;
using AirlineWeb.Dtos;
using AirlineWeb.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace AirlineWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightDetailsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly AirlineDbContext _airlineDbContext;

        public FlightDetailsController(AirlineDbContext airlineDbContext, IMapper mapper)
        {
            _airlineDbContext = airlineDbContext;
            _mapper = mapper;
        }

        [HttpGet("{flightCode}", Name = "GetFlightDetailsByCode")]
        public ActionResult<FlightDetailsReadDto> GetFlightDetailsByCode([FromRoute] string flightCode)
        {
            var flightDetails = this._airlineDbContext.FlightDetails.FirstOrDefault(x => x.FlightCode == flightCode);

            if(flightDetails == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<FlightDetailsReadDto>(flightDetails));
        }

        [HttpPost]
        public ActionResult<FlightDetailsReadDto> CrateFlight([FromBody] FlightDetailsCreateDto flightDetailsCreateDto)
        {
            var flight = this._airlineDbContext.FlightDetails.FirstOrDefault(x => x.FlightCode == flightDetailsCreateDto.FlightCode);

            if(flight == null)
            {
                var flightDetailModel = _mapper.Map<FlightDetails>(flightDetailsCreateDto);

                try
                {
                    _airlineDbContext.FlightDetails.Add(flightDetailModel);
                    _airlineDbContext.SaveChanges();
                }
                catch(Exception ex)
                {
                    return BadRequest(ex.Message);
                }

                return CreatedAtRoute(
                    nameof(GetFlightDetailsByCode), 
                    new { flightCode = flightDetailModel.FlightCode },
                    _mapper.Map<FlightDetailsReadDto>(flightDetailModel));
            }
            else
            {
                return NoContent();
            }
        }

        [HttpPut("{id}")]
        public ActionResult UpdateFlightDetails(int id, FlightDetailsUpdateDto flightDetailsUpdateDto)
        {
            var flight = _airlineDbContext.FlightDetails.FirstOrDefault(f => f.Id == id);

            if(flight == null)
            {
                return NotFound();
            }

            _mapper.Map(flightDetailsUpdateDto, flight);
            _airlineDbContext.SaveChanges();

            return NoContent();
        }


    }
}
