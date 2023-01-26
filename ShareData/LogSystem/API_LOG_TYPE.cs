using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareData.LogSystem
{
    public enum API_LOG_TYPE : int
    {
        NONE = 0,
        CASH_IN = 1,
        CASH_OUT = 2,
        CMS = 3,
        TRACKING = 4,
        SQL = 5,
        RECEIVE_API = 6,
        JOB = 7,
        UTILS = 8,
        RECEIVE_CONFIG = 9,
        RECEIVE_LOBBY = 10,
        RECEIVE_LIVE = 11,
        REDIS = 12,
        LOGIC = 13,
    }
}
