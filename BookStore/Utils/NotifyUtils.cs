using DAO.DAOImp;
using Newtonsoft.Json;
using RedisSystem;
using ShareData.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerEventTet2023.Utils
{
    public class NotifyUtils
    {

        public string GetNotifyAdmin(string nameEvent)
        {
            string keyRedis = "NotifyAdmin:"+ nameEvent;
            string value = "";
            try
            {
                value = RedisGatewayManager<string>.Inst.GetDataFromCache(keyRedis);
                if (string.IsNullOrEmpty(value))
                {
                    //call cms get notify admin
                    List<LauncherNotifyModel> model = LauncherNotificationDAO.Inst.SP_Launcher_Notification_Event_ChienGioi_Get_Notify();
                    if (model != null)
                    {
                        model = model.Where(x => x.TimeExpires > DateTime.UtcNow).OrderByDescending(x => x.Id).ToList();
                        value = JsonConvert.SerializeObject(model);
                        RedisGatewayManager<string>.Inst.SaveData(keyRedis, value, 10);
                    }
                    else {
                        value = "[]";
                    }
                }
            }
            catch
            {
            }
            return value;
        }

    }
}
