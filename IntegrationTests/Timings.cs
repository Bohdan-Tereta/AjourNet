using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace IntegrationTests
{
    public static class Timings
    {
        static Timings()
        {
            Default_ms = Int32.Parse(WebConfigurationManager.AppSettings["Default_ms"] );   
        } 

        public static int Default_ms { get; private set; }
    }
}
