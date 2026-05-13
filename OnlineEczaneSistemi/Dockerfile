FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /app
COPY . .
RUN dotnet publish "OnlineEczaneSistemi.csproj" -c Release -o /app/publish
WORKDIR /app/publish
EXPOSE 8080
ENTRYPOINT ["dotnet", "OnlineEczaneSistemi.dll"]