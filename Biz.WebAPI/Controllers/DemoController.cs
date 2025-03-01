using Biz.WebAPI.Models;
using Biz.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace Biz.WebAPI.Controllers;

[Route("Demo/[action]")]
[ApiController]
public class DemoController : ControllerBase
{
    private readonly ILogger<DemoController> _logger;
    private DemoService _demoService;
    public DemoController(ILogger<DemoController> logger, DemoService demoService)
    {
        _logger = logger;
        _demoService = demoService;
    }

    [HttpGet(Name = "QueryForList")]

    public IEnumerable<int> QueryForList()
    {
        _logger.LogInformation("!!!!!!!!!QueryForList!!!!!!!!");
        return _demoService.QueryForList();
    }


    [HttpPost(Name = "QueryForModels")]
    public List<DemoModel> QueryForModels()
    {
        _logger.LogInformation("!!!!!!!!!QueryForModels!!!!!!!!");
        return _demoService.QueryForModels();
    }
}