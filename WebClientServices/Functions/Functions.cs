using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebClientServices
{
    public class Functions
    {
        public static async Task<string> GetFinalHtml(string url, int sectionNum)
        {
            //Do not start the chrome window
            /*ChromeOptions options = new ChromeOptions();
            options.AddArgument("headless");

            //Close the Chrome Driver console
            ChromeDriverService driverService = ChromeDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;*/


            ChromeDriver driver = new ChromeDriver();

             driver.Navigate().GoToUrl(url);

            /* title = driver.Title;
            Console.WriteLine($"Title: {title}");
            //Scroll the page to the bottom
            Console.Write("Page scrolling, please wait a moment");

            for (int i = 1; i <= sectionNum; i++)
            {
                string jsCode = "window.scrollTo({top: document.body.scrollHeight / " + sectionNum + " * " + i + ", behavior: \"smooth\"});";
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                js.ExecuteScript(jsCode);
                Console.Write(".");
                Thread.Sleep(1000);
            }
            Console.WriteLine();*/

            var html =driver.PageSource;
            driver.Quit();

            return html;
        }

        public static async Task<string> WebRequestHtml(string url)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault;
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            string resultHtml = "";

                #pragma warning disable SYSLIB0014 // Type or member is obsolete
            WebRequest request = WebRequest.Create(url);
                #pragma warning restore SYSLIB0014 // Type or member is obsolete

            ((HttpWebRequest)request).UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.4758.82 Safari/537.36";

            WebResponse response = request.GetResponse();
            await using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string line = "";
                    while ((line = reader.ReadLine()) != null)
                    {
                        resultHtml = line;
                    }
                }
            }
            response.Close();
            return resultHtml;
        }
    }
}
