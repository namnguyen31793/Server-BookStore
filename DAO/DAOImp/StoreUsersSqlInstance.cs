using DAO.Utitlities;
using LoggerService;
using ShareData.DB.Users;
using ShareData.ErrorCode;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace DAO.DAOImp
{
    public class StoreUsersSqlInstance
    {
        private static ILoggerManager _logger;

        private static object _syncObject = new object();

        private static StoreUsersSqlInstance _inst;

        public static StoreUsersSqlInstance Inst
        {
            get
            {
                if (_inst == null)
                {
                    lock (_syncObject)
                    {
                        if (_inst == null)
                        {
                            _inst = new StoreUsersSqlInstance();
                            _logger = new LoggerManager();
                        }
                    }
                }
                return _inst;
            }
        }
        #region APP
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
                pars[5] = new SqlParameter("@_AccountId", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
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
            if (string.IsNullOrEmpty(phoneNumber)) phoneNumber = "";
            if (string.IsNullOrEmpty(email)) email = "";
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
                db.ExecuteNonQuerySP("SP_Store_Users_Register", 4, pars);
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
                var pars = new SqlParameter[5];
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
        public int CheckRefreshToken(string ResfreshToken, ref long accountId)
        {
            DBHelper db = null;
            var response = -9999;
            accountId = 0;
            try
            {
                db = new DBHelper(ConfigDb.StoreUsersConnectionString);
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_ResfreshToken", ResfreshToken);
                pars[1] = new SqlParameter("@_AccountId", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[2] = new SqlParameter("@_ResponseStatus", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_Store_Users_CheckRefreshToken", 4, pars);
                response = Convert.ToInt32(pars[2].Value);
                if (response == 0) {
                    accountId = Convert.ToInt64(pars[1].Value);
                }
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
        public AccountModelDb GetAccountInfo(long accountId, ref int responseStatus)
        {
            DBHelper db = null;
            var response = new AccountModelDb();
            try
            {

                db = new DBHelper(ConfigDb.StoreUsersConnectionString);
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_AccountId", accountId);
                pars[1] = new SqlParameter("@_ResponseStatus", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                response = db.GetInstanceSP<AccountModelDb>("SP_Store_Users_Get_UserInfo", 4, pars);
                responseStatus = Convert.ToInt32(pars[1].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-GetAccountInfo()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return response;
        }

        public AccountModelDb UpdateEmail(long AccountId, string email, ref int responseStatus)
        {
            DBHelper db = null;
            var response = new AccountModelDb();
            try
            {
                db = new DBHelper(ConfigDb.StoreUsersConnectionString);
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_AccountId", AccountId);
                pars[1] = new SqlParameter("@_Email", email);
                pars[2] = new SqlParameter("@_ResponseStatus", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_Store_Users_CheckRefreshToken", 4, pars);
                response = db.GetInstanceSP<AccountModelDb>("SP_Store_Users_CheckRefreshToken", 4, pars);
                responseStatus = Convert.ToInt32(pars[2].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-UpdateEmail()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return response;
        }
        public AccountModelDb UpdateInfo(long AccountId, string NickName, string AvataId, string PhoneNumber, string BirthDay, string Adress, ref int responseStatus)
        {
            DBHelper db = null;
            var response = new AccountModelDb();
            if (string.IsNullOrEmpty(NickName)) NickName = "";
            if (string.IsNullOrEmpty(AvataId)) AvataId = "";
            if (string.IsNullOrEmpty(PhoneNumber)) PhoneNumber = "";
            if (string.IsNullOrEmpty(BirthDay)) BirthDay = "";
            if (string.IsNullOrEmpty(Adress)) Adress = "";
            try
            {
                db = new DBHelper(ConfigDb.StoreUsersConnectionString);
                var pars = new SqlParameter[7];
                pars[0] = new SqlParameter("@_AccountId", AccountId);
                pars[1] = new SqlParameter("@_Nickname", NickName);
                pars[2] = new SqlParameter("@_AvatarId", AvataId);
                pars[3] = new SqlParameter("@_PhoneNumber", PhoneNumber);
                pars[4] = new SqlParameter("@_BirthDay", BirthDay);
                pars[5] = new SqlParameter("@_Adress", Adress);
                pars[6] = new SqlParameter("@_ResponseStatus", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                response = db.GetInstanceSP<AccountModelDb>("SP_Store_Users_Update_UserInfo", 4, pars);
                responseStatus = Convert.ToInt32(pars[6].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-UpdateInfo()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return response;
        }

        public int ValidateUser(long AccountId)
        {
            DBHelper db = null;
            var response = -9999;
            try
            {
                db = new DBHelper(ConfigDb.StoreUsersConnectionString);
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_AccountId", AccountId);
                pars[1] = new SqlParameter("@_ResponseStatus", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_Store_Users_Update_Validate", 4, pars);
                response = Convert.ToInt32(pars[1].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-ValidateUser()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return response;
        }
        #endregion

        #region CMS
        public long DoLoginCms(string accountName, string passwordMd5, int merchantId, string remoteIp, int ostype, ref int userRole, ref int responseStatus)
        {
            DBHelper db = null;
            long accountId = -1;
            try
            {
                db = new DBHelper(ConfigDb.StoreUsersConnectionString);
                var pars = new SqlParameter[8];
                pars[0] = new SqlParameter("@_UserName", accountName);
                pars[1] = new SqlParameter("@_PasswordMD5", passwordMd5);
                pars[2] = new SqlParameter("@_RemoteIP", remoteIp);
                pars[3] = new SqlParameter("@_PlatformId", ostype);
                pars[4] = new SqlParameter("@_SourceId", merchantId);
                pars[5] = new SqlParameter("@_AccountId", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[6] = new SqlParameter("@_UserRole", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[7] = new SqlParameter("@_ResponseStatus", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_Store_Users_CMS_DoLogin", 4, pars);
                responseStatus = Convert.ToInt32(pars[7].Value.ToString());
                if (responseStatus == 0)
                {
                    accountId = Convert.ToInt64(pars[5].Value.ToString());
                    userRole = Convert.ToInt32(pars[6].Value.ToString());
                }
            }
            catch (Exception exception)
            {
                responseStatus = -9999;
                Task.Run(async () => await _logger.LogError("SQL-DoLoginCms()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return accountId;
        }
        public int CheckRefreshTokenCms(string ResfreshToken, ref int role, ref long accountId)
        {
            DBHelper db = null;
            var response = -9999;
            accountId = 0;
            try
            {
                db = new DBHelper(ConfigDb.StoreUsersConnectionString);
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@_ResfreshToken", ResfreshToken);
                pars[1] = new SqlParameter("@_RoleUser", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[2] = new SqlParameter("@_AccountId", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[3] = new SqlParameter("@_ResponseStatus", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_Store_Users_CMS_CheckRefreshToken", 4, pars);
                response = Convert.ToInt32(pars[3].Value);
                if (response == 0)
                {
                    role = Convert.ToInt32(pars[1].Value);
                    accountId = Convert.ToInt64(pars[2].Value);
                }
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-CheckRefreshTokenCms()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return response;
        }

        public int UpdateRole(long accountId, int role)
        {
            DBHelper db = null;
            var response = -9999;
            accountId = 0;
            try
            {
                db = new DBHelper(ConfigDb.StoreUsersConnectionString);
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_AccountId", accountId);
                pars[1] = new SqlParameter("@_RoleUser", role);
                pars[2] = new SqlParameter("@_ResponseStatus", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_Store_Users_Update_Role", 4, pars);
                response = Convert.ToInt32(pars[2].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-UpdateRole()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return response;
        }
        #endregion

        #region VOURCHER
        public List<AccountVourcherModel> GetVourcherAccount(long accountId, out int reponseStatus)
        {
            DBHelper db = null;
            List<AccountVourcherModel> listConfig = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_AccountId", accountId);
                pars[1] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                listConfig = db.GetListSP<AccountVourcherModel>("SP_Store_Vourcher_Account_Get", 4, pars);
                reponseStatus = Convert.ToInt32(pars[1].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-GetVourcherAccount()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return listConfig;
        }
        #endregion
    }
}
