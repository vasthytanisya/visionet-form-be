using MediatR;
using Microsoft.EntityFrameworkCore;
using Visionet.Form.Contracts.RequestModels.Employees;
using Visionet.Form.Contracts.ResponseModels.Employees;
using Visionet.Form.Entities;

namespace Visionet.Form.Commons.RequestHandlers.Employees
{
    public class ListEmployeeHandler : IRequestHandler<ListEmployeeRequest, ListEmployeeResponse>
    {
        private readonly FormDbContext _db;

        public ListEmployeeHandler(FormDbContext db)
        {
            _db = db;
        }
        public async Task<ListEmployeeResponse> Handle(ListEmployeeRequest request, CancellationToken cancellationToken)
        {
            var datas = await _db.Employees.Select(Q => new EmployeeData
            {
                Id = Q.Id,
                Name = Q.Name,
            }).AsNoTracking().ToListAsync(cancellationToken);

            return new ListEmployeeResponse
            {
                Datas = datas
            };

        }
    }
}
