using System.Web.Mvc;

namespace TestFlask.Assistant.Areas.TestFlask
{
    public class TestFlaskAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "TestFlask";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                 "TestFlask_Default",
                 "TestFlask/{controller}/{action}/{id}",
                 new { controller = "Assistant", action = "Index", id = UrlParameter.Optional },
                 new string[] { "TestFlask.Assistant.Controllers" }
             );
        }
    }
}