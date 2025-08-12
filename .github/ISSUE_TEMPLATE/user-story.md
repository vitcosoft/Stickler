---
name: User story
about: Describe functionality from the developer user perspective
title: "[STORY]"
labels: ''
assignees: ''

---

## User Story

**As a** [developer type - e.g., "developer implementing Clean Architecture", "team lead establishing governance"]  
**I want** [specific capability needed]  
**So that** [architectural testing benefit provided]

## Requirements

### What should this do?
[Core functionality description - focus on behavior, not implementation]

### How should it integrate with the API?
[Fluent API integration - new filter, assertion, or configuration option]

### Usage Example
```csharp
// Show how a developer would use this functionality
var result = Types.From(assembly)
    .That().[filtering]
    .Should().[assertion]
    .Execute();
```

## Acceptance Criteria

- [ ] [Specific testable criterion]
- [ ] [Another verifiable requirement]
- [ ] [Additional criteria as needed]

## Dependencies

**Requires:** [List blocking issues or external dependencies]

## Quality Checklist

- [ ] **Description clear and actionable** - Issue provides sufficient detail for implementation with clear acceptance criteria
- [ ] **Quality assured** - Unit, integration, performance, and regression testing requirements identified and planned
- [ ] **Thread safety implications** - Changes reviewed against established thread safety contracts
- [ ] **Performance impact** - Execution time, memory usage, and scalability implications evaluated
- [ ] **Documentation complete** - Updates planned for README, XML docs, Docus site, ADRs, and Mermaid diagrams as applicable
- [ ] **Compatibility and versioning** - Breaking changes identified with semantic versioning impact determined (major/minor/patch)
- [ ] **Rollback strategy** - Plan defined for reverting changes if implementation causes problems
- [ ] **Implementation approach validated** - Technical approach aligns with architectural commitments

## Definition of Done

- [ ] Functionality implemented and tested
- [ ] Unit and integration tests passing
- [ ] Documentation updated
- [ ] Code review completed
- [ ] Quality checklist verified
