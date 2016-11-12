using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace TheWorld.Entities
{
    public class WorldUser : IdentityUser
    {
        public DateTime FirstTrip { get; set; }
    }
}