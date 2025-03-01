
using Oracle.ManagedDataAccess.Client;

namespace Biz.WebAPI.DBContext;

public class OracleDbContext
{
    private ILogger<OracleDbContext> _log;
    private IConfiguration _config;

    public OracleDbContext(ILogger<OracleDbContext> log, IConfiguration config)
    {
        _log = log;
        _config = config;
    }

    internal OracleConnection GetConnection()
    {
        return new OracleConnection(_config["ConnectionStrings:HAND_SIDE_DB"]);
    }



}