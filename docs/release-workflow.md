# Release workflow

This repository publishes builds of the desktop app from `RLUpkSuite/RLUpkSuite/RLUpkSuite.csproj` via the `Build and Release RLUpkSuite` GitHub Actions workflow.

## Triggering a release

1. Commit and push the changes you want to ship to the default branch.
2. Create an annotated Git tag following the `v*` convention (for example `v1.4.0`).
3. Push the tag to GitHub:
   ```pwsh
   git push origin v1.4.0
   ```

Pushing the tag kicks off the workflow, which produces a release and uploads a ZIP archive of the published binaries. The release version is inferred from the tag name.

## What the workflow does

- Restores, builds, and tests the entire solution using the .NET 9.0 SDK.
- Publishes the WPF app in Release configuration.
- Zips the published output and stores it both as a workflow artifact and as a GitHub release asset.
- Creates release notes automatically so you get a changelog for free.

If you need multiple variants (for example, self-contained or single-file builds), extend the publish step in `.github/workflows/release.yml` with additional `dotnet publish` invocations.
