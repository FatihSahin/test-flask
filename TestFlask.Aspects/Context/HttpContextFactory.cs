using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TestFlask.Aspects.Context
{
    public class HttpContextFactory
    {
        private static HttpContextBase context;
        public static HttpContextBase Current
        {
            get
            {
                if (context != null)
                    return context;

                if (HttpContext.Current == null)
                    throw new InvalidOperationException("HttpContext not available");

                return new HttpContextWrapper(HttpContext.Current);
            }
            set
            {
                context = value;
            }
        }
    }
}
