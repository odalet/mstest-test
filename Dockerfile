FROM mcr.microsoft.com/dotnet/sdk:5.0.202-buster-slim-amd64

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
