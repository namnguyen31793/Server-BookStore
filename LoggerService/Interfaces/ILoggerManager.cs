using LoggerService.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LoggerService
{
    public interface ILoggerManager
    {
        Task LogInfo(string title, string message, string status);
        Task LogWarn(string message);
        Task LogDebug(string title, string message);
        Task LogError(string title, string message);
        Task LogBuyBook(string title, long AccountId, string barcode, long price);

    }
}
