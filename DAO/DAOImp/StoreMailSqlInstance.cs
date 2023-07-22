using DAO.Utitlities;
using LoggerService;
using ShareData.DB.Mail;
using ShareData.ErrorCode;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace DAO.DAOImp
{
    public class StoreMailSqlInstance
    {
        private static ILoggerManager _logger;

        private static object _syncObject = new object();

        private static StoreMailSqlInstance _inst;

        public static StoreMailSqlInstance Inst
        {
            get
            {
                if (_inst == null)
                {
                    lock (_syncObject)
                    {
                        if (_inst == null)
                        {
                            _inst = new StoreMailSqlInstance();
                            _logger = new LoggerManager();
                        }
                    }
                }
                return _inst;
            }
        }

        public List<MailObject> DeleteMail(long accountId, long mailId, out int reponseStatus)
        {
            DBHelper db = null;
            List<MailObject> mCurrentMail = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreMailConnectionString);
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_AccountId", accountId);
                pars[1] = new SqlParameter("@_MailId", mailId);
                pars[2] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                mCurrentMail = db.GetListSP<MailObject>("SP_Store_Mail_DeleteMail", 4, pars);
                reponseStatus = Convert.ToInt32(pars[2].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-DeleteMail()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return mCurrentMail;
        }
        public MailObject RestoreMail(long mailId, out int reponseStatus)
        {
            DBHelper db = null;
            MailObject mCurrentMail = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreMailConnectionString);
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_MailId", mailId);
                pars[1] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                mCurrentMail = db.GetInstanceSP<MailObject>("SP_Store_Mail_RestoreMail", 4, pars);
                reponseStatus = Convert.ToInt32(pars[1].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-RestoreMail()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return mCurrentMail;
        }
        public List<MailObject> GetAllMailListByAccountId(long accountId, int page, int row)
        {
            DBHelper db = null;
            List<MailObject> mCurrentMail = null;
            int responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreMailConnectionString);
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@_AccountId", accountId);
                pars[1] = new SqlParameter("@_Index", page);
                pars[2] = new SqlParameter("@_NUMBER_GET", row);
                pars[3] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                mCurrentMail = db.GetListSP<MailObject>("SP_Store_Mail_GetAllMailByAccountId", 4, pars);
                responseStatus = Convert.ToInt32(pars[3].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-GetAllMailListByAccountId()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return mCurrentMail;
        }

        public List<MailObject> GetMailListByAccountId(long accountId)
        {
            DBHelper db = null;
            List<MailObject> mCurrentMail = null;
            int responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreMailConnectionString);
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_AccountId", accountId);
                pars[1] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                mCurrentMail = db.GetListSP<MailObject>("SP_Store_Mail_GetMailByAccountId", 4, pars);
                responseStatus = Convert.ToInt32(pars[1].Value);
            }
            catch (Exception exception)
            {
                 Task.Run(async () => await _logger.LogError("SQL-GetMailListByAccountId()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return mCurrentMail;
        }


        public void SP_Store_Mail_AcceptMail(long accountId, int mailId, out long _MailMoney, out long CurrentAccountBalance, out string _RewardBonusDescription)
        {
            _MailMoney = -9999;
            CurrentAccountBalance = -9999;
            _RewardBonusDescription = "";
            DBHelper db = null;
            List<MailObject> mCurrentMail = null;
            int responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreMailConnectionString);
                var pars = new SqlParameter[6];
                pars[0] = new SqlParameter("@_AccountId", accountId);
                pars[1] = new SqlParameter("@_MailId", mailId);
                pars[2] = new SqlParameter("@_MailMoney", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[3] = new SqlParameter("@_AccountBalance", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[4] = new SqlParameter("@_RewardBonusDescription", SqlDbType.VarChar, -1) { Direction = ParameterDirection.Output };
                pars[5] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                mCurrentMail = db.GetListSP<MailObject>("SP_Store_Mail_AcceptMail", 4, pars);
                responseStatus = Convert.ToInt32(pars[5].Value);
                if (responseStatus >= 0)
                {
                    _MailMoney = Convert.ToInt64(pars[2].Value);
                    CurrentAccountBalance = Convert.ToInt64(pars[3].Value);
                    _RewardBonusDescription = pars[4].Value.ToString();
                }
            }
            catch (Exception exception)
            {
                 Task.Run(async () => await _logger.LogError("SQL-SP_BanCaBoss_Mail_AcceptMail()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }

        }

        public MailObject SendMail(long accountId, string senderNickname, string mailHeader, string mailContent, out int responseStatus, long _MainMoney = 0, string _RewardBonusDescription = "")
        {
            DBHelper db = null;
            MailObject mCurrentMail = null;
            responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreMailConnectionString);
                var pars = new SqlParameter[7];
                pars[0] = new SqlParameter("@_AccountId", accountId);
                pars[1] = new SqlParameter("@_SenderNickname", senderNickname);
                pars[2] = new SqlParameter("@_MailHeader", mailHeader);
                pars[3] = new SqlParameter("@_MailContent", mailContent);
                pars[4] = new SqlParameter("@_MainMoney", _MainMoney);
                pars[5] = new SqlParameter("@_RewardBonusDescription", _RewardBonusDescription);
                pars[6] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                mCurrentMail = db.GetInstanceSP<MailObject>("SP_Store_Mail_AddMail", 4, pars);
                responseStatus = Convert.ToInt32(pars[6].Value);
            }
            catch (Exception exception)
            {
                 Task.Run(async () => await _logger.LogError("SQL-SendMail()", exception.ToString()).ConfigureAwait(false));
                return mCurrentMail;
            }
            finally
            {
                db?.Close();
            }
            return mCurrentMail;
        }

        public void SendAllMail(string senderNickname, string mailHeader, string mailContent, out int responseStatus, long _MainMoney = 0, string _RewardBonusDescription = "")
        {
            DBHelper db = null;
            responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreMailConnectionString);
                var pars = new SqlParameter[6];
                pars[0] = new SqlParameter("@_SenderNickname", senderNickname);
                pars[1] = new SqlParameter("@_MailHeader", mailHeader);
                pars[2] = new SqlParameter("@_MailContent", mailContent);
                pars[3] = new SqlParameter("@_MainMoney", _MainMoney);
                pars[4] = new SqlParameter("@_RewardBonusDescription", _RewardBonusDescription);
                pars[5] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_Store_Mail_All_AddMail", 4, pars);
                responseStatus = Convert.ToInt32(pars[5].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-SendMail()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
        }

        public void UpdateReadedMail(long mailId, out int responseStatus)
        {
            DBHelper db = null;

            responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreMailConnectionString);
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_MailId", mailId);
                pars[1] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_Store_Mail_UpdateReadMail", 4, pars);
                responseStatus = Convert.ToInt32(pars[1].Value);
            }
            catch (Exception exception)
            {
                 Task.Run(async () => await _logger.LogError("SQL-UpdateReadedMail()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
        }
        public void UpdateReadedAllMail(long AccountId, out int responseStatus)
        {
            DBHelper db = null;
            responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreMailConnectionString);
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_AccountId", AccountId);
                pars[1] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_Store_Mail_UpdateReadAllMail", 4, pars);
                responseStatus = Convert.ToInt32(pars[1].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-UpdateReadedAllMail()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
        }

        #region EMAIL
        public List<EmailObject> GetListEmailAdmin()
        {
            DBHelper db = null;
            List<EmailObject> mCurrentMail = null;
            int responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreMailConnectionString);
                var pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                mCurrentMail = db.GetListSP<EmailObject>("SP_Store_List_Email_GetListEmail", 4, pars);
                responseStatus = Convert.ToInt32(pars[0].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-GetListEmailAdmin()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return mCurrentMail;
        }

        public EmailObject AddEmailAdmin(string Username, string Password, bool Status, out int responseStatus)
        {
            DBHelper db = null;
            EmailObject mCurrentMail = null;
            responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreMailConnectionString);
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@_Username", Username);
                pars[1] = new SqlParameter("@_Password", Password);
                pars[2] = new SqlParameter("@_Status", Status);
                pars[3] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                mCurrentMail = db.GetInstanceSP<EmailObject>("SP_Store_List_Email_AddEmail", 4, pars);
                responseStatus = Convert.ToInt32(pars[3].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-AddEmailAdmin()", exception.ToString()).ConfigureAwait(false));
                return mCurrentMail;
            }
            finally
            {
                db?.Close();
            }
            return mCurrentMail;
        }

        public List<EmailObject> UpdateEmailAdmin(long EmailId, string Username, string Password, bool Status, out int responseStatus)
        {
            DBHelper db = null;
            List<EmailObject> mCurrentMail = new List<EmailObject>();
            responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreMailConnectionString);
                var pars = new SqlParameter[5];
                pars[0] = new SqlParameter("@_Username", EmailId);
                pars[1] = new SqlParameter("@_Username", Username);
                pars[2] = new SqlParameter("@_Password", Password);
                pars[3] = new SqlParameter("@_Status", Status);
                pars[4] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                mCurrentMail = db.GetListSP<EmailObject>("SP_Store_List_Email_UpdateEmail", 4, pars);
                responseStatus = Convert.ToInt32(pars[4].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-UpdateEmailAdmin()", exception.ToString()).ConfigureAwait(false));
                return mCurrentMail;
            }
            finally
            {
                db?.Close();
            }
            return mCurrentMail;
        }

        public int GetEmailSaveDatabase(out string Username, out string Password) {
            int responseStatus = EStatusCode.DATABASE_ERROR;
            Username = "";
            Password = "";
            DBHelper db = null;
            try
            {
                db = new DBHelper(ConfigDb.StoreMailConnectionString);
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_Username", SqlDbType.NVarChar, 512) { Direction = ParameterDirection.Output };
                pars[1] = new SqlParameter("@_Password", SqlDbType.NVarChar, 512) { Direction = ParameterDirection.Output };
                pars[2] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_Store_List_Email_GetEmail", 4, pars);
                responseStatus = Convert.ToInt32(pars[2].Value);
                if (responseStatus >= 0)
                {
                    Username = pars[0].Value.ToString();
                    Password = pars[1].Value.ToString();
                }
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-GetEmailSaveDatabase()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return responseStatus;
        }
        #endregion
    }
}
