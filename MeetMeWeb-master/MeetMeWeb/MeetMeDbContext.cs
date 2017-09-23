using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using MeetMeWeb.Models;

namespace MeetMeWeb
{

    public class MeetMeDbContext : IdentityDbContext<User>
    {
        public MeetMeDbContext()
            : base("name=MeetMeDB")
        {

        }

        public static MeetMeDbContext Create()
        {
            return new MeetMeDbContext();
        }

        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<Meeting> Meetings { get; set; }
        public virtual DbSet<Connection> Connections { get; set; }
        public virtual DbSet<ConnectionNotification> ConnectionNotifications { get; set; }
        public virtual DbSet<MeetingRequest> MeetingRequests { get; set; }
    }
}