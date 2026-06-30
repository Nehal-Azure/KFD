using Microsoft.Playwright;
namespace TestProject2.Pages;

public class LoginPage : BasePage
{
    public LoginPage(IPage page) : base(page)
    {
    }
 
    protected override IReadOnlyDictionary<string, string> ElementMap => new Dictionary<string, string>
    {
        ["UsernameInput"] = "#user-name",
        ["PasswordInput"] = "#password",
        ["LoginButton"] = "#login-button",
        ["ErrorMessage"] = "[data-test='error']"
    };
    
}