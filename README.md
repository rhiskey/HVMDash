# HVMDash
Dashboard of music service. Blazor WebAssembly + PWA

[![giphy.gif](https://media.giphy.com/media/SsXuhZs8iJlAwaoMcC/giphy.gif)](https://media.giphy.com/media/SsXuhZs8iJlAwaoMcC/giphy.gif)

Create appsettings.json with 2 connection strings (MySQL) in Server folder:
```
{
  "ConnectionStrings": {
    "EfConnection": "server=;user=;database=;port=;password=",
    "UserConnection": "server=;user=;database=;port=;password="
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "IdentityServer": {
    "Clients": {
      "WAAuth.Client": {
        "Profile": "IdentityServerSPA"
      }
    }
  },
"AllowedHosts": "*"
}

```