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
    public class EmailDAO
    {
        private static ILoggerManager _logger;
        private static object _syncObject = new object();

        private static EmailDAO _inst;

        public static EmailDAO Inst
        {
            get
            {
                if (_inst == null)
                {
                    lock (_syncObject)
                    {
                        if (_inst == null)
                        {
                            _inst = new EmailDAO();
                            _logger = new LoggerManager();
                        }
                    }
                }
                return _inst;
            }
        }

        public EmailModel SendEmailToUser(int accountId, string mailHeader, string mailContent)
        {
            EmailModel response;
            DBHelper db = null;
            try
            {
                db = new DBHelper(ConfigDb.EmailConnectionString);
                var pars = new SqlParameter[6];
                pars[0] = new SqlParameter("@_AccountId", accountId);
                pars[1] = new SqlParameter("@_SenderNickname", "Admin");
                pars[2] = new SqlParameter("@_MailHeader", mailHeader);
                pars[3] = new SqlParameter("@_MailContent", mailContent);
                pars[4] = new SqlParameter("@_MainMoney", 0);
                pars[5] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                response = db.GetInstanceSP<EmailModel>("SP_BanCaBoss_Mail_AddMail", 4, pars);
            }
            catch (Exception exception)
            {
                response = new EmailModel();
                Task.Run(async () => await _logger.LogError("SQL-SendEmailToUser()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return response;
        }

        public int SendEmailAddGold(int accountId, string mailHeader, string mailContent, long money)
        {
            int responseStatus = -9999;
            DBHelper db = null;
            try
            {
                db = new DBHelper(ConfigDb.EmailConnectionString);
                var pars = new SqlParameter[6];
                pars[0] = new SqlParameter("@_AccountId", accountId);
                pars[1] = new SqlParameter("@_SenderNickname", "Admin");
                pars[2] = new SqlParameter("@_MailHeader", mailHeader);
                pars[3] = new SqlParameter("@_MailContent", mailContent);
                pars[4] = new SqlParameter("@_MainMoney", money);
                pars[5] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var response = db.GetInstanceSP<EmailModel>("SP_BanCaBoss_Mail_AddMail", 4, pars);
                responseStatus = Int32.Parse(pars[5].Value.ToString());
            }
            catch (Exception exception)
            {
                responseStatus = -9999;
                Task.Run(async () => await _logger.LogError("SQL-SendEmailAddGold()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return responseStatus;
        }
    }
}
