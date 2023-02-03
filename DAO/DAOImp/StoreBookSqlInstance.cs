using DAO.Utitlities;
using LoggerService;
using ShareData.DB.Books;
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
    public class StoreBookSqlInstance
    {
        private static ILoggerManager _logger;

        private static object _syncObject = new object();

        private static StoreBookSqlInstance _inst;

        public static StoreBookSqlInstance Inst
        {
            get
            {
                if (_inst == null)
                {
                    lock (_syncObject)
                    {
                        if (_inst == null)
                        {
                            _inst = new StoreBookSqlInstance();
                            _logger = new LoggerManager();
                        }
                    }
                }
                return _inst;
            }
        }

        public List<RateCommentObject> GetListRateComment(string barcode, out int reponseStatus)
        {
            DBHelper db = null;
            List<RateCommentObject> mCurrentMail = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_Barcode", barcode);
                pars[1] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                mCurrentMail = db.GetListSP<RateCommentObject>("SP_Store_Book_Rate_Get_List_By_Barcodes", 4, pars);
                reponseStatus = Convert.ToInt32(pars[1].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-GetListRateComment()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return mCurrentMail;
        }

        public int GetAvgRate(string barcode, out float starRate)
        {
            DBHelper db = null;
            starRate = 0;
            int reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_Barcode", barcode);
                pars[1] = new SqlParameter("@_StarRate", SqlDbType.Float) { Direction = ParameterDirection.Output };
                pars[2] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_Store_Book_CMS_Barcodes_Rate_Star", 4, pars);
                reponseStatus = Convert.ToInt32(pars[2].Value);
                if (reponseStatus == 0) {
                    float.TryParse(pars[2].Value.ToString(), out starRate);
                }
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-GetListRateComment()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return reponseStatus;

        }

        public RateCommentObject SendComment(long accountId, string Barcode, int Rate, string Comment, string ActionTime, string NickName, out int responseStatus)
        {
            DBHelper db = null;
            RateCommentObject mCurrentMail = null;
            responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[7];
                pars[0] = new SqlParameter("@_AccountId", accountId);
                pars[1] = new SqlParameter("@_Barcode", Barcode);
                pars[2] = new SqlParameter("@_Rate", Rate);
                pars[3] = new SqlParameter("@_Comment", Comment);
                pars[4] = new SqlParameter("@_ActionTime", ActionTime);
                pars[5] = new SqlParameter("@_NickName", NickName);
                pars[6] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                mCurrentMail = db.GetInstanceSP<RateCommentObject>("SP_Store_Book_Set_Account_Rate", 4, pars);
                responseStatus = Convert.ToInt32(pars[6].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-SendComment()", exception.ToString()).ConfigureAwait(false));
                return mCurrentMail;
            }
            finally
            {
                db?.Close();
            }
            return mCurrentMail;
        }

        public List<SimpleBookModel> GetListSimpleBook(int page, int row, out int reponseStatus)
        {
            DBHelper db = null;
            List<SimpleBookModel> mCurrentMail = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_Index", page);
                pars[1] = new SqlParameter("@_NUMBER_GET", row);
                pars[2] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                mCurrentMail = db.GetListSP<SimpleBookModel>("SP_Store_Book_Get_Info_Book_List_Simple", 4, pars);
                reponseStatus = Convert.ToInt32(pars[2].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-GetListSimpleBook()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return mCurrentMail;
        }
        public List<SimpleBookModel> GetListSimpleBookByName(string name, out int reponseStatus)
        {
            DBHelper db = null;
            List<SimpleBookModel> mCurrentMail = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_BookName", name);
                pars[1] = new SqlParameter("@_Top", 100);
                pars[2] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                mCurrentMail = db.GetListSP<SimpleBookModel>("SP_Store_Book_Get_Info_Book_List_Simple_ByName", 4, pars);
                reponseStatus = Convert.ToInt32(pars[2].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-GetListSimpleBook()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return mCurrentMail;
        }
        public List<SimpleBookModel> GetListSimpleBookByTag(string Tag, int page, int row, out int reponseStatus)
        {
            DBHelper db = null;
            List<SimpleBookModel> mCurrentMail = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@_BookName", Tag);
                pars[1] = new SqlParameter("@_Index", page);
                pars[2] = new SqlParameter("@_NUMBER_GET", row);
                pars[3] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                mCurrentMail = db.GetListSP<SimpleBookModel>("SP_Store_Book_Get_Info_Book_List_Simple_ByTag", 4, pars);
                reponseStatus = Convert.ToInt32(pars[3].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-GetListSimpleBookByTag()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return mCurrentMail;
        }
    }
}
