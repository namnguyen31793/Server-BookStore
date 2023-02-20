using System.Threading.Tasks;

namespace JobWindowsProject.Helpers.Interfaces
{
    public interface IHelperService
    {
        Task PerformService(string schedule);
    }
}
