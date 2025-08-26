using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;

namespace OSN.Test.E2E.PageObjects;

public class NotesPage
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;

    private const string NewNoteButtonXPath = "//button[text()='New Note']";
    private const string TitleInputXPath = "//input[@placeholder='Title']";
    private const string ContentDivXPath = "//div[@contenteditable='true']";
    private const string ProfileButtonXPath = "//span[text()='My Profile']/ancestor::div[@role='button']";
    private const string NoteItemsXPath = "//li[contains(@class,'MuiListItem-root')]";
    private const string NoteTitleRelativeXPath = ".//span[contains(@class,'MuiListItemText-primary')]";
    private const string DeleteOptionXPath = "//li[text()='Delete']";
    private const string ConfirmDeleteButtonXPath = "//button[text()='Delete']";
    private const string LogoutOptionXPath = "//span[text()='Logout']/ancestor::li[@role='menuitem']";
    private const string ProfileSectionXPath = "//span[text()='My Profile']";
    private const string NoteByTitleXPath = "//span[text()='{0}']/ancestor::li";
    private const string OverflowMenuByTitleXPath = "//span[text()='{0}']/ancestor::li//button[contains(@class,'MuiIconButton-root')]";

    public NotesPage(IWebDriver driver, WebDriverWait wait)
    {
        _driver = driver;
        _wait = wait;
    }

    public void ClickNewNote()
    {
        var element = _wait.Until(d => d.FindElement(By.XPath(NewNoteButtonXPath)));
        _wait.Until(d => element.Displayed && element.Enabled);
        element.Click();
    }

    public void SetTitle(string title)
    {
        var titleInput = _wait.Until(d => d.FindElement(By.XPath(TitleInputXPath)));
        _wait.Until(d => titleInput.Displayed && titleInput.Enabled);
        titleInput.Clear();
        titleInput.SendKeys(title);
    }

    public void SetContent(string content)
    {
        var contentDiv = _wait.Until(d => d.FindElement(By.XPath(ContentDivXPath)));
        _wait.Until(d => contentDiv.Displayed && contentDiv.Enabled);
        contentDiv.Clear();
        contentDiv.SendKeys(content);
    }

    public void ClickNoteInSidebar(int index)
    {
        var noteItems = _wait.Until(d => d.FindElements(By.XPath(NoteItemsXPath)));
        if (index < noteItems.Count)
        {
            var element = noteItems[index];
            _wait.Until(d => element.Displayed && element.Enabled);
            element.Click();
        }
    }

    public void ClickNoteInSidebarByTitle(string title)
    {
        var xpath = string.Format(NoteByTitleXPath, title);
        var element = _wait.Until(d => d.FindElement(By.XPath(xpath)));
        _wait.Until(d => element.Displayed && element.Enabled);
        element.Click();
    }

    public void ClickOverflowMenuForNoteByTitle(string title)
    {
        var noteXPath = string.Format(NoteByTitleXPath, title);
        var noteElement = _wait.Until(d => d.FindElement(By.XPath(noteXPath)));
        
        new Actions(_driver).MoveToElement(noteElement).Perform();
        
        var overflowXPath = string.Format(OverflowMenuByTitleXPath, title);
        var element = _wait.Until(d => d.FindElement(By.XPath(overflowXPath)));
        _wait.Until(d => element.Displayed && element.Enabled);
        element.Click();
    }

    public void ClickDeleteOption()
    {
        var element = _wait.Until(d => d.FindElement(By.XPath(DeleteOptionXPath)));
        _wait.Until(d => element.Displayed && element.Enabled);
        element.Click();
    }

    public void ConfirmDelete()
    {
        var element = _wait.Until(d => d.FindElement(By.XPath(ConfirmDeleteButtonXPath)));
        _wait.Until(d => element.Displayed && element.Enabled);
        element.Click();
    }

    public void Logout()
    {
        var profileButton = _wait.Until(d => d.FindElement(By.XPath(ProfileButtonXPath)));
        _wait.Until(d => profileButton.Displayed && profileButton.Enabled);
        profileButton.Click();
        
        var logoutOption = _wait.Until(d => d.FindElement(By.XPath(LogoutOptionXPath)));
        _wait.Until(d => logoutOption.Displayed && logoutOption.Enabled);
        logoutOption.Click();
    }

    public int GetNoteCount()
    {
        try
        {
            return _wait.Until(d => d.FindElements(By.XPath(NoteItemsXPath))).Count;
        }
        catch
        {
            return 0;
        }
    }

    public string GetCurrentTitle()
    {
        var element = _wait.Until(d => d.FindElement(By.XPath(TitleInputXPath)));
        _wait.Until(d => element.Displayed);
        return element.GetAttribute("value") ?? "";
    }

    public string GetCurrentContent()
    {
        var element = _wait.Until(d => d.FindElement(By.XPath(ContentDivXPath)));
        _wait.Until(d => element.Displayed);
        return element.Text ?? "";
    }

    public List<string> GetNoteTitlesFromSidebar()
    {
        var noteItems = _wait.Until(d => d.FindElements(By.XPath(NoteItemsXPath)));
        return noteItems.Where(item => item.Displayed)
            .Select(item => item.FindElement(By.XPath(NoteTitleRelativeXPath)).Text)
            .Where(text => !string.IsNullOrEmpty(text) && text != "Untitled Note")
            .ToList();
    }

    public void WaitForSidebarTitleUpdate(string expectedTitle, TimeSpan? timeout = null)
    {
        var wait = timeout.HasValue ? new WebDriverWait(_driver, timeout.Value) : _wait;
        wait.Until(d => GetNoteTitlesFromSidebar().Contains(expectedTitle));
    }

    public void WaitForNoteCountUpdate(int expectedCount)
    {
        _wait.Until(d => GetNoteCount() == expectedCount);
    }

    public bool IsNotesPageDisplayed()
    {
        try
        {
            var newNoteButtons = _wait.Until(d => d.FindElements(By.XPath(NewNoteButtonXPath)));
            var profileSections = _wait.Until(d => d.FindElements(By.XPath(ProfileSectionXPath)));
            return newNoteButtons.Count > 0 && profileSections.Count > 0 && 
                   newNoteButtons[0].Displayed && profileSections[0].Displayed;
        }
        catch
        {
            return false;
        }
    }
}