FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Directory.Build.props", "Loodsen.SalaryCalculator/"]
COPY ["Loodsen.SalaryCalculator/Loodsen.SalaryCalculator.csproj", "Loodsen.SalaryCalculator/"]
RUN dotnet restore "Loodsen.SalaryCalculator/Loodsen.SalaryCalculator.csproj"
COPY . .
WORKDIR "/src/Loodsen.SalaryCalculator"
RUN dotnet build "Loodsen.SalaryCalculator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Loodsen.SalaryCalculator.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Loodsen.SalaryCalculator.dll"]
CMD ["dotnet", "Loodsen.SalaryCalculator.dll"]
