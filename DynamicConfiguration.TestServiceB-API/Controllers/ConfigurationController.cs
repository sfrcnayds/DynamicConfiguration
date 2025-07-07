using DynamicConfiguration.Library.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DynamicConfiguration.TestServiceB_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConfigurationController : ControllerBase
    {
        private readonly IConfigurationReader _configurationReader;

        public ConfigurationController(IConfigurationReader configurationReader)
        {
            _configurationReader = configurationReader;
        }

        [HttpGet("{configurationName:required}")]
        public ActionResult<string> Get(string configurationName)
        {
            return _configurationReader.GetValue<string>(configurationName);
        }
    }
}