using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace OSN.Test.E2E.Infrastructure;

public class WebDriverFactory : IDisposable
{
    private IWebDriver? _driver;

    public IWebDriver GetDriver()
    {
        if (_driver == null)
        {
            var options = new ChromeOptions();
            options.AddArguments("--no-sandbox");
            options.AddArguments("--disable-dev-shm-usage");
            options.AddArguments("--disable-gpu");
            options.AddArguments("--window-size=1920,1080");
            options.AddArguments("--disable-web-security");
            options.AddArguments("--allow-running-insecure-content");
            
            var chromeDriverPath = TestConfiguration.ChromeDriverPath;
            _driver = new ChromeDriver(chromeDriverPath, options);
            _driver.Manage().Window.Maximize();

            var implicitWaitTimeout = TimeSpan.FromSeconds(TestConfiguration.ImplicitWaitTimeoutSeconds);
            _driver.Manage().Timeouts().ImplicitWait = implicitWaitTimeout;
        }

        return _driver;
    }

    public void Dispose()
    {
        _driver?.Quit();
        _driver?.Dispose();
    }
}