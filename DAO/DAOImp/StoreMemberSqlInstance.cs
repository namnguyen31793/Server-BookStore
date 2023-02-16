using DAO.Utitlities;
using LoggerService;
using ShareData.DB.Member;
using ShareData.ErrorCode;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DAO.DAOImp
{
    public class StoreMemberSqlInstance
    {
        private static ILoggerManager _logger;

        private static object _syncObject = new object();

        private static StoreMemberSqlInstance _inst;

        public static StoreMemberSqlInstance Inst
        {
            get
            {
                if (_inst == null)
                {
                    lock (_syncObject)
                    {
                        if (_inst == null)
                        {
                            _inst = new StoreMemberSqlInstance();
                            _logger = new LoggerManager();
                        }
                    }
                }
                return _inst;
            }
        }

        public int IncPointAccountByBook(long accountId, long Point, out long CurrentPoint, out long CurrentVip, out bool isNextLevel, out string vipName, out string rewardLevelUp)
        {
            DBHelper db = null;
            var reponseStatus = EStatusCode.DATABASE_ERROR;
            CurrentPoint = 0;
            CurrentVip = 1;
            isNextLevel = false;
            rewardLevelUp = "";
            vipName = "";
            try
            {
                db = new DBHelper(ConfigDb.StoreMemberConnectionString);
                var pars = new SqlParameter[9];
                pars[0] = new SqlParameter("@_AccountId", accountId);
                pars[1] = new SqlParameter("@_Point", Point);
                pars[2] = new SqlParameter("@_CurrentPoint", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[3] = new SqlParameter("@_CurrentVip", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[4] = new SqlParameter("@_IsNextLevel", SqlDbType.Bit) { Direction = ParameterDirection.Output };
                pars[5] = new SqlParameter("@_IsFirstTime", SqlDbType.Bit) { Direction = ParameterDirection.Output };
                pars[6] = new SqlParameter("@_Reward", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Output };
                pars[7] = new SqlParameter("@_VipName", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Output };
                pars[8] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_StoreMember_Account_Inc_Point", 4, pars);
                reponseStatus = Convert.ToInt32(pars[8].Value);
                CurrentPoint = Convert.ToInt64(pars[2].Value);
                CurrentVip = Convert.ToInt64(pars[3].Value);
                isNextLevel = Convert.ToBoolean(pars[4].Value);
                rewardLevelUp = pars[6].Value.ToString();
                vipName = pars[7].Value.ToString(); 
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-IncPointAccountByBook()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return reponseStatus;
        }

        public void GetCurrentPointAccount(long accountId, out long CurrentPoint, out long CurrentVip, out string vipName, out int reponseStatus)
        {
            DBHelper db = null;
            CurrentPoint = 0;
            CurrentVip = 1;
            vipName = "";
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreMemberConnectionString);
                var pars = new SqlParameter[5];
                pars[0] = new SqlParameter("@_AccountId", accountId);
                pars[1] = new SqlParameter("@_CurrentPoint", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[2] = new SqlParameter("@_CurrentVip", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[3] = new SqlParameter("@_VipName", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Output };
                pars[4] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_StoreMember_Account_Get_Current_Point", 4, pars);
                reponseStatus = Convert.ToInt32(pars[4].Value);
                CurrentPoint = Convert.ToInt64(pars[1].Value);
                CurrentVip = Convert.ToInt64(pars[2].Value);
                vipName = pars[3].Value.ToString();
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-GetCurrentPointAccount()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
        }
        public List<VipInfoModel> GetVipInfoModel(out int reponseStatus)
        {
            DBHelper db = null;
            List<VipInfoModel> listConfig = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreMemberConnectionString);
                var pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                listConfig = db.GetListSP<VipInfoModel>("SP_StoreMember_Vip_Point_Config_Get", 4, pars);
                reponseStatus = Convert.ToInt32(pars[0].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-GetVipInfoModel()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return listConfig;
        }
    }
}
