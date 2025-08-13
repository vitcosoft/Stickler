# Stickler 🎯

*High-performance .NET architectural testing that teaches as it tests*

[![License](https://img.shields.io/badge/license-Apache%202.0-blue.svg)](LICENSE)
[![.NET 8+](https://img.shields.io/badge/.NET-8%2B-purple.svg)]()
[![Status](https://img.shields.io/badge/status-proof%20of%20concept-orange.svg)]()

## What is Architectural Testing? 🏗️

Architectural testing validates that your code follows the structural rules and design patterns you've established for
your application. When you decide that Controllers should not directly reference Data Access classes, or that your
Domain layer should remain independent of Infrastructure concerns, architectural tests automatically verify these
constraints.

These tests examine the static structure of your code—the classes, namespaces, dependencies, and relationships between
components—rather than testing runtime behavior. They catch architectural violations during your build process,
preventing design erosion before it reaches production. This automated enforcement means your team can focus on feature
development while maintaining consistent architectural quality.

## Why Stickler? 🤔

I love the pioneering work of [NetArchTest](https://github.com/BenMorris/NetArchTest)
and [ArchUnit](https://github.com/TNG/ArchUnit) (the Java inspiration). They proved that architectural testing belongs
in every serious development toolkit. Stickler builds on their foundation with some key differences:

**Performance First**: Uses `System.Reflection.Metadata` instead of reflection for 5-10x faster analysis. Your CI/CD
pipeline will thank you.

**Teaching Through Testing**: Rich diagnostic information doesn't just tell you what's wrong—it explains why it matters
and how to fix it.

**Modern .NET**: Built for .NET 8+ to leverage cutting-edge runtime optimizations and language features.

**Guided Discovery**: The fluent API teaches architectural thinking by making good patterns obvious and bad patterns
difficult.

## The Vision (Coming Soon!) ✨

```csharp
// Ensure your Controllers stay in their lane
var result = Types.From(assembly)
    .That().ResideInNamespace("Controllers")
    .Should().Not().DependOn("Data")
    .Execute();

// Validate Clean Architecture boundaries
var domainResult = Types.From(assembly)
    .That().ResideInNamespace("Domain")
    .Should().Not().HaveDependencyOn("Infrastructure")
    .Execute();

// Rich diagnostics help you learn and fix issues
if (!result.IsSuccessful)
{
    Console.WriteLine(result.Summary);
    // "Controller 'UserController' violates architecture:
    //  Field 'userRepository' creates direct dependency on Data.UserRepository
    //  Consider: Inject IUserRepository interface instead
    //  Learn more: [link to dependency inversion explanation]"
}
```

## Current Status 🚧

*"Art is never finished, only abandoned."* — Leonardo da Vinci

**Reality Check**: This is a proof-of-concept in active development. I'm building something awesome, but it's not ready
for production use yet.

The core architecture is designed, the performance benchmarks are planned, and the first working APIs should emerge in
the coming weeks. If you're interested in architectural testing or high-performance .NET libraries, this journey might
be worth following. Consider starring the repo to stay updated, or if you're feeling
generous, [sponsoring the work](https://github.com/sponsors/vitcosoft)
helps [keep the coffee](https://buymeacoffee.com/vitcosoft) flowing! ☕

## Design Philosophy 🎨

*"Haec autem ita fieri debent, ut habeatur ratio firmitatis, utilitatis, venustatis."* — Vitruvius  
*("All these [buildings] should have strength, utility, and beauty.")*

Stickler makes some deliberate choices that shape everything we build:

**Single Assertion Per Chain**: Each fluent expression tests one thing clearly, making failures easy to understand and
fix.

**Performance Over Compatibility**: Targeting modern .NET means we can use the fastest possible approaches without
legacy compromises.

**Clarity Over Cleverness**: The API teaches architectural concepts through natural usage patterns rather than requiring
deep framework knowledge.

**Rich Diagnostics**: When tests fail, you get educational explanations that help you understand the architectural
principles involved.

## Standing on the Shoulders of Giants 🙏

*"If I have seen further, it is by standing on the shoulders of giants."* — Isaac Newton

Huge appreciation to the creators of [NetArchTest](https://github.com/BenMorris/NetArchTest)
and [ArchUnit](https://github.com/TNG/ArchUnit). They proved that architectural testing could be practical, valuable,
and accessible. Stickler exists because they showed us the way.

## Roadmap 🗺️

*"Make it work, make it right, make it fast."* — Kent Beck

I'm building this like a proper three-course meal — can't serve dessert before the appetizer:

**Phase 1**: Core performance foundation and basic fluent API  
**Phase 2**: Rich diagnostics and comprehensive rule coverage  
**Phase 3**: Documentation, examples, and community enablement

A detailed roadmap will be shared soon for those who enjoy watching the sausage being made.

## Contributing 🤝

*"Workers of the world, unite!"* — Karl Marx & Friedrich Engels

I'm not accepting contributions yet—this is still architectural foundation work where too many cooks would definitely
spoil the broth. After version 1.0.0, I'll open things up for community contributions with proper guidelines and
infrastructure.

For now, feel free to watch the repo, open issues for feedback, or start discussions about architectural testing
approaches you'd like to see supported.

## License 📄

Apache 2.0 - see [LICENSE](LICENSE) file for details.
