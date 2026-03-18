# Forking and Cloning DR_Admin

[Back to Index](index.md)

## Overview

This guide walks through the process of forking the DR_Admin repository on GitHub and cloning your fork to a local directory. This is the recommended way to obtain the source code for local development.

## Why Fork Instead of Clone Directly?

Forking creates your own copy of the repository under your GitHub account. This allows you to:

- Make changes without affecting the original repository.
- Submit pull requests back to the upstream repository when you want to contribute.
- Keep your fork in sync with upstream changes at your own pace.

## Prerequisites

- A [GitHub](https://github.com/) account.
- [Git](https://git-scm.com/downloads) installed on your local machine.
- (Optional) [GitHub CLI](https://cli.github.com/) for a streamlined workflow.

## Step 1 — Fork the Repository

1. Navigate to the upstream repository on GitHub:  
   **<https://github.com/hansos/DR_Admin>**
2. Click the **Fork** button in the upper-right corner of the page.
3. Select your personal account (or the target organisation) as the owner of the fork.
4. Leave the default settings and click **Create fork**.

After the fork is created you will have your own copy at:

```
https://github.com/<your-username>/DR_Admin
```

## Step 2 — Clone Your Fork Locally

Open a terminal and run:

```powershell
# Navigate to the directory where you want the project to live
cd C:\Source2

# Clone your fork
git clone https://github.com/<your-username>/DR_Admin.git

# Move into the cloned directory
cd DR_Admin
```

> **Tip:** Replace `<your-username>` with your actual GitHub username.

## Step 3 — Add the Upstream Remote

Adding the original repository as an upstream remote lets you pull future changes from the main project:

```powershell
git remote add upstream https://github.com/hansos/DR_Admin.git
```

Verify the remotes are set up correctly:

```powershell
git remote -v
```

Expected output:

```
origin    https://github.com/<your-username>/DR_Admin.git (fetch)
origin    https://github.com/<your-username>/DR_Admin.git (push)
upstream  https://github.com/hansos/DR_Admin.git (fetch)
upstream  https://github.com/hansos/DR_Admin.git (push)
```

## Step 4 — Keep Your Fork Up to Date

Periodically pull changes from the upstream repository to stay current:

```powershell
# Fetch upstream changes
git fetch upstream

# Switch to your local master branch
git checkout master

# Merge upstream changes into your local master
git merge upstream/master

# Push the updated master to your fork on GitHub
git push origin master
```

## Workflow Diagram

```mermaid
flowchart TD
    A[Upstream Repository\nhansos/DR_Admin] -->|Fork| B[Your Fork\nyour-username/DR_Admin]
    B -->|git clone| C[Local Repository\nC:\Source2\DR_Admin]
    C -->|git push origin| B
    A -->|git fetch upstream| C
    C -->|Pull Request| A
```

## Importance for API Development

If you are developing solutions that consume the **DR_Admin API**, forking and maintaining a local copy of the repository is essential for the following reasons:

- **Access to API source & contracts** — The repository contains the API project, DTOs, and endpoint definitions. Having the source locally lets you inspect request/response models, understand validation rules, and review authentication requirements directly from the code rather than relying solely on external documentation.
- **Running the API locally** — A local clone allows you to spin up the API on your development machine, making it easy to test your integration against a live instance without depending on a remote environment.
- **Staying in sync with API changes** — By keeping your fork up to date with the upstream repository (see Step 4), you are immediately aware of breaking changes, new endpoints, or deprecated DTOs, giving you time to adapt your consuming applications before deploying.
- **Contributing fixes & improvements** — If you discover a bug or need an enhancement in the API while building your solution, your fork lets you make the change and submit a pull request back to the upstream project.

> **Recommendation:** Even if you are only consuming the API and not modifying the core DR_Admin code, maintaining an up-to-date fork ensures you always have a reliable local reference of the latest API contracts and can run integration tests against the real API codebase.
