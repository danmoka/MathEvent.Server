using MathEvent.Contracts;
using MathEvent.Entities.Entities;
using MathEvent.Services.Services;
using Quartz;
using System;
using System.Threading.Tasks;

namespace ScheduledJobs.Jobs
{
    /// <summary>
    /// Выполняет работу по оповещению пользователей о начале событий через неделю
    /// </summary>
    public class WeekLeftBeforeTheEventEmailNotificationJob : IJob
    {
        private readonly IEmailSender _emailSender;

        private readonly EventService _eventService;

        private readonly UserService _userService;

        public WeekLeftBeforeTheEventEmailNotificationJob(
            IEmailSender emailSender,
            EventService eventService,
            UserService userService)
        {
            _emailSender = emailSender;
            _eventService = eventService;
            _userService = userService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var nextWeekDayDate = DateTime.Now.AddDays(7);
            var eventsResult = await _eventService.GetEventsByDate(nextWeekDayDate, nextWeekDayDate);

            if (eventsResult.Succeeded)
            {
                var events = eventsResult.Entity;

                foreach (var ev in events)
                {
                    foreach (var userModel in ev.ApplicationUsers)
                    {
                        var userResult = await _userService.RetrieveAsync(userModel.Id);

                        if (userResult.Succeeded)
                        {
                            var user = userResult.Entity;
                            var message = CreateNotificationMessage(user.Email, ev.Name, ev.StartDate);
                            _emailSender.SendEmail(message);
                        }
                    }
                }
            }
        }

        private static Message CreateNotificationMessage(string email, string eventName, DateTime eventDate)
        {
            var subject = "Напоминание: до события осталась одна неделя!";
            var content = $"Событие \"{eventName}\" состоится через неделю ({eventDate})";

            return new Message(
                new string[] { email },
                subject,
                content);
        }
    }
}
