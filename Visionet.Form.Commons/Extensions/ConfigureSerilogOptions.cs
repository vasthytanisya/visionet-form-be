
namespace Visionet.Form.Commons.Extensions
{
    public class ConfigureSerilogOptions
    {
        public string? WriteErrorLogsToFile { get; set; }

        public bool WriteJsonToConsoleLog { get; set; }

        public bool WriteToSentry { get; set; }
    }
}
