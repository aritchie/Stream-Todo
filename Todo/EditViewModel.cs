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
                             IUserDialogs dialogs,
                             IGeofenceManager geofences,
                             INotificationManager notifications,
                             ITodoService todoService)
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

            this.SetLocation = navigator.NavigateCommand("LocationPage");
            this.Save = ReactiveCommand.CreateFromTask(
                async () =>
                {
                    var item = this.existingItem ?? new TodoItem();
                    item.Title = this.ReminderTitle;
                    item.Notes = this.Notes;
                    if (this.RemindOnDay)
                        item.DueDateUtc = this.AlarmDate;

                    if (this.RemindOnLocation)
                    {
                        item.GpsLatitude = this.Latitude;
                        item.GpsLongitude = this.Longitude;
                    }
                    await todoService.Save(item);
                    await navigator.GoBack();
                },
                this.WhenAny(
                    x => x.ReminderTitle,
                    x => !x.GetValue().IsEmpty()
                )
            );
            this.BindBusy(this.Save);
        }


        TodoItem existingItem;
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
                    if (!parameters.ContainsKey("Item"))
                    {
                        this.Title = "New Todo";

                    }
                    else
                    {
                        this.existingItem = parameters.GetValue<TodoItem>("Item");
                        this.Title = this.existingItem.Title;
                    }
                    break;
            }
        }
    }
}

