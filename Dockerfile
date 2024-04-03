FROM mcr.microsoft.com/dotnet/aspnet:8.0.3-alpine3.19-amd64 AS base-env

# Preparing the runtime environment
WORKDIR /app
EXPOSE 8080

# Install the required dependencies to handle the internationalization
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    LC_ALL=en_US.UTF-8 \
    LANG=en_US.UTF-8

RUN apk add --no-cache \
            icu-libs \
            icu-data-full \
            tzdata

RUN apk upgrade musl

USER app

ENV ASPNETCORE_ENVIRONMENT Production
ENV ASPNETCORE_URLS http://*:8080



FROM mcr.microsoft.com/dotnet/sdk:8.0.203-alpine3.19-amd64 AS build-env

WORKDIR /src

# Copy just the project files and restore the nuget packages
COPY ./src/Demo.Api/*.csproj ./Demo.Api/
COPY ./src/Demo.Application/*.csproj ./Demo.Application/
COPY ./src/Demo.Domain/*.csproj ./Demo.Domain/
COPY ./src/Demo.Infrastructure/*.csproj ./Demo.Infrastructure/

# Restore nuget packages
RUN dotnet restore ./Demo.Api/*.csproj --runtime linux-musl-x64

# Copy all the source code and build
COPY ./src/Demo.Api/ ./Demo.Api/
COPY ./src/Demo.Application/ ./Demo.Application/
COPY ./src/Demo.Domain/ ./Demo.Domain/
COPY ./src/Demo.Infrastructure/ ./Demo.Infrastructure/

# Build and publish the application. Used the "--no-restore" and "--no-build" to benefit the layer caches
RUN dotnet build -c Release ./Demo.Api/*.csproj --runtime linux-musl-x64

RUN dotnet publish ./Demo.Api/*.csproj \
    -c Release \
    --runtime linux-musl-x64 \
    --no-build \
    --no-restore \
    --self-contained true \
    -o /publish



FROM base-env AS run-env

WORKDIR /app
COPY --from=build-env /publish .

ENTRYPOINT ["dotnet", "Demo.Api.dll"]
