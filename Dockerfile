# ====== 编译阶段 ======
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# 复制 csproj 并 restore（利用 Docker 缓存层）
COPY StockSystem.csproj .
RUN dotnet restore

# 复制全部源码并编译发布
COPY . .
RUN dotnet publish -c Release -o /app

# ====== 运行阶段 ======
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# 创建日志目录
RUN mkdir -p /app/logs

COPY --from=build /app .

EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000

ENTRYPOINT ["dotnet", "StockSystem.dll"]
