# Changelog

All notable changes to this package will be documented in this file.

## Unreleased

### Changed

- iOS native dependency now uses Swift Package Manager (`bitlabs-ios-sdk` 4.2.0) instead of CocoaPods. The Swift bridge imports `BitLabsUnity` instead of `BitLabs`.
- Replaced EDM4U (`com.google.external-dependency-manager`) with Unity's External Dependency Manager (`com.unity.external-dependency-manager`), installed automatically as a package dependency. EDM4U no longer needs to be added manually.

## 4.0.0

### Added

- UPM package structure
- Replaced UnitySendMessage with callback-based architecture
