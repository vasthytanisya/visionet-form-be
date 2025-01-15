using MediatR;
using Visionet.Form.Contracts.ResponseModels.Transactions;

namespace Visionet.Form.Contracts.RequestModels.Transactions
{
    public class CreateTransactionRequest : IRequest<CreateTransactionResponse>
    {
        public string TypeCustomer { get; set; } = string.Empty;
        public int Point { get; set; }
        public int TotalGrocery { get; set; }
    }
}
