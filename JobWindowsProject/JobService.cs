using JobWindowsProject.Helpers;
using JobWindowsProject.Helpers.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JobWindowsProject
{
    public class JobService : BackgroundService
    {
        public JobService()
        {
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CCuJob.CallSaveCCu();
                await Task.Delay(60000, stoppingToken);
            }
        }


        //public static async Task Start(object stateInfo)
        //{
        //    var _scheduler = await GetScheduler();

        //    await _scheduler.Start();

        //    var getCCuJob = JobBuilder.Create<CCuJob>().Build();
        //    var getCCuTrigger = TriggerBuilder.Create().StartNow().WithSimpleSchedule(s => s.WithIntervalInMinutes(1).RepeatForever()).Build();
        //    await _scheduler.ScheduleJob(getCCuJob, getCCuTrigger);
        //}
        //public static async Task Stop()
        //{
        //    var _scheduler = await GetScheduler();
        //    await _scheduler.Shutdown();
        //}

        //private static async Task<IScheduler> GetScheduler()
        //{
        //    var props = new NameValueCollection { { "quartz.serializer.type", "binary" } };
        //    var factory = new StdSchedulerFactory(props);
        //    var scheduler = await factory.GetScheduler();
        //    return scheduler;
        //}
    }
}
