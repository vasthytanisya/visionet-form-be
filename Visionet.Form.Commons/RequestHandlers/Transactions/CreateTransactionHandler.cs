using MediatR;
using Microsoft.EntityFrameworkCore;
using Visionet.Form.Contracts.RequestModels.Transactions;
using Visionet.Form.Contracts.ResponseModels.Transactions;
using Visionet.Form.Entities;

namespace Visionet.Form.Commons.RequestHandlers.Transactions
{
    public class CreateTransactionHandler : IRequestHandler<CreateTransactionRequest, CreateTransactionResponse>
    {
        private readonly FormDbContext _db;

        public CreateTransactionHandler(FormDbContext db)
        {
            _db = db;
        }

        public async Task<CreateTransactionResponse> Handle(CreateTransactionRequest request, CancellationToken cancellationToken)
        {
            var currentDate = DateTime.UtcNow.ToString("yyyyMMdd");

            var checkDateNumber = await _db.Transactions.Where(t => t.Id.Contains(currentDate)).CountAsync();
            checkDateNumber += 1;

            var transactionId = $"{currentDate}_{checkDateNumber:D5}";

            var transaction = new Transaction
            {
                Id = transactionId,
                TypeCustomer = request.TypeCustomer,
                Point = request.Point,
                TotalGrocery = request.TotalGrocery,
            };

            var discount = 0;

            if(request.TypeCustomer == "platinum")
            {
                if(request.Point >= 100 && request.Point <= 300)
                {
                    discount = (int)(request.TotalGrocery * (0.5)) + 35;
                }
                else if(request.Point >= 301 && request.Point <= 500)
                {
                    discount = (int)(request.TotalGrocery * (0.5)) + 50;
                }
                else if(request.Point > 500)
                {
                    discount = (int)(request.TotalGrocery * (0.5)) + 68;
                }
            }
            else if(request.TypeCustomer == "gold")
            {
                if (request.Point >= 100 && request.Point <= 300)
                {
                    discount = (int)(request.TotalGrocery * (0.25)) + 25;
                }
                else if (request.Point >= 301 && request.Point <= 500)
                {
                    discount = (int)(request.TotalGrocery * (0.25)) + 34;
                }
                else if (request.Point > 500)
                {
                    discount = (int)(request.TotalGrocery * (0.25)) + 52;
                }
            }
            else if(request.TypeCustomer == "silver")
            {
                if (request.Point > 100 && request.Point < 300)
                {
                    discount = (int)(request.TotalGrocery * (0.1)) + 12;
                }
                else if (request.Point >= 301 && request.Point <= 500)
                {
                    discount = (int)(request.TotalGrocery * (0.1)) + 27;
                }
                else if (request.Point > 500)
                {
                    discount = (int)(request.TotalGrocery * (0.1)) + 39;
                }
            }

            transaction.Discount = discount;
            transaction.TotalPayment = request.TotalGrocery - discount;

            await _db.AddAsync(transaction, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);

            return new CreateTransactionResponse { Success = "Success" };
        }
    }
}
