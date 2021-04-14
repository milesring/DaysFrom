using Acr.UserDialogs;
using DaysFrom.Models;
using DaysFrom.Services;
using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DaysFrom.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        INotificationManager notificationManager;
        
        public MainPageViewModel()
        {
            notificationManager = DependencyService.Get<INotificationManager>();

            //TODO: Grouping by Future/Past/Favorite?
            Events = new ObservableRangeCollection<Event>();
            RefreshCommand = new AsyncCommand(Refresh);
            AddEventCommand = new AsyncCommand(AddEvent);
            RemoveEventCommand = new AsyncCommand<Event>(RemoveEvent);
            EditEventCommand = new AsyncCommand<Event>(EditEvent);
        }

        #region PropertyBackers
        private Event _selectedEvent;
        #endregion

        #region Properties
        public ObservableRangeCollection<Event> Events
        {
            get;
        }

        public Event SelectedEvent
        {
            get => _selectedEvent;
            set => SetProperty(ref _selectedEvent, value);
        }
        #endregion

        #region Commands
        public AsyncCommand RefreshCommand { get; }
        public AsyncCommand AddEventCommand { get; }
        public AsyncCommand<Event> RemoveEventCommand { get; }
        public AsyncCommand<Event> EditEventCommand { get; }

        #endregion

        #region CommandActions
        async Task Refresh()
        {
            if (IsBusy)
            {
                return;
            }
            IsBusy = true;
            Events.Clear();
            var events = await EventDataService.GetEvent();
            Events.AddRange(events);
            IsBusy = false;
        }

        async Task AddEvent()
        {
            string name = await Application.Current.MainPage.DisplayPromptAsync("Event Name", "Name of event");
            if (string.IsNullOrEmpty(name))
            {
                return;
            }
            string description = await Application.Current.MainPage.DisplayPromptAsync("Event description", "Description of event");
            if (string.IsNullOrEmpty(description))
            {
                return;
            }

            var dateResult = await UserDialogs.Instance.DatePromptAsync(new DatePromptConfig
            {
                IsCancellable = true,
                MinimumDate = DateTime.Now.AddYears(-100),
                MaximumDate = DateTime.Now.AddYears(100),
            });
            if (!dateResult.Ok)
            {
                return;
            }
            var selectedDate = dateResult.SelectedDate;

            bool timeRequest = await Application.Current.MainPage.DisplayAlert("Specify Time", "Would you like to specify a time?", "Yes", "No");
            if (timeRequest)
            {
                var timeResult = await UserDialogs.Instance.TimePromptAsync(new TimePromptConfig
                {
                    IsCancellable = true
                });
                if (!timeResult.Ok)
                {
                    return;
                }
                selectedDate = selectedDate.Add(timeResult.SelectedTime);
            }
            

            await EventDataService.AddEvent(new Event() { Name = name, Description = description, EventDate = selectedDate });
            notificationManager.SendNotification("DaysFrom", $"Event {name} added!");
            await Refresh();
        }

        async Task RemoveEvent(Event eventModel)
        {
            await EventDataService.RemoveEvent(eventModel.Id);
            await Refresh();
        }

        async Task EditEvent(Event eventModel)
        {
            string name = await Application.Current.MainPage.DisplayPromptAsync("Event Name", "Name of event", placeholder:eventModel.Name, initialValue:eventModel.Name);
            if (string.IsNullOrEmpty(name))
            {
                return;
            }
            string description = await Application.Current.MainPage.DisplayPromptAsync("Event description", "Description of event", placeholder: eventModel.Description, initialValue:eventModel.Description);
            if (string.IsNullOrEmpty(description))
            {
                return;
            }

            var result = await UserDialogs.Instance.DatePromptAsync(new DatePromptConfig
            {
                IsCancellable = true,
                MinimumDate = DateTime.Now.AddYears(-100),
                MaximumDate = DateTime.Now.AddYears(100),
                SelectedDate = eventModel.EventDate
            });
            if (!result.Ok)
            {
                return;
            }
            var selectedDate = result.SelectedDate;

            bool timeRequest = await Application.Current.MainPage.DisplayAlert("Specify Time", "Would you like to specify a time?", "Yes", "No");
            if (timeRequest)
            {
                var timeResult = await UserDialogs.Instance.TimePromptAsync(new TimePromptConfig
                {
                    IsCancellable = true,
                    SelectedTime = eventModel.EventDate.TimeOfDay
                });
                if (!timeResult.Ok)
                {
                    return;
                }

                selectedDate = selectedDate.Add(timeResult.SelectedTime);
            }

            if (!name.Equals(eventModel.Name))
            {
                eventModel.Name = name;
            }

            if (!description.Equals(eventModel.Description))
            {
                eventModel.Description = description;
            }

            if(result.SelectedDate != eventModel.EventDate)
            {
                eventModel.EventDate = selectedDate;
            }
            await EventDataService.AddEvent(eventModel);
            await Refresh();
        }
        #endregion




    }
}
