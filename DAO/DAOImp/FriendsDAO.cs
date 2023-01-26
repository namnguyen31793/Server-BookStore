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
    public class FriendsDAO
    {
        private static ILoggerManager _logger;
        private static object _syncObject = new object();

        private static FriendsDAO _inst;

        public static FriendsDAO Inst
        {
            get
            {
                if (_inst == null)
                {
                    lock (_syncObject)
                    {
                        if (_inst == null)
                        {
                            _inst = new FriendsDAO();
                            _logger = new LoggerManager();
                        }
                    }
                }
                return _inst;
            }
        }

        public int InviteFriend(string accountId, string invitedAccountId)
        {
            int response = 0;
            DBHelper db = null;
            try
            {

                db = new DBHelper(ConfigDb.SystemFriendConnectionString);
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_AccountId", accountId);
                pars[1] = new SqlParameter("@_InvitedAccountId", invitedAccountId);
                pars[2] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_FriendSystem_Contact_Invite", 4, pars);
                int.TryParse(pars[2].Value.ToString(), out response);
            }
            catch (Exception exception)
            {
                response = -99;
                Task.Run(async () => await _logger.LogError("SQL-InviteFriend()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return response;
        }

        public List<FriendInviteModel> GetListInviteByAccountId(string accountId)
        {
            DBHelper db = null;
            List<FriendInviteModel> response;
            try
            {
                db = new DBHelper(ConfigDb.SystemFriendConnectionString);
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_AccountId", accountId);
                pars[1] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                response = db.GetListSP<FriendInviteModel>("SP_FriendSystem_Contact_Get_FriendInvited", 4, pars);
            }
            catch (Exception exception)
            {
                response = new List<FriendInviteModel>();
                Task.Run(async () => await _logger.LogError("SQL-GetListInviteByAccountId()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return response;
        }

        public List<FriendInviteModel> GetListFriendByAccountId(string accountId)
        {
            DBHelper db = null;
            List<FriendInviteModel> response;
            try
            {
                db = new DBHelper(ConfigDb.SystemFriendConnectionString);
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_AccountId", accountId);
                pars[1] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                response = db.GetListSP<FriendInviteModel>("SP_FriendSystem_Contact_Get_FriendList", 4, pars);
            }
            catch (Exception exception)
            {
                response = new List<FriendInviteModel>();
                Task.Run(async () => await _logger.LogError("SQL-GetListFriendByAccountId()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return response;
        }

        public int AcceptFriend(string accountId, string sendedFriendId, bool acceptStatus)
        {
            int response = 0;
            DBHelper db = null;
            try
            {
                db = new DBHelper(ConfigDb.SystemFriendConnectionString);
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@_ReceivedFriendId", accountId);
                pars[1] = new SqlParameter("@_SendedFriendId", sendedFriendId);
                pars[2] = new SqlParameter("@_AcceptStatus", acceptStatus);
                pars[3] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_FriendSystem_Contact_AcceptFriend", 4, pars);
                int.TryParse(pars[3].Value.ToString(), out response);
            }
            catch (Exception exception)
            {
                response = -99;
                Task.Run(async () => await _logger.LogError("SQL-AcceptFriend()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return response;
        }

        public int RemovetFriend(string contactId)
        {
            int response = 0;
            DBHelper db = null;
            try
            {
                db = new DBHelper(ConfigDb.SystemFriendConnectionString);
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_ContactId", contactId);
                pars[1] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_FriendSystem_Contract_RemoveFriend", 4, pars);
                int.TryParse(pars[1].Value.ToString(), out response);
            }
            catch (Exception exception)
            {
                response = -99;
                Task.Run(async () => await _logger.LogError("SQL-RemovetFriend()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return response;
        }
    }
}
