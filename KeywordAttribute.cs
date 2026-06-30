namespace TestProject2.Core;

/// <summary>
/// Marks a static method as the implementation of a keyword that can appear in the
/// "Keyword" column of a CSV test data file.
///
/// Convention every method must follow so the registry can call it generically:
///     [Keyword("EnterText")]
///     public static async Task EnterText(IPage page, TestStep step) { ... }
///
/// Adding a brand-new keyword to the framework means: write one method like this,
/// decorate it, and it is automatically discovered - nothing else to register.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class KeywordAttribute : Attribute
{
    
    public string Name { get; }
 
    public KeywordAttribute(string name)
    {
        Name = name;
    }
}