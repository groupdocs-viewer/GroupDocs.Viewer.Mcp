---
id: 001
date: 2026-05-01
version: 26.5.0
type: feature
---

# Initial public release of GroupDocs.Viewer MCP Server

## What changed
- NuGet package `GroupDocs.Viewer.Mcp` published with `McpServer` package type.
- Two MCP tools exposed:
  - `RenderPage` — render a single document page as a PNG image. Returns a `CallToolResult` with both a `TextContentBlock` (saved file path: `<source-stem>_page<N>.png`) and an `ImageContentBlock` (PNG bytes inline as `image/png`). Supports PDF, Word, Excel, PowerPoint, ODT, RTF, HTML, and 170+ more formats; takes optional `page` (1-based, default 1) and `password`.
  - `GetViewInfo` — return file type, page count, and per-page dimensions as JSON. No rendering performed. Optional `password` for protected documents.
- Installable via `dnx GroupDocs.Viewer.Mcp@26.5.0 --yes` (.NET 10 SDK required) or `dotnet tool install -g`.
- Docker image published to `ghcr.io/groupdocs-viewer/viewer-net-mcp` and `docker.io/groupdocs/viewer-net-mcp`.
- Environment variables: `GROUPDOCS_MCP_STORAGE_PATH`, optional `GROUPDOCS_MCP_OUTPUT_PATH`, `GROUPDOCS_LICENSE_PATH`.
- Linux native graphics deps wired up: `SkiaSharp.NativeAssets.Linux.NoDependencies` (3.119.1) is referenced because `GroupDocs.Viewer`'s nuspec declares the SkiaSharp managed and native packages — we pin explicitly so transitive resolution stays deterministic. `libgdiplus` + `libfontconfig1` are installed in the Docker image and the `System.Drawing.EnableUnixSupport` runtime flag is set because Viewer's Slides, image post-processing, archive-to-Word, mail/Outlook saving, and barcode generation paths still call `System.Drawing.Common`.

## Why
Fourth product MCP server in the GroupDocs MCP framework (after Metadata, Conversion, and Comparison). Exposes
GroupDocs.Viewer for .NET as AI-callable tools for Claude, Cursor,
VS Code / GitHub Copilot, and other MCP-compatible agents.

`RenderPage` is the first tool in the GroupDocs MCP family to return a `CallToolResult` directly with an inline `ImageContentBlock` — AI clients can display the rendered page without a separate fetch.

## Migration / impact
First release — no migration required.
