[![Nuget](https://img.shields.io/nuget/v/Database.ADO?style=for-the-badge)](https://www.nuget.org/packages/Database.ADO)
[![Nuget](https://img.shields.io/nuget/dt/Database.ADO?style=for-the-badge)](https://www.nuget.org/packages/Database.ADO)

# DigitalDoor.Database.ADO
Basic access to database using classes. But also can use direct sql queries. This ORM is only tested with MS-SQL for now.

## Register ServiceProvider

```csharp
// Add services to the container.
builder.Services.Configure<DatabaseOptions>(options => builder.Configuration.GetSection(DatabaseOptions.SectionName).Bind(options));
builder.Services.AddScoped<DataBaseWithADO>();
```

### appsettings
```json
  "DatabaseOptions": {
    "ConnectionString": "FHCLARKSRV\\SQLEXPRESS;Initial Catalog=databsename;Persist Security Info=false;User ID=username;Password=***************;Max Pool Size=100;",
    "Options": {
      "LogOptions": {
        "LogResults": true,
        "LogFolder": "logs"
      },
      "EnableSqlInjectionControl": true,
      "EnableCharChrControl": true
    },
    "Params": [
      {
        "ColumnName": "DefaultIndexColum",
        "Value": 1
      },
      {
        "ColumnName": "DefaultFilterColumn",
        "Value": "Some"
      }
    ]
  }
```

## DatabaseOptions
ConnectionString => how to connect with the database using ADO.
Options => How to configure DatabseWithAdo object:
Options.LogOptions => Login parameters using default login. Also can use IDbLog with a personalized loger
Options.LogOptions.LogResults => true to log db results.
Options.LogOptions.LogFolder => folder where will store the log file using a default loger.
Options.EnableSqlInjectionControl => true to prevent sql injection. Enable this not allowed comments in the SQL queries or commands.
Options.EnableCharControl => If EnableSqlInjectionControl is true set to true to block use CHR in the queries or commands.
Params => Enabled a default column when using class with a default value in a where if have a property match with the column name.
Params = IEnumerable < ParamValue > where ParamValue { ColumnName = "PropertyName", "Value" = (object)DefaultValue }

# Load data
```csharp
DataTable dataTable = database.GetDataTable<Customers>();
DataSet ds = database.GetDataSet<Customers>();
DataView dv = database.GetDataView<Customers>();
List<Customers> list = database.List<Customers>();

// with pagination (startIndex =  numPage * numElements)
DataTable dataTable = database.GetDataTable<Customers>(nameof(Customers.Id), 0, 50);
DataSet ds = database.GetDataSet<Customers>(nameof(Customers.Id), 10, 50);
DataView dv = database.GetDataView<Customers>(nameof(Customers.Id), 100, 50);
List<Customers> list = database.List<Customers>(nameof(Customers.Id), 1000, 50);
```

