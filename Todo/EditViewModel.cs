using System;
using System.Reactive.Linq;
using System.Windows.Input;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Shiny.Locations;
using Shiny.Notifications;


namespace Todo
{
    public class EditViewModel : ViewModel
    {
        public EditViewModel(INavigationService navigator,
                             IDataService data,
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
                    var todo = await data.Create(newItem =>
                    {
                        newItem.Title = this.ReminderTitle;
                        newItem.Notes = this.Notes;
                        newItem.DueDateUtc = this.AlarmDate;
                    });

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
                            });
                        }
                    }
                    //if (access == Shiny.AccessState.Denied)
                    //{
                    //    await App.Current.MainPage.DisplayAlert("NO NOTIFICATIONS FOR YOU");
                    //}
                    await navigator.GoBack();
                },
                this.WhenAny(
                    x => x.ReminderTitle,
                    x => !String.IsNullOrWhiteSpace(x.GetValue())
                )
            );
            this.BindBusy(this.Save);
        }


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
            //parameters.GetValue<Reminder>("");
            if (parameters.GetNavigationMode() == NavigationMode.Back)
            {
                this.Latitude = parameters.GetValue<double>(nameof(this.Latitude));
                this.Longitude = parameters.GetValue<double>(nameof(this.Longitude));
            }
        }
    }
}

