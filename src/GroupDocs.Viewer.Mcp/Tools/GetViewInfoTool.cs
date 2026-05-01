using System.ComponentModel;
using System.Text.Json;
using GroupDocs.Mcp.Core;
using GroupDocs.Mcp.Core.Licensing;
using GroupDocs.Viewer;
using GroupDocs.Viewer.Options;
using ModelContextProtocol.Server;

namespace GroupDocs.Viewer.Mcp.Tools;

[McpServerToolType]
public static class GetViewInfoTool
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    [McpServerTool, Description(
        "Gets document view information including file type, page count, and per-page dimensions — without rendering. " +
        "Supports PDF, DOCX, XLSX, PPTX, ODT, RTF, HTML, EML, MSG, and 170+ more document formats. " +
        "Call this tool whenever the user asks for view info, page dimensions, or wants to inspect a document before rendering. " +
        "Do NOT pre-check whether files exist — just pass the filename the user provided. " +
        "The tool resolves files from storage and returns an error with available files if a name is not found. " +
        "Returns a JSON object with `fileName`, `fileType`, `pageCount`, and `pages` (array of `{number, width, height, name, visible}`).")]
    public static async Task<string> GetViewInfo(
        IFileResolver resolver,
        ILicenseManager licenseManager,
        OutputHelper output,
        FileInput file,
        [Description("Password for protected documents")] string? password = null)
    {
        licenseManager.SetLicense();
        using var resolved = await resolver.ResolveAsync(file);

        var loadOptions = new LoadOptions { Password = password };
        using var viewer = new Viewer(resolved.Stream, loadOptions);

        var viewInfoOptions = ViewInfoOptions.ForPngView();
        var viewInfo = viewer.GetViewInfo(viewInfoOptions);

        var result = new
        {
            fileName = resolved.FileName,
            fileType = viewInfo.FileType?.ToString(),
            pageCount = viewInfo.Pages.Count,
            pages = viewInfo.Pages.Select(p => new
            {
                number = p.Number,
                width = p.Width,
                height = p.Height,
                name = string.IsNullOrEmpty(p.Name) ? null : p.Name,
                visible = p.Visible
            }).ToList()
        };

        var json = JsonSerializer.Serialize(result, JsonOptions);
        return output.TruncateText(json);
    }
}
