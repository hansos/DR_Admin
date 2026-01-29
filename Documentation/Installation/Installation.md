# Getting the DR_Admin Source Code

You can obtain the source code for **DR_Admin** either by cloning the repository with Git or downloading it as a ZIP file.

---

## 1. Cloning the Repository (Recommended for Development)

If you plan to contribute or actively develop the project, using Git is recommended.

1. **Install Git**  
   - Windows: [Git for Windows](https://git-scm.com/download/win)  
   - macOS: Install via Homebrew `brew install git` or download from [git-scm.com](https://git-scm.com/download/mac)  
   - Linux: Use your package manager, e.g., `sudo apt install git`

2. **Open a terminal or command prompt.**

3. **Clone the repository:**
```bash
git clone https://github.com/hansos/DR_Admin.git
cd DR_Admin
git branch -a
git checkout branch-name

```

---

## 2. Downloading as a ZIP (No Git Needed)

If you just want to explore or run the code without contributing:

1. Go to the repository page: [https://github.com/hansos/DR_Admin](https://github.com/hansos/DR_Admin)   

2. Click the green **Code** button.

3. Select **Download ZIP**.

4. Extract the ZIP file to a folder on your computer.

---

## 3. Opening the Project in Visual Studio

Since DR_Admin is a .NET project:

1. Open Visual Studio.

2. Select Open a project or solution.

3. Navigate to the folder containing the cloned or extracted source.

4. Open the .sln file to load the solution.

5. Restore NuGet packages if prompted:
```bash
Tools → NuGet Package Manager → Restore Packages
```

---
## Notes for Developers

- Using Git keeps your copy in sync with updates and allows you to contribute.
- Using ZIP is simpler but static — you won’t get updates automatically.
- Ensure your environment matches the project requirements (NET 10 SDK).
 
---
## Next step
Before starting the API application, modify the `appsettings.json` configuration file. See the [Configuration page](Installation/Configuration.md) for more information.

