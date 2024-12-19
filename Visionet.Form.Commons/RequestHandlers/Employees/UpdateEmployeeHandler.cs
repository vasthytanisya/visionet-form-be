using MediatR;
using Microsoft.EntityFrameworkCore;
using Visionet.Form.Contracts.RequestModels.Employees;
using Visionet.Form.Contracts.ResponseModels.Employees;
using Visionet.Form.Entities;

namespace Visionet.Form.Commons.RequestHandlers.Employees
{
    public class UpdateEmployeeHandler : IRequestHandler<UpdateEmployeeRequest, UpdateEmployeeResponse>
    {
        private readonly FormDbContext _db;

        public UpdateEmployeeHandler(FormDbContext db)
        {
            _db = db;
        }

        public async Task<UpdateEmployeeResponse> Handle(UpdateEmployeeRequest request, CancellationToken cancellationToken)
        {
            var employeeId = await _db.Employees.Where(Q => Q.Id == request.Id).FirstOrDefaultAsync(cancellationToken);

            if (employeeId == null)
            {
                return new UpdateEmployeeResponse
                {
                    Success = "Data Employee Not Found"
                };
            }

            employeeId.Name = request.Name;
            employeeId.BornDate = request.BornDate;
            employeeId.Skills = request.Skills;

            await _db.SaveChangesAsync(cancellationToken);
            return new UpdateEmployeeResponse { Success = "Data Employee Success Update" };
        }
    }
}
