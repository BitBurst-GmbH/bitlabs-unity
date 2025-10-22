# UPM Package Migration Study for BitLabs Unity SDK

Based on research into Unity Package Manager (UPM) and analysis of the current project structure, this is a comprehensive guide on converting from .unitypackage to UPM distribution.

---

## Table of Contents

- [What is UPM?](#what-is-upm)
- [Key Differences: .unitypackage vs UPM](#key-differences-unitypackage-vs-upm)
- [Required UPM Package Structure](#required-upm-package-structure)
- [Critical: package.json Manifest](#critical-packagejson-manifest)
- [Assembly Definitions (MANDATORY)](#assembly-definitions-mandatory)
- [Distribution Methods](#distribution-methods)
- [Migration Steps](#migration-steps)
- [Special Considerations for BitLabs](#special-considerations-for-bitlabs)
- [Recommendations](#recommendations)
- [Documentation Updates Needed](#documentation-updates-needed)
- [Benefits for Users](#benefits-for-users)
- [Next Steps](#next-steps)

---

## What is UPM?

Unity Package Manager (UPM) is Unity's modern package management system that provides:
- **Version control** and dependency management
- **Easier updates** for users through Package Manager UI
- **Better code organization** with mandatory Assembly Definitions
- **Non-destructive installation** (packages don't pollute Assets folder)
- **Multiple distribution methods** (Git, Scoped Registry, Local)

---

## Key Differences: .unitypackage vs UPM

| Aspect | .unitypackage (Current) | UPM Package |
|--------|------------------------|-------------|
| **Installation** | Import into Assets folder | Lives in Packages folder |
| **Updates** | Manual re-import, overwrites | Managed via Package Manager |
| **Dependencies** | Manual management | Automatic via package.json |
| **Assembly Definitions** | Optional | **Mandatory** for code |
| **Structure** | Flexible | Strict folder conventions |
| **Removal** | Can leave orphaned files | Clean removal |
| **Samples** | Mixed with SDK | Separate Samples~ folder |

---

## Required UPM Package Structure

Your package needs to be restructured to follow this convention:

```
com.bitburst.bitlabs/                    # Package root
├── package.json                         # REQUIRED: Package manifest
├── README.md                            # Package description
├── CHANGELOG.md                         # Version history
├── LICENSE.md                           # MIT License text
├── THIRD PARTY NOTICES.md              # If using 3rd party code
│
├── Runtime/                             # Runtime code (moved from Source/Runtime)
│   ├── BitLabs.cs
│   ├── BitLabsWidget.cs
│   ├── SurveyContainer.cs
│   ├── LeaderboardScript.cs
│   ├── UIGradient.cs
│   ├── UIGradientUtils.cs
│   ├── BitLabs.Runtime.asmdef          # REQUIRED: Assembly Definition
│   └── Models/
│       ├── Survey.cs
│       ├── User.cs
│       ├── Leaderboard.cs
│       ├── Category.cs
│       ├── Currency.cs
│       ├── Reward.cs
│       └── Symbol.cs
│
├── Editor/                              # Editor-only code (if any)
│   └── BitLabs.Editor.asmdef           # REQUIRED if folder exists
│
├── Plugins/                             # Native plugins
│   ├── iOS/
│   │   ├── BitLabsWrapper.h
│   │   ├── BitLabsWrapper.mm
│   │   ├── WidgetWrapper.h
│   │   └── WidgetWrapper.mm
│   └── Android/
│       ├── mainTemplate.gradle
│       ├── settingsTemplate.gradle
│       └── gradleTemplate.properties
│
├── Resources/                           # Prefabs and Sprites
│   ├── Prefabs/
│   │   ├── SimpleWidget.prefab
│   │   ├── CompactWidget.prefab
│   │   ├── FullWidthWidget.prefab
│   │   ├── Leaderboard.prefab
│   │   └── ...
│   └── Sprites/
│       └── [sprite assets]
│
├── Samples~/                            # Note the tilde! (hidden in Unity)
│   └── DemoScene/
│       ├── Demo.cs
│       ├── Demo.unity
│       └── DemoScene.asmdef            # Assembly def for sample
│
├── Tests/                               # Unit tests (optional but recommended)
│   ├── Runtime/
│   │   └── BitLabs.Tests.asmdef
│   └── Editor/
│       └── BitLabs.Editor.Tests.asmdef
│
└── Documentation~/                      # Local documentation (optional)
    └── com.bitburst.bitlabs.md
```

---

## Critical: package.json Manifest

This is the heart of your UPM package. Create this at the package root:

```json
{
  "name": "com.bitburst.bitlabs",
  "version": "3.2.10",
  "displayName": "BitLabs SDK",
  "description": "Official Unity SDK for BitLabs survey monetization platform. Integrate surveys into your Unity games to create a new revenue stream through user engagement.",
  "unity": "2022.3",
  "unityRelease": "25f1",

  "keywords": [
    "BitLabs",
    "surveys",
    "monetization",
    "ads",
    "revenue",
    "rewards",
    "leaderboard",
    "mobile"
  ],

  "author": {
    "name": "BitBurst GmbH",
    "email": "support@bitlabs.ai",
    "url": "https://www.bitlabs.ai"
  },

  "license": "MIT",
  "licensesUrl": "https://github.com/BitBurst-GmbH/bitlabs-unity/blob/master/LICENSE",

  "documentationUrl": "https://developer.bitlabs.ai/docs/unity-sdk-v3",
  "changelogUrl": "https://github.com/BitBurst-GmbH/bitlabs-unity/blob/master/CHANGELOG.md",

  "dependencies": {
    "com.unity.textmeshpro": "3.0.6",
    "com.unity.ugui": "1.0.0"
  },

  "samples": [
    {
      "displayName": "BitLabs Demo Scene",
      "description": "Example integration showing how to initialize BitLabs SDK, display surveys, handle rewards, and implement leaderboards.",
      "path": "Samples~/DemoScene"
    }
  ]
}
```

**Key Naming Convention**: The package name MUST follow reverse domain notation with at least 3 segments: `com.[company].[package-name]`

---

## Assembly Definitions (MANDATORY)

UPM packages **require** Assembly Definition files (.asmdef) for all code. This improves compilation times and enforces proper code organization.

### Runtime/BitLabs.Runtime.asmdef

```json
{
  "name": "BitLabs.Runtime",
  "rootNamespace": "BitLabs",
  "references": [
    "Unity.TextMeshPro"
  ],
  "includePlatforms": [],
  "excludePlatforms": [],
  "allowUnsafeCode": false,
  "overrideReferences": false,
  "precompiledReferences": [],
  "autoReferenced": true,
  "defineConstraints": [],
  "versionDefines": [],
  "noEngineReferences": false
}
```

### Samples~/DemoScene/DemoScene.asmdef

```json
{
  "name": "BitLabs.Samples.Demo",
  "rootNamespace": "BitLabs.Samples",
  "references": [
    "BitLabs.Runtime"
  ],
  "includePlatforms": [],
  "excludePlatforms": [],
  "allowUnsafeCode": false,
  "overrideReferences": false,
  "precompiledReferences": [],
  "autoReferenced": true,
  "defineConstraints": [],
  "versionDefines": [],
  "noEngineReferences": false
}
```

---

## Distribution Methods

You have three main options for distributing your UPM package:

### Comparison: Unity Editor UI Browsing

| Distribution Method | Browsable in Unity Editor UI | Installation Method | Public/Private Support |
|---------------------|------------------------------|---------------------|------------------------|
| **Git Repository** | ❌ No - Manual URL entry required | Add from Git URL | ✅ Public & Private (with auth) |
| **OpenUPM** | ✅ Yes - Under "My Registries" | Browse & Install button | ⚠️ Public only |
| **Custom Scoped Registry** | ✅ Yes - Under "My Registries" | Browse & Install button | ✅ Public & Private |

### 1. Git Repository (Recommended for Open Source)

**Pros:**
- Free and easy to set up
- Users install via Git URL
- Automatic updates when users refresh
- Supports versioning via Git tags
- **Works with private repositories** (requires authentication setup)

**Cons:**
- ❌ **Not browsable in Unity Editor UI** - Users must manually enter Git URL
- Requires users to have Git installed
- No built-in version picker in Unity UI (users must specify tags manually)
- Slightly more technical for end users
- Private repos require credential configuration

**How it works:**
1. Restructure your repository with package.json at root (or in a subfolder)
2. Create Git tags for versions (e.g., `v3.2.10`)
3. Users install via Package Manager → Add package from git URL:

```
https://github.com/BitBurst-GmbH/bitlabs-unity.git
```

Or with specific version:
```
https://github.com/BitBurst-GmbH/bitlabs-unity.git#v3.2.10
```

Or if package is in subfolder:
```
https://github.com/BitBurst-GmbH/bitlabs-unity.git?path=/Packages/com.bitburst.bitlabs
```

**Git Tag Requirements:**
- Use semantic versioning: `v3.2.10`
- Tag the commit when you release a version
- Unity will use tags for version resolution

**Private Repository Support:**

Git URLs work with private repositories, but require authentication setup:

**For HTTPS URLs (Recommended for Private Repos):**
1. Users must configure Git Credential Manager once:
   ```bash
   git config --global credential.helper manager
   ```
2. Test access once to store credentials:
   ```bash
   git ls-remote --heads https://github.com/BitBurst-GmbH/bitlabs-unity HEAD
   ```
3. Enter credentials (or Personal Access Token if using 2FA)
4. Package Manager will automatically use stored credentials

**For SSH URLs:**
1. Users must have SSH keys configured
2. Add SSH key to GitHub/GitLab account
3. If using passphrase-protected keys, must set up SSH agent
4. Use SSH URL format:
   ```
   git@github.com:BitBurst-GmbH/bitlabs-unity.git
   ```

**Important Limitation:** Unity Package Manager has **no terminal for entering credentials**, so interactive authentication doesn't work. Credentials must be pre-configured via Git Credential Manager or SSH keys.

### 2. OpenUPM (Community Registry)

**Pros:**
- Free community registry
- ✅ **Browsable in Unity Editor UI** - Shows under "My Registries"
- Provides proper version picker UI
- Better discoverability
- Can use OpenUPM CLI for installation
- One-click install button in Package Manager

**Cons:**
- ❌ **Public repositories only** - Cannot use for private packages
- Requires submitting to OpenUPM
- Must follow their conventions
- Slight approval process
- Requires users to add scoped registry configuration

**How it works:**
1. Submit your package to OpenUPM: https://openupm.com/docs/adding-upm-package.html
2. Users add scoped registry to `Packages/manifest.json`:

```json
{
  "scopedRegistries": [
    {
      "name": "OpenUPM",
      "url": "https://package.openupm.com",
      "scopes": [
        "com.bitburst"
      ]
    }
  ],
  "dependencies": {
    "com.bitburst.bitlabs": "3.2.10"
  }
}
```

Or via OpenUPM CLI:
```bash
openupm add com.bitburst.bitlabs
```

### 3. Custom Scoped Registry (Enterprise)

**Pros:**
- Full control over distribution
- Professional solution
- ✅ **Browsable in Unity Editor UI** - Shows under "My Registries"
- Built-in version picker in Unity
- **Can host private packages** with authentication
- One-click install button in Package Manager
- Searchable in Package Manager (if registry supports search API)
- Best user experience - looks like official Unity packages

**Cons:**
- Requires hosting infrastructure (Verdaccio, Azure Artifacts, npm registry, etc.)
- More complex setup
- Potential hosting costs
- Requires users to add scoped registry configuration to their project

**How it works:**
1. Set up a npm-compatible registry (e.g., Verdaccio)
2. Publish package to your registry
3. Users add your scoped registry to their project

---

## Migration Steps

Here's the step-by-step process to convert your current package:

### Phase 1: Restructure
1. Create new folder structure following UPM conventions
2. Move `Assets/BitLabs/Source/Runtime/` → `Runtime/`
3. Move `Assets/BitLabs/Source/Prefabs/` → `Resources/Prefabs/`
4. Move `Assets/BitLabs/Source/Sprites/` → `Resources/Sprites/`
5. Move `Assets/BitLabs/Samples/` → `Samples~/` (note the tilde!)
6. Keep `Plugins/` structure as-is (iOS/Android)

### Phase 2: Create Required Files
1. Create `package.json` with proper metadata
2. Create `Runtime/BitLabs.Runtime.asmdef`
3. Create `Samples~/DemoScene/DemoScene.asmdef`
4. Create/update `CHANGELOG.md`
5. Create/update `LICENSE.md`
6. Update `README.md` with UPM installation instructions

### Phase 3: Test Locally
1. Copy restructured package to `Packages/com.bitburst.bitlabs/` in a test project
2. Verify compilation with assembly definitions
3. Test sample scene imports correctly
4. Verify all prefabs and resources load
5. Test iOS and Android builds

### Phase 4: Version Control
1. Create Git tag for current version: `git tag v3.2.10`
2. Push tag: `git push origin v3.2.10`
3. Test Git URL installation in fresh project

### Phase 5: Distribution
Choose your distribution method:
- **Git only**: Update documentation with Git URL
- **OpenUPM**: Submit package to OpenUPM
- **Custom registry**: Set up and publish to registry

### Phase 6: Backward Compatibility
Consider maintaining both distribution methods temporarily:
- Keep generating .unitypackage for existing users
- Provide UPM option for new installations
- Eventually deprecate .unitypackage after migration period

---

## Special Considerations for BitLabs

### 1. External Dependency Manager
Your project uses External Dependency Manager (EDM4U) for iOS/Android dependencies. This works with UPM packages, but ensure:
- Include EDM4U dependency files in your package
- Document that EDM4U may auto-install if not present
- Test dependency resolution on clean projects

### 2. Native Plugins
Your iOS/Android native plugins will work in UPM, but:
- Keep them in `Plugins/iOS/` and `Plugins/Android/`
- Ensure .meta files are properly configured
- Test platform-specific builds thoroughly

### 3. TextMeshPro Dependency
Already correctly specified in your dependencies. Unity will auto-install TMP when users add your package.

### 4. Samples Folder
The `Samples~` folder (with tilde) is hidden from Unity's Project window. Users import samples via Package Manager UI:
- Package Manager → BitLabs SDK → Samples → Import
- This copies samples to `Assets/Samples/BitLabs SDK/3.2.10/DemoScene/`

---

## Recommendations

### For BitLabs, I recommend:

1. **Start with Git distribution** - It's the easiest to set up and your repo is already on GitHub
2. **Add to OpenUPM** - After Git distribution is stable, submit to OpenUPM for better discoverability
3. **Keep dual distribution** - Maintain .unitypackage generation for 1-2 versions during transition
4. **Version strategy**: Use semantic versioning consistently across both formats

### Migration Timeline:
- **Week 1**: Restructure and create package.json locally
- **Week 2**: Test thoroughly with assembly definitions
- **Week 3**: Release as Git-based UPM package alongside .unitypackage
- **Month 2**: Submit to OpenUPM
- **Month 3+**: Deprecate .unitypackage distribution

---

## Documentation Updates Needed

Update your integration docs (https://developer.bitlabs.ai/docs/unity-sdk-v3) to include:

```markdown
## Installation

### Via Git URL (Unity 2019.3+)
1. Open Package Manager (Window → Package Manager)
2. Click the + button → Add package from git URL
3. Enter: `https://github.com/BitBurst-GmbH/bitlabs-unity.git`

### Via OpenUPM
```bash
openupm add com.bitburst.bitlabs
```

### Via Unity Package (Legacy)
Download the .unitypackage from releases and import into your project.
```

---

## Benefits for Users

Moving to UPM provides:
- ✅ **Easier updates** - One click in Package Manager
- ✅ **Cleaner projects** - SDK doesn't clutter Assets folder
- ✅ **Dependency management** - TextMeshPro auto-installs
- ✅ **Faster compilation** - Assembly definitions improve build times
- ✅ **Non-destructive** - Easy to remove/update without leftover files
- ✅ **Version control friendly** - No SDK code in project git history

---

## Next Steps

Potential next actions:
1. **Create the package.json** and assembly definition files
2. **Generate the complete UPM folder structure** with migrated files
3. **Write a migration script** to automate the restructuring
4. **Create updated documentation** with installation instructions

---

## References

- [Unity Manual: Custom Packages](https://docs.unity3d.com/Manual/CustomPackages.html)
- [Unity Manual: Package Manifest](https://docs.unity3d.com/Manual/upm-manifestPkg.html)
- [Unity Manual: Git Dependencies](https://docs.unity3d.com/Manual/upm-git.html)
- [OpenUPM Documentation](https://openupm.com/docs/)
- [OpenUPM: Adding UPM Package](https://openupm.com/docs/adding-upm-package.html)