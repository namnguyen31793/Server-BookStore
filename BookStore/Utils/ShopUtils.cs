using DAO.DAOImp;
using Newtonsoft.Json;
using RedisSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerEventTet2023.Utils
{
    public class ShopUtils
    {
        public string GiftCodeConfig()
        {
            string keyRedis = "GiftCodeConfig";
            string value = "";
            try
            {
                value = RedisGatewayManager<string>.Inst.GetDataFromCache(keyRedis);
                if (string.IsNullOrEmpty(value))
                {
                    var model = SlotMachineEventDAO.Inst.SP_NewSlotMachine_Event_Get_GiftCodeConfig();
                    if (model != null)
                    {
                        value = JsonConvert.SerializeObject(model);
                        RedisGatewayManager<string>.Inst.SaveData(keyRedis, value, 600);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return value;
        }
    }
}
