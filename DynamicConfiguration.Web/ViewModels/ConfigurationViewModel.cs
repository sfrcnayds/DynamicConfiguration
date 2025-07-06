using System.ComponentModel.DataAnnotations;

namespace DynamicConfiguration.Web.ViewModels
{
    public class ConfigurationViewModel
    {
        public string Id { get; set; }

        [Required] public string Name { get; set; }

        [Required] public string Type { get; set; }

        [Required] public string Value { get; set; }

        [Required] public string ApplicationName { get; set; }
        
        [Required] public int Version { get; set; }

        public bool IsActive { get; set; }
    }
}