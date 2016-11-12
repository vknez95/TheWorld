using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TheWorld.Entities;
using TheWorld.Services;
using TheWorld.ViewModels;

namespace TheWorld.Controllers.Api
{
    [Route("/api/trips/{tripName}/stops")]
    [Authorize]
    public class StopsController : Controller
    {
        private IWorldRepository _repository;
        private ILogger<TripsController> _logger;
        private GeoCoordsService _coordsService;

        public StopsController(IWorldRepository repository, ILogger<TripsController> logger, GeoCoordsService coordsService)
        {
            _repository = repository;
            _logger = logger;
            _coordsService = coordsService;
        }

        [HttpGet("")]
        public IActionResult Get(string tripName)
        {
            try
            {
                var trip = _repository.GetUserTripByName(tripName, User.Identity.Name);
                var stops = Mapper.Map<IEnumerable<StopViewModel>>(trip.Stops.OrderBy(s => s.Order));

                return Ok(stops);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get stops: {0}", ex);
            }
            return BadRequest("Failed to get stops");
        }

        [HttpPost("")]
        public async Task<IActionResult> Post(string tripName, [FromBody]StopViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var newStop = Mapper.Map<Stop>(vm);

                    var result = await _coordsService.GetCoordsAsync(newStop.Name);
                    if (result.Success)
                    {
                        newStop.Longitude=result.Longitude;
                        newStop.Latitude=result.Latitude;
                        string username = User.Identity.Name;
                        
                        _repository.AddStop(tripName, newStop, username);

                        if (await _repository.SaveChangesAsync())
                        {
                            return Created($"/api/trips/{tripName}/stops/{newStop.Name}", Mapper.Map<StopViewModel>(newStop));
                        }
                    }
                    else
                    {
                        _logger.LogError(result.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to save new Stop: {0}", ex);
            }
            return BadRequest("Failed to save new stop");
        }
    }
}