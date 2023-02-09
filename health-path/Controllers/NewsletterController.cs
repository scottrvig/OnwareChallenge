using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace health_path.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NewsletterController : ControllerBase
{
    private readonly ILogger<NewsletterController> _logger;
    private readonly IDbConnection _connection;

    public NewsletterController(ILogger<NewsletterController> logger, IDbConnection connection)
    {
        _logger = logger;
        _connection = connection;
    }

    [HttpPost]
    public ActionResult Subscribe(string Email)
    {
        // First we should Trim() and ToLower() the email to remove case-insensitive duplication and issues with spaces
        Email = Email.ToLower().Trim();

        if (Email.EndsWith("@gmail.com")) {
            // Gmail ignores periods prior to the '@' so we should trim periods off 
            // Note we only care about Gmail, other email providers don't ignore periods

            // First get the address without the Gmail domain
            var emailWithoutDomain = Email.Split("@").First();
            
            // Replace period character with empty string and restore the Gmail domain
            Email = emailWithoutDomain.Replace(".", "") + "@gmail.com";
        }

        var inserted = _connection.Execute(@"
            INSERT INTO NewsletterSubscription (Email)
            SELECT *
            FROM ( VALUES (@Email) ) AS V(Email)
            WHERE NOT EXISTS ( SELECT * FROM NewsletterSubscription e WHERE e.Email = v.Email )
        ", new { Email = Email });

        return inserted == 0 ? Conflict("email is already subscribed") : Ok();
    }
}
