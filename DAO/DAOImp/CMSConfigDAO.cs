using DAO.Utitlities;
using LoggerService;
using ShareData.DAO;
using ShareData.LogSystem;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace DAO.DAOImp
{
    public class CMSConfigDAO
    {
        private static ILoggerManager _logger;

        private static object _syncObject = new object();
        private static CMSConfigDAO _inst;
        public static CMSConfigDAO Inst
        {
            get
            {
                if (_inst == null)
                {
                    lock (_syncObject)
                    {
                        if (_inst == null)
                        {
                            _inst = new CMSConfigDAO();
                            _logger = new LoggerManager();
                        }
                    }
                }
                return _inst;
            }
        }

        #region FEATURE CONFIG
        public List<CmsFeatureConfig> GetFeatureConfigs()
        {
            DBHelper db = null;
            List<CmsFeatureConfig> response;
            try
            {
                db = new DBHelper(ConfigDb.CMSConfigConnectionString);
                var pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                response = db.GetListSP<CmsFeatureConfig>("SP_BanCaBoss_CMSConfig_Get_FeatureStatus", 4, pars);
            }
            catch (Exception exception)
            {
                response = new List<CmsFeatureConfig>();
                Task.Run(async () => await _logger.LogError("SQL-GetFeatureConfigs()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return response;
        }

        public List<CmsFeatureConfig> GetBillingFeatureConfigs()
        {
            DBHelper db = null;
            List<CmsFeatureConfig> response;
            try
            {
                db = new DBHelper(ConfigDb.CMSConfigConnectionString);
                var pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                response = db.GetListSP<CmsFeatureConfig>("SP_BanCaBoss_CMSConfig_Get_BillingFeatureStatus", 4, pars);
            }
            catch (Exception exception)
            {
                response = new List<CmsFeatureConfig>();
                Task.Run(async () => await _logger.LogError("SQL-GetBillingFeatureConfigs()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return response;
        }

        public int SetFeatureConfig(CmsFeatureConfig dataSet)
        {
            int response;
            DBHelper db = null;
            try
            {
                db = new DBHelper(ConfigDb.CMSConfigConnectionString);
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_FeatureName", dataSet.FeatureName);
                pars[1] = new SqlParameter("@_FeatureStatus", dataSet.FeatureStatus);
                pars[2] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_BanCaBoss_CMSConfig_Set_FeatureStatus", 4, pars);
                response = Int32.Parse(pars[2].Value.ToString());
            }
            catch (Exception exception)
            {
                response = -9999;
                Task.Run(async () => await _logger.LogError("SQL-SetFeatureConfig()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return response;
        }

        public int SetBillingFeatureConfig(CmsFeatureConfig dataSet)
        {
            int response;
            DBHelper db = null;
            try
            {
                db = new DBHelper(ConfigDb.CMSConfigConnectionString);
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_FeatureName", dataSet.FeatureName);
                pars[1] = new SqlParameter("@_FeatureStatus", dataSet.FeatureStatus);
                pars[2] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_BanCaBoss_CMSConfig_Set_BillingFeatureStatus", 4, pars);
                response = Int32.Parse(pars[2].Value.ToString());
            }
            catch (Exception exception)
            {
                response = -9999;
                Task.Run(async () => await _logger.LogError("SQL-SetBillingFeatureConfig()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return response;
        }
        #endregion



        public List<CardCashInModel> GetCardCashinModels()
        {
            List<CardCashInModel> returnList = null;
            DBHelper db = null;
            try
            {
                db = new DBHelper(ConfigDb.CmsConnectionString);
                var pars = new SqlParameter[0];
                returnList = db.GetListSP<CardCashInModel>("SP_BanCaBoss_CMS_Get_TopupCardConfig", 4, pars);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-GetCardCashinModels()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return returnList;
        }

        public List<CardCashOutModel> GetCardCashOutModels()
        {
            List<CardCashOutModel> returnList = null;
            DBHelper db = null;
            try
            {
                db = new DBHelper(ConfigDb.CmsConnectionString);
                var pars = new SqlParameter[0];
                returnList = db.GetListSP<CardCashOutModel>("SP_BanCaBoss_CMS_Get_CashOutCardConfig", 4, pars);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-GetCardCashOutModels()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return returnList;
        }

        public List<CashinServiceModel> GetCashinServiceModelList()
        {
            List<CashinServiceModel> returnList = null;
            DBHelper db = null;
            try
            {
                db = new DBHelper(ConfigDb.CmsConnectionString);
                var pars = new SqlParameter[0];
                returnList = db.GetListSP<CashinServiceModel>("SP_BanCaBoss_Telco_GetCashinService_New", 4, pars);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-GetCashinServiceModelList()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return returnList;
        }
        public CmsTelcoMultiConfig GetTelcoMultiConfigs()
        {
            DBHelper db = null;
            CmsTelcoMultiConfig response;
            try
            {
                db = new DBHelper(ConfigDb.CmsConnectionString);
                var pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                response = db.GetInstanceSP<CmsTelcoMultiConfig>("SP_BanCaBoss_CMS_Get_TelCoMultiConfig", 4, pars);
            }
            catch (Exception exception)
            {
                response = new CmsTelcoMultiConfig();
                Task.Run(async () => await _logger.LogError("SQL-GetTelcoMultiConfigs()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return response;
        }

        public List<FeatureConfig> GetFeatureConfig()
        {
            DBHelper db = null;
            List<FeatureConfig> response;
            try
            {
                db = new DBHelper(ConfigDb.CmsConnectionString);
                var pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                response = db.GetListSP<FeatureConfig>("SP_BanCaBoss_CMS_Get_FeatureStatus", 4, pars);
            }
            catch (Exception exception)
            {
                response = new List<FeatureConfig>();
                Task.Run(async () => await _logger.LogError("SQL-GetFeatureConfig()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return response;
        }

        public List<ServiceStatusModel> GetServiceStatusConfig()
        {
            DBHelper db = null;
            List<ServiceStatusModel> response;
            try
            {
                db = new DBHelper(ConfigDb.CmsConnectionString);
                var pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                response = db.GetListSP<ServiceStatusModel>("SP_BanCaBoss_CMS_Get_ServiceStatus", 4, pars);
            }
            catch (Exception exception)
            {
                response = new List<ServiceStatusModel>();
                Task.Run(async () => await _logger.LogError("SQL-GetServiceStatusConfig()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return response;
        }
        #region NOTIFY MESSAGE LOGIN
        public List<LoginMessageModel> GetLoginMessageInfo()
        {
            DBHelper db = null;
            List<LoginMessageModel> response;
            try
            {
                db = new DBHelper(ConfigDb.CMSConfigConnectionString);
                var pars = new SqlParameter[0];
                response = db.GetListSP<LoginMessageModel>("SP_BanCaBoss_CMSConfig_Get_ListLoginPopUpMessage", 4, pars);
            }
            catch (Exception exception)
            {
                response = new List<LoginMessageModel>();
                Task.Run(async () => await _logger.LogError("SQL-GetLoginMessageInfo()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return response;
        }

        public int SetLoginMessageConfig(int messageId, string message)
        {
            int response;
            DBHelper db = null;
            try
            {
                db = new DBHelper(ConfigDb.CMSConfigConnectionString);
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_MessageId", messageId);
                pars[1] = new SqlParameter("@_Message", message);
                pars[2] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_BanCaBoss_CMSConfig_Update_LoginPopUpMessage", 4, pars);
                response = Int32.Parse(pars[2].Value.ToString());
            }
            catch (Exception exception)
            {
                response = -9999;
                Task.Run(async () => await _logger.LogError("SQL-SetLoginMessageConfig()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return response;
        }
        #endregion

        #region NEWS CONFIG
        public List<NewsConfigModel> GetNewsConfigModels()
        {
            List<NewsConfigModel> returnList = null;
            DBHelper db = null;
            try
            {
                db = new DBHelper(ConfigDb.CMSConfigConnectionString);
                var pars = new SqlParameter[0];
                returnList = db.GetListSP<NewsConfigModel>("SP_BanCaBoss_CMSConfig_GetAll_NewsConfig", 4, pars);
                return returnList;
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-GetNewsConfigModels()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            returnList = new List<NewsConfigModel>();
            return returnList;
        }


        public NewsConfigModel GetNewsConfigModelById(int id)
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(ConfigDb.CMSConfigConnectionString);
                var pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@_Id", id);
                NewsConfigModel returnList = db.GetInstanceSP<NewsConfigModel>("SP_BanCaBoss_CMSConfig_NewsConfig_GetById", 4, pars);
                return returnList;
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-GetNewsConfigModelById()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return new NewsConfigModel();
        }

        public int UpdateNewsConfigModel(NewsConfigModel newsModel)
        {
            DBHelper db = null;
            try
            {
                if (newsModel.ImageUrl == null) newsModel.ImageUrl = string.Empty;
                db = new DBHelper(ConfigDb.CMSConfigConnectionString);
                var pars = new SqlParameter[6]
                {
                    new SqlParameter("@_Id",newsModel.Id),
                    new SqlParameter("@_Title", newsModel.Title),
                    new SqlParameter("@_Content",newsModel.Content),
                    new SqlParameter("@_ImageUrl",newsModel.ImageUrl),
                    new SqlParameter("@_Active",newsModel.Active),
                    new SqlParameter("@_Order",newsModel.Order)
                };

                int result = db.ExecuteNonQuerySP("SP_BanCaBoss_CMSConfig_NewsConfig_Update", pars);
                return result;

            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-UpdateNewsConfigModel()", exception.ToString()).ConfigureAwait(false));
                return -1;
            }
            finally
            {
                db?.Close();
            }
        }
        #endregion

        #region URL HELPER

        public List<UrtPortConfigModel> GetUrlSupportConfig(int IndexConfig)
        {
            DBHelper db = null;
            List<UrtPortConfigModel> response;
            try
            {
                db = new DBHelper(ConfigDb.CMSConfigConnectionString);
                var pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@_IndexConfig", IndexConfig);
                response = db.GetListSP<UrtPortConfigModel>("[SP_BanCaBoss_CMSConfig_Get_UrlSupportConfig]", 4, pars);
            }
            catch (Exception exception)
            {
                response = new List<UrtPortConfigModel>();
                Task.Run(async () => await _logger.LogError("SQL-GetUrlSupportConfig()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return response;
        }
        public int SetUrlSupportConfig(int id, string Hotline, string FanpageUrl, string FanpageSmsUrl, string ZaloUrl)
        {
            int response;
            DBHelper db = null;
            try
            {
                db = new DBHelper(ConfigDb.CMSConfigConnectionString);
                var pars = new SqlParameter[6];
                pars[0] = new SqlParameter("@_IndexConfig", id);
                pars[1] = new SqlParameter("@_HotLine", Hotline);
                pars[2] = new SqlParameter("@_FanpageUrl", FanpageUrl);
                pars[3] = new SqlParameter("@_FanpageSmsUrl", FanpageSmsUrl);
                pars[4] = new SqlParameter("@_ZaloUrl", ZaloUrl);
                pars[5] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_BanCaBoss_CMSConfig_Set_UrlSupportConfig", 4, pars);
                response = Int32.Parse(pars[5].Value.ToString());
            }
            catch (Exception exception)
            {
                response = -9999;
                Task.Run(async () => await _logger.LogError("SQL-SetUrlPostConfig()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return response;
        }
        #endregion
    }
}
