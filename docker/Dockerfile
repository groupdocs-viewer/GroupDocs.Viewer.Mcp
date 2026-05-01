FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish src/GroupDocs.Viewer.Mcp/GroupDocs.Viewer.Mcp.csproj \
    -c Release -o /app --no-self-contained

FROM base AS final

# Native graphics deps GroupDocs.Viewer's rendering paths need on Linux:
# Slides, image post-processing, archive-to-Word, mail/Outlook saving, and
# barcode generation all call System.Drawing.Common internally. libgdiplus is
# the GDI+ implementation System.Drawing.Common P/Invokes; libfontconfig1 is
# its hidden font dependency. Pair this with <RuntimeHostConfigurationOption
# Include="System.Drawing.EnableUnixSupport" Value="true" /> in the consuming
# csproj — without both, rendering throws PlatformNotSupportedException on Linux.
USER root
RUN apt-get update \
 && apt-get install -y --no-install-recommends libgdiplus libfontconfig1 \
 && rm -rf /var/lib/apt/lists/*

# OCI image labels. VERSION is supplied by the publish workflow via
# --build-arg VERSION=YY.MM.N. Default 'dev' makes local `docker build` runs
# self-identify instead of inheriting the base image's misleading version.
ARG VERSION=dev
LABEL org.opencontainers.image.title="GroupDocs.Viewer.Mcp" \
      org.opencontainers.image.description="MCP server exposing GroupDocs.Viewer for .NET as AI-callable tools (RenderPage, GetViewInfo) for Claude, Cursor, GitHub Copilot, and other MCP agents." \
      org.opencontainers.image.version="${VERSION}" \
      org.opencontainers.image.source="https://github.com/groupdocs-viewer/GroupDocs.Viewer.Mcp" \
      org.opencontainers.image.url="https://www.nuget.org/packages/GroupDocs.Viewer.Mcp" \
      org.opencontainers.image.licenses="MIT" \
      org.opencontainers.image.vendor="Aspose Pty Ltd"

COPY --from=build /app .

ENV GROUPDOCS_MCP_STORAGE_PATH=/data
VOLUME /data

USER $APP_UID

ENTRYPOINT ["dotnet", "GroupDocs.Viewer.Mcp.dll"]
