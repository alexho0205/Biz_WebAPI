using System.Data;
using Biz.WebAPI.DBContext;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

/// <summary>
/// 提供調用 Oracle Package. 依據實際情況調整 Class Name.
/// </summary>
public class HandSideDBProvider
{
    private ILogger<HandSideDBProvider> _logger;
    private OracleDbContext _oracleDbContext;

    public HandSideDBProvider(ILogger<HandSideDBProvider> logger, OracleDbContext oracleDbContext)
    {
        _logger = logger;
        _oracleDbContext = oracleDbContext;
    }

    ///
    /// 2025.3.1 alex_ho 調用 Oracle-Package.
    /// 
    /// 从各side取得数据发料单 (HAND → MES)
    /*
    public RequestContainerModeler GetSyncRequestContainerHandSide()
    {
        string inputData = string.Empty;
        string packageName = "biz_wo_for_sms_pck.request_container";
        string outdata = string.Empty;
        List<RequestContainerModeler.RequestContaineroutput> returnlist = _CallPkgReturnModels<RequestContainerModeler.RequestContaineroutput>(packageName, inputData, out outdata);
        RequestContainerModeler outdata1 = new RequestContainerModeler();
        outdata1.message = outdata;
        outdata1.output = returnlist;
        return outdata1;
    } */


    /// <summary>
    /// 调用 Oracle Package , 回传资料模型阵列.
    /// 当 package 回传讯息为 -1@ 打头, 执行 Transaction Rollback.
    /// 儲存 API Log.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="packageName">package name</param>
    /// <param name="inputData">json format string</param>
    /// <returns>资料模型阵列</returns>
    private List<T> _CallPkgReturnModels<T>(string packageName, string inputData, out string outmessage)
    {
        outmessage = "";
        string oraclePkgOutputString = "";
        List<T> outdata = null;
        //long SerialID = _apiLogDB.RetrieveSiemensApiSeq();
        //SiemensApiLogModel apiLog = new SiemensApiLogModel(SerialID, packageName, MEDIA_TYPE_JSON);
        try
        {
            using (OracleConnection db = _oracleDbContext.GetConnection())
            {
                try
                {
                    //db.Open();
                    OracleTransaction transaction = db.BeginTransaction(IsolationLevel.ReadCommitted);
                    List<OracleParameter> inputParameters = new List<OracleParameter>();
                    List<OracleParameter> outputParameters = new List<OracleParameter>();

                    //添加CLOB输入参数
                    OracleParameter clobInput = new OracleParameter();
                    clobInput.ParameterName = "input";
                    clobInput.OracleDbType = OracleDbType.Varchar2;
                    clobInput.Value = inputData;
                    clobInput.Direction = ParameterDirection.Input;
                    inputParameters.Add(clobInput);

                    // 添加CURSOR输出参数
                    OracleParameter refCursorOutput = new OracleParameter();
                    refCursorOutput.ParameterName = "output";
                    refCursorOutput.OracleDbType = OracleDbType.RefCursor;
                    refCursorOutput.Direction = ParameterDirection.Output;
                    outputParameters.Add(refCursorOutput);

                    // 添加VARCHAR2输出参数
                    OracleParameter revarcharOutput = new OracleParameter();
                    revarcharOutput.ParameterName = "message";
                    revarcharOutput.OracleDbType = OracleDbType.Varchar2;
                    revarcharOutput.Size = 3000;
                    revarcharOutput.Direction = ParameterDirection.Output;
                    outputParameters.Add(revarcharOutput);

                    using (OracleCommand command = new OracleCommand())
                    {
                        int RETURN_VALUE_BUFFER_SIZE = 4000;
                        DataTable dataTable = new DataTable();
                        command.Connection = db;
                        if (transaction != null)
                        {
                            command.Transaction = transaction;
                        }
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = packageName;

                        foreach (OracleParameter iteams in inputParameters)
                        {
                            command.Parameters.Add(iteams);
                        }

                        foreach (OracleParameter iteams in outputParameters)
                        {
                            command.Parameters.Add(iteams);
                        }
                        command.ExecuteNonQuery();
                        oraclePkgOutputString = command.Parameters["message"].Value.ToString();
                        _logger.LogInformation("Package(" + packageName + ") return msg : " + oraclePkgOutputString);
                        outmessage = oraclePkgOutputString;
                        // 检查输出参数是否为OracleRefCursor
                        if (command.Parameters["output"].Value is OracleRefCursor outputCursor && !outputCursor.IsNull)
                        {
                            using (OracleDataReader cursorReader = outputCursor.GetDataReader())
                            {
                                outdata = _ConvertToModels<T>(cursorReader);
                            }
                        }
                    }

                    if (oraclePkgOutputString.StartsWith("-1@"))
                    {
                        _logger.LogError("Call HAND Package Return Error , Msg: " + oraclePkgOutputString);
                        transaction.Rollback();
                    }
                    else
                    {
                        _logger.LogInformation(" Oracle package executed , transaction committed.");
                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    outmessage = "-1@" + ex.Message;
                    _logger.LogError("Call HAND Package Return Error.", ex);
                }
                finally
                {
                    db.Dispose();
                    db.Close();
                }
            }
        }
        catch (Exception ex)
        {
            outmessage += ("-1@" + ex.Message);
            _logger.LogError("Call HAND Package Return Error.", ex);
        }
        return outdata;
    }

    /// <summary>
    /// 读取并转换为资料模式阵列.
    /// </summary>
    private List<T> _ConvertToModels<T>(OracleDataReader cursorReader)
    {
        List<T> modelList = new List<T>();
        T model = default(T);
        switch (typeof(T).Name)
        {
            case "String":
                while (cursorReader.Read())
                {
                    string data = cursorReader.GetString(0);
                    modelList.Add((T)(object)data);
                }
                cursorReader.Close();
                break;
            case "ConfirmMatReturnoutput":
                while (cursorReader.Read())
                {
                    //
                    // 2025.3.1 alex_ho 將 package 填入 Model.
                    //

                    // ConfirmMatReturnModeler.ConfirmMatReturnoutput confirmMatReturnModeler = new ConfirmMatReturnModeler.ConfirmMatReturnoutput();
                    // confirmMatReturnModeler.document_number = cursorReader.GetValue(0).ToString();
                    // confirmMatReturnModeler.barcode = cursorReader.GetValue(1).ToString();
                    // confirmMatReturnModeler.item_code = cursorReader.GetValue(2).ToString();
                    // confirmMatReturnModeler.product_rev = cursorReader.GetValue(3).ToString();
                    // confirmMatReturnModeler.code_qty = cursorReader.GetValue(4).ToString();
                    // confirmMatReturnModeler.primary_uom = cursorReader.GetValue(5).ToString();
                    // modelList.Add((T)(object)confirmMatReturnModeler);
                }
                cursorReader.Close();
                break;
            default:
                throw new Exception($"Error: Can't Convert To Model , No Match Case For Class : {typeof(T).Name}");
        }
        return modelList;
    }

}