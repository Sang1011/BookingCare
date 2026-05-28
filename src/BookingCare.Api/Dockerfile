FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/BookingCare.Api/BookingCare.Api.csproj", "src/BookingCare.Api/"]
COPY ["src/BookingCare.Application/BookingCare.Application.csproj", "src/BookingCare.Application/"]
COPY ["src/BookingCare.Infrastructure/BookingCare.Infrastructure.csproj", "src/BookingCare.Infrastructure/"]
COPY ["src/BookingCare.Domain/BookingCare.Domain.csproj", "src/BookingCare.Domain/"]

RUN dotnet restore "src/BookingCare.Api/BookingCare.Api.csproj"

COPY . .

WORKDIR "/src/src/BookingCare.Api"
RUN dotnet publish "BookingCare.Api.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BookingCare.Api.dll"]