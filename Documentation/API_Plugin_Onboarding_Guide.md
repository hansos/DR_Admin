# API Plugin Onboarding Guide

This document explains how to add a new plugin type or plugin implementation to the API using the shared plugin architecture.

---

## Simple guide

Use these quick steps:

1. Add or reuse shared plugin primitives in `PluginLib` (`PluginType`, `IPlugin`, selector/registry contracts).
2. Add provider-neutral settings in the feature library (for example `EmailSettings.Selection`).
3. Implement one concrete plugin class per provider under the feature library `Plugins` area.
4. Register plugin instances in the feature factory/orchestrator.
5. Resolve plugins using `IPluginSelector` with default and fallback keys.
6. Keep API/business flows dependent on abstractions only (never concrete provider classes).

---

## Detailed instructions

### 1) Shared contracts and runtime (`PluginLib`)

- Add new plugin kind to `PluginLib/Plugins/PluginType.cs` when introducing a new plugin category.
- Keep shared contracts in `PluginLib`, not in feature libraries:
  - `IPlugin`
  - `IPluginSelector`
  - `PluginRegistry`
  - `PluginSelector`
- Ensure plugin metadata is present:
  - `Type`
  - `Key`
  - `DisplayName`
  - `Version`
  - `Capabilities`
  - `IsEnabled`

### 2) Feature library plugin contract

For each feature library (`EmailSenderLib`, `SMSSenderLib`, `PaymentGatewayLib`, etc.):

- Create a feature-specific plugin contract extending `IPlugin`.
- Keep provider creation and provider-specific mapping in plugin classes only.
- Keep shared policy decisions in the factory/orchestration layer.

Example pattern:

- `IEmailSenderPlugin : IPlugin`
- `CanCreate(settings)` validates provider configuration.
- `Create(settings)` returns provider implementation.

### 3) Configuration model

Use provider-neutral selection config in the feature settings object:

- `DefaultPluginKey`
- `FallbackPluginKeys`
- `DisabledPluginKeys`

Keep provider credentials/endpoints in provider-specific sections.

### 4) Runtime selection and fallback

- Build plugin list from enabled plugins.
- Initialize `PluginRegistry` and `PluginSelector`.
- Select with order:
  1. explicit runtime key,
  2. configured provider/default key,
  3. fallback chain,
  4. deterministic final fallback (first available plugin).
- If selected plugin is not configured, walk fallback keys and fail with a clear error if none are usable.

### 5) API integration rules

- API services/controllers should call feature facades/factories only.
- Do not instantiate provider classes directly in API code.
- Do not hardcode provider names in business workflows.
- Keep tenant/use-case override logic policy-driven.

### 6) Validation checklist

Before merging:

- Plugin key is unique inside its `PluginType`.
- Disabled plugin behavior is respected.
- Default and fallback selection is deterministic.
- Plugin metadata is complete.
- Failure in one provider does not break unrelated plugin types.
- Build succeeds for the updated projects.
