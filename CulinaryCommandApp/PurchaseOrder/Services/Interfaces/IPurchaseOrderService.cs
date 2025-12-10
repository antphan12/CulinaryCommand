using System.Threading;
using System.Threading.Tasks;
using CulinaryCommand.PurchaseOrder.DTOs;

namespace CulinaryCommand.PurchaseOrder.Services
{
    public interface IPurchaseOrderService
    {
        Task<PurchaseOrderDTO> CreateDraftAsync(CreatePurchaseOrderDTO request, CancellationToken cancellationToken = default);
    }
}