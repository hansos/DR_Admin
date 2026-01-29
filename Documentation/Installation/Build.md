# Build the solution

Below is a **precise, end-to-end explanation** of how to build a **.NET 10** application when:

- **Solution root**: `DR_Admin`

- **Project folder**: `DR_Admin/DR_Admin`

- Target tools:
  - **Visual Studio**
  - **Command line (dotnet CLI)**

The instructions apply whether you are building the **entire solution** or **a single project**.

## 1. Folder Structure

```
DR_Admin/
│
├─ DR_Admin.sln
│
└─ DR_Admin/
   ├─ DR_Admin.csproj
   ├─ Program.cs
   └─ ...
```

## 2. Build with Visual Studio (Windows)

### Build the entire solution

1. Open Visual Studio
2. Select File → Open → Project/Solution
3. Open `DR_Admin/DR_Admin.sln`
4. Select configuration:
  - Debug or Release
  - Target framework: .NET 10
5. Build
  - Menu: Build → Build Solution
  - Shortcut: Ctrl + Shift + B

✔ This builds all projects in the solution.

### Build a single project only

1. In Solution Explorer, Right-click: DR_Admin (project)
2. Select Build

## 3. Build from command line

### Build solution

From `DR_Admin`:
```bash
dotnet build
```

or explicitly:
```bash
dotnet build DR_Admin.sln
```

### Build single project

From solution root:
```bash
dotnet build DR_Admin/DR_Admin/DR_Admin.csproj
```

From project folder:
```bash
cd DR_Admin/DR_Admin
dotnet build
```

---
### Build in Release mode

```bash
dotnet build -c Release
```

### Clean and rebuild
```bash
dotnet clean
dotnet build
```

Or:
```bash
dotnet clean DR_Admin.sln
dotnet build DR_Admin.sln
```
## 4. Build vs Run vs Publish (Important Distinction)

| Command          | Purpose                    |
| ---------------- | -------------------------- |
| `dotnet build`   | Compiles code only         |
| `dotnet run`     | Builds + runs              |
| `dotnet publish` | Produces deployable output |

Example publish:

```bash
dotnet publish DR_Admin/DR_Admin.csproj -c Release
```

Output:

```bash
DR_Admin/DR_Admin/bin/Release/net10.0/publish/
```

---

## 6. Common .NET 10 Notes

- Ensure `global.json` (if present) matches SDK version
- Verify target framework in `.csproj`: `<TargetFramework>net10.0</TargetFramework>`

---