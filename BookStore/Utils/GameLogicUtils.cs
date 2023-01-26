using DAO.DAOImp;
using Newtonsoft.Json;
using RedisSystem;
using ShareData.DataEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UtilsSystem.Utils;
using ServerEventTet2023.Instance;

namespace ServerEventTet2023.Utils
{
    public class GameLogicUtils
    {

        public bool CheckUserSpin(long AccountId)
        {
            try
            {
                string infoString = RedisGatewayCacheManager.Inst.GetDataFromCache("Spin:" + AccountId);
                if (!string.IsNullOrEmpty(infoString))
                {
                    return true;
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        public void SaveUserSpin(long AccountId){
            RedisGatewayCacheManager.Inst.SaveDataSecond("Spin:" + AccountId, AccountId.ToString(), 2);
        }
        public bool CheckUserSwapCat(long AccountId)
        {
            try
            {
                string infoString = RedisGatewayCacheManager.Inst.GetDataFromCache("SwapCat:" + AccountId);
                if (!string.IsNullOrEmpty(infoString))
                {
                    return true;
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        public void SaveUserSwapCat(long AccountId)
        {
            RedisGatewayCacheManager.Inst.SaveDataSecond("SwapCat:" + AccountId, AccountId.ToString(), 2);
        }
    }
}
