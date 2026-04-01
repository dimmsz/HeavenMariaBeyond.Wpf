# HeavenMaria.Wpf

WPF client for browsing and managing HeavenMaria data.

## Quick Start

### Requirements

- .NET SDK 9.0
- Windows (WPF target: `net9.0-windows7.0`)

### Build

```powershell
dotnet build .\HeavenMaria.Wpf.sln -c Release
```

### Test

```powershell
dotnet test .\HeavenMaria.Wpf.sln -c Release
```

### Run

```powershell
dotnet run --project .\HeavenMaria.Wpf\HeavenMaria.Wpf.csproj -c Debug
```

## Project Structure

- `HeavenMaria.Wpf`: UI layer
- `HeavenMaria.Service`: use cases, orchestration, DI extension
- `HeavenMaria.Infrastructure`: DB and external system implementations
- `HeavenMaria.Domain`: domain models and contracts
- `HeavenMaria.Service.Tests`: service and architecture guard tests

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
