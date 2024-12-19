using MediatR;
using Visionet.Form.Contracts.ResponseModels.Employees;

namespace Visionet.Form.Contracts.RequestModels.Employees
{
    public class DeleteEmployeeRequest : IRequest<DeleteEmployeeResponse>
    {
        public Guid Id { get; set; }
    }
}
