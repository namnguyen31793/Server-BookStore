using DAO.Utitlities;
using ShareData.LogSystem;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO.SqlManager
{
    public class Sql_AccountSystem_Manager
    {
        public static Sql_AccountSystem_Manager Inst
        {
            get
            {
                return lazy.Value;
            }
        }
        private static readonly Lazy<Sql_AccountSystem_Manager> lazy = new Lazy<Sql_AccountSystem_Manager>(() => new Sql_AccountSystem_Manager());

        private Sql_AccountSystem_Manager() { }



        public void SP_AccountSystem_Register(string @_Username, string @_PasswordMd5, string @_RemoteIP, int @_RegisterType, int @_PlayerType, out string @_AccountID, out int @_ResponseStatus)
        {
            DBHelper db = null;
            @_AccountID = "";
            @_ResponseStatus = -8888;
            try
            {
                db = new DBHelper(DBConnectionInfo.Sql_AccountSystem_ConnectionString);
                var pars = new SqlParameter[7];
                pars[0] = new SqlParameter("@_Username", @_Username);
                pars[1] = new SqlParameter("@_PasswordMd5", @_PasswordMd5);
                pars[2] = new SqlParameter("@_RemoteIP", @_RemoteIP);
                pars[3] = new SqlParameter("@_RegisterType", @_RegisterType);
                pars[4] = new SqlParameter("@_PlayerType", @_PlayerType);
                pars[5] = new SqlParameter("@_AccountID", SqlDbType.VarChar, 10) { Direction = ParameterDirection.Output };
                pars[6] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };


                db.ExecuteNonQuerySP("SP_AccountSystem_Register", 4, pars);
                @_ResponseStatus = Convert.ToInt32(pars[6].Value);
                if (@_ResponseStatus >= 0)
                {
                    @_AccountID = pars[5].Value.ToString();
                }
            }
            catch (Exception exception)
            {
            }
            finally
            {
                db?.Close();
            }
        }

        public void SP_AccountSystem_Login(string @_Username, string @_PasswordMd5, out string @_AccountID, out int @_ResponseStatus)
        {
            DBHelper db = null;
            @_AccountID = "";
            @_ResponseStatus = -8888;
            try
            {
                db = new DBHelper(DBConnectionInfo.Sql_AccountSystem_ConnectionString);
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@_Username", @_Username);
                pars[1] = new SqlParameter("@_PasswordMd5", @_PasswordMd5);
                pars[2] = new SqlParameter("@_AccountID", SqlDbType.VarChar, 10) { Direction = ParameterDirection.Output };
                pars[3] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_AccountSystem_Login", 4, pars);
                @_ResponseStatus = Convert.ToInt32(pars[3].Value);
                if (@_ResponseStatus >= 0)
                {
                    @_AccountID = pars[2].Value.ToString();
                }
            }
            catch (Exception exception)
            {
            }
            finally
            {
                db?.Close();
            }
        }

        public void SP_AccountSystem_LastLogin_InsertOrUpdate(string @_AccountID, out int @_ResponseStatus)
        {
            DBHelper db = null;

            @_ResponseStatus = -8888;
            try
            {
                db = new DBHelper(DBConnectionInfo.Sql_AccountSystem_ConnectionString);
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_AccountID", @_AccountID);
                pars[1] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_AccountSystem_LastLogin_InsertOrUpdate", 4, pars);
                @_ResponseStatus = Convert.ToInt32(pars[1].Value);

            }
            catch (Exception exception)
            {
            }
            finally
            {
                db?.Close();
            }
        }

        public void SP_AccountSystem_DisplayName_Get(string @_AccountID, out string @_DisplayName, out int @_ResponseStatus)
        {
            @_DisplayName = "";
            @_ResponseStatus = -8888;
            DBHelper db = null;

            @_ResponseStatus = -8888;
            try
            {
                db = new DBHelper(DBConnectionInfo.Sql_AccountSystem_ConnectionString);
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_AccountID", @_AccountID);
                pars[1] = new SqlParameter("@_DisplayName", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Output };
                pars[2] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_AccountSystem_DisplayName_Get", 4, pars);
                @_ResponseStatus = Convert.ToInt32(pars[2].Value);
                if (@_ResponseStatus >= 0)
                    @_DisplayName = pars[1].Value.ToString();

            }
            catch (Exception exception)
            {
            }
            finally
            {
                db?.Close();
            }
        }

        public void SP_AccountSystem_DisplayName_Set(string @_AccountID, string @_DisplayName, out int @_ResponseStatus)
        {

            @_ResponseStatus = -8888;
            DBHelper db = null;

            @_ResponseStatus = -8888;
            try
            {
                db = new DBHelper(DBConnectionInfo.Sql_AccountSystem_ConnectionString);
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_AccountID", @_AccountID);
                pars[0] = new SqlParameter("@_DisplayName", @_DisplayName);
                pars[2] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_AccountSystem_DisplayName_Set", 4, pars);
                @_ResponseStatus = Convert.ToInt32(pars[2].Value);


            }
            catch (Exception exception)
            {
            }
            finally
            {
                db?.Close();
            }
        }

        public void SP_AccountSystem_AccountBalance_Get(string @_AccountID, out long @_AccountBalance, out int @_ResponseStatus)
        {
            @_AccountBalance = -8888;
            @_ResponseStatus = -8888;
            DBHelper db = null;

            @_ResponseStatus = -8888;
            try
            {
                db = new DBHelper(DBConnectionInfo.Sql_AccountSystem_ConnectionString);
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_AccountID", @_AccountID);
                pars[1] = new SqlParameter("@_AccountBalance", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[2] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_AccountSystem_AccountBalance_Get", 4, pars);
                @_ResponseStatus = Convert.ToInt32(pars[2].Value);
                if (@_ResponseStatus >= 0)
                    @_AccountBalance = Int64.Parse(pars[1].Value.ToString());

            }
            catch (Exception exception)
            {
            }
            finally
            {
                db?.Close();
            }
        }
    }
}
