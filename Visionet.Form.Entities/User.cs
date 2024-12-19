using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Visionet.Form.Entities.EntityDescriptors;

namespace Visionet.Form.Entities
{
    public class User : IdentityUser, IHaveCreateAndUpdateAudit
    {
        [StringLength(64)]
        public string FullName { set; get; } = "";

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        [StringLength(256)]
        public string? CreatedBy { get; set; }

        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

        [StringLength(256)]
        public string? UpdatedBy { get; set; }
    }
}
