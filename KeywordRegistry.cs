using System.Reflection;
using Microsoft.Playwright;
namespace TestProject2.Core;

 
/// <summary>
/// Builds a Keyword name -> implementation lookup by scanning the assembly once,
/// at startup, for every static method tagged [Keyword("...")].
///
/// This is what makes the framework "keyword-driven": the CSV file only ever
/// needs to know the keyword's *name*. Where the implementation actually lives
/// (which static class, which file) is irrelevant to the test data - new keywords
/// are added purely by writing code in Keywords/, never by touching this class.
/// </summary>
public class KeywordRegistry
{
    
    private static readonly Dictionary<string, Func<IPage, TestStep, Task>> Keywords = Build();
 
    public static IReadOnlyCollection<string> AllKeywords => Keywords.Keys;
 
    public static bool IsRegistered(string keyword) => Keywords.ContainsKey(keyword);
 
    public static Func<IPage, TestStep, Task> Resolve(string keyword)
    {
        if (Keywords.TryGetValue(keyword, out var implementation))
        {
            return implementation;
        }
 
        throw new KeyNotFoundException(
            $"Keyword '{keyword}' is not implemented. Available keywords: " +
            string.Join(", ", Keywords.Keys.OrderBy(k => k)));
    }
 
    private static Dictionary<string, Func<IPage, TestStep, Task>> Build()
    {
        var map = new Dictionary<string, Func<IPage, TestStep, Task>>(StringComparer.OrdinalIgnoreCase);
 
        var taggedMethods = Assembly.GetExecutingAssembly()
            .GetTypes()
            .SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.Static))
            .Select(method => (method, attribute: method.GetCustomAttribute<KeywordAttribute>()))
            .Where(x => x.attribute is not null);
 
        foreach (var (method, attribute) in taggedMethods)
        {
            var name = attribute!.Name;
 
            if (map.ContainsKey(name))
            {
                throw new InvalidOperationException(
                    $"Duplicate keyword registration: '{name}' is implemented by more than one method. " +
                    "Keyword names must be unique across the whole assembly.");
            }
 
            ValidateSignature(method, name);
 
            map[name] = (page, step) => (Task)method.Invoke(null, new object[] { page, step })!;
        }
 
        return map;
    }
 
    private static void ValidateSignature(MethodInfo method, string keywordName)
    {
        var parameters = method.GetParameters();
        var validSignature =
            parameters.Length == 2 &&
            parameters[0].ParameterType == typeof(IPage) &&
            parameters[1].ParameterType == typeof(TestStep) &&
            typeof(Task).IsAssignableFrom(method.ReturnType);
 
        if (!validSignature)
        {
            throw new InvalidOperationException(
                $"Keyword '{keywordName}' on {method.DeclaringType?.Name}.{method.Name} has an invalid signature. " +
                "Expected: public static Task MethodName(IPage page, TestStep step)");
        }
    }
    
}