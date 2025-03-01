using Biz.WebAPI.DataProviders;
using Biz.WebAPI.Models;

namespace Biz.WebAPI.Services;
public class DemoService
{

    private readonly ILogger<DemoService> _log;
    private DemoDataProvider _demoDataProvider;

    public DemoService(ILogger<DemoService> logger, DemoDataProvider demoDataProvider)
    {
        _log = logger;
        _demoDataProvider = demoDataProvider;
    }

    public List<int> QueryForList()
    {
        List<int> datas = _demoDataProvider.QueryForList();
        return datas;
    }

    
    public List<DemoModel> QueryForModels()
    {
        List<DemoModel> models = _demoDataProvider.QueryForModels();
        return models;
    }


}