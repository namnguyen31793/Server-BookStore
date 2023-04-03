using LoggerService;
using LoggerService.Report;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Schedule
{
    public class SaveLogJob : IJob
    {
        private readonly ILoggerManager _logger;
        public SaveLogJob(ILoggerManager logger)
        {
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await AddLogHoursOnline();
        }

        private async Task AddLogHoursOnline()
        {
            try
            {
                //get log by hour
                string lastHour = DateTime.Now.AddHours(-1).ToString("yyyy-MM-dd HH");

                var listData = await Tracking_Online_Manager.Inst.GetListOnlineHourseByTime(DateTime.Now.Date.AddDays(-1), DateTime.Now);
                var model = listData.FirstOrDefault(x => x.TimePlay.Equals(lastHour));
                if (model != null)
                {
                    await Tracking_Online_Manager.Inst.AddLogHoursOnline(lastHour, model.TimeOnline);
                }
            }
            catch (Exception exception)
            {
                await _logger.LogError("SQL-AddLogHoursOnline()", exception.ToString()).ConfigureAwait(false);
            }
        }
    }
}
