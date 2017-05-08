using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;



namespace TestWrapper
{
   public static class Browser
    {
        static string chromeDriverPath = AppDomain.CurrentDomain.BaseDirectory;
        public static IWebDriver webDriver = new ChromeDriver(chromeDriverPath + "\\Resources");

        public static IWebDriver WebDriver
        {
            get { return webDriver ?? new ChromeDriver(chromeDriverPath + "\\Resources"); }
            set { webDriver = new ChromeDriver(chromeDriverPath + "\\Resources"); }
        }


        public static IWebElement ElementVisible(string path)
        {
             WebDriverWait wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(20));
            return wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(path)));
        }

        public static void Goto(string url)
        {
            webDriver.Navigate().GoToUrl(url);
        }


        public static string Title
        {
            get { return webDriver.Title; }
        }


        public static string Url
        {
            get { return webDriver.Url; }
        }


        public static void Quit()
        {
            if (webDriver != null) 
            webDriver.Quit();
        }


        public static bool IsAt(string title)
        {
            return webDriver.Title == title;
        }


        public static void SendKeysTo(string inputId, string value, bool clearFirst)
        {
            WebDriverWait wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(20));
            IWebElement element = wait.Until(ExpectedConditions.ElementIsVisible(By.Id(inputId)));
            if (clearFirst)
            {
                element.Clear();
            }

            element.SendKeys(value);
        }

        public static void SendEnter(string inputId)
        {
            IWebElement element = webDriver.FindElement(By.Id(inputId));
            webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(3));
            element.SendKeys(Keys.Enter); 
        }

        public static void SendEnter(IWebElement el)
        {
            webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(3));
            el.SendKeys(Keys.Enter);
        }

        public static void Wait(int seconds)
        {
            webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(seconds));
            }


        public static bool HasElement(string link)
        {

            try
        {
                webDriver.FindElement(By.LinkText(link));
                webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(3));

        }

            catch (NoSuchElementException)
        {
                return false;
        }

            return true;
        }

        public static bool HasElementByXPath(string path)
        {
            try
            {
               webDriver.FindElement(By.XPath(path));
                
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            return true;
        }

        public static string GetText(string xpath)
        {
            IWebElement el = webDriver.FindElement(By.XPath(xpath));
            ElementVisible(xpath);
            string text = el.Text;
            return text;
        }

        public static string GetText(string xpath, int index)
        {
            IWebElement el = webDriver.FindElements(By.XPath(xpath))[index];
            webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(3));
            string text = el.Text;
            return text;
        }


        public static void ClickOnLink(string text, int index = 0)
        {
            WebDriverWait wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(20));
            IWebElement el = wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText(text)));
            el = webDriver.FindElement(By.LinkText(text));
            if (index > 0)
                el = webDriver.FindElements(By.LinkText(text)).ToList()[index];
            webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
            el.Click();
        }


        public static void ClickByXPath(string text)
        {
            IWebElement el = ElementVisible(text);
             el.Click();
        }

        public static void SelectOption(IWebElement element, string value)
        {
            var selected = new SelectElement(element);
            selected.SelectByText(value);
        }

        public static void SelectOption(string id, int option)
        {
            var list = webDriver.FindElement(By.Id(id));
            webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(3));
            IWebElement el = list.FindElements(By.TagName("option"))[option];
            webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(3));
            el.Click();
        }


        public static void SelectOption(string id, string name)
        {
            var list = webDriver.FindElement(By.Id(id));
            webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(3));
            var selected = new SelectElement(list);
            selected.SelectByText(name);
        }


        public static void SelectByValue(string xpath, string value)
        {
            var list = webDriver.FindElement(By.XPath(xpath));
            webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(3));
            var selected = new SelectElement(list);
            selected.SelectByValue(value);
        }

        public static int Count(string path)
        {
            List <IWebElement> el = webDriver.FindElements(By.XPath(path)).ToList();
            webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(3));
            
            return   el.Count();
        }


        public static string IWebElementToString(string path, int index, string tagName, string tagAttribute = "")
        {
            List<IWebElement> list = webDriver.FindElements(By.XPath(path)).ToList();
            if (index > 0)
            {
                if (tagAttribute == "")
                    return list[index].FindElement(By.TagName(tagName)).Text;
                else
                    return list[index].FindElement(By.TagName(tagName)).GetAttribute(tagAttribute);

            }
            if (tagAttribute == "")
                return webDriver.FindElement(By.XPath(path)).FindElement(By.TagName(tagName)).Text;
            else
                return webDriver.FindElement(By.XPath(path)).FindElement(By.TagName(tagName)).GetAttribute(tagAttribute);

        }

        public static void SelectDatePickerValue(string date, int nextMounth)
        {

            if (nextMounth > 0)
            {
                for (int i = 0; i < nextMounth; i++)
                {
                    IWebElement next = webDriver.FindElement(By.XPath("//a[@data-handler='next']"));
                    next.Click();
                    i++;
                }
        }
            webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(3));

            IWebElement dateWidget = webDriver.FindElement(By.Id("ui-datepicker-div"));

            List<IWebElement> rows = dateWidget.FindElements(By.TagName("tr")).ToList();
            List<IWebElement> columns = dateWidget.FindElements(By.TagName("td")).ToList();
            WebDriverWait wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(20));

            foreach (IWebElement cell in columns.Where(c => !c.GetAttribute("class").Contains("week-col")))
            {
                if (cell.Text == date)
                {
                    cell.FindElement(By.LinkText(date));
                    wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText(date)));
                    cell.Click();
                    break;
                }
            }
        }

        public static string SelectedMonth(int nextMounth)
        {
            int month = DateTime.Now.AddMonths(nextMounth).Month;
            if (month < 10)
                return String.Format("0" + month.ToString());
            else return month.ToString();
        }

        public static ReadOnlyCollection<IWebElement> FindElementsByXPath(string xPath)
        {
            return webDriver.FindElements(By.XPath(xPath));
        }

       public static void ClickOnWebElement(IWebElement element)
       {
           element.Click();
       }

       public static WebDriverWait wait(int seconds)
       {
           return new WebDriverWait(webDriver, new TimeSpan(0, 0, seconds));
       }

       public static IWebElement FindElementByLinkText(string text)
       {
           return webDriver.FindElement(By.LinkText(text));
       }

       public static IWebElement FindElementByXPath(string xPath)
       {
           return webDriver.FindElement(By.XPath(xPath));
       }

       public static void SendKeysTo(IWebElement element, string value, bool clearFirst)
       {
           if (clearFirst)
           {
               element.Clear();
           }

           element.SendKeys(value);
       }

       public static void Refresh()
       {
           webDriver.Navigate().Refresh();
       }

       public static void ClickOnDatePicker()
       {
           IWebElement datepicker = webDriver.FindElement(By.XPath("//img[@class='ui-datepicker-trigger']"));
           datepicker.Click();

           Browser.wait(5);
           IWebElement todayDate = webDriver.FindElement(By.XPath("//td[contains(@class,' ui-datepicker-today')]/a"));
           todayDate.Click();
       }

       public static void WaitAjax()
       {
           if (WebDriver != null)
           {
               var ready = new Func<bool>(() => (bool)((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("return (typeof($) === 'undefined') ? true : !$.active;"));
               Contract.Assert(WaitHelper.SpinWait(ready, TimeSpan.FromSeconds(60), TimeSpan.FromMilliseconds(100)));
           }
       }

       public static bool SpinWait(Func<bool> condition, TimeSpan timeout, TimeSpan pollingInterval)
       {
           return WaitHelper.WithTimeout(timeout, pollingInterval).WaitFor(condition).IsSatisfied;
       }

       public static void SendKeys(IWebElement element,  string key)
       {
           element.SendKeys(key);
       }

       public static void Clear(IWebElement element)
       {
           element.Clear();
       }

       public static IWebElement FindElementByClassName(string text)
       {
           return webDriver.FindElement(By.ClassName(text));
       }

       public static ReadOnlyCollection <IWebElement> FindElementsByClassName(string text)
       {
           return webDriver.FindElements(By.ClassName(text));
       } 

       public static bool IsTabSelectedAndActive(string xPath)
       {
           string elementClass = webDriver.FindElement(By.XPath(xPath)).GetAttribute("class");
           webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(3));
           if (elementClass.Split(' ').Contains("ui-tabs-selected") && elementClass.Split(' ').Contains("ui-state-active"))
               return true;
           else
               return false; 
       }

       public static bool HasJQueryTab(string id)
       {
           string elementClass = webDriver.FindElement(By.Id(id)).GetAttribute("class");
           webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(3));
           if (elementClass.Contains("ui-tabs ui-widget ui-widget-content ui-corner-all"))
               return true;
           else
               return false;
       }

       public static string IsDropDownList(IWebElement dropDownList, string id, String[] values, String[] text, string selectedValue)
       {

           if (dropDownList == null)
               return "Element not found"; 
           if(id != dropDownList.GetAttribute("id"))
               return "Wrong id";
           SelectElement select = new SelectElement(dropDownList);
           if(values.Count() != select.Options.Count())
               return "Different number of options. Expected: " + values.Count() + "but was: " + values.Count();
           for (int i=0; i<select.Options.Count; i++)
           {
               if (select.Options[i].GetAttribute("value") != values[i])
                   return "Values of options are not equal. Expected: " + values[i] + " but was: " + select.Options[i].GetAttribute("value");
               if (select.Options[i].Text != text[i])
                   return "Options are not equal. Expected: " + text[i] + " but was: " + select.Options[i].GetAttribute("value"); 
           }
           if(selectedValue==select.SelectedOption.GetAttribute("value"))
               return "Selected valuesAre not Equal. Expected: " + selectedValue + "but was: " + select.SelectedOption.GetAttribute("value");
           return "True"; 
       }

       public static IWebElement FindElement(By by, int timeoutInSeconds)
       {
           if (timeoutInSeconds > 0)
           {
               var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(timeoutInSeconds));
               return wait.Until(drv => drv.FindElement(by));
           }
           return webDriver.FindElement(by);
       }

       public static ReadOnlyCollection<IWebElement> FindElements(By by, int timeoutInSeconds)
       {
           if (timeoutInSeconds > 0)
           {
               var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(timeoutInSeconds));
               return wait.Until(drv => drv.FindElements(by));
           }
           return webDriver.FindElements(by);
       } 

       public static string GetInnerHTML(IWebElement element)
       {
           return (String)((IJavaScriptExecutor)webDriver).ExecuteScript("return arguments[0].innerHTML;", element); 
       }
    }
}
