using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFlask.Aspects.ApiClient
{
    public static class TestFlaskApiFactory
    {
        private static ITestFlaskApi testFlaskApi;
        public static ITestFlaskApi TestFlaskApi
        {
            get
            {
                if (testFlaskApi != null)
                {
                    return testFlaskApi;
                }

                return new TestFlaskApi();
            }
            set
            {
                testFlaskApi = value;
            }
        }
    }
}
