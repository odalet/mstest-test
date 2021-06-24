FROM mcr.microsoft.com/dotnet/sdk:5.0.202-buster-slim-amd64
# FROM mcr.microsoft.com/dotnet/sdk:3.1.410-buster

# Install .NET Core 3.1 runtime and ASP.NET (we don't need the 3.1 SDK, only the libs)
# NB: Do not copy the whole runtime, only what's in shared: we don't want dotnet executable to be overwritten
COPY --from=mcr.microsoft.com/dotnet/runtime:3.1.14-buster-slim ["/usr/share/dotnet/shared/Microsoft.NETCore.App", "/usr/share/dotnet/shared/Microsoft.NETCore.App"]

COPY --from=mcr.microsoft.com/dotnet/aspnet:3.1.14-buster-slim ["/usr/share/dotnet/shared/Microsoft.AspNetCore.App", "/usr/share/dotnet/shared/Microsoft.AspNetCore.App"]

# Linux update
RUN apt-get update \
    && apt-get dist-upgrade -y \
    && apt-get install -y git-lfs lsb-release \
    && apt-get -q autoremove \
    && apt-get -q clean -y \
    && rm -rf /var/lib/apt/lists/* /var/cache/apt/*.bin \
    # Trigger first run experience by running arbitrary cmd
    && dotnet help \
    # List installed runtimes and sdks
    && dotnet --list-sdks \
    && dotnet --list-runtimes

ENV PATH="/root/.dotnet/tools:${PATH}"

RUN dotnet tool install --global GitVersion.Tool --version 5.6.8
