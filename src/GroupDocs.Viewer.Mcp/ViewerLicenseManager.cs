using GroupDocs.Mcp.Core;
using GroupDocs.Mcp.Core.Licensing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GroupDocs.Viewer.Mcp;

public class ViewerLicenseManager : LicenseManager
{
    public ViewerLicenseManager(IOptions<McpConfig> config, ILogger<LicenseManager> logger) : base(config, logger) { }
    protected override void SetLicenseFromPath(string licensePath)
    {
        new GroupDocs.Viewer.License().SetLicense(licensePath);
    }
}
