using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visionet.Form.Entities.EntityDescriptors
{
    public interface IHaveCreateAndUpdateAudit : IHaveCreateOnlyAudit
    {
        public DateTimeOffset UpdatedAt { get; set; }

        public string? UpdatedBy { get; set; }
    }
}
