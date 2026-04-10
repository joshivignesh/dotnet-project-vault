# 08-sonarqube-github-actions

## Purpose
Planned CI quality sample for static analysis and automated quality gates.

## Stack
- .NET 10
- C# 14
- GitHub Actions
- SonarQube/SonarCloud

## Status
Scaffolded

## Next step
Wire SonarCloud/SonarQube tokens in repository secrets and tune quality gates.

## Current scaffold
- SonarGithubActions.slnx
- src/SonarGithubActions.Api
- tests/SonarGithubActions.Tests
- sonarqube.properties
- .github/workflows/sonarqube-dotnet.yml