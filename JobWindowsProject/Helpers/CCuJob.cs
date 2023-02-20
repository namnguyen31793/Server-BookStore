using JobWindowsProject.Helpers.Interfaces;
using LoggerService;
using LoggerService.Interfaces;
using Quartz;
using RedisSystem;
using System;
using System.Threading.Tasks;

namespace JobWindowsProject.Helpers
{
    public class CCuJob : ICcuJob
    {
        private static ILoggerManager _logger;
        private static IReportLog _report;
        public CCuJob()
        {
            _logger = new LoggerManager();
            _report = new ReportManager();
        }

        public static async Task CallSaveCCu()
        {
            try
            {
                long ccu = 0;
                string keyRedis = "Token:";
                ccu = await RedisGatewayCacheManager.Inst.CountItemByString(keyRedis);
                await _report.LogCCu(DateTime.Now, ccu);
            }
            catch (Exception ex)
            {
                await _logger.LogError("JOB-CCuJob()", ex.ToString()).ConfigureAwait(false);
            }
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                long ccu = 0;
                string keyRedis = "Token:";
                ccu = await RedisGatewayCacheManager.Inst.CountItemByString(keyRedis);
                await _report.LogCCu(DateTime.Now, ccu);
            }
            catch (Exception ex) {
                await _logger.LogError("JOB-CCuJob()", ex.ToString()).ConfigureAwait(false);
            }
        }
    }
}
