
namespace Visionet.Form.Contracts.ResponseModels.Employees
{
    public class ListEmployeeResponse
    {
        public List<EmployeeData> Datas { get; set; } = new List<EmployeeData>();
       
    }

    public class EmployeeData
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
