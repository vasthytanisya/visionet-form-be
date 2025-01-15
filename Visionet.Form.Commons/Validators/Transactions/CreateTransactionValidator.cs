using FluentValidation;
using Visionet.Form.Contracts.RequestModels.Transactions;

namespace Visionet.Form.Commons.Validators.Transactions
{
    public class CreateTransactionValidator: AbstractValidator<CreateTransactionRequest>
    {
        public CreateTransactionValidator()
        {
            RuleFor(Q => Q.TypeCustomer).NotEmpty().WithMessage("Type Customer cannot be empty");
        }
    }
}
