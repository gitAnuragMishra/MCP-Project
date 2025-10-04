# MCP-Project
For POC purposes


Actual FastAPI appsettings.json file is hidden for security reasons, but here is a sample structure:
```json
{
    "connectionStrings": {
        "DefaultConnection": "Server=<server name>;Database=<initial catalog>;UID=<user id>;PWD=<password>;"
    }
}
```


## Release Pattern:

v1.0.0: Basic working POC

v1.1.0: Remote MCP-Server with WithHttpTransport()

v1.1.1: Added more tools; accessible from VS Code

v1.2.0: Complete Https Server + Client integration

v1.3.0: CustomMcpClientFactory and User session