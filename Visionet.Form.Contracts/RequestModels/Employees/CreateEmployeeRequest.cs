using MediatR;
using Visionet.Form.Contracts.ResponseModels.Employees;

namespace Visionet.Form.Contracts.RequestModels.Employees
{
    public class CreateEmployeeRequest : IRequest<CreateEmployeeResponse>
    {
        public string Name { get; set; } = string.Empty;
        public DateTime BornDate { get; set; }
        public List<string> Skills { get; set; } = new List<string>();
    }
}
