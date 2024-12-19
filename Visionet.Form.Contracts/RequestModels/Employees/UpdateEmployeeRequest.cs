using MediatR;
using Visionet.Form.Contracts.ResponseModels.Employees;

namespace Visionet.Form.Contracts.RequestModels.Employees
{
    public class UpdateEmployeeRequest : IRequest<UpdateEmployeeResponse>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public DateTime BornDate { get; set; }
        public List<string> Skills { get; set; } = new List<string>();
    }
}
