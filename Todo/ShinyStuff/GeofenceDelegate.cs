using System;
using System.Threading.Tasks;
using Shiny.Locations;
using Shiny.Notifications;


namespace Todo.Infrastructure
{
    public class GeofenceDelegate : IGeofenceDelegate
    {
        readonly INotificationManager notifications;
        readonly IDataService data;


        public GeofenceDelegate(INotificationManager notifications, IDataService data)
        {
            this.notifications = notifications;
            this.data = data;
        }


        public async Task OnStatusChanged(GeofenceState newStatus, GeofenceRegion region)
        {
            var todoId = Guid.Parse(region.Identifier);
            var todo = await this.data.GetById(todoId);

            await this.notifications.Send(todo.Title, todo.Notes);
        }
    }
}
