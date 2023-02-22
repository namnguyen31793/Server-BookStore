using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobWindowsService.Mongo
{
    public static class NO_SQL_CONFIG
    {
        public static readonly string HOST_CONFIG = "mongodb://localhost:27017";
        public static readonly string API_TRACKING_SYSTEM_DATABASE_NAME = "TrackingLogSystem";

        public static readonly string API_LOG_CCU_COLLECTION = "CcuLog_Collection";
    }
}
