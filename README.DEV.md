# HeavenMaria.Wpf Developer Notes

This document summarizes the refactoring context and current technical baseline.

## Goals of Refactoring

- Reduce responsibilities in `HeavenMaria.Wpf` ViewModels
- Move use-case logic into `HeavenMaria.Service`
- Remove direct UI dependency on `Infrastructure`
- Enforce quality gates with CI (`build + test + analyzer`)

## Current Dependency Direction

- `Wpf -> Service -> Domain`
- `Service -> Infrastructure -> Domain`
- `Wpf -X-> Infrastructure` (disallowed)

## What Was Refactored

### 1) Query use-cases extracted from `MainWindowViewModel`

Added:

- `IMainWindowQueryService`
- `MainWindowQueryService`

Moved from ViewModel:

- visible girls load and sorting
- diaries by selected girl
- movie diaries load
- girls-by-key query
- `GetGirlById`

### 2) Status update use-cases extracted

Added:

- `IMainWindowStatusService`
- `MainWindowStatusService`

Moved from ViewModel:

- `HideThisGirl`
- `ReverseStatus` branching logic

### 3) SelectMode use-cases extracted

Added:

- `ISelectModeService`
- `SelectModeService`

Result:

- `SelectModeViewModel` no longer calls repositories directly

### 4) External launch concerns extracted

Added:

- `IExternalLauncherService`
- `ExternalLauncherService`

Moved out of UI:

- `Process.Start`
- directory/file probing logic

### 5) Diary display use-case extracted

Added:

- `IDiaryDisplayService`
- `DiaryDisplayService`

Moved out of ViewModel:

- diary retrieval + HTML generation pipeline

### 6) Select mode state centralized

Added:

- `ISelectModeStateService`
- `SelectModeStateService`

Result:

- Removed direct dependency: `SelectModeViewModel -> MainWindowViewModel`
- Replaced direct `NowInSelectMode` flag with shared state service

### 7) DI boundary reorganized

Added:

- `ContainerRegistryExtensions.RegisterHeavenMariaCore()`

Result:

- `App.xaml.cs` now delegates core registrations
- `HeavenMaria.Wpf.csproj` no longer references `HeavenMaria.Infrastructure` directly

### 8) Tests added

Created test project:

- `HeavenMaria.Service.Tests`

Implemented:

- `MainWindowStatusServiceTests`
- `SelectModeServiceTests`
- `ArchitectureGuardTests`

Architecture guards validate:

- `Wpf` project file does not reference `Infrastructure`
- app bootstrapper uses `RegisterHeavenMariaCore()`

### 9) Warning cleanup and strict CI readiness

Fixed nullable and obsolete warnings across Domain/Infrastructure to enable strict build:

- `dotnet build -warnaserror` passes (CI mode)

## CI / Quality Gate

Files:

- `.github/workflows/dotnet-ci.yml`
- `Directory.Build.props`

Behavior:

- analyzers enabled
- warnings treated as errors on CI (`CI=true`)
- restore -> build (`-warnaserror`) -> test

## Current Validation Status

Verified commands:

```powershell
dotnet build .\HeavenMaria.Wpf.sln -c Release -warnaserror
dotnet test .\HeavenMaria.Wpf.sln -c Release
```

Current expected result:

- build: success
- tests: all pass

## Suggested Next Steps

1. Introduce ValueObjects (`GirlId`, `DiaryCode`, `ProfileUrl`)
2. Continue SQL parameterization cleanup
3. Expand tests for query/display services
