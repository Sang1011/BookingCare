FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/BookingCare.Worker/BookingCare.Worker.csproj", "src/BookingCare.Worker/"]
COPY ["src/BookingCare.Application/BookingCare.Application.csproj", "src/BookingCare.Application/"]
COPY ["src/BookingCare.Infrastructure/BookingCare.Infrastructure.csproj", "src/BookingCare.Infrastructure/"]
COPY ["src/BookingCare.Domain/BookingCare.Domain.csproj", "src/BookingCare.Domain/"]
RUN dotnet restore "src/BookingCare.Worker/BookingCare.Worker.csproj"
COPY . .
WORKDIR "/src/src/BookingCare.Worker"
RUN dotnet build "BookingCare.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BookingCare.Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BookingCare.Worker.dll"]