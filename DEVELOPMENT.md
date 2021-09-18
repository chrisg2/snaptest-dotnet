# Development Workflow Notes

## Sample DotNET CLI commands

### Run core library unit tests

```shell
dotnet test src/*sln
```

Run tests and force snapshots to be refreshed:
```shell
SNAPTEST_REFRESH=yes dotnet test src/*sln
```

Run tests and create missing snapshots:
```shell
SNAPTEST_CREATE_MISSING_SNAPSHOTS=yes dotnet test src/*sln
```


### Run example tests

```shell
dotnet test examples/*sln
```


### Publish packages to NuGet Gallery

```shell
dotnet pack src/*sln
dotnet nuget push src/SnapTest/bin/Debug/*nupkg --api-key "$KEY" --source https://api.nuget.org/v3/index.json
```

NuGet Gallery API keys can be managed at https://www.nuget.org/account/apikeys


### Publish packages to GitHub Packages

```shell
dotnet nuget add source https://nuget.pkg.github.com/chrisg2/index.json -n github -u chrisg2 -p "$PAT" --store-password-in-clear-text

dotnet pack src/*sln
dotnet nuget push src/SnapTest/bin/Release/*nupkg --source github
```

GitHub Personal Access Tokens can be managed at https://github.com/settings/tokens
