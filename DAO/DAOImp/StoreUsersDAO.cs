﻿using DAO.Utitlities;
using LoggerService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace DAO.DAOImp
{
    public class StoreUsersDAO
    {
        private static ILoggerManager _logger;

        private static object _syncObject = new object();

        private static StoreUsersDAO _inst;

        public static StoreUsersDAO Inst
        {
            get
            {
                if (_inst == null)
                {
                    lock (_syncObject)
                    {
                        if (_inst == null)
                        {
                            _inst = new StoreUsersDAO();
                            _logger = new LoggerManager();
                        }
                    }
                }
                return _inst;
            }
        }

        public long DoLogin(string accountName, string passwordMd5, int merchantId, string remoteIp, int ostype, ref int responseStatus)
        {
            DBHelper db = null;
            long accountId = -1;
            try
            {
                db = new DBHelper(ConfigDb.StoreUsersConnectionString);
                var pars = new SqlParameter[7];
                pars[0] = new SqlParameter("@_UserName", accountName);
                pars[1] = new SqlParameter("@_PasswordMD5", passwordMd5);
                pars[2] = new SqlParameter("@_RemoteIP", remoteIp);
                pars[3] = new SqlParameter("@_PlatformId", ostype);
                pars[4] = new SqlParameter("@_SourceId", merchantId);
                pars[5] = new SqlParameter("@_AccountId", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[6] = new SqlParameter("@_ResponseStatus", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_Store_Users_DoLogin", 4, pars);
                responseStatus = Convert.ToInt32(pars[6].Value.ToString());
                accountId = Convert.ToInt64(pars[5].Value.ToString());
            }
            catch (Exception exception)
            {
                responseStatus = -9999;
                Task.Run(async () => await _logger.LogError("SQL-DoLogin()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return accountId;
        }

        public int Register(int registerType, string accountName, string nickName, string passwordMd5, int merchantId, string remoteIp, string deviceId, int platfromId, string phoneNumber, string email, out int accountId)
        {
            if (string.IsNullOrEmpty(deviceId)) deviceId = "Webgl";
            DBHelper db = null;
            var response = -9999;
            accountId = 0;
            try
            {
                db = new DBHelper(ConfigDb.StoreUsersConnectionString);
                var pars = new SqlParameter[12];
                pars[0] = new SqlParameter("@_RegisterType", registerType);
                pars[1] = new SqlParameter("@_UserName", accountName);
                pars[2] = new SqlParameter("@_NickName", nickName);
                pars[3] = new SqlParameter("@_PasswordMD5", passwordMd5);
                pars[4] = new SqlParameter("@_MerchantId", merchantId);
                pars[5] = new SqlParameter("@_RemoteIp", remoteIp);
                pars[6] = new SqlParameter("@_DeviceId", deviceId);
                pars[7] = new SqlParameter("@_PlatformId", platfromId);
                pars[8] = new SqlParameter("@_PhoneNumber", phoneNumber);
                pars[9] = new SqlParameter("@_Email", email);
                pars[10] = new SqlParameter("@_AccountId", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[11] = new SqlParameter("@_ResponseStatus", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_RegisterAccount", 4, pars);
                response = Convert.ToInt32(pars[11].Value);
                accountId = Convert.ToInt32(pars[10].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-Register()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return response;
        }

        public int AddToken(long AccountId, string ResfreshToken)
        {
            DBHelper db = null;
            var response = -9999;
            try
            {
                db = new DBHelper(ConfigDb.StoreUsersConnectionString);
                var pars = new SqlParameter[12];
                pars[0] = new SqlParameter("@_AccountId", AccountId);
                pars[1] = new SqlParameter("@_ResfreshToken", ResfreshToken);
                pars[2] = new SqlParameter("@_ClientID", "BOOKSTORE");
                pars[3] = new SqlParameter("@_ProtectedTicket", "");
                pars[4] = new SqlParameter("@_ResponseStatus", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_Store_Users_AddRefreshToken", 4, pars);
                response = Convert.ToInt32(pars[4].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-AddToken()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return response;
        }
        public int CheckRefreshToken(string ResfreshToken)
        {
            DBHelper db = null;
            var response = -9999;
            try
            {
                db = new DBHelper(ConfigDb.StoreUsersConnectionString);
                var pars = new SqlParameter[12];
                pars[0] = new SqlParameter("@_ResfreshToken", ResfreshToken);
                pars[1] = new SqlParameter("@_ResponseStatus", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_Store_Users_CheckRefreshToken", 4, pars);
                response = Convert.ToInt32(pars[1].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-ResfreshToken()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return response;
        }
    }
}
