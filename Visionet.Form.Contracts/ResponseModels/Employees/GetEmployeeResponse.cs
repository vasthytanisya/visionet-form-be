namespace Visionet.Form.Contracts.ResponseModels.Employees
{
    public class GetEmployeeResponse
    {
        public string Name { get; set; } = "";
        public DateTime BornDate { get; set; }
        public List<string> Skills { get; set; } = new List<string>();
    }
}
