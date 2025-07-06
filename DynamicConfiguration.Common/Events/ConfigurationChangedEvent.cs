using System;

namespace DynamicConfiguration.Common.Events
{
    public class ConfigurationChangedEvent
    {
        public string ApplicationName { get; set; }
        
        public string Action { get; set; }
        
        public DateTime Timestamp { get; set; }
    }
}