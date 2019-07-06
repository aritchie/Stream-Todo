using System;
using System.Reactive.Linq;
using System.Windows.Input;
using Acr.UserDialogs.Forms;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Shiny;
using Shiny.Locations;
using Shiny.Notifications;


namespace Todo
{
    public class EditViewModel : ViewModel
    {
        public EditViewModel(INavigationService navigator,
                             IDataService data,
                             IUserDialogs dialogs,
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

            this.WhenAnyValue(x => x.RemindOnDay)
                .Skip(1)
                .Where(x => x)
                .SubscribeAsync(async () =>
                {
                    var access = await notifications.RequestAccess();
                    if (access != AccessState.Available)
                    {
                        this.RemindOnDay = false;
                        await dialogs.Alert("Permission denied for notifications");
                    }
                });

            this.WhenAnyValue(x => x.RemindOnLocation)
                .Skip(1)
                .Where(x => x)
                .SubscribeAsync(async () =>
                {
                    var access = await geofences.RequestAccess();
                    if (access != AccessState.Available)
                    {
                        this.RemindOnDay = false;
                        await dialogs.Alert("Permission denied for geofences");
                    }
                });

            this.WhenAnyValue(
                x => x.RemindOnLocation,
                x => x.Latitude,
                x => x.Longitude
            )
            .Select(x =>
            {
                if (!x.Item1)
                    return String.Empty;

                return $"({x.Item2} - {x.Item3})";
            })
            .ToPropertyEx(this, x => x.Location);

            this.Date = DateTime.Now.AddDays(1);
            this.Time = this.Date.TimeOfDay;

            this.SetLocation = navigator.NavigateCommand("LocationPage");
            this.Save = ReactiveCommand.CreateFromTask(
                async () =>
                {
                    // TODO: if existing, I may have to create geofences and notifications

                    // if new
                    var todo = await data.Create(newItem =>
                    {
                        newItem.Title = this.ReminderTitle;
                        newItem.Notes = this.Notes;
                        if (this.RemindOnDay)
                            newItem.DueDateUtc = this.AlarmDate;
                    });

                    if (this.RemindOnDay)
                    {
                        await notifications.Send(new Notification
                        {
                            Title = this.ReminderTitle,
                            Message = this.Notes ?? String.Empty,
                            ScheduleDate = this.AlarmDate
                        });
                    }

                    if (this.RemindOnLocation)
                    {
                        await geofences.StartMonitoring(new GeofenceRegion(
                            todo.Id.ToString(),
                            new Position(1, 1),
                            Distance.FromMeters(200)
                        )
                        {
                            NotifyOnEntry = true,
                            NotifyOnExit = false
                        });
                    }
                    await navigator.GoBack();
                },
                this.WhenAny(
                    x => x.ReminderTitle,
                    x => !x.GetValue().IsEmpty()
                )
            );
            this.BindBusy(this.Save);
        }


        ITodoItem existingItem;
        public IReactiveCommand Save { get; }
        public ICommand SetLocation { get; }
        public DateTime AlarmDate { [ObservableAsProperty] get; }
        public string Location { [ObservableAsProperty] get; }

        [Reactive] public string ReminderTitle { get; set; }
        [Reactive] public bool RemindOnDay { get; set; }
        [Reactive] public string Notes { get; set; }
        [Reactive] public DateTime Date { get; set; }
        [Reactive] public TimeSpan Time { get; set; }
        [Reactive] public double Latitude { get; set; }
        [Reactive] public double Longitude { get; set; }
        [Reactive] public bool RemindOnLocation { get; set; }


        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            var navMode = parameters.GetNavigationMode();
            switch (navMode)
            {
                case NavigationMode.Back:
                    if (!parameters.ContainsKey(nameof(this.Latitude)))
                        return;

                    this.Latitude = parameters.GetValue<double>(nameof(this.Latitude));
                    this.Longitude = parameters.GetValue<double>(nameof(this.Longitude));
                    break;

                case NavigationMode.New:
                    //if (parameters.ContainsKey("Item"))
                    //    this.existingItem = parameters.GetValue<ITodoItem>("Item");
                    this.Title = "New Todo";
                    break;
            }
        }
    }
}

