using PaymentGateway.Api.Models;

namespace PaymentGateway.Api.Tests.TestFixtures;

public class BankResponseBuilder
{
    private bool _authorized = true;
    private string? _authorizationCode = "AUTH123";

    public static BankResponseBuilder CreateDefault() => new();

    public static BankResponseBuilder CreateAuthorized() => 
        new BankResponseBuilder().WithAuthorized(true).WithAuthorizationCode("AUTH123");

    public static BankResponseBuilder CreateDeclined() => 
        new BankResponseBuilder().WithAuthorized(false).WithAuthorizationCode(null);

    public BankResponseBuilder WithAuthorized(bool authorized)
    {
        _authorized = authorized;
        return this;
    }

    public BankResponseBuilder WithAuthorizationCode(string? code)
    {
        _authorizationCode = code;
        return this;
    }

    public BankResponse Build()
    {
        return new BankResponse
        {
            Authorized = _authorized,
            AuthorizationCode = _authorizationCode!
        };
    }
}


