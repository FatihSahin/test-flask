﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFlask.Aspects.Config
{
    public interface ITestFlaskConfig
    {
        ProjectConfig Project { get; }
        ApiConfig Api { get; }
    }

    public class TestFlaskConfig : ITestFlaskConfig
    {
        private static TestFlaskConfig instance;
        public static TestFlaskConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    var section = ConfigurationManager.GetSection("testFlaskConfig/testFlask") as TestFlaskConfigSection;

                    instance = new TestFlaskConfig
                    {
                        Api = new ApiConfig
                        {
                            Url = section.Api.Url
                        },
                        Project = new ProjectConfig
                        {
                            Key = section.Project.Key
                        }
                    };
                }

                return instance;
            }
        }

        public ProjectConfig Project { get; private set; }

        public ApiConfig Api { get; private set; }
    }

    public class ProjectConfig
    {
        public string Key { get; set; }
    }

    public class ApiConfig
    {
        public string Url { get; set; }
    }
}
