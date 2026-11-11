# HeavenMariaBeyond.Wpf

WPF client for browsing and managing HeavenMariaBeyond data.

## Quick Start

### Requirements

- .NET SDK 9.0
- Windows (WPF target: `net9.0-windows7.0`)

### Build

```powershell
dotnet build .\HeavenMariaBeyond.sln -c Release
```

### Test

```powershell
dotnet test .\HeavenMariaBeyond.sln -c Release
```

### Run

```powershell
dotnet run --project .\HeavenMariaBeyond.Wpf\HeavenMariaBeyond.Wpf.csproj -c Debug
```

## Project Structure

- `HeavenMariaBeyond.Wpf`: UI layer
- `HeavenMariaBeyond.Service`: use cases, orchestration, DI extension
- `HeavenMariaBeyond.Infrastructure`: DB and external system implementations
- `HeavenMariaBeyond.Domain`: domain models and contracts
- `HeavenMariaBeyond.Service.Tests`: service and architecture guard tests

## Architecture Rule

- `Wpf -> Service -> Domain`
- `Service -> Infrastructure -> Domain`
- `Wpf` must not directly reference `Infrastructure`

## CI

CI runs on GitHub Actions:

- restore
- build (`-warnaserror`)
- test

See:

- `.github/workflows/dotnet-ci.yml`
- `Directory.Build.props`

## Developer Notes

Detailed refactoring history and technical notes are in:

- `README.DEV.md`
