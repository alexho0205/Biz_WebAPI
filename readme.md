# Build Template Project
這個文件說明如何建立一個專案樣板.並如何使用此樣板.

主要為以下階段
1. 建立空專案 ( 或使用模版建立 )
1. 開發客製代碼
1. 打包專案樣版
1. 使用樣版生成專案


## (1) 建立空專案 
首先需要建立空專案.
``` bash
# 建立頂層資料夾 
mkdir Biz_T_WebAPi_Nlog_Dapper

# 建立解決方案
cd Biz_T_WebAPi_Nlog_Dapper
dotnet new sln -n Biz_T_WebAPi_Nlog_Dapper

# 建立專案 ,  這裡使用 webapi 模版
dotnet new webapi --use-controllers -o Biz.T.WebApi.Nlog.Dapper

# 加入解決方案 (反斜線為power-shell自動生成)
dotnet sln .\Biz_T_WebAPi_Nlog_Dapper.sln add .\Biz.T.WebApi.Nlog.Dapper\Biz.T.WebApi.Nlog.Dapper.csproj

```


### Edit Program.cs
``` c#
//app.MapControllers();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
```

### Add Package 
  - 因為 VSCODE terminal 不明原因無法使用,改用 ctrl + p 輸入指令
  - 安裝 Dapper / nlog ...
  ```
  按 ctrl+p  
  在框內輸入 >Nuget Add Nuget ...
  輸入 Dapper > enter
  輸入 Nlog > enter
  ```

### 配置 NLOG.config
請參考NLOG.config


## (2) 配置客製程式
接下來將專案加入客製化代碼,例如 services/data provider ...
等等基礎架構.
   

### Run Project
測試專案是否可以運行 , port 參考 launchSettings.json

http://localhost:5176/swagger/index.html


## (3) 打包專案樣版.

### 事前準備
- 在專案根目錄加入資料夾&檔案
>  ```.template.config\template.json```
- 配置重點
  - 專案結構必須是 Solution > Project , 根目錄不能有 .csproj
  - 這幾個名稱必須一致: template.json/identity = template.json/sourceName , 
  - 專案名稱必須 = identity
  - namesapce 必須 = identity

### 執行打包
參考以下指令將專案打包為樣板

  ``` bash
  # 移到動專案根目錄
  cd C:\workspace\Template\Biz_T_WebAPi_Nlog_Dapper

  # 打包專案模板
  dotnet new install .

  # 檢查是否成功 , 模版將依據 .template.config 內容生成
  dotnet new list

  # 若發生問題,將模板下架
  dotnet new uninstall .

  ```

## (4) 使用樣版生成專案
接下來可以使用樣版生成新專案 , 並自訂 namesapce

``` bash
# 生成專案 , 自訂 namespace = Biz.Test.Api
dotnet new biz_webapi -o Biz.Test.Api
```
完成後打開新專案檢查是否依照新的 Namespace 生成專案.
```
http://localhost:53211/swagger
```
清除 readme.md , 換上適合的內容.

> 收工 ✌️
