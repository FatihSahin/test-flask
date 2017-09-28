using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFlask.Assistant.Config
{
    public class TestFlaskAssistantConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("enabled")]
        public bool Enabled
        {
            get
            {
                return (bool)this["enabled"];
            }
            set
            {
                this["enabled"] = value;
            }
        }

        [ConfigurationProperty("api")]
        public TestFlaskApiElement Api
        {
            get
            {
                return (TestFlaskApiElement)this["api"];
            }
            set
            {
                this["api"] = value;
            }
        }

        [ConfigurationProperty("manager")]
        public TestFlaskManagerElement Manager
        {
            get
            {
                return (TestFlaskManagerElement)this["manager"];
            }
            set
            {
                this["manager"] = value;
            }
        }

        [ConfigurationProperty("project")]
        public ProjectElement Project
        {
            get
            {
                return (ProjectElement)this["project"];
            }
            set
            {
                this["project"] = value;
            }
        }
    }

    public class TestFlaskApiElement : ConfigurationElement
    {
        [ConfigurationProperty("url", IsRequired = true)]
        public string Url
        {
            get
            {
                return (string)this["url"];
            }
            set
            {
                this["url"] = value;
            }
        }
    }

    public class TestFlaskManagerElement : ConfigurationElement
    {
        [ConfigurationProperty("url", IsRequired = true)]
        public string Url
        {
            get
            {
                return (string)this["url"];
            }
            set
            {
                this["url"] = value;
            }
        }
    }

    public class ProjectElement : ConfigurationElement
    {
        [ConfigurationProperty("key", IsRequired = true)]
        public string Key
        {
            get
            {
                return (string)this["key"];
            }
            set
            {
                this["key"] = value;
            }
        }
    }
}
