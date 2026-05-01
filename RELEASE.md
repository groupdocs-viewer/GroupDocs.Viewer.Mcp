# Release Process — GroupDocs.Viewer.Mcp

End-to-end checklist for releasing a new version to NuGet.org + ghcr.io + Docker Hub + MCP Registry.

## Versioning — CalVer `YY.MM.N`

- `YY` — 2-digit year (e.g. `26` = 2026)
- `MM` — month without leading zero (e.g. `5` = May)
- `N` — patch increment starting at `0`; increment for hotfixes within the same month

Example: `26.5.0`, `26.5.1`, `26.6.0`.

---

## Day-to-day work (no release)

Just push to `master`:

```bash
git add <files>
git commit -m "…"
git push
```

`build_packages.yml` and `run_tests.yml` run on every push — `publish_prod.yml` and `publish_docker.yml` do **not**. Commits never create a tag, a NuGet release, a GitHub Release, or a Docker image tag. Changelog edits, code changes, and prop bumps are all free actions — you can commit them whenever without triggering a release.

---

## Releasing a new version

### 1. Prepare the release commit

All edits below go in **one commit on `master`**:

1. **Bump the package version** in [build/dependencies.props](build/dependencies.props):

   ```xml
   <GroupDocsViewerMcp>{NEW_VERSION}</GroupDocsViewerMcp>
   ```

2. **Bump both `.mcp/server.json` versions** — [src/GroupDocs.Viewer.Mcp/.mcp/server.json](src/GroupDocs.Viewer.Mcp/.mcp/server.json) has **two** `version` fields that must both match.

   > `build.ps1` refuses to build if any of these drift from `dependencies.props`. The release workflow also double-checks at the start and fails fast.

3. **Update pinned versions in `README.md`** — search for the old version and replace every occurrence (`dnx GroupDocs.Viewer.Mcp@…`, Claude Desktop config, VS Code `mcp.json`). Typically 4 places.

4. **Add a changelog entry** — new file `changelog/NNN-short-slug.md` with `version: {NEW_VERSION}` in the frontmatter. Format in [changelog/README.md](changelog/README.md).

5. *(Optional, rare)* Bump external dependency versions in the same props file.

6. Commit + push:

   ```bash
   git add build/dependencies.props src/GroupDocs.Viewer.Mcp/.mcp/server.json README.md changelog/NNN-*.md
   git commit -m "Release {NEW_VERSION}"
   git push
   ```

### 2. Verify locally (optional but recommended)

```powershell
./build.ps1
dotnet test src/GroupDocs.Viewer.Mcp.sln -c Release
docker build -f docker/Dockerfile -t viewer-net-mcp:test .
```

### 3. Wait for CI green on `master`

`build_packages.yml` + `run_tests.yml` must be green before releasing.

### 4. Trigger the release

Two ways — **pick one, not both**.

#### Option A — UI dispatch

1. GitHub → **Actions** → **Publish Prod** → **Run workflow** → enter `{NEW_VERSION}` → Run.
2. GitHub → **Actions** → **Publish Docker Image** → **Run workflow** → enter `{NEW_VERSION}` → Run.

#### Option B — tag push

```bash
git tag {NEW_VERSION}
git push origin {NEW_VERSION}
```

The tag push fires both workflows simultaneously.

> Tag must be `YY.MM.N` — no `v` prefix, no suffix.

### 5. CI takes over

**`publish_prod.yml`** signs the .nupkg with SSL.com eSigner, pushes to NuGet.org, and creates the GitHub Release.

**`publish_docker.yml`** builds the multi-arch image and pushes both `ghcr.io/groupdocs-viewer/viewer-net-mcp:{NEW_VERSION}` and `docker.io/groupdocs/viewer-net-mcp:{NEW_VERSION}` (plus `:latest`).

**`publish_prod.yml → publish_mcp_registry`** waits for NuGet.org to index the new version, then publishes `src/GroupDocs.Viewer.Mcp/.mcp/server.json` to `https://registry.modelcontextprotocol.io` via GitHub OIDC.

### 6. Post-release verification

- [ ] **NuGet**: package listed at new version, signed-badge visible
- [ ] **GitHub Release** created with nupkg assets + changelog body
- [ ] `ghcr.io/groupdocs-viewer/viewer-net-mcp:{NEW_VERSION}` pullable
- [ ] `docker.io/groupdocs/viewer-net-mcp:{NEW_VERSION}` pullable
- [ ] Smoke test: `dnx GroupDocs.Viewer.Mcp@{NEW_VERSION} --yes`
- [ ] MCP Registry: `https://registry.modelcontextprotocol.io/v0/servers/io.github.groupdocs-viewer%2Fgroupdocs-viewer-mcp/versions/latest` returns 200 with the new version

---

## Required GitHub secrets & variables

**Secrets**:

| Secret | Purpose |
|---|---|
| `NUGET_API_KEY_PROD` | NuGet.org API key scoped to `GroupDocs.Viewer.Mcp` |
| `ES_USERNAME` / `ES_PASSWORD` / `ES_TOTP_SECRET` / `CODE_SIGN_CLIENT_ID` | SSL.com eSigner credentials |
| `DOCKERHUB_USERNAME` / `DOCKERHUB_TOKEN` | Docker Hub access |

**Variables**:

| Variable | Default | Purpose |
|---|---|---|
| `CODE_SIGN_TOOL_VERSION` | `1.3.0` | CodeSignTool release tag |

> `GITHUB_TOKEN` is provisioned automatically — no setup needed for ghcr.io pushes or MCP Registry OIDC auth.

---

## MCP Registry namespace verification

Auto-verified via GitHub OIDC because the repo lives under `github.com/groupdocs-viewer/*`. No DNS records or manual tokens needed.

---

## Yanking a bad release

Never re-upload the same version. Unlist on NuGet.org, delete the affected Docker tag, bump `N` (`26.5.0` → `26.5.1`), add a `type: fix` changelog entry, and re-release.
