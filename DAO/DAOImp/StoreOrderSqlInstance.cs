﻿using DAO.Utitlities;
using LoggerService;
using ShareData.DB.Books;
using ShareData.DB.Order;
using ShareData.ErrorCode;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace DAO.DAOImp
{
    public class StoreOrderSqlInstance
    {
        private static ILoggerManager _logger;

        private static object _syncObject = new object();

        private static StoreOrderSqlInstance _inst;

        public static StoreOrderSqlInstance Inst
        {
            get
            {
                if (_inst == null)
                {
                    lock (_syncObject)
                    {
                        if (_inst == null)
                        {
                            _inst = new StoreOrderSqlInstance();
                            _logger = new LoggerManager();
                        }
                    }
                }
                return _inst;
            }
        }
        public OrderInfoObject CreateNewOrderCMS(long AccountId, string CustomerName, string CustomerMobile, string CustomerEmail, string CustomerAddress, string type, string Description, string Barcodes, string Numbers, string PaymentMethod, long ShipMoney, long TotalDiscountMoney, long VourcherMoney, out int reponseStatus)
        {
            DBHelper db = null;
            OrderInfoObject modelData = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreOrderConnectionString);
                var pars = new SqlParameter[14];
                pars[0] = new SqlParameter("@_AccountId", AccountId);
                pars[1] = new SqlParameter("@_CustomerName", CustomerName);
                pars[2] = new SqlParameter("@_CustomerMobile", CustomerMobile);
                pars[3] = new SqlParameter("@_CustomerEmail", CustomerEmail);
                pars[4] = new SqlParameter("@_CustomerAddress", CustomerAddress);
                pars[5] = new SqlParameter("@_Type", type);
                pars[6] = new SqlParameter("@_Description", Description);
                pars[7] = new SqlParameter("@_Barcodes", Barcodes);
                pars[8] = new SqlParameter("@_Numbers", Numbers);
                pars[9] = new SqlParameter("@_PaymentMethod", PaymentMethod);
                pars[10] = new SqlParameter("@_ShipMoney", ShipMoney);
                pars[11] = new SqlParameter("@_TotalDiscountMoney", TotalDiscountMoney);
                pars[12] = new SqlParameter("@_VourcherMoney", VourcherMoney);
                pars[13] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                modelData = db.GetInstanceSP<OrderInfoObject>("SP_Store_Order_CMS_Order_New", 10, pars);
                reponseStatus = Convert.ToInt32(pars[13].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-CreateNewOrderCMS()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return modelData;
        }

        public OrderInfoObject CreateNewOrder(long AccountId,long CustomerId, string type, string Description, string Barcodes, string Numbers, int VourcherId, string PaymentMethod, int CityCode, out int reponseStatus)
        {
            DBHelper db = null;
            OrderInfoObject modelData = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreOrderConnectionString);
                var pars = new SqlParameter[10];
                pars[0] = new SqlParameter("@_AccountId", AccountId);
                pars[1] = new SqlParameter("@_CustomerId", CustomerId);
                pars[2] = new SqlParameter("@_Type", type);
                pars[3] = new SqlParameter("@_Description", Description);
                pars[4] = new SqlParameter("@_Barcodes", Barcodes);
                pars[5] = new SqlParameter("@_Numbers", Numbers);
                pars[6] = new SqlParameter("@_VourcherId", VourcherId);
                pars[7] = new SqlParameter("@_PaymentMethod", PaymentMethod);
                pars[8] = new SqlParameter("@_CityCode", CityCode);
                pars[9] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                modelData = db.GetInstanceSP<OrderInfoObject>("SP_Store_Order_Order_New", 10, pars);
                reponseStatus = Convert.ToInt32(pars[9].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-CreateNewOrder()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return modelData;
        }

        public OrderInfoObject CreateNewOrder_v2(long AccountId, long CustomerId, string type, string Description, string Barcodes, string Numbers, int VourcherId, string PaymentMethod, int CityCode, out int reponseStatus)
        {
            DBHelper db = null;
            OrderInfoObject modelData = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreOrderConnectionString);
                var pars = new SqlParameter[10];
                pars[0] = new SqlParameter("@_AccountId", AccountId);
                pars[1] = new SqlParameter("@_CustomerId", CustomerId);
                pars[2] = new SqlParameter("@_Type", type);
                pars[3] = new SqlParameter("@_Description", Description);
                pars[4] = new SqlParameter("@_Barcodes", Barcodes);
                pars[5] = new SqlParameter("@_Numbers", Numbers);
                pars[6] = new SqlParameter("@_VourcherId", VourcherId);
                pars[7] = new SqlParameter("@_PaymentMethod", PaymentMethod);
                pars[8] = new SqlParameter("@_CityCode", CityCode);
                pars[9] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                modelData = db.GetInstanceSP<OrderInfoObject>("SP_Store_Order_Order_New_Ver2", 10, pars);
                reponseStatus = Convert.ToInt32(pars[9].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-CreateNewOrder_v2()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return modelData;
        }

        public OrderInfoObject ChangeOrderProcess(long OrderId, long ShipMoney, string PrivateDescription, int AllowTest, out int reponseStatus)
        {
            DBHelper db = null;
            OrderInfoObject modelData = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreOrderConnectionString);
                var pars = new SqlParameter[5];
                pars[0] = new SqlParameter("@_OrderId", OrderId);
                pars[1] = new SqlParameter("@_AllowTest", AllowTest);
                pars[2] = new SqlParameter("@_PrivateDescription", PrivateDescription);
                pars[3] = new SqlParameter("@_ShipMoney", ShipMoney);
                pars[4] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                modelData = db.GetInstanceSP<OrderInfoObject>("SP_Store_Order_Order_Update_Status_Process", 4, pars);
                reponseStatus = Convert.ToInt32(pars[4].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-ChangeOrderProcess()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return modelData;
        }
        public OrderInfoObject EndOrder(long OrderId, int Status, string PrivateDescription, out int reponseStatus)
        {
            DBHelper db = null;
            OrderInfoObject modelData = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreOrderConnectionString);
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@_OrderId", OrderId);
                pars[1] = new SqlParameter("@_PrivateDescription", PrivateDescription);
                pars[2] = new SqlParameter("@_Status", Status);
                pars[3] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                modelData = db.GetInstanceSP<OrderInfoObject>("SP_Store_Order_Order_Update_Status_End", 4, pars);
                reponseStatus = Convert.ToInt32(pars[3].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-EndOrder()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return modelData;
        }
        public OrderInfoObject UserEndOrder(long AccountId, long OrderId, int Status, string PrivateDescription, out int reponseStatus)
        {
            DBHelper db = null;
            OrderInfoObject modelData = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreOrderConnectionString);
                var pars = new SqlParameter[5];
                pars[0] = new SqlParameter("@_AccountId", AccountId);
                pars[1] = new SqlParameter("@_OrderId", OrderId);
                pars[2] = new SqlParameter("@_PrivateDescription", PrivateDescription);
                pars[3] = new SqlParameter("@_Status", Status);
                pars[4] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                modelData = db.GetInstanceSP<OrderInfoObject>("SP_Store_Order_User_Update_Status_End", 4, pars);
                reponseStatus = Convert.ToInt32(pars[4].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-UserEndOrder()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return modelData;
        }
        public List<OrderInfoObject> GetOrderById(long OrderId, out int reponseStatus)
        {
            DBHelper db = null;
            List<OrderInfoObject> modelData = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreOrderConnectionString);
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_OrderId", OrderId);
                pars[1] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                modelData = db.GetListSP<OrderInfoObject>("SP_Store_Order_Order_Get_By_Id", 4, pars);
                reponseStatus = Convert.ToInt32(pars[1].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-GetOrderByAccountId()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return modelData;
        }

        public List<OrderInfoObject> GetOrderByAccountId(long AccountId, out int reponseStatus)
        {
            DBHelper db = null;
            List<OrderInfoObject> modelData = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreOrderConnectionString);
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_AccountId", AccountId);
                pars[1] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                modelData = db.GetListSP<OrderInfoObject>("SP_Store_Order_Order_Get_By_Account_Id", 4, pars);
                reponseStatus = Convert.ToInt32(pars[1].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-GetOrderByAccountId()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return modelData;
        }

        public List<OrderInfoObject> GetOrderByTime(DateTime startTime, DateTime endTime, out int reponseStatus)
        {
            DBHelper db = null;
            List<OrderInfoObject> modelData = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreOrderConnectionString);
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_StartTime", startTime);
                pars[1] = new SqlParameter("@_EndTime", endTime);
                pars[2] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                modelData = db.GetListSP<OrderInfoObject>("SP_Store_Order_Order_Get_By_Time", 4, pars);
                reponseStatus = Convert.ToInt32(pars[2].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-GetOrderByTime()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return modelData;
        }

        #region CUSTOMER
        public List<CustomerInfoModel> CreateCustomerInfo(long AccountId, string CustomerName, string CustomerMobile, string CustomerEmail, string CustomerAddress, bool Defaut, out int reponseStatus)
        {
            DBHelper db = null;
            List<CustomerInfoModel> modelData = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreOrderConnectionString);
                var pars = new SqlParameter[7];
                pars[0] = new SqlParameter("@_AccountId", AccountId);
                pars[1] = new SqlParameter("@_CustomerName", CustomerName);
                pars[2] = new SqlParameter("@_CustomerMobile", CustomerMobile);
                pars[3] = new SqlParameter("@_CustomerEmail", CustomerEmail);
                pars[4] = new SqlParameter("@_CustomerAddress", CustomerAddress);
                pars[5] = new SqlParameter("@_Defaut", Defaut);
                pars[6] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                modelData = db.GetListSP<CustomerInfoModel>("SP_Store_Order_Customer_CreateNew", 4, pars);
                reponseStatus = Convert.ToInt32(pars[6].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-CreateCustomerInfo()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return modelData;
        }

        public List<CustomerInfoModel> DeleteCustomerInfo(long CustomerId, out int reponseStatus)
        {
            DBHelper db = null;
            List<CustomerInfoModel> modelData = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreOrderConnectionString);
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_CustomerId", CustomerId);
                pars[1] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                modelData = db.GetListSP<CustomerInfoModel>("SP_Store_Order_Customer_Delete", 4, pars);
                reponseStatus = Convert.ToInt32(pars[1].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-DeleteCustomerInfo()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return modelData;
        }

        public List<CustomerInfoModel> GetCustomerInfo(long AccountId, out int reponseStatus)
        {
            DBHelper db = null;
            List<CustomerInfoModel> modelData = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreOrderConnectionString);
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_AccountId", AccountId);
                pars[1] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                modelData = db.GetListSP<CustomerInfoModel>("SP_Store_Order_Customer_Get_By_AccountId", 4, pars);
                reponseStatus = Convert.ToInt32(pars[1].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-GetCustomerInfo()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return modelData;
        }
        public List<CustomerInfoModel> UpdateCustomerInfo(long CustomerId, string CustomerName, string CustomerMobile, string CustomerEmail, string CustomerAddress, bool Defaut, out int reponseStatus)
        {
            DBHelper db = null;
            bool status = true;
            List<CustomerInfoModel> modelData = null;
            reponseStatus = EStatusCode.DATABASE_ERROR;
            try
            {
                db = new DBHelper(ConfigDb.StoreOrderConnectionString);
                var pars = new SqlParameter[8];
                pars[0] = new SqlParameter("@_CustomerId", CustomerId);
                pars[1] = new SqlParameter("@_CustomerName", CustomerName);
                pars[2] = new SqlParameter("@_CustomerMobile", CustomerMobile);
                pars[3] = new SqlParameter("@_CustomerEmail", CustomerEmail);
                pars[4] = new SqlParameter("@_CustomerAddress", CustomerAddress);
                pars[5] = new SqlParameter("@_Defaut", Defaut);
                pars[6] = new SqlParameter("@_Status", status);
                pars[7] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                modelData = db.GetListSP<CustomerInfoModel>("SP_Store_Order_Customer_Update", 4, pars);
                reponseStatus = Convert.ToInt32(pars[7].Value);
            }
            catch (Exception exception)
            {
                Task.Run(async () => await _logger.LogError("SQL-UpdateCustomerInfo()", exception.ToString()).ConfigureAwait(false));
            }
            finally
            {
                db?.Close();
            }
            return modelData;
        }
        #endregion
    }
}
