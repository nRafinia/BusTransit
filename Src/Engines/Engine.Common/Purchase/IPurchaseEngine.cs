using System.Threading.Tasks;

namespace Engine.Common.Purchase
{
    public interface IPurchaseEngine
    {
        Task<PurchaseEngineResponse> PurchaseQuery(PurchaseEngineRequest data);
    }
}