using MediatR;
using Visionet.Form.Contracts.RequestModels.Employees;
using Visionet.Form.Contracts.ResponseModels.Employees;
using Visionet.Form.Entities;

namespace Visionet.Form.Commons.RequestHandlers.Employees
{
    public class CreateEmployeeHandler : IRequestHandler<CreateEmployeeRequest, CreateEmployeeResponse>
    {
        private readonly FormDbContext _db;

        public CreateEmployeeHandler(FormDbContext db)
        {
            _db = db;
        }

        public async Task<CreateEmployeeResponse> Handle(CreateEmployeeRequest request, CancellationToken cancellationToken)
        {
            var newEmployee = new Employee
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                BornDate = request.BornDate,
                Skills = request.Skills,
            };

            await _db.AddAsync(newEmployee, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);

            return new CreateEmployeeResponse
            {
                Success = "New Employee Created"
            };

        }
    }
}
