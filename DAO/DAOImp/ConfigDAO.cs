using DAO.Utitlities;
using LoggerService;
using ShareData.Config.Model;
using ShareData.LogSystem;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace DAO.DAOImp
{
    public class ConfigDAO
    {
        private static ILoggerManager _logger;
        private static object _syncObject = new object();

        private static ConfigDAO _inst;

        public static ConfigDAO Inst
        {
            get
            {
                if (_inst == null)
                {
                    lock (_syncObject)
                    {
                        if (_inst == null)
                        {
                            _inst = new ConfigDAO();
                            _logger = new LoggerManager();
                        }
                    }
                }
                return _inst;
            }
        }
        public async Task<List<LauncherEventTimeModel>> GetEventTimeConfig(string eventName)
        {
            List<LauncherEventTimeModel> returnList = null;
            DBHelper db = null;
            try
            {
                db = new DBHelper(ConfigDb.LauncherConfigConnectionString);
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_EventName", eventName);
                pars[1] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                returnList = db.GetListSP<LauncherEventTimeModel>("SP_Launcher_Config_Event_Time_Config_Get_By_Name", 4, pars);
            }
            catch (Exception exception)
            {
                await _logger.LogError("SQL-GetEventTimeConfig()", exception.ToString()).ConfigureAwait(false);
            }
            finally
            {
                db?.Close();
            }
            return returnList;
        }

        public async Task<string> GetConfigLinkByName(string ConfigName)
        {
            DBHelper db = null;
            string configLink = "";
            try
            {
                db = new DBHelper(ConfigDb.LauncherConfigConnectionString);
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_ConfigName", ConfigName);
                pars[1] = new SqlParameter("@_ConfigLink", SqlDbType.VarChar) { Direction = ParameterDirection.Output, Size = -1 };
                pars[2] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_Launcher_Config_Get_ConfigLink_ByName", 4, pars);
                var response = Int32.Parse(pars[2].Value.ToString());
                if (response >= 0)
                {
                    configLink = pars[1].Value.ToString();
                }
            }
            catch (Exception exception)
            {
                await _logger.LogError("SQL-GetConfigLinkByName()", exception.ToString()).ConfigureAwait(false);
            }
            finally
            {
                db?.Close();
            }
            return configLink;
        }
        #region DONATE

        public List<DonateConfigModel> GetDonateConfig()
        {
            List<DonateConfigModel> returnList = null;
            DBHelper db = null;
            try
            {
                db = new DBHelper(ConfigDb.LauncherConfigConnectionString);
                var pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                returnList = db.GetListSP<DonateConfigModel>("SP_Launcher_Config_Donate_Config_Get", 4, pars);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-GetDonateConfig()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return returnList;
        }

        #endregion
    }
}
