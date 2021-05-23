using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using TravelAgentWeb.Data;
using TravelAgentWeb.Dtos;

namespace TravelAgentWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly TravelAgentDbContext _context;

        public NotificationController(TravelAgentDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public ActionResult FlightChanged(FlightDetailsUpdateDto flightDetailUpdateDto)
        {
            Console.WriteLine($"Webhook Recieved from: {flightDetailUpdateDto.Publisher}");

            var secretModel = _context.SubscriptionSecrets.FirstOrDefault(s =>
                s.Publisher == flightDetailUpdateDto.Publisher &&
                s.Secret == flightDetailUpdateDto.Secret);

            if(secretModel == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid Secret - Ignore Webhook");
                Console.ResetColor();
                return Ok();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Valid Webhook");
                Console.WriteLine($"Old PRice {flightDetailUpdateDto.OldPrice}");
                Console.ResetColor();
                return Ok();
            }
        }
    }
}
