using DAO.Utitlities;
using LoggerService;
using ShareData.Cms.Model;
using ShareData.LogSystem;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace DAO.DAOImp
{
    public class AdminLogDAO
    {
        private static ILoggerManager _logger;

        private static object _syncObject = new object();

        private static AdminLogDAO _inst;

        public static AdminLogDAO Inst
        {
            get
            {
                if (_inst == null)
                {
                    lock (_syncObject)
                    {
                        if (_inst == null)
                        {
                            _inst = new AdminLogDAO();
                            _logger = new LoggerManager();
                        }
                    }
                }
                return _inst;
            }
        }

        public int InSertAdminLog(string adminName, int actionid, string actionExtend, string actionDescription)
        {
            int logId = -1;
            DBHelper db = null;
            try
            {
                db = new DBHelper(ConfigDb.CmsLogConnectionString);
                var pars = new SqlParameter[6];
                pars[0] = new SqlParameter("@_AdminName", adminName);
                pars[1] = new SqlParameter("@_ActionId", actionid);
                pars[2] = new SqlParameter("@_ActionExtend", actionExtend);
                pars[3] = new SqlParameter("@_ActionDescription", actionDescription);
                pars[4] = new SqlParameter("@_ActionLogId", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[5] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("XTeam_InsertAdminLog", 4, pars);
                var responseStatus = -1;
                Int32.TryParse(pars[4].Value.ToString(), out responseStatus);
                if (responseStatus >= 0)
                    logId = Int32.Parse(pars[4].Value.ToString());
            }
            catch (Exception exception)
            {
                logId = -1;
                Task.Run(async () => await _logger.LogError("SQL-InSertAdminLog()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return logId;
        }

        public void UpdateAdminLog(int logId, int result)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(ConfigDb.CmsLogConnectionString);
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_AdminLogId", logId);
                pars[1] = new SqlParameter("@_Results", result);
                pars[2] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("XTeam_UpdateAdminLog", 4, pars);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-UpdateAdminLog()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
        }

        public List<AdminLogModels> GetActionLogAdmin(string adminName, DateTime startDate, DateTime endDate)
        {
            DBHelper db = null;
            List<AdminLogModels> response;
            try
            {
                db = new DBHelper(ConfigDb.CmsLogConnectionString);
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@_AdminName", adminName);
                pars[1] = new SqlParameter("@_StartTime", startDate);
                pars[2] = new SqlParameter("@_EndTime", endDate);
                pars[3] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                response = db.GetListSP<AdminLogModels>("XTeam_GetAdminActionLog", 4, pars);
            }
            catch (Exception exception)
            {
                response = new List<AdminLogModels>();
                Task.Run(async () => await _logger.LogError("SQL-GetActionLogAdmin()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return response;
        }
    }
}
