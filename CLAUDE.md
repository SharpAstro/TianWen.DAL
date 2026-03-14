# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

TianWen.DAL is a vendor-neutral device access layer (DAL) for consumer astronomical hardware (CMOS cameras from ZWO, SvBony, etc.). It is a pure interface/abstraction library — no concrete implementations live here. Vendor-specific implementations exist in separate repositories under the SharpAstro organization.

Published as a NuGet package (`TianWen.DAL`).

## Build Commands

```bash
dotnet restore
dotnet build
dotnet build -c Release
```

No test project exists in this repository.

## Architecture

This is a .NET Standard 2.0 class library defining interfaces and enums only — no concrete classes beyond `NativeDeviceIteratorBase<T>` and the `Common` utility.

**Core abstractions (top-down):**

- `INativeDeviceIterator<T>` — enumerates connected devices of a given type. `NativeDeviceIteratorBase<T>` provides a base implementation using `DeviceCount()` + `GetDeviceInfo(index)`.
- `INativeDeviceInfo` — represents a connected device (ID, name, serial number, USB3 detection, open/close lifecycle).
- `ICMOSNativeInterface` — extends `INativeDeviceInfo` with full CMOS camera control: sensor properties, exposure workflow (start/stop/status/download), ROI/binning, gain/temperature controls, pulse guiding, and cooler management.

**Supporting enums:**

- `CMOSControlType` — camera control parameters (gain, exposure, temperature, cooler, etc.)
- `CMOSErrorCode` — error codes returned by camera operations
- `ExposureStatus` — exposure state machine (Idle → Working → Success/Failed)
- `BayerPattern` — Bayer filter patterns encoded as bit-packed values, with `BayerPatternEx.GetOffsets()` for debayering offset calculation
- `PixelDataFormat` — pixel formats (RAW8, RAW16, RGB24, Y8)
- `GuideDirection` — ST4 guide port directions

## CI/CD

GitHub Actions workflow (`.github/workflows/dotnet.yml`) builds on .NET 10, generates a NuGet package, and publishes to nuget.org on push to `main`.
