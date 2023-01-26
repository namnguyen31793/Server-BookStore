using ShareData.DAO;

namespace DAO.DAO
{
    public interface IAccountDao
    {
        int DoLogin(string accountName, string passwordMd5, int merchantId, string remoteIp, int ostype, ref int responseStatus);
        RawAccountModel UpdateInfo(long accountId, string nickname, ref int responseStatus);
        int Register(int registerType, string accountName, string nickName, string passwordMd5, int merchantId, string remoteIp, string deviceId, int platfromId, out int accountId);
    }
}