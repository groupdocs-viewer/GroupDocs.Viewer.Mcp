using System.ComponentModel;
using GroupDocs.Mcp.Core;
using GroupDocs.Mcp.Core.Licensing;
using GroupDocs.Viewer;
using GroupDocs.Viewer.Options;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace GroupDocs.Viewer.Mcp.Tools;

[McpServerToolType]
public static class RenderPageTool
{
    [McpServerTool, Description(
        "Renders a single document page as a PNG image and returns it inline (plus saves a copy to storage). " +
        "Supports PDF, DOCX, XLSX, PPTX, ODT, RTF, HTML, EML, MSG, and 170+ more document formats. " +
        "Call this tool immediately whenever the user asks to render, preview, or get a page image from a document. " +
        "Do NOT pre-check whether files exist — just pass the filename the user provided. " +
        "The tool resolves files from storage and returns an error with available files if a name is not found. " +
        "Returns a CallToolResult containing both a TextContentBlock (saved file path under `<source-stem>_page<N>.png`) and an ImageContentBlock (the PNG bytes inline as `image/png`).")]
    public static async Task<CallToolResult> RenderPage(
        IFileResolver resolver,
        IFileStorage storage,
        ILicenseManager licenseManager,
        OutputHelper output,
        FileInput file,
        [Description("Page number to render (1-based)")] int page = 1,
        [Description("Password for protected documents")] string? password = null)
    {
        licenseManager.SetLicense();
        using var resolved = await resolver.ResolveAsync(file);

        var baseName = Path.GetFileNameWithoutExtension(resolved.FileName);
        var outputName = $"{baseName}_page{page}.png";

        var ms = new MemoryStream();
        try
        {
            var loadOptions = new LoadOptions { Password = password };
            using var viewer = new Viewer(resolved.Stream, loadOptions);

            var options = new PngViewOptions(_ => ms);
            viewer.View(options, page);

            var bytes = ms.ToArray();
            var savedPath = await storage.WriteFileAsync(outputName, bytes, rewrite: true);
            var fileInfo = await output.BuildFileOutputAsync(savedPath, $"Page {page} of '{resolved.FileName}'");

            var prefix = licenseManager.IsLicensed
                ? string.Empty
                : "[Evaluation mode] Output may include watermarks.\n\n";

            return new CallToolResult
            {
                Content =
                [
                    new TextContentBlock { Text = prefix + fileInfo },
                    ImageContentBlock.FromBytes(bytes, "image/png")
                ]
            };
        }
        finally
        {
            await ms.DisposeAsync();
        }
    }
}
