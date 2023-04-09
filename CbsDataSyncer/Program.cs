using System;
using CbsDataSyncer.App_Code;
using Quartz;
using Quartz.Impl;
using System.Collections.Specialized;
using System.Configuration;
using Quartz.Impl.Calendar;

namespace CbsDataSyncer
{
    public class Program
    {
        private static Int32 Hour
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["Hour"]);
            }
        }

        private static Int32 Minute
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["Minute"]);
            }
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("Starting Cbs Syncer");
            new Program().ExecuteCbsSyncerCronJob();
            Console.Read();
        }

        public async void ExecuteCbsSyncerCronJob()
        {
            // construct a scheduler factory
            NameValueCollection props = new NameValueCollection { { "quartz.serializer.type", "binary" } };
            StdSchedulerFactory factory = new StdSchedulerFactory(props);

            // get a scheduler
            IScheduler sched = await factory.GetScheduler();
            await sched.Start();

            HolidayCalendar calendar = new HolidayCalendar();
            // define the job and tie it to our CbsSyncer class
            IJobDetail job = JobBuilder.Create<CbsSyncer>()
                .WithIdentity("CbsSyncer", "cbsSyncer")
                .Build();

            // Trigger the job to run now, and then every 40 seconds
            ITrigger trigger = TriggerBuilder.Create()
                                             .StartNow()
                                             .WithIdentity("CbsSyncer", "cbsSyncer")
                                             .WithCronSchedule("0 0 1 * * ?") // runs everyday at 1 am.
                                             .Build();

            await sched.ScheduleJob(job, trigger);
        }
    }
}
