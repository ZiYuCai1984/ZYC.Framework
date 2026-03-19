# Agent Guide for ZYC.Framework.Build.Doc

## Scope
- Applies to `src/ZYC.Framework.Build.Doc` and all descendants.
- Covers both the documentation generator code and the template sources under `Templates/`.

## Source of Truth
- Treat `Templates/README/README.md` and `Templates/docs/*` as the source of truth for generated documentation.
- Do not directly edit generated files at the repository root such as `README.md`, `README.*.md`, or `docs/*` unless the user explicitly asks for an output-only hotfix.
- When documentation content must change, update the template first. Regenerate outputs only when the user asks for it.

## Template Variables
- `$(Name)` placeholders must resolve from `src/version.props`, `src/nuget.props`, `ProductInfo`, `ProductInfoExtended`, or `Templates/variables.json`.
- Before introducing a new placeholder, ensure one of those sources provides it.
- Keep placeholder names stable and case-consistent.

## README Localization Blocks
- Preserve the exact `<!--doc-l10n:begin ...-->`, `<!--doc-l10n:locale ...-->`, and `<!--doc-l10n:end-->` structure.
- Keep the default content first, followed by locale-specific content.
- Keep locale coverage, section order, and section boundaries aligned across languages.
- Do not rename locale codes or add new locales without updating the generator behavior and the README language links together.

## Template Metadata
- `<!--doc-meta: ... -->` comments are editor-only metadata and are stripped from generated output.
- Keep metadata concise and avoid placing user-visible content inside `doc-meta` comments.

## Change Style
- Prefer small, surgical edits that fit the existing generator design.
- Preserve current output paths, naming, and generation flow unless the user explicitly asks for a change.
- Do not introduce a new templating syntax, parser rule, or dependency without an explicit request.
