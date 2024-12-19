using MediatR;
using Microsoft.EntityFrameworkCore;
using Visionet.Form.Contracts.RequestModels.Employees;
using Visionet.Form.Contracts.ResponseModels.Employees;
using Visionet.Form.Entities;

namespace Visionet.Form.Commons.RequestHandlers.Employees
{
    public class DeleteEmployeeHandler : IRequestHandler<DeleteEmployeeRequest, DeleteEmployeeResponse>
    {
        private readonly FormDbContext _db;

        public DeleteEmployeeHandler(FormDbContext db)
        {
            _db = db;
        }

        public async Task<DeleteEmployeeResponse> Handle(DeleteEmployeeRequest request, CancellationToken cancellationToken)
        {
            var deleteId = await _db.Employees.Where(Q => Q.Id == request.Id).FirstOrDefaultAsync(cancellationToken);

            if(deleteId == null)
            {
                return new DeleteEmployeeResponse { Success = "Data Not Found" };
            }

           _db.Employees.Remove(deleteId);
            await _db.SaveChangesAsync(cancellationToken);

            return new DeleteEmployeeResponse { Success = "Data Delete Success" };
        }
    }
}
