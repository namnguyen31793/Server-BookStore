using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareData.RequestCode
{
    public class RequestGetGameLobby
    {
        public int GameType { get; set; }
        public int GameTag { get; set; }
        public int GameId { get; set; }
        public int Index { get; set; }
    }

    public class RequestGetGameLobbyByListType
    {
        public string ListGameType { get; set; }
        public int Index { get; set; }
    }
}
