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

        #region RATE & COMMENT
        public List<RateCommentObject> GetListRateComment(string barcode, out int reponseStatus)
        {
            DBHelper db = null;
            List<RateCommentObject> modelData = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_Barcode", barcode);
                pars[1] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                modelData = db.GetListSP<RateCommentObject>("SP_Store_Book_Rate_Get_List_By_Barcodes", 4, pars);
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
            return modelData;
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
            RateCommentObject modelData = null;
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
                modelData = db.GetInstanceSP<RateCommentObject>("SP_Store_Book_Set_Account_Rate", 4, pars);
                responseStatus = Convert.ToInt32(pars[6].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-SendComment()", exception.ToString()).ConfigureAwait(false));
                return modelData;
            }
            finally
            {
                db?.Close();
            }
            return modelData;
        }
        #endregion

        #region BOOK
        public List<SimpleBookModel> GetListSimpleBook(int page, int row, out int reponseStatus)
        {
            DBHelper db = null;
            List<SimpleBookModel> modelData = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_Index", page);
                pars[1] = new SqlParameter("@_NUMBER_GET", row);
                pars[2] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                modelData = db.GetListSP<SimpleBookModel>("SP_Store_Book_Get_Info_Book_List_Simple", 4, pars);
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
            return modelData;
        }
        public List<SimpleBookModel> GetListSimpleBookByName(string name, out int reponseStatus)
        {
            DBHelper db = null;
            List<SimpleBookModel> modelData = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_BookName", name);
                pars[1] = new SqlParameter("@_Top", 100);
                pars[2] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                modelData = db.GetListSP<SimpleBookModel>("SP_Store_Book_Get_Info_Book_List_Simple_ByName", 4, pars);
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
            return modelData;
        }
        public List<SimpleBookModel> GetListSimpleBookByTag(string Tag, int page, int row, out int reponseStatus)
        {
            DBHelper db = null;
            List<SimpleBookModel> modelData = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@_BookName", Tag);
                pars[1] = new SqlParameter("@_Index", page);
                pars[2] = new SqlParameter("@_NUMBER_GET", row);
                pars[3] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                modelData = db.GetListSP<SimpleBookModel>("SP_Store_Book_Get_Info_Book_List_Simple_ByTag", 4, pars);
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
            return modelData;
        }
        public SimpleBookModel GetListSimpleBookByBarcode(string barcode, out int reponseStatus)
        {
            DBHelper db = null;
            SimpleBookModel modelData = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_Barcode", barcode);
                pars[1] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                modelData = db.GetInstanceSP<SimpleBookModel>("SP_Store_Book_Get_Info_Book_Simple_By_BarCode", 4, pars);
                reponseStatus = Convert.ToInt32(pars[1].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-GetListSimpleBookByBarcode()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return modelData;
        }

        public DownloadBookModel GetDownloadBookByBarcode(long accountId, string barcode, out int reponseStatus)
        {
            DBHelper db = null;
            DownloadBookModel modelData = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_AccountId", accountId);
                pars[1] = new SqlParameter("@_Barcode", barcode);
                pars[2] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                modelData = db.GetInstanceSP<DownloadBookModel>("SP_Store_Book_Get_Info_Book_Download", 4, pars);
                reponseStatus = Convert.ToInt32(pars[2].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-GetDownloadBookByBarcode()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return modelData;
        }

        public FullDemoBookModel GetFullBookDemoByBarcode(string barcode, out int reponseStatus)
        {
            DBHelper db = null;
            FullDemoBookModel modelData = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_Barcode", barcode);
                pars[1] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                modelData = db.GetInstanceSP<FullDemoBookModel>("SP_Store_Book_Get_Info_Book_Content", 4, pars);
                reponseStatus = Convert.ToInt32(pars[1].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-GetFullBookDemoByBarcode()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return modelData;
        }
        #endregion

        #region AUTHOR

        public AuthorInfoModel GetAuthorById(int authorId, out int reponseStatus)
        {
            DBHelper db = null;
            AuthorInfoModel modelData = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_AuthorId", authorId);
                pars[1] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                modelData = db.GetInstanceSP<AuthorInfoModel>("SP_Store_Book_Get_Info_Author", 4, pars);
                reponseStatus = Convert.ToInt32(pars[1].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-GetAuthorById()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return modelData;
        }
        public AuthorInfoModel AddNewAuthor(AuthorInfoModel model, out int reponseStatus)
        {
            DBHelper db = null;
            AuthorInfoModel data = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@_AuthorName", model.AuthorName);
                pars[1] = new SqlParameter("@_AuthorBirday", model.AuthorBirday);
                pars[2] = new SqlParameter("@_AuthorAdress", model.AuthorAdress);
                pars[3] = new SqlParameter("@_AuthorIntroduction", model.AuthorIntroduction);
                pars[4] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                data = db.GetInstanceSP<AuthorInfoModel>("SP_Store_Book_CMS_Add_Author", 4, pars);
                reponseStatus = Convert.ToInt32(pars[4].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-AddNewAuthor()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return data;
        }
        #endregion

        #region COLOR
        public List<BookColorConfig> GetListColorConfig(out int reponseStatus)
        {
            DBHelper db = null;
            List<BookColorConfig> listConfig = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                listConfig = db.GetListSP<BookColorConfig>("SP_Store_Book_Color_Get", 4, pars);
                reponseStatus = Convert.ToInt32(pars[0].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-GetListColorConfig()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return listConfig;
        }

        public List<BookColorConfig> GetListColorConfigAll(out int reponseStatus)
        {
            DBHelper db = null;
            List<BookColorConfig> listConfig = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                listConfig = db.GetListSP<BookColorConfig>("SP_Store_Book_Color_Get_All", 4, pars);
                reponseStatus = Convert.ToInt32(pars[0].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-GetListColorConfigAll()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return listConfig;
        }

        public int AddColorConfig(BookColorConfig model)
        {
            DBHelper db = null;
            int reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@_Cover", model.Cover);
                pars[1] = new SqlParameter("@_Media", model.Media);
                pars[2] = new SqlParameter("@_Status", model.Status);
                pars[3] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_Store_Book_Color_Add", 4, pars);
                reponseStatus = Convert.ToInt32(pars[3].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-AddColorConfig()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return reponseStatus;
        }
        public int UpdateColorConfig(BookColorConfig model)
        {
            DBHelper db = null;
            int reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[5];
                pars[0] = new SqlParameter("@_ColorId", model.ColorId);
                pars[1] = new SqlParameter("@_Cover", model.Cover);
                pars[2] = new SqlParameter("@_Media", model.Media);
                pars[3] = new SqlParameter("@_Status", model.Status);
                pars[4] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_Store_Book_Color_Update", 4, pars);
                reponseStatus = Convert.ToInt32(pars[4].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-UpdateColorConfig()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return reponseStatus;
        }
        #endregion

        #region TRANSACTION
        public List<BookBuyModel> GetBookBuyAccount(long accountId, out int reponseStatus)
        {
            DBHelper db = null;
            List<BookBuyModel> listConfig = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_AccountId", accountId);
                pars[1] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                listConfig = db.GetListSP<BookBuyModel>("SP_Store_Book_Color_Get", 4, pars);
                reponseStatus = Convert.ToInt32(pars[1].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-GetBookBuyAccount()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return listConfig;
        }
        public int AccountBuyBarcode(long accountId, string barcode)
        {
            DBHelper db = null;
            int reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[5];
                pars[0] = new SqlParameter("@_AccountId", accountId);
                pars[1] = new SqlParameter("@_Barcode", barcode);
                pars[2] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_Store_Book_Set_Account_Buy_Barcode", 4, pars);
                reponseStatus = Convert.ToInt32(pars[2].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-AccountBuyBarcode()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return reponseStatus;
        }
        #endregion
    }
}
