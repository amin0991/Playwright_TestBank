FROM mcr.microsoft.com/dotnet/sdk:10.0

WORKDIR /app
COPY ParaBankAutomation/ ./ParaBankAutomation/
WORKDIR /app/ParaBankAutomation

RUN dotnet restore
RUN dotnet build --configuration Release --no-restore

RUN apt-get update -qq && apt-get install -y --no-install-recommends \
    libnss3 libnspr4 libatk1.0-0 libatk-bridge2.0-0 libcups2 \
    libdrm2 libdbus-1-3 libxkbcommon0 libxcomposite1 libxdamage1 \
    libxfixes3 libxrandr2 libgbm1 libasound2t64 libpango-1.0-0 libcairo2 \
    wget ca-certificates

RUN wget -q -O /tmp/powershell.tar.gz \
    https://github.com/PowerShell/PowerShell/releases/download/v7.4.0/powershell-7.4.0-linux-x64.tar.gz && \
    mkdir -p /opt/powershell && \
    tar -xzf /tmp/powershell.tar.gz -C /opt/powershell && \
    ln -s /opt/powershell/pwsh /usr/local/bin/pwsh

RUN pwsh /app/ParaBankAutomation/bin/Release/net10.0/playwright.ps1 install chromium

ENV PLAYWRIGHT_HEADLESS=true

CMD ["dotnet", "test", "--no-build", "--configuration", "Release"]