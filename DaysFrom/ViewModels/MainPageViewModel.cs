using Acr.UserDialogs;
using DaysFrom.Models;
using DaysFrom.Services;
using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

            Events = new ObservableRangeCollection<Event>();
            EventGroups = new ObservableCollection<EventGroup>();
            RefreshCommand = new AsyncCommand(Refresh);
            AddEventCommand = new AsyncCommand(AddEvent);
            RemoveEventCommand = new AsyncCommand<Event>(RemoveEvent);
            EditEventCommand = new AsyncCommand<Event>(EditEvent);
            FavoriteEventCommand = new AsyncCommand<Event>(FavoriteEvent);
        }

        #region PropertyBackers
        private Event _selectedEvent;
        #endregion

        #region Properties
        public ObservableRangeCollection<Event> Events
        {
            get;
        }
        public ObservableCollection<EventGroup> EventGroups
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

        public AsyncCommand<Event> FavoriteEventCommand { get; }

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
            EventGroups.Clear();
            var events = await EventDataService.GetEvents();
            var futureList = new List<Event>();
            var pastList = new List<Event>();
            var favoriteList = new List<Event>();
            var currentList = new List<Event>();
            foreach (var eventItem in events)
            {
                if (eventItem.Favorite)
                {
                    favoriteList.Add(eventItem);
                }
                else if (eventItem.EventDate <= DateTime.Now && eventItem.EventEndDate >= DateTime.Now)
                {
                    currentList.Add(eventItem);
                }
                else if (eventItem.EventDate <= DateTime.Now)
                {
                    pastList.Add(eventItem);
                }
                else
                {
                    futureList.Add(eventItem);
                }
            }
            var groups = new EventGroup[] {
                new EventGroup("Favorite Events", favoriteList),
                new EventGroup("Current Events", currentList),
                new EventGroup("Future Events", futureList),
                new EventGroup("Past Events", pastList)
            };


            //this avoids empty headers
            for (int i = 0; i < groups.Length; i++)
            {
                if (groups[i].Count > 0)
                {
                    EventGroups.Add(groups[i]);
                }
            }

            Events.AddRange(events);
            IsBusy = false;
        }

        async Task AddEvent()
        {
            //TODO: Remove acrPopups and use XTC implementation
            var newEvent = new Event();
            newEvent.Name = await Application.Current.MainPage.DisplayPromptAsync("Event Name", "Name of event");
            if (string.IsNullOrEmpty(newEvent.Name))
            {
                return;
            }
            newEvent.Description = await Application.Current.MainPage.DisplayPromptAsync("Event description", "Description of event");
            if (string.IsNullOrEmpty(newEvent.Description))
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
            newEvent.EventDate = dateResult.SelectedDate;

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
                newEvent.EventDate = newEvent.EventDate.Add(timeResult.SelectedTime);
            }

            var endDateRequest = await Application.Current.MainPage.DisplayAlert("End Date", "Would you like to speficy an end date for this event?", "Yes", "No");
            if (endDateRequest)
            {
                var endDateResult = await UserDialogs.Instance.DatePromptAsync(new DatePromptConfig
                {
                    IsCancellable = true,
                    MinimumDate = DateTime.Now.AddYears(-100),
                    MaximumDate = DateTime.Now.AddYears(100)
                });
                if (!endDateResult.Ok)
                {
                    return;
                }
                newEvent.EventEndDate = endDateResult.SelectedDate;
                timeRequest = await Application.Current.MainPage.DisplayAlert("Specify Time", "Would you like to specify a time for the end date?", "Yes", "No");
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
                    newEvent.EventEndDate = newEvent.EventEndDate.Add(timeResult.SelectedTime);
                }
            }
            await EventDataService.AddEvent(newEvent);
            //notificationManager.SendNotification("DaysFrom", $"Event {name} added!");
            await Refresh();
        }

        async Task RemoveEvent(Event eventModel)
        {
            await EventDataService.RemoveEvent(eventModel.Id);
            await EventNotificationDataService.RemoveEventNotificationByEventId(eventModel.Id);
            await Refresh();
        }

        async Task EditEvent(Event eventModel)
        {

            //TODO: Edit event end date 
            eventModel.Name = await Application.Current.MainPage.DisplayPromptAsync("Event Name", "Name of event", placeholder: eventModel.Name, initialValue: eventModel.Name);
            if (string.IsNullOrEmpty(eventModel.Name))
            {
                return;
            }
            eventModel.Description = await Application.Current.MainPage.DisplayPromptAsync("Event description", "Description of event", placeholder: eventModel.Description, initialValue: eventModel.Description);
            if (string.IsNullOrEmpty(eventModel.Description))
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

            eventModel.EventDate = result.SelectedDate.Add(eventModel.EventDate.TimeOfDay);

            var timeRequestTitle = "Specify Time";
            var timeRequestString = "Would you like to specify a time?";
            if (eventModel.EventDate.TimeOfDay != DateTime.MinValue.TimeOfDay)
            {
                timeRequestTitle = "Edit Time";
                timeRequestString = "Would you like to edit the event time?";
            }
            bool timeRequest = await Application.Current.MainPage.DisplayAlert(timeRequestTitle, timeRequestString, "Yes", "No");
            if (timeRequest)
            {
                var eventTimeResult = await UserDialogs.Instance.TimePromptAsync(new TimePromptConfig
                {
                    IsCancellable = true,
                    SelectedTime = eventModel.EventDate.TimeOfDay
                });
                if (!eventTimeResult.Ok)
                {
                    return;
                }

                eventModel.EventDate = eventModel.EventDate.Add(eventTimeResult.SelectedTime);
            }
            bool editEndDateResult = await Application.Current.MainPage.DisplayAlert("Edit End Date", "Would you like to add or edit the event's end date or time?", "Yes", "No");
            if (editEndDateResult)
            {
                result = await UserDialogs.Instance.DatePromptAsync(new DatePromptConfig
                {
                    IsCancellable = true,
                    MinimumDate = eventModel.EventDate,
                    MaximumDate = DateTime.Now.AddYears(100),
                    SelectedDate = eventModel.EventEndDate
                });
                if (!result.Ok)
                {
                    return;
                }
                eventModel.EventEndDate = result.SelectedDate.Add(eventModel.EventEndDate.TimeOfDay);

                timeRequestTitle = "Specify Time";
                timeRequestString = "Would you like to specify a time for the event end date?";
                if (eventModel.EventDate.TimeOfDay != DateTime.MinValue.TimeOfDay)
                {
                    timeRequestTitle = "Edit Time";
                    timeRequestString = "Would you like to edit the event end time?";
                }
                bool editEndDateTimeResult = await Application.Current.MainPage.DisplayAlert(timeRequestTitle, timeRequestString, "Yes", "No");
                if (editEndDateTimeResult)
                {
                    var endEventTimeResult = await UserDialogs.Instance.TimePromptAsync(new TimePromptConfig
                    {
                        IsCancellable = true,
                        SelectedTime = eventModel.EventEndDate.TimeOfDay
                    });
                    if (!endEventTimeResult.Ok)
                    {
                        return;
                    }
                    eventModel.EventEndDate = eventModel.EventEndDate.Add(endEventTimeResult.SelectedTime);
                }
            }

            await EventDataService.AddEvent(eventModel);
            await Refresh();
        }

        async Task FavoriteEvent(Event eventModel)
        {
            eventModel.Favorite = !eventModel.Favorite;
            await EventDataService.AddEvent(eventModel);
            await Refresh();
        }
        #endregion




    }
}
