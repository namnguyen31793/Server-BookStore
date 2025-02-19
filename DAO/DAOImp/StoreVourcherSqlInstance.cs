﻿using DAO.Utitlities;
using LoggerService;
using ShareData.DB.Mail;
using ShareData.DB.Vourcher;
using ShareData.ErrorCode;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
namespace DAO.DAOImp
{
    public class StoreVourcherSqlInstance
    {
        private static ILoggerManager _logger;

        private static object _syncObject = new object();

        private static StoreVourcherSqlInstance _inst;

        public static StoreVourcherSqlInstance Inst
        {
            get
            {
                if (_inst == null)
                {
                    lock (_syncObject)
                    {
                        if (_inst == null)
                        {
                            _inst = new StoreVourcherSqlInstance();
                            _logger = new LoggerManager();
                        }
                    }
                }
                return _inst;
            }
        }

        public List<VourcherModelVer2> GetAllVourcher()
        {
            DBHelper db = null;
            var response = new List<VourcherModelVer2>();
            try
            {
                db = new DBHelper(ConfigDb.StoreVourcherConnectionString);
                var pars = new SqlParameter[0];
                response = db.GetListSP<VourcherModelVer2>("SP_StoreVourcher_Config_Ver2_Get_All", 4, pars);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-GetAllVourcher()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return response;
        }

        public int UserGetVourcher_ById(int vourcherId, out int VourcherType, out string VourcherName, out string VourcherReward, out string Target)
        {
            DBHelper db = null;
            var responseStatus = -99;
            VourcherType = 0;
            VourcherName = "";
            VourcherReward = "";
            Target = "";
            try
            {
                db = new DBHelper(ConfigDb.StoreVourcherConnectionString);
                var pars = new SqlParameter[6];
                pars[0] = new SqlParameter("@_VourcherId", vourcherId);
                pars[1] = new SqlParameter("@_VourcherType", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[2] = new SqlParameter("@_VourcherName", SqlDbType.NVarChar, 512) { Direction = ParameterDirection.Output };
                pars[3] = new SqlParameter("@_VourcherReward", SqlDbType.NVarChar, 512) { Direction = ParameterDirection.Output };
                pars[4] = new SqlParameter("@_Target", SqlDbType.NVarChar, 512) { Direction = ParameterDirection.Output };
                pars[5] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_StoreVourcher_Config_Ver2_Get_Reward_By_VourcherId", 4, pars);
                responseStatus = Convert.ToInt32(pars[5].Value);
                if (responseStatus >= 0)
                {
                    VourcherType = Convert.ToInt32(pars[1].Value);
                    VourcherName = pars[2].Value.ToString();
                    VourcherReward = pars[3].Value.ToString();
                    Target = pars[4].Value.ToString();
                }
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-UserGetVourcher_ById()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return responseStatus;
        }
        public List<VourcherModelVer2> UserGetAllVourcher()
        {
            DBHelper db = null;
            var response = new List<VourcherModelVer2>();
            try
            {
                db = new DBHelper(ConfigDb.StoreVourcherConnectionString);
                var pars = new SqlParameter[0];
                response = db.GetListSP<VourcherModelVer2>("SP_StoreVourcher_Config_Ver2_User_Get_All", 4, pars);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-UserGetAllVourcher()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return response;
        }

        public VourcherModelVer2 GetVourcherById(int vourcherId, ref int responseStatus)
        {
            DBHelper db = null;
            var response = new VourcherModelVer2();
            try
            {

                db = new DBHelper(ConfigDb.StoreVourcherConnectionString);
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_VourcherId", vourcherId);
                pars[1] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                response = db.GetInstanceSP<VourcherModelVer2>("SP_StoreVourcher_Config_Ver2_Get_By_VourcherId", 4, pars);
                responseStatus = Convert.ToInt32(pars[1].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-GetVourcherById()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return response;
        }
        public VourcherModelVer2 AddVourcher(string vourcherName, int vourcherType, int countUse, string vourcherReward, string vourcherDescription,
            string thumbnail, string target, DateTime start, DateTime end, bool status, out int responseStatus)
        {
            DBHelper db = null;
            VourcherModelVer2 mCurrentMail = null;
            responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreVourcherConnectionString);
                var pars = new SqlParameter[11];
                pars[0] = new SqlParameter("@_VourcherName", vourcherName);
                pars[1] = new SqlParameter("@_VourcherType", vourcherType);
                pars[2] = new SqlParameter("@_CountUse", countUse);
                pars[3] = new SqlParameter("@_VourcherReward", vourcherReward);
                pars[4] = new SqlParameter("@_VourcherDescription", vourcherDescription);
                pars[5] = new SqlParameter("@_thumbnail", thumbnail);
                pars[6] = new SqlParameter("@_Targets", target);
                pars[7] = new SqlParameter("@_StartTime", start);
                pars[8] = new SqlParameter("@_EndTime", end);
                pars[9] = new SqlParameter("@_Status", status);
                pars[10] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                mCurrentMail = db.GetInstanceSP<VourcherModelVer2>("SP_StoreVourcher_Config_Ver2_Add", 4, pars);
                responseStatus = Convert.ToInt32(pars[10].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-AddVourcher()", exception.ToString()).ConfigureAwait(false));
                return mCurrentMail;
            }
            finally
            {
                db?.Close();
            }
            return mCurrentMail;
        }

        public VourcherModelVer2 UpdateVourcher(int vourcherId, string vourcherName, int vourcherType, int countUse, string vourcherReward, string vourcherDescription,
            string thumbnail, string target, DateTime start, DateTime end, bool status, out int responseStatus)
        {
            DBHelper db = null;
            var response = new VourcherModelVer2();
            responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreVourcherConnectionString);
                var pars = new SqlParameter[12];
                pars[0] = new SqlParameter("@_VourcherId", vourcherId);
                pars[1] = new SqlParameter("@_VourcherName", vourcherName);
                pars[2] = new SqlParameter("@_VourcherType", vourcherType);
                pars[3] = new SqlParameter("@_CountUse", countUse);
                pars[4] = new SqlParameter("@_VourcherReward", vourcherReward);
                pars[5] = new SqlParameter("@_VourcherDescription", vourcherDescription);
                pars[6] = new SqlParameter("@_thumbnail", thumbnail);
                pars[7] = new SqlParameter("@_Targets", target);
                pars[8] = new SqlParameter("@_StartTime", start);
                pars[9] = new SqlParameter("@_EndTime", end);
                pars[10] = new SqlParameter("@_Status", status);
                pars[11] = new SqlParameter("@_ResponseStatus", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                response = db.GetInstanceSP<VourcherModelVer2>("SP_StoreVourcher_Config_Ver2_Update", 4, pars);
                responseStatus = Convert.ToInt32(pars[11].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-UpdateVourcher()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return response;
        }
        public List<VourcherCountUse> GetTopVourcher(DateTime start, DateTime end)
        {
            DBHelper db = null;
            var response = new List<VourcherCountUse>();
            try
            {
                db = new DBHelper(ConfigDb.StoreVourcherConnectionString);
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@StartTime", start);
                pars[1] = new SqlParameter("@EndTime", end);
                response = db.GetListSP<VourcherCountUse>("SP_StoreVourcher_Ver2_Log_GetTop_Use_ByTime", 4, pars);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-GetTopVourcher()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return response;
        }
    }
}
