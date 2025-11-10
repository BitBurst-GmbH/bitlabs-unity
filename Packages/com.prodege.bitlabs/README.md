# BitLabs Unity SDK

Official Unity SDK for [BitLabs](https://www.bitlabs.ai/) - Monetize your Unity games with surveys and more.

[![Unity Version](https://img.shields.io/badge/Unity-6000.0.2f-blue.svg)](https://unity.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

> **Note:** This SDK was built and tested on Unity 6000.0.2f (Unity 6). It should work on Unity 2021.3+ with proper Android build configuration (see Requirements below).

## Installation

### Prerequisites

This SDK requires **External Dependency Manager for Unity (EDM4U)** to resolve Android and iOS native dependencies.

**Install EDM4U first:**

Add to your `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.google.external-dependency-manager": "https://github.com/googlesamples/unity-jar-resolver.git?path=upm#v1.2.186"
  }
}
```

Or download from: [EDM4U](https://github.com/googlesamples/unity-jar-resolver)

### Option 1: Install via UPM (Unity Package Manager)

Add to your `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.prodege.bitlabs": "https://github.com/BitBurst-GmbH/bitlabs-unity.git?path=Packages/com.prodege.bitlabs"
  }
}
```

### Option 2: Install via .unitypackage

1. Download the latest `.unitypackage` from [Releases](https://github.com/BitBurst-GmbH/bitlabs-unity/releases)
2. Import into your Unity project: **Assets → Import Package → Custom Package**

## Quick Start

1. **Get your API Token** from [BitLabs Dashboard](https://dashboard.bitlabs.ai/)

2. **Initialize the SDK:**

```csharp
using BitLabs;

void Start()
{
    BitLabs.Init("YOUR_API_TOKEN", "USER_ID");
}
```

3. **Show surveys:**

```csharp
BitLabs.ShowOfferwall();
```

For a complete integration example, check the **Demo Scene** included with the package:

- Import via **Package Manager → BitLabs → Samples → Demo Scene**
- Reference implementation in `Demo.cs`

## Documentation

Full documentation: [BitLabs Unity SDK Guide](https://developer.bitlabs.ai/docs/unity-sdk-v3)

## Requirements

- **Unity:** 2021.3 or higher recommended
  - Unity 2023.1+ works out of the box
  - Unity 2021.3 - 2022.3: Requires manual Android build configuration (see below)
- **Android:** API Level 21+ (Android 5.0+)
- **iOS:** iOS 11.0+

### Android Build Configuration (Unity 2021.3 - 2022.3)

If using Unity versions older than 2023.1, you must manually configure Android build settings to match the native SDK requirements:

**Required Versions:**

- **Minimum SDK:** 21
- **Target SDK:** 35
- **Compile SDK:** 35
- **AGP (Android Gradle Plugin):** 8.6.0
- **Kotlin:** 2.1.21

These can be configured in:

- **Edit → Project Settings → Player → Android → Other Settings**
- Or by editing `mainTemplate.gradle` and `gradleTemplate.properties`

**Note:** Unity 2023.1+ handles these configurations automatically.

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for development setup and guidelines.

## Support

- Email: support@bitlabs.ai
- Documentation: https://developer.bitlabs.ai

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
