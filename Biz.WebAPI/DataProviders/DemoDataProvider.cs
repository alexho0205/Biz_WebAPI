
using Biz.WebAPI.DBContext;
using Biz.WebAPI.Models;
using Dapper;

namespace Biz.WebAPI.DataProviders;

public class DemoDataProvider
{
    private ILogger<DemoDataProvider> _logger;
    private OracleDbContext _oracleDbContext;

    public DemoDataProvider(ILogger<DemoDataProvider> logger, OracleDbContext oracleDbContext)
    {
        _logger = logger;
        _oracleDbContext = oracleDbContext;
    }

    /// <summary>
    ///  Dapper - Query For List.
    /// </summary>
    internal List<int> QueryForList()
    {
        List<int> datas = new List<int>();
        try
        {
            using (var conn = _oracleDbContext.GetConnection())
            {
                datas = conn.Query<int>(@"
                SELECT LEVEL AS num 
                    FROM DUAL 
                    CONNECT BY LEVEL <= 5
                ").ToList();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Get Datas Error!");
        }
        return datas;
    }

    /// <summary>
    ///  Dapper - Query For Models.
    /// </summary>
    internal List<DemoModel> QueryForModels()
    {
        List<DemoModel> models = new List<DemoModel>();
        try
        {
            using (var conn = _oracleDbContext.GetConnection())
            {
                models = conn.Query<DemoModel>(@"
                SELECT LEVEL AS num , SYSDATE  AS NOW 
                    FROM DUAL 
                    CONNECT BY LEVEL <= 5
                ").ToList();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Get Datas Error!");
        }
        return models;
    }


}