using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWrapper
{
    public class Pages
    {
        public static BaseURL BaseURL
        {
            get
            {
                var logInPage = new BaseURL();
                return logInPage;
            }
        }
    }


    public class BaseURL
    {
        public static string title;
        public static string url;

        public void Goto(string url)
        {
            Browser.Goto(url);
        }

        public static void Quit()
        {
            if (Browser.webDriver == null) return;

            Browser.webDriver.Quit();
        }

        public bool IsAt(string title)
        {
            return Browser.Title == title;
        }


        }

    }

