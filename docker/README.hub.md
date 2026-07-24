# GroupDocs.Viewer MCP Server

MCP server that exposes [GroupDocs.Viewer](https://products.groupdocs.com/viewer) as AI-callable tools for Claude, Cursor, GitHub Copilot, and other MCP agents.

## Quick start

```bash
docker run --rm -i \
  -v $(pwd)/documents:/data \
  groupdocs/viewer-net-mcp:latest
```

## Use with Claude Desktop

```json
{
  "mcpServers": {
    "groupdocs-viewer": {
      "command": "docker",
      "args": ["run", "--rm", "-i", "-v", "/path/to/documents:/data", "groupdocs/viewer-net-mcp:latest"]
    }
  }
}
```

## Tools

- **RenderPage** — Renders a single document page as a PNG image and returns it inline (plus saves a copy to storage as `<source-stem>_page<N>.png`). Supports PDF, Word, Excel, PowerPoint, ODT, RTF, HTML, and 170+ more formats; optional `password` for protected documents.
- **GetViewInfo** — Returns file type, page count, and per-page dimensions as JSON — without rendering. Useful as a pre-flight check before deciding which page(s) to render. Optional `password` for protected documents.

## Tags & environment

- Tags: `latest` + an immutable version tag per release matching NuGet (e.g. `26.7.1`).
  Platforms: `linux/amd64`, `linux/arm64`. Also on GHCR: `ghcr.io/groupdocs-viewer/viewer-net-mcp`.
- `GROUPDOCS_MCP_STORAGE_PATH` (default `/data`), `GROUPDOCS_MCP_OUTPUT_PATH` (optional),
  `GROUPDOCS_LICENSE_PATH` — mount your license and point at it to leave evaluation mode
  (see the Licensing section in the GitHub README for the exact evaluation limits).

Full docs, one-click installs for other clients, and licensing details:
**https://github.com/groupdocs-viewer/GroupDocs.Viewer.Mcp**
