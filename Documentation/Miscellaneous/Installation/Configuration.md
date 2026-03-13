# Configuration
The `appsettings.json` file can largely be used without modification. However, a few parameters must be adjusted to match your local environment.

The example below demonstrates how to configure the application to run with a **SQLite database**, which is the recommended setup for development and testing scenarios. In addition, you must update the **Serilog `WriteTo` file path** to point to a valid directory on your system where the application has write permissions.
The database file will be created automatically on startup.
```json

  "ConnectionStrings": {
    "DefaultConnection": "Data Source=E:\\Databases\\Dr_Admin\\DR_Admin.db"
  },
  ...
  "DbSettings": {
    "DatabaseType": "SQLITE"
  },
  ...
  "Serilog": {
  ...
    "WriteTo": [
		...
        "Args": {
          "path": "D:\\LogFiles\\DR_Admin\\DR_Admin-.log",
          ...
        }
      }
    ]
  },
   
```

---
## Next step
You are now ready to build the solution. See the [Build page](Installation/Build.md) for more information.
