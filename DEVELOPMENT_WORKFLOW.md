# Development Workflow

This document describes the working process used for implementing new functionality, from task definition to pull request completion.

## Overview

The development workflow is based on close collaboration with GitHub Copilot and related tools, using an iterative and review-driven approach to ensure correctness, completeness, and code quality.

## Workflow Steps

### 1. Task Definition and Planning

- For each new feature or change, the task requirements and details are clearly described upfront.
- A **Plan** is created to outline the intended implementation approach.
- GitHub Copilot is used to generate an initial draft based on this plan.
- The generated content is carefully reviewed.
- If any parts are missing, unclear, or incorrect, Copilot is instructed to refine or extend the solution.

### 2. Issue Creation

- Once the plan and implementation details are validated, the task is moved to Cloud.
- Copilot is then asked to create a corresponding GitHub issue based on the finalized description.

### 3. Pull Request Generation

- GitHub Copilot generates a Pull Request (PR) implementing the described functionality.
- The PR is thoroughly reviewed:
  - Code quality
  - Logic correctness
  - Missing edge cases
  - Consistency with project standards

### 4. Review and Iteration

- Any issues found during review are written as comments directly in the PR.
- Copilot reads these comments and applies the necessary fixes.
- This review–fix cycle continues until all concerns are resolved.

### 5. Pull Request Completion

- Once the implementation meets all requirements and no further issues remain, the pull request is approved and completed (merged).

### 6. Agent Mode for Small Fixes

- For small or straightforward issues, **Agent Mode** is sometimes used instead of the full Cloud workflow.
- This approach is faster, as it allows quick fixes without waiting for full PR regeneration.

### 7. Tooling

- During implementation, the **GitHub MCP server** is used to support development and automation tasks.
