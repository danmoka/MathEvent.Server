﻿using MathEvent.Contracts.Services;
using MathEvent.Entities.Entities;
using Quartz;
using System;
using System.Threading.Tasks;

namespace MathEvent.ScheduledJobs.Jobs
{
    /// <summary>
    /// Выполняет работу по оповещению пользователей о начале событий через неделю
    /// </summary>
    public class WeekLeftBeforeTheEventEmailNotificationJob : IJob
    {
        private readonly IEmailSender _emailSender;

        private readonly IEventService _eventService;

        private readonly IUserService _userService;

        public WeekLeftBeforeTheEventEmailNotificationJob(
            IEmailSender emailSender,
            IEventService eventService,
            IUserService userService)
        {
            _emailSender = emailSender;
            _eventService = eventService;
            _userService = userService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var nextWeekDayDate = DateTime.Now.AddDays(7);
            var events = await _eventService.GetEventsByDate(nextWeekDayDate, nextWeekDayDate);

            foreach (var ev in events)
            {
                foreach (var userModel in ev.ApplicationUsers)
                {
                    var user = await _userService.RetrieveAsync(userModel.Id);

                    if (user is not null)
                    {
                        var message = CreateNotificationMessage(user.Email, ev.Name, ev.StartDate);
                        _emailSender.SendEmail(message);
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