using System;
using System.Reactive.Linq;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Shiny.Locations;
using Shiny.Notifications;
using Todo.Models;

namespace Todo
{
    public class EditViewModel : ViewModel
    {
        public EditViewModel(TodoSqliteConnection conn, 
                             INotificationManager notifications, 
                             IGeofenceManager geofences)
        {
            this.WhenAnyValue(
                x => x.Date,
                x => x.Time
            )
            .Select(x => new DateTime(
                x.Item1.Year,
                x.Item1.Month,
                x.Item1.Day,
                x.Item2.Hours,
                x.Item2.Minutes,
                0
            ))
            .ToPropertyEx(this, x => x.AlarmDate);

            this.Save = ReactiveCommand.CreateFromTask(
                async () =>
                {
                    var todo = new TodoItem
                    {
                        Title = this.ReminderTitle,
                        Notes = this.Notes,
                        DueDate = this.AlarmDate
                    };
                    await conn.InsertAsync(todo);

                    if (this.RemindOnDay)
                    {
                        var access = await notifications.RequestAccess();
                        if (access == Shiny.AccessState.Available)
                        {
                            await notifications.Send(new Notification
                            {
                                Title = this.ReminderTitle,
                                Message = this.Notes,
                                ScheduleDate = this.AlarmDate
                            });
                        }
                    }

                    if (this.RemindOnLocation)
                    {
                        var access = await geofences.RequestAccess();
                        if (access == Shiny.AccessState.Available)
                        {
                            await geofences.StartMonitoring(new GeofenceRegion(
                                todo.Id.ToString(),
                                new Position(1, 1),
                                Distance.FromMeters(200)
                            )
                            {
                                NotifyOnEntry = true,
                                NotifyOnExit = false
                                //SingleUse = true
                            });
                        }
                    }
                    //if (access == Shiny.AccessState.Denied)
                    //{
                    //    await App.Current.MainPage.DisplayAlert("NO NOTIFICATIONS FOR YOU");
                    //}
                },
                this.WhenAny(
                    x => x.ReminderTitle,
                    x => String.IsNullOrWhiteSpace(x.GetValue())
                )
            );
            this.BindBusy(this.Save);
        }


        public IReactiveCommand Save { get; }
        public IReactiveCommand SetLocation { get; }
        public DateTime AlarmDate { [ObservableAsProperty] get; }

        [Reactive] public string ReminderTitle { get; set; }
        [Reactive] public bool RemindOnDay { get; set; }
        [Reactive] public string Notes { get; set; }
        [Reactive] public DateTime Date { get; set; }
        [Reactive] public TimeSpan Time { get; set; }
        [Reactive] public bool RemindOnLocation { get; set; }

        //public override void OnNavigatedTo(INavigationParameters parameters)
        //{
        //    parameters.GetValue<Reminder>("");
        //}
    }
}

