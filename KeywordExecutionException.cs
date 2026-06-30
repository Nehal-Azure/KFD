namespace TestProject2.Core;
 
/// <summary>
/// Wraps any exception thrown while executing a step with enough context
/// (step number, keyword, target element) to debug a CSV-driven failure without
/// having to open the framework source code.
/// </summary>
public class KeywordExecutionException : Exception
{
    public TestStep Step { get; }
 
    public KeywordExecutionException(TestStep step, Exception inner)
        : base(
            $"Step {step.StepOrder} failed - Keyword='{step.Keyword}' " +
            $"Target='{step.PageObject}.{step.Element}' Value='{step.Value}': {inner.Message}",
            inner)
    {
        Step = step;
    }
    
}