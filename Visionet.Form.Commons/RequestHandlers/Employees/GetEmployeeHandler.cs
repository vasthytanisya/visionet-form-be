using MediatR;
using Microsoft.EntityFrameworkCore;
using Visionet.Form.Contracts.RequestModels.Employees;
using Visionet.Form.Contracts.ResponseModels.Employees;
using Visionet.Form.Entities;

namespace Visionet.Form.Commons.RequestHandlers.Employees
{
    public class GetEmployeeHandler : IRequestHandler<GetEmployeeRequest, GetEmployeeResponse>
    {
        private readonly FormDbContext _db;

        public GetEmployeeHandler(FormDbContext db)
        {
            _db = db;
        }

        public async Task<GetEmployeeResponse> Handle(GetEmployeeRequest request, CancellationToken cancellationToken)
        {
            var employeeId = await _db.Employees.Where(Q => Q.Id == request.Id).FirstOrDefaultAsync(cancellationToken);

            if (employeeId == null)
            {
                return new GetEmployeeResponse() { };
            }

            return new GetEmployeeResponse
            {
                Name = employeeId.Name,
                BornDate = employeeId.BornDate,
                Skills = employeeId.Skills,
            };
        }
    }
}
