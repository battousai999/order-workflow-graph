# order-workflow-graph

This is just a quick program to visualize an order workflow as an SVG file.

## Database Connection

This program uses .NET "User Secrets" to locally store the database connection that it uses.

In order to run this program, one would need to use the following command to store the connection string:
```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<connection-string-here>"
```
