using System;
using System.Threading.Tasks;
using Shiny.Locations;
using Shiny.Notifications;
using Todo.Models;

namespace Todo.Infrastructure
{
    public class GeofenceDelegate : IGeofenceDelegate
    {
        readonly INotificationManager notifications;
        readonly TodoSqliteConnection conn;


        public GeofenceDelegate(INotificationManager notifications, TodoSqliteConnection conn)
        {
            this.notifications = notifications;
            this.conn = conn;
        }


        public async Task OnStatusChanged(GeofenceState newStatus, GeofenceRegion region)
        {
            var todoId = Guid.Parse(region.Identifier);
            var todo = await this.conn.GetAsync<TodoItem>(todoId);

            await this.notifications.Send(todo.Title, todo.Notes);

            todo.CompletionDate = DateTime.Now;
            await this.conn.UpdateAsync(todo);
        }
    }
}
