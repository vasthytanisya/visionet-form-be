using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visionet.Form.Entities
{
    public class Employee
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime BornDate { get; set; }
        public List<string> Skills { get; set; } = new List<string>();
    }
}
