using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace LTEReboot
{
    class Program
    {
        static ILogger logger;

        static Dictionary<string, string> LogEntries { get; set; } = new Dictionary<string, string>();

        static void Main(string[] args)
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("LTEReboot.Program", LogLevel.Debug)
                    .AddConsole()
                    .AddEventLog();
            });
            logger = loggerFactory.CreateLogger<Program>();

            if (args.Length > 0)
            {
                string url = args[0];
                string password = args[1];
                string ipToPing = args[2];
                IPAddress ip;
                System.Uri uriResult;

                if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(ipToPing) && IPAddress.TryParse(ipToPing, out ip) && System.Uri.TryCreate(url, System.UriKind.Absolute, out uriResult))
                {
                    LogEntries.Add("Starting IP-Probe: ", ipToPing);
                    // Use Task class to start and wait for a Ping.
                    if (!Test(ipToPing))
                    {
                        LogEntries.Add("Starting Restart: ", url);
                        Restart(url, password);
                    }
                    string log = string.Empty;
                    foreach (var item in LogEntries) {
                        log += item.Key + item.Value + Environment.NewLine;
                            }
                    logger.LogInformation(log);
                    return;
                }
            }

            Console.WriteLine("Syntax: LTEReboot <RouterURL> <Password> <ProbeIP>");

        }

        static void Restart(string url, string password)
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                try
                {
                    LogEntries.Add("Loading URL: ", url);
                    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                    WebDriverWait wait = new WebDriverWait(driver, System.TimeSpan.FromSeconds(10));
                    driver.Navigate().GoToUrl(url);
                    try
                    {
                        driver.FindElement(By.Id("txtLoginPass")).SendKeys(password + Keys.Enter);
                    }
                    catch (Exception)
                    {
                    }
                    Thread.Sleep(2000);
                    js.ExecuteScript("LoadMain('other_reboot.cgi')");
                    Thread.Sleep(10000);
                    driver.SwitchTo().Frame(driver.FindElement(By.ClassName("mainIframe")));
                    driver.FindElement(By.Id("btnApply")).Click();
                    Thread.Sleep(2000);
                    driver.SwitchTo().ParentFrame();
                    driver.FindElement(By.ClassName("cOk")).Click();
                    Thread.Sleep(30000);
                }
                catch (Exception)
                {
                }
                driver.Close();
            }
        }

        static bool Test(string ipToPing)
        {
            // Create Ping instance.
            Ping ping = new Ping();
            // Send a ping.
            PingReply reply = ping.SendPingAsync(ipToPing).Result;
            if (reply.Status == IPStatus.DestinationHostUnreachable)
            {
                return false;
            }
            // Display the result.
            LogEntries.Add("ADDRESS:", reply.Address.ToString());
            LogEntries.Add("TIME:", reply.RoundtripTime.ToString());

            return true;
        }
    }
}
