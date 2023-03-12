using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LoggerService.Interfaces
{
    public interface IReportLog
    {
        Task LogBuyBook(string title, long AccountId, string barcode, long price);
        Task LogCCu(DateTime time, long ccu);

        Task TrackingActionUser(string data);
        Task TrackingActionHome(string data);
        Task TrackingFindBook(string data);
        Task TrackingListenAudio(string data);
    }
}
