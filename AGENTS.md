# Agent Guide for ZYC.Framework

This file defines house rules for any AI/code assistant working in this repository.
It applies to the entire repository unless the user explicitly overrides it.

## Scope
- Applies to all directories under this repository root.
- Higher-precedence instructions may appear in subfolders (additional AGENTS.md files).

## Review Focus (Do)
- Prioritize API design, naming consistency, visibility, dependency boundaries, and correctness.
- Prefer minimal, surgical changes aligned with existing architecture and style.
- Provide module dependency graphs as Mermaid when requested (graph TD).
- Keep explanations concise and action-oriented.

## Out-of-Scope Topics (Don’t)
- Do NOT suggest enabling or changing code analyzers, StyleCop/FXCop/Roslyn rules, or linting tools.
- Do NOT discuss or recommend modifying `RunAnalyzersDuringBuild` (treat its current value as a given).
- Do NOT propose changes to the build pipeline, CI, or solution-wide props/targets unless the user asks.
- Do NOT introduce large new dependencies or frameworks without an explicit request.

## Technology Boundaries
- Abstractions projects target `net10.0` with `UseWPF=false`.
- It is acceptable to reference `System.Windows.Input.ICommand` from Abstractions; this does not require a Windows TFM. Do not propose moving it or changing TFMs unless asked.

## Naming & Conventions
- Interfaces: `I` prefix, PascalCase (e.g., `IUpdateManager`).
- Methods: PascalCase; async methods must end with `Async` when returning `Task`/`ValueTask`.
- Events and DTOs: PascalCase; avoid typos. For TaskManager events, use the pattern `ManagedTask*Event` (e.g., `ManagedTaskCompletedEvent`, `ManagedTaskFaultedEvent`, `ManagedTaskCreationFaultedEvent`).
- Namespaces: file-scoped; match folder structure where practical.
- Avoid adding `sealed` unless it is required for correctness, API contract, or an explicit user request.
- XML docs: required for public APIs in `*.Abstractions` projects; optional elsewhere.
- New files must use `CRLF` line endings and `UTF-8 with BOM` encoding.

## Safety & Change Policy
- When renaming types, update file names and all references across the solution.
- Preserve existing target frameworks, output paths, and packaging structure.
- Prefer additive and backward-compatible changes unless the user approves breaking changes.
- Avoid running tests or local builds unless the user explicitly asks for them.

## Response Style for Assistants
- Be concise and specific; avoid filler.
- When delivering diagrams, use Mermaid `graph TD`.
- When listing files/paths in responses, use absolute or repo-root relative paths.

## Exceptions
- If the user explicitly asks for analyzer/build/CI discussion or changes, you may cover them for that request only.
