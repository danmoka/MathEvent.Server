FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 5003

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["MathEvent.Api/MathEvent.Api.csproj", "MathEvent.Api/"]
COPY ["MathEvent.Services/MathEvent.Services.csproj", "MathEvent.Services/"]
COPY ["MathEvent.Converters/MathEvent.Converters.csproj", "MathEvent.Converters/"]
COPY ["MathEvent.Contracts/MathEvent.Contracts.csproj", "MathEvent.Contracts/"]
COPY ["MathEvent.Models/MathEvent.Models.csproj", "MathEvent.Models/"]
COPY ["MathEvent.Entities/MathEvent.Entities.csproj", "MathEvent.Entities/"]
COPY ["MathEvent.DTOs/MathEvent.DTOs.csproj", "MathEvent.DTOs/"]
COPY ["MathEvent.Enums/MathEvent.Enums.csproj", "MathEvent.Enums/"]
COPY ["MathEvent.Repository/MathEvent.Repository.csproj", "MathEvent.Repository/"]
COPY ["MathEvent.Database/MathEvent.Database.csproj", "MathEvent.Database/"]
COPY ["MathEvent.Handlers/MathEvent.AuthorizationHandlers.csproj", "MathEvent.Handlers/"]
COPY ["MathEvent.Constants/MathEvent.Constants.csproj", "MathEvent.Constants/"]
COPY ["MathEvent.ScheduledJobs/MathEvent.ScheduledJobs.csproj", "MathEvent.ScheduledJobs/"]
COPY ["MathEvent.Validation/MathEvent.Validation.csproj", "MathEvent.Validation/"]
COPY ["MathEvent.Email/MathEvent.Email.csproj", "MathEvent.Email/"]
RUN dotnet restore "MathEvent.Api/MathEvent.Api.csproj"
COPY . .
WORKDIR "/src/MathEvent.Api"
RUN dotnet build "MathEvent.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MathEvent.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MathEvent.Api.dll"]