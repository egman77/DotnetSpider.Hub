# DotnetSpider.Enterprise

### Comments

+ get ip: ifconfig | grep -E 'inet.[0-9]' | grep -v '127.0.0.1' | awk '{ print $2}'

### Agent

**RUN:** dotnet DotnetSpider.Enterprise.Agent.dll
**RUN DAEMON:** nohup dotnet DotnetSpider.Enterprise.Agent.dll --daemon &