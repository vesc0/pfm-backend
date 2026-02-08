# 1) Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy only the csproj and restore so layers cache nicely
COPY Directory.Packages.props ./
COPY ["PFM.API/PFM.API.csproj", "PFM.API/"]
RUN dotnet restore "PFM.API/PFM.API.csproj"

# Copy everything else & publish
COPY . .
WORKDIR "/src/PFM.API"
RUN dotnet publish -c Release -o /app/out

# 2) Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY PFM.API/auto-categorize-rules.yml ./
COPY --from=build /app/out ./
EXPOSE 8080
ENTRYPOINT ["dotnet", "PFM.API.dll"]
