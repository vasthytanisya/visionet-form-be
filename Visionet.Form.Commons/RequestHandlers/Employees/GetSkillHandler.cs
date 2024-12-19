using MediatR;
using Microsoft.EntityFrameworkCore;
using Visionet.Form.Contracts.RequestModels.Employees;
using Visionet.Form.Contracts.ResponseModels.Employees;
using Visionet.Form.Entities;

namespace Visionet.Form.Commons.RequestHandlers.Employees
{
    public class GetSkillHandler : IRequestHandler<GetSkillRequest, List<GetSkillResponse>>
    {
        private readonly FormDbContext _db;

        public GetSkillHandler(FormDbContext db)
        {
            _db = db;
        }

        public async Task<List<GetSkillResponse>> Handle(GetSkillRequest request, CancellationToken cancellationToken)
        {
            return await _db.Skills.Select(c => new GetSkillResponse()
            {
                Id = c.Id,
                Name = c.Name
            }).AsNoTracking().ToListAsync(cancellationToken);
        }
    }
}
