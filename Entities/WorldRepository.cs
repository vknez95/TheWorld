using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace TheWorld.Entities
{
    public class WorldRepository : IWorldRepository
    {
        private WorldContext _context;
        private ILogger<WorldRepository> _logger;

        public WorldRepository(WorldContext context, ILogger<WorldRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IEnumerable<Trip> GetAllTrips()
        {
            _logger.LogInformation("Getting All Trips from the Database");
            return _context.Trips.ToList();
        }
        public IEnumerable<Trip> GetTripsByUsername(string username)
        {
            return _context.Trips
                           .Include(t => t.Stops)
                           .Where(t => t.UserName == username)
                           .ToList();
        }
        public Trip GetTripByName(string tripName)
        {
            return _context.Trips
                            .Include(t => t.Stops)
                            .Where(t => t.Name == tripName)
                            .FirstOrDefault();
        }
        public Trip GetTripById(int tripId)
        {
            return _context.Trips
                            .Include(t => t.Stops)
                            .Where(t => t.Id == tripId)
                            .FirstOrDefault();
        }
        public Trip GetUserTripByName(string tripName, string username)
        {
            return _context.Trips
                            .Include(t => t.Stops)
                            .Where(t => t.Name == tripName && t.UserName == username)
                            .FirstOrDefault();
        }
        public Stop GetStopById(int stopId)
        {
            return _context.Stops
                            .Where(t => t.Id == stopId)
                            .FirstOrDefault();
        }

        public void AddTrip(Trip trip)
        {
            _context.Add(trip);
        }
        public void AddStop(string tripName, Stop newStop, string username)
        {
            var trip = GetUserTripByName(tripName, username);

            if (trip != null)
            {
                trip.Stops.Add(newStop);
                _context.Stops.Add(newStop);
            }
        }

        public void RemoveTrip(Trip trip)
        {
            RemoveStops(trip.Stops);
            _context.Remove(trip);
        }
        public void RemoveTripById(int tripId)
        {
            var trip = GetTripById(tripId);
            if (trip != null)
            {
                RemoveTrip(trip);
            }
        }
        public void RemoveStop(Stop stop)
        {
            _context.Remove(stop);
        }
        public void RemoveStopById(int stopId)
        {
            var stop = GetStopById(stopId);
            if (stop != null)
            {
                _context.Remove(stop);
            }
        }
        public void RemoveStops(ICollection<Stop> stops)
        {
            _context.RemoveRange(stops);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}