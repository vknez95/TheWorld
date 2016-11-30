using System.Collections.Generic;
using System.Threading.Tasks;

namespace TheWorld.Entities
{
    public interface IWorldRepository
    {
        IEnumerable<Trip> GetAllTrips();
        IEnumerable<Trip> GetTripsByUsername(string username);
        Trip GetTripByName(string tripName);
        Trip GetTripById(int tripId);
        Trip GetUserTripByName(string tripName, string username);
        Stop GetStopById(int stopId);
        void RemoveStops(ICollection<Stop> stops);

        void AddTrip(Trip trip);
        void AddStop(string tripName, Stop newStop, string username);

        void RemoveTrip(Trip trip);
        void RemoveTripById(int tripId);
        void RemoveStop(Stop stop);
        void RemoveStopById(int stopId);
        
        Task<bool> SaveChangesAsync();
    }
}