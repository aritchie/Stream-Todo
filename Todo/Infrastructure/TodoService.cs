using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shiny.Locations;
using Shiny.Notifications;

namespace Todo.Infrastructure
{
    public class TodoService : ITodoService
    {
        readonly IDataService dataService;
        readonly INotificationManager notificationManager;
        readonly IGeofenceManager geofenceManager;


        public TodoService(IDataService dataService,
                           INotificationManager notificationManager,
                           IGeofenceManager geofenceManager)
        {
            this.dataService = dataService;
            this.notificationManager = notificationManager;
            this.geofenceManager = geofenceManager;
        }


        public Task<IList<TodoItem>> GetList(bool includeCompleted) => this.dataService.GetAll(includeCompleted);

        public async Task Remove(Guid todoItemId)
        {
            await this.dataService.Delete(todoItemId);
            await this.geofenceManager.StopMonitoring(todoItemId.ToString());
            //this.notificationManager.Cancel(todoItemId.ToString());
        }

        public Task Save(TodoItem todo)
        {
            if (todo.Id == default(Guid))
                return this.Create(todo);

            return this.Update(todo);
        }


        async Task Create(TodoItem todo)
        {
            // TODO: if existing, I may have to create geofences and notifications

            // if new
            await this.dataService.Create(todo);

            if (todo.DueDateUtc != null)
            {
                await this.notificationManager.Send(new Notification
                {
                    Title = todo.Title,
                    Message = todo.Notes ?? String.Empty,
                    ScheduleDate = todo.DueDateUtc.Value
                });
            }

            if (todo.GpsLatitude != null)
            {
                await this.geofenceManager.StartMonitoring(new GeofenceRegion(
                    todo.Id.ToString(),
                    new Position(todo.GpsLatitude.Value, todo.GpsLongitude.Value),
                    Distance.FromMeters(200)
                )
                {
                    NotifyOnEntry = true,
                    NotifyOnExit = false
                });
            }
        }


        async Task Update(TodoItem todo)
        {
            // TODO: for unit testing later
            // TODO: cancel notification & geofence if completed

        }
    }
}
