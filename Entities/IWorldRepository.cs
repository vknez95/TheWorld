using System.Collections.Generic;
using System.Threading.Tasks;

namespace TheWorld.Entities
{
    public interface IWorldRepository
    {
        IEnumerable<Trip> GetAllTrips();
        Trip GetTripByName(string tripName);
        void AddTrip(Trip trip);
        void AddStop(string tripName, Stop newStop);
        Task<bool> SaveChangesAsync();
    }
}