using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visionet.Form.Entities.EntityDescriptors
{
    public interface IHaveCreateOnlyAudit
    {
        public DateTimeOffset CreatedAt { get; set; }

        public string? CreatedBy { get; set; }
    }
}
