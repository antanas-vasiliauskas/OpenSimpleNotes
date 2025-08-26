using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace OSN.Test.E2E.PageObjects;

public class LoginPage
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;

    private const string GuestLoginButtonXPath = "//button[text()='Continue as Guest']";
    private const string SignInTitleXPath = "//h1[text()='Sign In']";

    public LoginPage(IWebDriver driver, WebDriverWait wait)
    {
        _driver = driver;
        _wait = wait;
    }

    public void ClickGuestLogin()
    {
        var element = _wait.Until(d => d.FindElement(By.XPath(GuestLoginButtonXPath)));
        _wait.Until(d => element.Displayed && element.Enabled);
        element.Click();
    }

    public bool IsLoginPageDisplayed()
    {
        try
        {
            return _wait.Until(d => d.FindElements(By.XPath(SignInTitleXPath)).Count > 0);
        }
        catch
        {
            return false;
        }
    }
}