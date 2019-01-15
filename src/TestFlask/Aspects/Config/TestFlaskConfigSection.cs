using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFlask.Aspects.Config
{
    public class TestFlaskConfigSection : ConfigurationSection
    {
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
