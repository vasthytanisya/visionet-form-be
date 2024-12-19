
namespace Visionet.Form.Commons.Extensions
{
    public class ApplicationServicesOptions
    {
        public string PostgreSqlConnectionString { get; set; } = "";

        public bool AddWebAppOnlyServices { set; get; }

        public bool AlwaysSecureCookiePolicy { get; set; }
    }
}
