# HVMDash
Dashboard of music service. 
Blazor WebAssembly + PWA + Authentication (IdentityServer)

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

https://thegreenerman.medium.com/set-up-https-on-local-with-net-core-and-docker-7a41f030fc76
```
docker run --rm -it -p 5000:80  -p 5001:443  -e ASPNETCORE_URLS="https://+;http://+"  -e ASPNETCORE_HTTPS_PORT=5001  -e ASPNETCORE_ENVIRONMENT=Development -v $Env:APPDATA\microsoft\UserSecrets\:/root/.microsoft/usersecrets -v $Env:USERPROFILE\.aspnet\https:/root/.aspnet/https/ HVMDash.Server:latest
```