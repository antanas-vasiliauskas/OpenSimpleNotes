using OSN.Test.E2E.Infrastructure;
using OSN.Test.E2E.PageObjects;

namespace OSN.Test.E2E.Tests;

public class NotesE2ETests : BaseTest
{
    private LoginPage _loginPage;
    private NotesPage _notesPage;

    public NotesE2ETests()
    {
        _loginPage = new LoginPage(Driver, Wait);
        _notesPage = new NotesPage(Driver, Wait);
    }

    [Fact]
    public void CompleteE2EWorkflow()
    {
        AnonymousSignIn();
        NotesCrud();
        NotesPersist();
    }

    private void AnonymousSignIn()
    {
        ClearLocalStorage();
        NavigateToApp();

        _loginPage.IsLoginPageDisplayed().Should().BeTrue();
        _loginPage.ClickGuestLogin();
        
        _notesPage.IsNotesPageDisplayed().Should().BeTrue();
    }

    private void NotesCrud()
    {
        for (int i = 0; i < 3; i++)
        {
            _notesPage.ClickNewNote();
            _notesPage.WaitForNoteCountUpdate(i+1);
        }

        _notesPage.GetNoteCount().Should().Be(3);

        _notesPage.ClickNoteInSidebar(0);
        _notesPage.GetCurrentTitle().Should().Be("New note");
        _notesPage.GetCurrentContent().Should().BeEmpty();

        _notesPage.SetTitle("Test Title");
        _notesPage.WaitForSidebarTitleUpdate("Test Title", TimeSpan.FromSeconds(5));
        _notesPage.GetNoteTitlesFromSidebar().Should().Contain("Test Title");

        _notesPage.SetContent("This is test content for the first note");

        _notesPage.ClickNoteInSidebar(1);
        _notesPage.SetTitle("Test Title 2");
        _notesPage.SetContent("This is content for the second note");

        _notesPage.ClickNoteInSidebar(0);
        _notesPage.GetCurrentTitle().Should().Be("Test Title");
        _notesPage.GetCurrentContent().Should().Contain("This is test content for the first note");

        _notesPage.ClickNoteInSidebar(2);
        _notesPage.SetTitle("Test Title 3");
        _notesPage.SetContent("Content for third note");

        _notesPage.ClickOverflowMenuForNoteByTitle("Test Title 2");
        _notesPage.ClickDeleteOption();
        _notesPage.ConfirmDelete();
        _notesPage.WaitForNoteCountUpdate(2);
        
        _notesPage.GetNoteCount().Should().Be(2);
        var remainingTitles = _notesPage.GetNoteTitlesFromSidebar();
        remainingTitles.Should().Contain("Test Title");
        remainingTitles.Should().Contain("Test Title 3");
        remainingTitles.Should().NotContain("Test Title 2");
    }

    private void NotesPersist()
    {
        _notesPage.Logout();
        _loginPage.IsLoginPageDisplayed().Should().BeTrue();

        _loginPage.ClickGuestLogin();
        _notesPage.IsNotesPageDisplayed().Should().BeTrue();

        var titlesAfterRelogin = _notesPage.GetNoteTitlesFromSidebar();
        titlesAfterRelogin.Should().Contain("Test Title");
        titlesAfterRelogin.Should().Contain("Test Title 3");
        titlesAfterRelogin.Count.Should().Be(2);

        _notesPage.ClickNoteInSidebarByTitle("Test Title 3");
        _notesPage.GetCurrentTitle().Should().Be("Test Title 3");
        _notesPage.GetCurrentContent().Should().Contain("Content for third note");
    }
}