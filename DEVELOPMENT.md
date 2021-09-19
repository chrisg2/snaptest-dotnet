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

1. Update `<Version>` in any of the following project files as appropriate:
   1. SnapTest/SnapTest.csproj
   1. SnapTest.NUnit/SnapTest.NUnit.csproj
   1. SnapTest.Xunit/SnapTest.Xunit.csproj

1. Ensure all tests pass:
    ```shell
    dotnet test src/*sln
    ```

1. Then build and push packages to NuGet Gallery:

    ```shell
    KEY="<NuGet Gallery API key>"
    VERSION="<Version to push>"

    dotnet pack --configuration Release src/*sln

    dotnet nuget push src/SnapTest/bin/Release/SnapTest.$VERSION.nupkg --api-key "$KEY" --source https://api.nuget.org/v3/index.json

    dotnet nuget push src/SnapTest.NUnit/bin/Release/SnapTest.NUnit.$VERSION.nupkg --api-key "$KEY" --source https://api.nuget.org/v3/index.json

    dotnet nuget push src/SnapTest.Xunit/bin/Release/SnapTest.Xunit.$VERSION.nupkg --api-key "$KEY" --source https://api.nuget.org/v3/index.json
    ```

    NuGet Gallery API keys can be managed at https://www.nuget.org/account/apikeys


### Publish packages to GitHub Packages

1. Configure GitHub as a NuGet source:
    ```shell
    PAT="<GitHub personal access token>"

    dotnet nuget add source https://nuget.pkg.github.com/chrisg2/index.json -n github -u chrisg2 -p "$PAT" --store-password-in-clear-text
    ```

1. Build and push packages to GitHub:
    ```shell
    VERSION="<Version to push>"

    dotnet pack --configuration Release src/*sln

    dotnet nuget push src/SnapTest/bin/Release/SnapTest.$VERSION.nupkg --source github

    dotnet nuget push src/SnapTest.NUnit/bin/Release/SnapTest.NUnit.$VERSION.nupkg --source github

    dotnet nuget push src/SnapTest.Xunit/bin/Release/SnapTest.Xunit.$VERSION.nupkg --source github
    ```

    GitHub Personal Access Tokens can be managed at https://github.com/settings/tokens

### Git tag a version

```shell
VERSION="<Version released>"
git tag $VERSION
git push --tag origin
```
