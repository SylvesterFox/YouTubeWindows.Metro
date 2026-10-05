# CI and releases

`build.yml` runs on pushes and pull requests to `main`/`master` and on manual dispatch, using `windows-2019`. It builds x64 and x86, validates the expected executables and ZIPs, and uploads the packages as workflow artifacts. NuGet cache is configured even though the current wrapper has no third-party packages.

`release.yml` runs for `v*` tags, derives the release version from the tag, builds both architectures, checks nonempty archives, and attaches them to a GitHub Release. Create and push a tag such as `v1.0.0` after pushing the branch. This project does not push or publish on the developer's behalf.
