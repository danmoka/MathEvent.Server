using Microsoft.Extensions.DependencyInjection;
using Quartz;
using ScheduledJobs.Jobs;

namespace ScheduledJobs.Extensions
{
    public static class ServiceExtension
    {
        public static void ConfigureQuartz(this IServiceCollection services)
        {
            services.AddTransient<WeekLeftBeforeTheEventEmailNotificationJob>();
            services.AddQuartz(q =>
            {
                q.SchedulerId = "Scheduler-Core";
                q.UseMicrosoftDependencyInjectionJobFactory();
                q.UseSimpleTypeLoader();
                q.UseInMemoryStore();
                q.UseDefaultThreadPool(tp =>
                {
                    tp.MaxConcurrency = 10;
                });

                q.ScheduleJob<WeekLeftBeforeTheEventEmailNotificationJob>(trigger => trigger
                    .WithIdentity("Daily 6am trigger")
                    .WithCronSchedule("0 0 6 * * ?")
                    .WithDescription("Daily 6am trigger for WeekLeftBeforeTheEventEmailNotificationJob")
                );
            });
            services.AddQuartzServer(options =>
            {
                options.WaitForJobsToComplete = true;
            });
        }
    }
}
