using System;
using System.Collections.Generic;
using System.Text;

namespace DAO.DAO
{
    public interface ICmsDAO
    {
        List<int> GetListGunPrice();
        List<int> GetListFishConfig();
    }
}
