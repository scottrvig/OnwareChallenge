using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using health_path.Model;

namespace health_path.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScheduleController : ControllerBase
{
    private readonly ILogger<ScheduleController> _logger;
    private readonly IDbConnection _connection;

    public ScheduleController(ILogger<ScheduleController> logger, IDbConnection connection)
    {
        _logger = logger;
        _connection = connection;
    }

    [HttpGet]
    public ActionResult<IEnumerable<ScheduleEvent>> Fetch()
    {
        var dbResults = ReadData();

        // First get distinct events
        var preparedResults = dbResults.GroupBy(p => p.Item1.Id)
                                       .Select(grp => grp.FirstOrDefault().Item1);
        
        // Add events that match the event ID
        preparedResults = preparedResults.Select(p => {
            p.Recurrences.AddRange(dbResults.Where(d => d.Item2.EventId == p.Id).Select(d => d.Item2));
            return p;
        });

        return Ok(preparedResults);
    }

    private IEnumerable<(ScheduleEvent, ScheduleEventRecurrence)> ReadData() {
        var sql = @"
            SELECT e.*, r.*
            FROM Event e
            JOIN EventRecurrence r ON e.Id = r.EventId
            ORDER BY e.Id, r.DayOfWeek, r.StartTime, r.EndTime
        ";
        return _connection.Query<ScheduleEvent, ScheduleEventRecurrence, (ScheduleEvent, ScheduleEventRecurrence)>(sql, (e, r) => (e, r));
    }
}
