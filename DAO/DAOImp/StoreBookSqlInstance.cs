using DAO.Utitlities;
using LoggerService;
using ShareData.DB.Books;
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

        public RateCommentObject GetRateComment(long AccountId, string barcode, out int reponseStatus)
        {
            DBHelper db = null;
            RateCommentObject modelData = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_AccountId", AccountId);
                pars[1] = new SqlParameter("@_Barcode", barcode);
                pars[2] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                modelData = db.GetInstanceSP<RateCommentObject>("SP_Store_Book_Rate_Get_Account_By_Barcodes", 4, pars);
                reponseStatus = Convert.ToInt32(pars[2].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-GetRateComment()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return modelData;
        }

        public int GetAvgRate(string barcode, out RateCountModel starRate)
        {
            DBHelper db = null;
            starRate = new RateCountModel() { StarRate = 0, StarRate1 = 0, StarRate2 = 0, StarRate3 = 0, StarRate4 = 0, StarRate5 = 0};
            int reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[8];
                pars[0] = new SqlParameter("@_Barcode", barcode);
                pars[1] = new SqlParameter("@_StarRate", SqlDbType.Float) { Direction = ParameterDirection.Output };
                pars[2] = new SqlParameter("@_StarRate1", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[3] = new SqlParameter("@_StarRate2", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[4] = new SqlParameter("@_StarRate3", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[5] = new SqlParameter("@_StarRate4", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[6] = new SqlParameter("@_StarRate5", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[7] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_Store_Book_Rate_Barcodes_Star", 4, pars);
                reponseStatus = Convert.ToInt32(pars[7].Value);
                if (reponseStatus == 0) {
                    float countAvg = 0;
                    float.TryParse(pars[1].Value.ToString(), out countAvg);
                    starRate.StarRate = countAvg;
                    int count = 0;
                    int.TryParse(pars[2].Value.ToString(), out count);
                    starRate.StarRate1 = count;
                    int.TryParse(pars[3].Value.ToString(), out count);
                    starRate.StarRate2 = count;
                    int.TryParse(pars[4].Value.ToString(), out count);
                    starRate.StarRate3 = count;
                    int.TryParse(pars[5].Value.ToString(), out count);
                    starRate.StarRate4 = count;
                    int.TryParse(pars[6].Value.ToString(), out count);
                    starRate.StarRate5 = count;
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

        public RateCommentObject SendComment(long accountId, string Barcode, int Rate, string Comment, string ActionTime, string NickName, out int responseStatus, out RateCountModel starRate)
        {
            DBHelper db = null;
            starRate = new RateCountModel() { StarRate = 0, StarRate1 = 0, StarRate2 = 0, StarRate3 = 0, StarRate4 = 0, StarRate5 = 0 };
            RateCommentObject modelData = null;
            responseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[13];
                pars[0] = new SqlParameter("@_AccountId", accountId);
                pars[1] = new SqlParameter("@_Barcode", Barcode);
                pars[2] = new SqlParameter("@_Rate", Rate);
                pars[3] = new SqlParameter("@_Comment", Comment);
                pars[4] = new SqlParameter("@_ActionTime", ActionTime);
                pars[5] = new SqlParameter("@_NickName", NickName);
                pars[6] = new SqlParameter("@_StarRate", SqlDbType.Float) { Direction = ParameterDirection.Output };
                pars[7] = new SqlParameter("@_StarRate1", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[8] = new SqlParameter("@_StarRate2", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[9] = new SqlParameter("@_StarRate3", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[10] = new SqlParameter("@_StarRate4", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[11] = new SqlParameter("@_StarRate5", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[12] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                modelData = db.GetInstanceSP<RateCommentObject>("SP_Store_Book_Rate_Account_Set", 4, pars);
                responseStatus = Convert.ToInt32(pars[12].Value);
                if (responseStatus == 0)
                {
                    float countAvg = 0;
                    float.TryParse(pars[6].Value.ToString(), out countAvg);
                    starRate.StarRate = countAvg;
                    int count = 0;
                    int.TryParse(pars[7].Value.ToString(), out count);
                    starRate.StarRate1 = count;
                    int.TryParse(pars[8].Value.ToString(), out count);
                    starRate.StarRate2 = count;
                    int.TryParse(pars[9].Value.ToString(), out count);
                    starRate.StarRate3 = count;
                    int.TryParse(pars[10].Value.ToString(), out count);
                    starRate.StarRate4 = count;
                    int.TryParse(pars[11].Value.ToString(), out count);
                    starRate.StarRate5 = count;
                }
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
        public int RemoveComment(string barcode, long accountId)
        {
            DBHelper db = null;
            int reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_AccountId", accountId);
                pars[1] = new SqlParameter("@_Barcode", barcode);
                pars[2] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_Store_Book_Rate_Remove_By_Barcodes", 4, pars);
                reponseStatus = Convert.ToInt32(pars[2].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-RemoveComment()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return reponseStatus;
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
                pars[0] = new SqlParameter("@_Tag", Tag);
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
        public DownloadBookModel GetDownloadBookByBarcode(string barcode, out int reponseStatus)
        {
            DBHelper db = null;
            DownloadBookModel modelData = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_Barcode", barcode);
                pars[1] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                modelData = db.GetInstanceSP<DownloadBookModel>("SP_Store_Book_Get_Info_Book_Download_Not_Id", 4, pars);
                reponseStatus = Convert.ToInt32(pars[1].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-GetDownloadBookByBarcode2()", exception.ToString()).ConfigureAwait(false));
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
        public AddBookModel AddNewBook(AddBookModel model, out int reponseStatus)
        {
            DBHelper db = null;
            AddBookModel data = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[16];
                pars[0] = new SqlParameter("@_Barcode", model.Barcode);
                pars[1] = new SqlParameter("@_ImageLink", GetDataString(model.ImageLink));
                pars[2] = new SqlParameter("@_BookName", GetDataString(model.BookName));
                pars[3] = new SqlParameter("@_AuthorId", model.AuthorId);
                pars[4] = new SqlParameter("@_AmountBase", model.AmountBase);
                pars[5] = new SqlParameter("@_AmountSale", model.AmountSale);
                pars[6] = new SqlParameter("@_TrialReadLink", GetDataString(model.TrialReadLink));
                pars[7] = new SqlParameter("@_AudioLink", GetDataString(model.AudioLink));
                pars[8] = new SqlParameter("@_KeyLink", GetDataString(model.KeyLink));
                pars[9] = new SqlParameter("@_NumberPage", model.NumberPage);
                pars[10] = new SqlParameter("@_ContentBook", GetDataString(model.ContentBook));
                pars[11] = new SqlParameter("@_Tags", GetDataString(model.Tags));
                pars[12] = new SqlParameter("@_RelatedBooks", GetDataString(model.RelatedBooks));
                pars[13] = new SqlParameter("@_ColorId", model.ColorId);
                pars[14] = new SqlParameter("@_Status", model.Status);
                pars[15] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                data = db.GetInstanceSP<AddBookModel>("SP_Store_Book_CMS_Book_Add", 4, pars);
                reponseStatus = Convert.ToInt32(pars[15].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-AddNewBook()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return data;
        }
        public AddBookModel UpdateBook(AddBookModel model, out int reponseStatus)
        {
            DBHelper db = null;
            AddBookModel data = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[16];
                pars[0] = new SqlParameter("@_Barcode", model.Barcode);
                pars[1] = new SqlParameter("@_ImageLink", GetDataString(model.ImageLink));
                pars[2] = new SqlParameter("@_BookName", GetDataString(model.BookName));
                pars[3] = new SqlParameter("@_AuthorId", model.AuthorId);
                pars[4] = new SqlParameter("@_AmountBase", model.AmountBase);
                pars[5] = new SqlParameter("@_AmountSale", model.AmountSale);
                pars[6] = new SqlParameter("@_TrialReadLink", GetDataString(model.TrialReadLink));
                pars[7] = new SqlParameter("@_AudioLink", GetDataString(model.AudioLink));
                pars[8] = new SqlParameter("@_KeyLink", GetDataString(model.KeyLink));
                pars[9] = new SqlParameter("@_NumberPage", model.NumberPage);
                pars[10] = new SqlParameter("@_ContentBook", GetDataString(model.ContentBook));
                pars[11] = new SqlParameter("@_Tags", GetDataString(model.Tags));
                pars[12] = new SqlParameter("@_RelatedBooks", GetDataString(model.RelatedBooks));
                pars[13] = new SqlParameter("@_ColorId", model.ColorId);
                pars[14] = new SqlParameter("@_Status", model.Status);
                pars[15] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                data = db.GetInstanceSP<AddBookModel>("SP_Store_Book_CMS_Book_Update", 4, pars);
                reponseStatus = Convert.ToInt32(pars[15].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-UpdateBook()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return data;
        }
        public AddBookModel GetBookCmsByBarcode(string barcode, out int reponseStatus)
        {
            DBHelper db = null;
            AddBookModel modelData = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_Barcode", barcode);
                pars[1] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                modelData = db.GetInstanceSP<AddBookModel>("SP_Store_Book_CMS_Book_Get_By_BarCode", 4, pars);
                reponseStatus = Convert.ToInt32(pars[1].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-GetBookCmsByBarcode()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return modelData;
        }
        public List<AddBookModel> GetListBookCms(int page, int row, out int reponseStatus)
        {
            DBHelper db = null;
            List<AddBookModel> modelData = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_Index", page);
                pars[1] = new SqlParameter("@_NUMBER_GET", row);
                pars[2] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                modelData = db.GetListSP<AddBookModel>("SP_Store_Book_CMS_Book_Get_List", 4, pars);
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
        #endregion

        #region AUTHOR

        public List<AuthorInfoModel> GetListAuthorConfigAll(out int reponseStatus)
        {
            DBHelper db = null;
            List<AuthorInfoModel> listConfig = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                listConfig = db.GetListSP<AuthorInfoModel>("SP_Store_Book_Author_Get_All", 4, pars);
                reponseStatus = Convert.ToInt32(pars[0].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-GetListAuthorConfigAll()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return listConfig;
        }
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
                modelData = db.GetInstanceSP<AuthorInfoModel>("SP_Store_Book_Author_Get_Info", 4, pars);
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
                var pars = new SqlParameter[5];
                pars[0] = new SqlParameter("@_AuthorName", model.AuthorName);
                pars[1] = new SqlParameter("@_AuthorBirday", model.AuthorBirday);
                pars[2] = new SqlParameter("@_AuthorAdress", model.AuthorAdress);
                pars[3] = new SqlParameter("@_AuthorIntroduction", model.AuthorIntroduction);
                pars[4] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                data = db.GetInstanceSP<AuthorInfoModel>("SP_Store_Book_Author_Add", 4, pars);
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
        public int UpdateAuthorConfig(AuthorInfoModel model)
        {
            DBHelper db = null;
            int reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[6];
                pars[0] = new SqlParameter("@_AuthorId", model.AuthorId);
                pars[1] = new SqlParameter("@_AuthorName", model.AuthorName);
                pars[2] = new SqlParameter("@_AuthorBirday", model.AuthorBirday);
                pars[3] = new SqlParameter("@_AuthorAdress", model.AuthorAdress);
                pars[4] = new SqlParameter("@_AuthorIntroduction", model.AuthorIntroduction);
                pars[5] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_Store_Book_Author_Update", 4, pars);
                reponseStatus = Convert.ToInt32(pars[5].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-UpdateNewAuthor()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return reponseStatus;
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
        public List<BookBuyModel> GetBookBuyAccount(long accountId, int page, int row, out int reponseStatus)
        {
            DBHelper db = null;
            List<BookBuyModel> listConfig = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@_AccountId", accountId);
                pars[1] = new SqlParameter("@_Index", page);
                pars[2] = new SqlParameter("@_NUMBER_GET", row);
                pars[3] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                listConfig = db.GetListSP<BookBuyModel>("SP_Store_Book_CMS_Account_Get_Barcodes", 4, pars);
                reponseStatus = Convert.ToInt32(pars[3].Value);
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
        public long GetCountBuyAccount(long accountId, out int reponseStatus)
        {
            DBHelper db = null;
            long countBuy = 0;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_AccountId", accountId);
                pars[1] = new SqlParameter("@_CountBookBuy", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[2] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_Store_Book_CMS_Account_Count_Barcodes", 4, pars);
                reponseStatus = Convert.ToInt32(pars[2].Value);
                countBuy = Convert.ToInt64(pars[1].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-GetCountBuyAccount()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return countBuy;
        }
        public SimpleBookModel AccountBuyBarcode(long accountId, string barcode, out int reponseStatus, out long price)
        {
            DBHelper db = null;
            SimpleBookModel data = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            price = 0;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@_AccountId", accountId);
                pars[1] = new SqlParameter("@_Barcode", barcode);
                pars[2] = new SqlParameter("@_BookPrice", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[3] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                data = db.GetInstanceSP<SimpleBookModel>("SP_Store_Book_Account_Buy_Barcode_Set", 4, pars);
                reponseStatus = Convert.ToInt32(pars[3].Value);
                price = Convert.ToInt64(pars[2].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-AccountBuyBarcode()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return data;
        }
        #endregion
        #region TAG
        public BookTagModel AddTagConfig(BookTagModel model, out int reponseStatus)
        {
            DBHelper db = null;
            BookTagModel data = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_TagName", model.TagName);
                pars[1] = new SqlParameter("@_LinkIcon", model.LinkIcon);
                pars[2] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                data = db.GetInstanceSP<BookTagModel>("SP_Store_Book_Tag_Add", 4, pars);
                reponseStatus = Convert.ToInt32(pars[2].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-AddTagConfig()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return data;
        }

        public BookTagModel UpdateTagConfig(BookTagModel model, out int reponseStatus)
        {
            DBHelper db = null;
            BookTagModel data = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@_TagId", model.TagId);
                pars[1] = new SqlParameter("@_TagName", model.TagName);
                pars[2] = new SqlParameter("@_LinkIcon", model.LinkIcon);
                pars[3] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                data = db.GetInstanceSP<BookTagModel>("SP_Store_Book_Tag_Update", 4, pars);
                reponseStatus = Convert.ToInt32(pars[3].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-UpdateTagConfig()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return data;
        }
        public List<BookTagModel> GetAllTagConfig(out int reponseStatus)
        {
            DBHelper db = null;
            List<BookTagModel> listConfig = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                listConfig = db.GetListSP<BookTagModel>("SP_Store_Book_Tag_Get_All", 4, pars);
                reponseStatus = Convert.ToInt32(pars[0].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-GetAllTagConfig()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return listConfig;
        }
        public BookTagModel GetTagConfig(int tagId, out int reponseStatus)
        {
            DBHelper db = null;
            BookTagModel data = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_TagId", tagId);
                pars[1] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                data = db.GetInstanceSP<BookTagModel>("SP_Store_Book_Tag_Get", 4, pars);
                reponseStatus = Convert.ToInt32(pars[1].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-GetTagConfig()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return data;
        }
        #endregion

        #region LIKE
        public LikeBookModel LikeBook(long accountId, string barcode, bool status, string actionTime, out int reponseStatus)
        {
            DBHelper db = null;
            LikeBookModel data = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[5];
                pars[0] = new SqlParameter("@_AccountId", accountId);
                pars[1] = new SqlParameter("@_Barcode", barcode);
                pars[2] = new SqlParameter("@_ActionTime", actionTime);
                pars[3] = new SqlParameter("@_Status", status);
                pars[4] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                data = db.GetInstanceSP<LikeBookModel>("SP_Store_Book_Like_Add", 4, pars);
                reponseStatus = Convert.ToInt32(pars[4].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-AddTagConfig()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return data;
        }
        public List<LikeBookModel> GetLikeBook(long accountId, int page, int row, out int reponseStatus)
        {
            DBHelper db = null;
            List<LikeBookModel> modelData = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@_AccountId", accountId);
                pars[1] = new SqlParameter("@_Index", page);
                pars[2] = new SqlParameter("@_NUMBER_GET", row);
                pars[3] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                modelData = db.GetListSP<LikeBookModel>("SP_Store_Book_Like_Account_Get_Join", 4, pars);
                reponseStatus = Convert.ToInt32(pars[3].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-GetLikeBook()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return modelData;
        }
        public List<LikeBookModel> GetLikeBookAll(long accountId, int page, int row, out int reponseStatus)
        {
            DBHelper db = null;
            List<LikeBookModel> modelData = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@_AccountId", accountId);
                pars[1] = new SqlParameter("@_Index", page);
                pars[2] = new SqlParameter("@_NUMBER_GET", row);
                pars[3] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                modelData = db.GetListSP<LikeBookModel>("SP_Store_Book_Like_Account_Get_All", 4, pars);
                reponseStatus = Convert.ToInt32(pars[3].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-GetLikeBookAll()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return modelData;
        }

        public long GetCountBookLike(long accountId, out int reponseStatus)
        {
            DBHelper db = null;
            long countLike = 0;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_AccountId", accountId);
                pars[1] = new SqlParameter("@_CountLike", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[2] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_Store_Book_Like_Account_Count", 4, pars);
                reponseStatus = Convert.ToInt32(pars[2].Value);
                countLike = Convert.ToInt64(pars[1].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-GetCountBookLike()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return countLike;
        }
        #endregion

        #region Feature book
        public List<FeaturedBookConfigModel> GetFeatureBookConfig(out int reponseStatus)
        {
            DBHelper db = null;
            List<FeaturedBookConfigModel> modelData = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                modelData = db.GetListSP<FeaturedBookConfigModel>("SP_Store_Book_Feature_Config_Get", 4, pars);
                reponseStatus = Convert.ToInt32(pars[0].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-GetFeatureBookConfig()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return modelData;
        }
        public List<FeaturedBookConfigModel> GetFeatureBookConfigAll(out int reponseStatus)
        {
            DBHelper db = null;
            List<FeaturedBookConfigModel> modelData = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                modelData = db.GetListSP<FeaturedBookConfigModel>("SP_Store_Book_Feature_Config_Get_All", 4, pars);
                reponseStatus = Convert.ToInt32(pars[0].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-GetFeatureBookConfigAll()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return modelData;
        }
        public FeaturedBookConfigModel AddFeatureBookConfig(FeaturedBookConfigModel model, out int reponseStatus)
        {
            DBHelper db = null;
            FeaturedBookConfigModel data = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@_FeatureName", model.FeatureName);
                pars[1] = new SqlParameter("@_LinkIcon", model.LinkIcon);
                pars[2] = new SqlParameter("@_Status", model.Status);
                pars[3] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                data = db.GetInstanceSP<FeaturedBookConfigModel>("SP_Store_Book_Feature_Config_Add", 4, pars);
                reponseStatus = Convert.ToInt32(pars[3].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-AddFeatureBookConfig()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return data;
        }

        public FeaturedBookConfigModel UpdateFeatureBookConfig(FeaturedBookConfigModel model, out int reponseStatus)
        {
            DBHelper db = null;
            FeaturedBookConfigModel data = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[5];
                pars[0] = new SqlParameter("@_FeatureType", model.FeatureType);
                pars[1] = new SqlParameter("@_FeatureName", model.FeatureName);
                pars[2] = new SqlParameter("@_LinkIcon", model.LinkIcon);
                pars[3] = new SqlParameter("@_Status", model.Status);
                pars[4] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                data = db.GetInstanceSP<FeaturedBookConfigModel>("SP_Store_Book_Feature_Config_Update", 4, pars);
                reponseStatus = Convert.ToInt32(pars[4].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-UpdateFeatureBookConfig()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return data;
        }

        public List<FeaturedBookDataModel> GetFeatureBookData(int featuredType, out int reponseStatus)
        {
            DBHelper db = null;
            List<FeaturedBookDataModel> modelData = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_FeatureType", featuredType);
                pars[1] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                modelData = db.GetListSP<FeaturedBookDataModel>("SP_Store_Book_Feature_Data_Get", 4, pars);
                reponseStatus = Convert.ToInt32(pars[1].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-GetFeatureBookData()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return modelData;
        }
        public FeaturedBookDataModel AddFeatureBookData(int FeatureType, string Barcode, out int reponseStatus)
        {
            DBHelper db = null;
            FeaturedBookDataModel data = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_FeatureType", FeatureType);
                pars[1] = new SqlParameter("@_Barcode", Barcode);
                pars[2] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                data = db.GetInstanceSP<FeaturedBookDataModel>("SP_Store_Book_Feature_Data_Add", 4, pars);
                reponseStatus = Convert.ToInt32(pars[2].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-AddFeatureBookData()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return data;
        }

        public int RemoveFeatureBookData(FeaturedBookDataModel model)
        {
            DBHelper db = null;
            int reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_FeatureId", model.FeatureId);
                pars[1] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_Store_Book_Feature_Data_Remove", 4, pars);
                reponseStatus = Convert.ToInt32(pars[1].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-AddFeatureBookData()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return reponseStatus;
        }
        #endregion

        #region Reprint
        public bool CheckReprintBook(string barcode)
        {
            DBHelper db = null;
            bool check = false;
            int reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_Barcode", barcode);
                pars[1] = new SqlParameter("@_Check", SqlDbType.Bit) { Direction = ParameterDirection.Output };
                pars[2] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_Store_Book_Reprint_Check_BarCode", 4, pars);
                check = Convert.ToBoolean(pars[1].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-SP_Store_Book_Reprint_Check_BarCode()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return check;
        }
        public void AddReprintBook(string barcode, out int reponseStatus)
        {
            DBHelper db = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_Barcode", barcode);
                pars[1] = new SqlParameter("@_Status", true);
                pars[2] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_Store_Book_Reprint_Add", 4, pars);
                reponseStatus = Convert.ToInt32(pars[2].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-SP_Store_Book_Reprint_Add()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
        }
        public void RemoveReprintBook(string barcode, out int reponseStatus)
        {
            DBHelper db = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_Barcode", barcode);
                pars[1] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                db.ExecuteNonQuerySP("SP_Store_Book_Reprint_Data_Remove", 4, pars);
                reponseStatus = Convert.ToInt32(pars[1].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-SP_Store_Book_Reprint_Data_Remove()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
        }
        public List<ReprintBookDataModel> GetAllReprintBook(out int reponseStatus)
        {
            DBHelper db = null;
            List<ReprintBookDataModel> data = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreBookConnectionString);
                var pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                data = db.GetListSP<ReprintBookDataModel>("SP_Store_Book_Reprint_Get_All", 4, pars);
                reponseStatus = Convert.ToInt32(pars[0].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-SP_Store_Book_Reprint_Get_All()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return data;
        }

        #endregion

        private string GetDataString(string data) {
            if (!string.IsNullOrEmpty(data))
                return data;
            else
                return "";
        }
    }
}
