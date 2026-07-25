# Codex CLI (OpenAI)

```bash
codex mcp add groupdocs-viewer -- dnx GroupDocs.Viewer.Mcp --yes
```

Or add to `~/.codex/config.toml`:

```toml
[mcp_servers.groupdocs-viewer]
command = "dnx"
args = ["GroupDocs.Viewer.Mcp", "--yes"]

[mcp_servers.groupdocs-viewer.env]
GROUPDOCS_MCP_STORAGE_PATH = "/path/to/documents"
# GROUPDOCS_LICENSE_PATH = "/path/to/GroupDocs.Total.lic"   # omit for evaluation mode
```

Pin a version by replacing `GroupDocs.Viewer.Mcp` with `GroupDocs.Viewer.Mcp@26.7.1`.
