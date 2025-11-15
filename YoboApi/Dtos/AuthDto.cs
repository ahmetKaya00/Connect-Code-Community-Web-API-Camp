namespace YoboApi.Dtos;

public class RegisterRequest
{
    public string Email { get; set;} = default!;
    public string Password { get; set;} = default!;
    public string? FullName { get; set;}
}

public class LoginRequest
{
    public string Email { get; set;} = default!;
    public string Password { get; set;} = default!;
}

public class AuthResponse
{
    public string Token {get; set;} = default!;
    public DateTime ExpiresAt {get;set;}
    public string Email {get; set;} = default!;
    public string? FullName{ get; set;}
}