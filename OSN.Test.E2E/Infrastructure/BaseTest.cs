using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace OSN.Test.E2E.Infrastructure;

public abstract class BaseTest : IDisposable
{
    protected readonly IWebDriver Driver;
    protected readonly WebDriverWait Wait;
    protected readonly string BaseUrl;

    protected BaseTest()
    {
        var driverFactory = new WebDriverFactory();
        Driver = driverFactory.GetDriver();
        
        var waitTimeout = TimeSpan.FromSeconds(TestConfiguration.ExplicitWaitTimeoutSeconds);
        Wait = new WebDriverWait(Driver, waitTimeout);
        
        BaseUrl = TestConfiguration.BaseUrl;
    }

    protected void NavigateToApp()
    {
        Driver.Navigate().GoToUrl(BaseUrl);
    }

    protected void ClearLocalStorage()
    {
        try
        {
            ((IJavaScriptExecutor)Driver).ExecuteScript("localStorage.clear();");
            ((IJavaScriptExecutor)Driver).ExecuteScript("sessionStorage.clear();");
        }
        catch (Exception)
        {
            // Ignore if localStorage is not available
        }
    }

    public virtual void Dispose()
    {
        Driver?.Quit();
        Driver?.Dispose();
    }
}