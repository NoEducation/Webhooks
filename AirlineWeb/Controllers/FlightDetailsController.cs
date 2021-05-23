using AirlineWeb.Data;
using AirlineWeb.Dtos;
using AirlineWeb.MessageBus;
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
        private readonly IMessageBusClient _messageBusClient;

        public FlightDetailsController(AirlineDbContext airlineDbContext,
            IMapper mapper, 
            IMessageBusClient messageBusClient)
        {
            _airlineDbContext = airlineDbContext;
            _mapper = mapper;
            _messageBusClient = messageBusClient;
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

            var oldPrice = flight.Price;
            _mapper.Map(flightDetailsUpdateDto, flight);

            try
            {
                _airlineDbContext.SaveChanges();
                if(oldPrice != flight.Price)
                {
                    var message = new NotificationMessageDto()
                    {
                        WebhookType = "pricechange",
                        OldPrice = oldPrice,
                        NewPrice = flight.Price,
                        FlightCode = flight.FlightCode
                    };

                    this._messageBusClient.SendMessage(message);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return NoContent(); 
        }
    }
}
