using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visionet.Form.Contracts.RequestModels.Employees;

namespace Visionet.Form.Commons.Validators.Employees
{
    public class UpdateEmployeeValidator : AbstractValidator<UpdateEmployeeRequest>
    {
        public UpdateEmployeeValidator()
        {
            RuleFor(Q => Q.Name).NotEmpty().WithMessage("Name cannot be empty");
        }
    }
}
