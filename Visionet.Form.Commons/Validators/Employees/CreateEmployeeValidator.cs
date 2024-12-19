using FluentValidation;
using Visionet.Form.Contracts.RequestModels.Employees;

namespace Visionet.Form.Commons.Validations.Employees
{
    public class CreateEmployeeValidator : AbstractValidator<CreateEmployeeRequest>
    {
        public CreateEmployeeValidator()
        {
            RuleFor(Q => Q.Name).NotEmpty().WithMessage("Name cannot be empty");
        }
    }


}
