using MediatR;
using Visionet.Form.Contracts.ResponseModels.Employees;

namespace Visionet.Form.Contracts.RequestModels.Employees
{
    public class GetEmployeeRequest : IRequest<GetEmployeeResponse>
    {
        public Guid Id { get; set; }
    }
}
