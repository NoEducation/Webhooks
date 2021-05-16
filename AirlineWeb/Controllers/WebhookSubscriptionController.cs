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
    public class WebhookSubscriptionController : ControllerBase
    {
        private readonly AirlineDbContext _airlineDbContext;
        private readonly IMapper _mapper;

        public WebhookSubscriptionController(AirlineDbContext airlineDbContext,
            IMapper mapper)
        {
            this._airlineDbContext = airlineDbContext;
            this._mapper = mapper;
        }

        [HttpGet("{secret}", Name = "GetSubscriptionBySecret")]
        public ActionResult<WebhookSubscriptionReadDto> GetSubscriptionBySecret([FromRoute] string secret)
        {
            var subscription = this._airlineDbContext.WebhookSubscriptions.FirstOrDefault(x => x.Secret == secret);

            if (subscription == null)
            {
                return NotFound();
            }

            return Ok(this._mapper.Map<WebhookSubscriptionReadDto>(subscription));
            
        }

        [HttpPost]
        public ActionResult<WebhookSubscriptionReadDto> CreateSubscription(
            WebhookSubscriptionCreateDto webhookSubscriptionDto)
        {

            var subscription = _airlineDbContext.WebhookSubscriptions.FirstOrDefault(s => s.WebhookUri == webhookSubscriptionDto.WebhookUri);

            if(subscription == null)
            {
                subscription = _mapper.Map<WebhookSubscription>(webhookSubscriptionDto);
                subscription.Secret = Guid.NewGuid().ToString();
                subscription.WebhookPublisher = "PanAus";

                try
                {
                    _airlineDbContext.WebhookSubscriptions.Add(subscription);
                    _airlineDbContext.SaveChanges();
                }
                catch(Exception ex)
                {
                    return BadRequest(ex.Message);
                }

                var subscriptionReadDto = _mapper.Map<WebhookSubscriptionReadDto>(subscription);

                return CreatedAtRoute(
                    nameof(GetSubscriptionBySecret), 
                    new { secret = subscriptionReadDto.Secret },
                    subscriptionReadDto);
            }
            else
            {
                return NoContent();
            }
        }

    }
}
