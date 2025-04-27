namespace MonkeyShelter.Application.Exceptions;

/// <summary>
/// Thrown when a business rule is violated in the domain or application layer.
/// </summary>
public class BusinessRuleException : Exception
{
    /// <summary>
    /// Optional identifier for the specific rule that was violated.
    /// </summary>
    public string? RuleName { get; }

    /// <summary>
    /// Creates a new business rule exception with a custom message.
    /// </summary>
    /// <param name="message">Description of the violated rule.</param>
    public BusinessRuleException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Creates a new business rule exception with a rule name and message.
    /// </summary>
    /// <param name="ruleName">Identifier of the violated rule.</param>
    /// <param name="message">Description of the violated rule.</param>
    public BusinessRuleException(string ruleName, string message)
        : base(message)
    {
        RuleName = ruleName;
    }
}