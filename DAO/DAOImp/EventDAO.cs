using DAO.Utitlities;
using ShareData.DAO;
using ShareData.LogSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAO.DAOImp
{
    public class EventDAO
    {
        private static readonly object SyncObject = new object();

        private static EventDAO _inst;

        public static EventDAO Inst
        {
            get
            {
                if (_inst == null)
                {
                    lock (SyncObject)
                    {
                        if (_inst == null)
                            _inst = new EventDAO();
                    }
                }
                return _inst;
            }
        }

        private List<EventConfigModel> EventConfigList;
        public void LoadAllEvents()
        {
            DBHelper db = null;
            try
            {
                db = new DBHelper(ConfigDb.EventConfigConnectionString);
                this.EventConfigList = db.GetListSP<EventConfigModel>("SP_BanCaBoss_Event_EventConfigs_GetAll_Active");
            }
            catch (Exception)
            {
            }
            finally
            {
                db?.Close();
            }
        }

        public List<EventConfigModel> GetAllActiveEvents()
        {
            if (this.EventConfigList == null || this.EventConfigList.Count <= 0)
            {
                this.LoadAllEvents();
            }
            if (this.EventConfigList == null) return null;
            return this.EventConfigList;
        }

        public EventConfigModel GetActiveEventByType(int eventType)
        {
            List<EventConfigModel> eventList = this.GetAllActiveEvents();
            if (eventList == null) return null;
            EventConfigModel eventModel = eventList.Find(v => v.EventId == eventType && v.Status);
            if (eventModel == null) return null;
            // time event
            if (eventModel.EndDate < DateTime.Now || eventModel.StartDate > DateTime.Now)
            {
                return null;
            }
            return eventModel;
        }
    }
}
