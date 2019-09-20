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
            var todo = await this.dataService.GetById(todoItemId);
            if (todo != null)
            {
                await this.CancelBgEvents(todo);
                await this.dataService.Delete(todo.Id);
            }
        }


        public Task Save(TodoItem todo)
        {
            if (todo.Id == default)
                return this.Create(todo);

            return this.Update(todo);
        }


        async Task Create(TodoItem todo)
        {
            await this.dataService.Create(todo);
            await this.SetupBgEvents(todo);
        }


        async Task Update(TodoItem todo)
        {
            await this.CancelBgEvents(todo);
            await this.SetupBgEvents(todo);
            await this.dataService.Update(todo);
        }


        async Task CancelBgEvents(TodoItem todo)
        {
            await this.dataService.Delete(todo.Id);
            await this.geofenceManager.StopMonitoring(todo.Id.ToString());
            // TODO: notification Id it integer based because android
            //this.notificationManager.Cancel(todoItemId.ToString());
        }


        async Task SetupBgEvents(TodoItem todo)
        {
            if (todo.DueDateUtc != null && todo.DueDateUtc > DateTime.UtcNow)
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
    }
}
