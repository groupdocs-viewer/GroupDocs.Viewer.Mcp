# Claude Code

```bash
claude mcp add groupdocs-viewer -- dnx GroupDocs.Viewer.Mcp --yes
```

With storage folder and license:

```bash
claude mcp add groupdocs-viewer -e GROUPDOCS_MCP_STORAGE_PATH=/path/to/documents -e GROUPDOCS_LICENSE_PATH=/path/to/GroupDocs.Total.lic -- dnx GroupDocs.Viewer.Mcp --yes
```

Pin a version by replacing `GroupDocs.Viewer.Mcp` with `GroupDocs.Viewer.Mcp@26.7.1`.
