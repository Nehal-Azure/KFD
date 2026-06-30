using Microsoft.Playwright;
namespace TestProject2.Pages;

public class DashboardPage : BasePage
{
    public DashboardPage(IPage page) : base(page)
    {
    }
    protected override IReadOnlyDictionary<string, string> ElementMap => new Dictionary<string, string>
    {
        ["PageTitle"] = ".title",
        ["InventoryList"] = ".inventory_list",
        ["CartIcon"] = ".shopping_cart_link",
        ["BurgerMenuButton"] = "#react-burger-menu-btn",
        ["LogoutLink"] = "#logout_sidebar_link"
    };
    
}