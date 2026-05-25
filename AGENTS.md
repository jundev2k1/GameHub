# Enterprise .NET Clean Architecture Rules

Use this document as `AGENTS.md`, `CLAUDE.md`, `.cursor/rules`, or an engineering handbook for enterprise .NET services. It is ordered by day-to-day development priority: coding rules first, then architecture, performance, AI generation, review, security, and tests.

## 1. Architecture And Folder Conventions

### Dependency Direction

Dependencies point inward:

```text
API -> Application -> Domain
Infrastructure -> Application
Persistence -> Application + Domain
Shared/Common -> stable primitives only
```

| Project        | Responsibility          | May Contain                                                                                            | Must Not Contain                                                          |
|----------------|-------------------------|--------------------------------------------------------------------------------------------------------|---------------------------------------------------------------------------|
| API            | HTTP adapter            | Controllers/endpoints, middleware, filters, auth policies, API requests/responses, OpenAPI, versioning | Business logic, DbContext, repository implementations, provider SDK calls |
| Application    | Use-case orchestration  | Commands, queries, handlers, validators, contracts, pipeline behaviors, app DTOs, mapping              | EF Core DbContext, concrete infrastructure, controllers, provider clients |
| Domain         | Business model          | Entities, aggregates, value objects, domain events, domain services, business rules                    | EF Core, MediatR, HTTP, logging, cache, provider DTOs                     |
| Persistence    | Database implementation | DbContext, EF configs, migrations, repositories, UoW, interceptors, seeds                              | Controllers, provider integrations, use-case workflows                    |
| Infrastructure | Technical integrations  | Cache, messaging, external APIs, jobs, file/email/security providers, resilience                       | Business ownership, API endpoints                                         |
| Shared/Common  | Stable primitives       | Constants, errors, generic results, simple extensions, serialization converters                        | Feature workflows, domain decisions, EF queries                           |
| Tests          | Quality gates           | Unit, integration, functional, architecture tests, fixtures                                            | Order-dependent tests, live-provider unit tests                           |

### Standard Source Layout

```text
src/
  Product.Api/
    Controllers/
    Middleware/
    Filters/
    Auth/
    Common/Requests/
    Common/Responses/
    Common/OpenApi/
    Common/Versioning/
    DependencyInjection.cs
    Program.cs

  Product.Application/
    Abstractions/
      CQRS/
      Data/
      Events/
      Identity/
      Messaging/
      Time/
    Behaviors/
    Common/
      Pagination/
      Filtering/
      Results/
    Contracts/
      Persistence/
      Infrastructure/
    Features/
      {Feature}/
        Commands/{UseCase}/
        Queries/{UseCase}/
        Dtos/
        Mapping/
        Specifications/
    Events/
    Exceptions/
    DependencyInjection.cs

  Product.Domain/
    Aggregates/
    Entities/
    ValueObjects/
    Events/
    Services/
    Enums/
    Exceptions/
    Rules/
    Abstractions/

  Product.Persistence/
    DbContexts/
    Configurations/
    Repositories/Read/
    Repositories/Write/
    Specifications/
    Migrations/
    Interceptors/
    Seeds/
    DependencyInjection.cs

  Product.Infrastructure/
    Caching/
    Messaging/
    ExternalServices/
    Identity/
    Security/
    BackgroundJobs/
    FileStorage/
    Email/
    Observability/
    Resilience/
    DependencyInjection.cs

  Product.Shared/
    Constants/
    Errors/
    Extensions/
    Abstractions/
    Serialization/

tests/
  Product.UnitTests/
  Product.IntegrationTests/
  Product.FunctionalTests/
  Product.ArchitectureTests/
```

### File Placement Rules

- Put every use case in `Application/Features/{Feature}/Commands|Queries/{UseCase}`.
- Keep command/query, handler, validator, and result DTO together unless shared by multiple use cases.
- Put repository interfaces in Application contracts and implementations in Persistence.
- Put external provider abstractions in Application only when handlers need them; put concrete clients/adapters in Infrastructure.
- Put API-only request/response transport models in API.
- Put business response DTOs in Application.
- Put reusable domain concepts in Domain, not Shared.
- Put generic cross-service primitives in Shared only if they are not feature-specific.

### API Rules

- Controllers or endpoints are thin adapters.
- Route names use plural resources: `/api/v1/orders`.
- Use command-style subroutes for actions: `/orders/{id}/cancel`.
- Use API versioning consistently.
- Use central response envelope and error envelope.
- Do not duplicate application validation in controllers.
- Do not inject repositories, DbContext, or provider SDK clients into controllers.

### Application Rules

- Commands mutate state. Queries do not.
- One handler per command/query.
- Handlers orchestrate; domain decides.
- Use pipeline behaviors for validation, logging, transactions, idempotency, and metrics.
- Use application services only for reusable orchestration that does not naturally fit one handler.

### Persistence Rules

- Repositories expose intention-revealing methods.
- Use read/write repository separation for complex aggregates:
  - `IOrderReadRepository`
  - `IOrderWriteRepository`
- Do not leak `IQueryable` outside Persistence unless this is an explicit team standard.
- Use Unit of Work for transaction boundaries.

### Onboarding Navigation

To understand a feature, read in this order:

1. API route/controller.
2. Command/query.
3. Validator.
4. Handler.
5. Domain entity/aggregate/value objects.
6. Repository contract.
7. Repository implementation and EF configuration.
8. Tests.
9. Infrastructure adapters used by the handler.
10. 

## 2. Core Coding And Design Rules

### General C# Rules

- Use `sealed` classes by default. Allow inheritance only when it is intentional and documented by the design.
- Use constructor injection only. Do not use service locator patterns in business, application, persistence, or infrastructure code.
- Store injected dependencies in `private readonly` fields unless primary constructors make fields unnecessary.
- Prefer immutable objects. Use `record`, `readonly record struct`, `required`, and `init` where appropriate.
- Avoid public mutable state. Public setters are forbidden for protected domain state.
- Use explicit nullable reference types. Do not silence nullability with `!` unless the lifecycle is guaranteed by framework construction or EF materialization.
- Use guard clauses over deep nesting.
- Do not throw generic `Exception`. Throw a specific domain, application, validation, authorization, conflict, infrastructure, or external-service exception.
- Use structured logging only: `logger.LogInformation("Order {OrderId} created", orderId)`.
- Use intent-revealing names. Avoid `Helper`, `Manager`, `Utils`, `Processor`, and `Common` unless the project has a precise existing meaning for the term.
- Avoid static mutable state. Static state is allowed only for constants, pure functions, compiled regexes, or immutable lookup tables.
- Keep methods small and purpose-specific. Extract private methods when a block has a distinct business meaning.
- Do not introduce new frameworks, libraries, or architecture styles without explicit approval.

### Async And Cancellation

- All async I/O methods must end with `Async`.
- Always propagate `CancellationToken` from API to handlers, repositories, EF Core, cache, queues, and external providers.
- Do not use `.Result`, `.Wait()`, or sync-over-async.
- Do not wrap I/O in `Task.Run`.
- Use DI-managed clients for pooled resources such as `HttpClient`, Refit clients, database contexts, and message producers.

### Nullability And Guards

- Treat nullable warnings as design feedback.
- Accept nullable inputs only when null is a valid business state.
- Do not return `null` for collections. Return `[]`.
- Use `T?` for optional repository results.
- Use required-return methods only when absence is exceptional.
- Validate external input at the boundary with validators and model binding.
- Validate domain invariants inside entities, value objects, factories, or domain services.
- Use `ArgumentNullException.ThrowIfNull` and `ArgumentException.ThrowIfNullOrWhiteSpace` for guard clauses.

### Logging

- Log use-case boundaries only when useful for operations.
- Log external calls with provider, operation, elapsed time, sanitized status, and retry count.
- Log unexpected exceptions once at the global boundary.
- Do not log and rethrow repeatedly across layers.
- Use `Information` for successful business milestones, `Warning` for recoverable anomalies, and `Error` for failed operations.
- Never log secrets, tokens, passwords, private keys, full payment data, or unnecessary PII.

### Dependency Rules

- Depend on abstractions across layer boundaries.
- Do not inject concrete infrastructure into Application.
- Do not inject repositories or DbContext into API controllers.
- Do not inject `IHttpContextAccessor` into Domain.
- Do not hide dependencies behind static accessors.
- Do not call external providers directly from Domain, API controllers, or persistence repositories.

## 3. Domain And Entity Design Rules

### Entity Defaults

- Default database primary key is `int` unless there is a documented reason for `long`, `Guid`, or string identity.
- All persisted entities inherit `BaseEntity<TKey>` or the template equivalent.
- UI/API-facing entities expose `PublicId` as `Guid`, generated with `Guid.CreateVersion7()`.
- Auditable entities implement `IAuditable`.
- Soft-delete entities implement `ISoftDeletable`.
- Concurrency-sensitive entities implement an optimistic concurrency token such as `RowVersion`, PostgreSQL `xmin`, or a numeric version.
- Entity creation must go through a constructor or factory that enforces invariants.
- Protected state must have private setters or private backing fields.
- Avoid anemic entities. State transitions with business meaning belong on the entity or aggregate root.

### Factories vs Constructors

- Use constructors when creation is simple and all invariants fit naturally in parameters.
- Use static factories when creation has business naming, generated identifiers, optional defaults, nested child creation, validation branches, or multiple creation paths.
- Use factory names that describe intent: `Create`, `Register`, `Schedule`, `From`, or `Of`.
- Factories must return valid objects only. Do not create partially valid entities that require callers to set required state later.

### Aggregate Roots

- Aggregate roots protect consistency boundaries.
- External code modifies child entities through aggregate-root methods.
- Repositories load and save aggregate roots, not arbitrary child entities, unless the child is its own aggregate.
- Keep aggregate transactions small. Do not load large object graphs for simple changes.
- Aggregate methods raise domain events when side effects are needed after state changes.

### Relationship Ownership

- The owning aggregate controls child lifecycle.
- Child entities must not expose methods that bypass parent invariants.
- Cross-aggregate references should use identifiers by default, not direct mutable object graphs.
- Do not model every database relationship as a public mutable navigation collection.
- Use private collections with read-only public exposure when child membership is controlled by behavior.

### Value Objects

- Use value objects for strongly typed concepts such as email, money, currency, country code, phone number, order number, date range, and status reason.
- Value objects are immutable.
- Value objects validate themselves at creation.
- Prefer value objects over primitive obsession in domain APIs.
- Value objects must not depend on persistence, HTTP, logging, or service providers.

### Domain Events

- Domain events describe facts in past tense: `OrderCreatedDomainEvent`.
- Raise domain events for side effects such as notifications, projections, integration events, cache invalidation, or background work.
- Domain events must not perform side effects themselves.
- Use outbox for domain/integration events that must survive process crashes.

## 4. Entity Relationships And EF Core Modeling Rules

### Ownership Boundaries

- Entity owns business state, behavior, invariants, and relationship rules.
- Factory owns valid creation paths, generated public IDs, default business values, and initial child entities.
- EF configuration owns table names, keys, columns, conversions, indexes, constraints, delete behavior, query filters, concurrency tokens, and owned mappings.

### EF Configuration Standards

- Configure persistence in `IEntityTypeConfiguration<TEntity>` files.
- Keep one configuration file per entity.
- Configure table name, key, required/optional properties, max lengths, conversions, indexes, relationships, delete behavior, concurrency, and query filters explicitly.
- Configure audit fields consistently for all `IAuditable` entities.
- Configure soft-delete query filters for `ISoftDeletable`.
- Configure optimistic concurrency tokens for contested entities.
- Use shadow properties only when the value is pure infrastructure metadata and has no domain meaning.
- Do not add EF-specific hacks to entities when configuration can express the mapping.

### Relationship Rules

- Required relationships use a non-null FK and required navigation when navigation is present.
- Optional relationships use nullable FK and nullable navigation.
- Do not rely on EF conventions for important relationships. Configure them.
- Cascade delete is allowed only for true owned children inside the same aggregate.
- Use `Restrict` or `NoAction` for cross-aggregate relationships by default.
- Use an explicit join entity for many-to-many relationships with metadata, audit fields, ordering, status, or behavior.
- Skip navigations are allowed only for simple lookup-style many-to-many relationships.
- Map owned value objects with `OwnsOne` or `OwnsMany` when they have no independent identity.

### Foreign Keys, Indexes, And Constraints

- FK property naming: `{PrincipalEntityName}Id` for internal PK references.
- Public references use `{PrincipalEntityName}PublicId` only when intentionally referencing public identifiers.
- Add indexes for foreign keys, lookup keys, status fields used in filters, timestamps used in ordering, and idempotency keys.
- Add unique constraints for business uniqueness. Do not enforce uniqueness only in application code.
- Name indexes and constraints consistently when the database benefits from explicit names.

### Change Tracking Standards

- Read queries use `AsNoTracking` by default.
- Methods returning tracked entities must include `ForUpdate`, `Tracked`, or `WithTracking` in the method name.
- Do not mix tracked and no-tracking entities in the same write workflow without a clear reason.
- Do not call `SaveChangesAsync` inside repositories when Unit of Work is the standard.

## 5. Enum Standards

- Store enums in the database as strings by default.
- Persist numeric enum values only when storage size or performance is proven important and documented.
- API responses return enum values as strings.
- Configure enum JSON converters centrally.
- Configure EF enum conversions centrally or consistently in entity configurations.
- Do not expose magic numeric enum values in public APIs.
- Renaming enum members is a breaking data/API change when stored or serialized as strings; handle migrations and compatibility.
- Use enums only for closed sets. Use lookup/reference tables for user-configurable or frequently changing values.

## 6. Mapping Rules

### Mapping Direction

- API request maps to command/query.
- Command/query maps to domain behavior inputs, not directly to entity mutation when rules exist.
- Domain entity, read model, or projection maps to response DTO/result.
- Persistence projection maps directly to DTO for read paths.

### Mapping Standards

- Prefer explicit mapping for public contracts and non-trivial transformations.
- AutoMapper, Mapster, or similar tools are allowed only for simple property-to-property mapping.
- Complex business mapping must be handwritten or expressed through domain/application methods.
- Mapping must not contain business rules, permission checks, database calls, or external service calls.
- Projection-first queries are preferred: project from EF query to DTO before materialization.
- Do not load full entities only to map list responses.
- Mapping configuration belongs near the feature or in a dedicated mapping folder.

## 7. Configuration Rules

- Use strongly typed options classes.
- Validate options at startup with data annotations or custom validators.
- Fail fast on missing required configuration.
- Do not use magic strings for config keys outside options binding.
- Keep environment-specific config separate.
- Store secrets outside source control: secret manager, environment variables, vault, Kubernetes secrets, or cloud secret stores.
- Do not log full configuration objects.
- Use feature flags for risky or staged behavior changes.
- Configuration classes end with `Options` or `Settings`; use one suffix consistently per service.
- External provider timeouts, retry counts, base URLs, credentials, and feature toggles must be configurable.

## 8. Naming Convention Handbook

### Repositories

- Required entity by ID: `GetByIdAsync`.
- Optional entity by ID: `FindByIdAsync`.
- List collection: `ListAsync`.
- Paginated list: `PageAsync` or `SearchAsync`.
- Existence check: `ExistsAsync`.
- Count: `CountAsync`.
- Tracked mutation load: `GetForUpdateAsync`, `GetTrackedAsync`, or `GetWithTrackingAsync`.
- Add: `AddAsync`, `AddRangeAsync`.
- Update: `Update` unless async work is required.
- Delete: `Delete`, `SoftDelete`, or `Remove` based on semantics.

### API

- Controllers end with `Controller`.
- Middleware ends with `Middleware`.
- Filters end with `Filter`.
- API request types end with `Request`.
- API response types end with `Response`.
- Authorization policies live behind constants such as `Policies.RequireAdmin`.
- Requirements end with `Requirement`.
- Authorization handlers end with `AuthorizationHandler`.

### Application

- Commands use imperative names: `CreateOrderCommand`.
- Queries use read names: `GetOrderByIdQuery`, `SearchOrdersQuery`.
- Handlers end with `Handler`.
- Validators end with `Validator`.
- Pipeline behaviors end with `Behavior`.
- Specifications end with `Specification`.
- Application interfaces describe capability, not technology.

### Domain

- Entities and aggregates are singular nouns.
- Value objects are singular concept names.
- Domain events are past-tense facts: `OrderSubmittedDomainEvent`.
- Domain services end with `Service` or `Policy`.
- Enums are singular nouns and should not end with `Enum`.
- Domain exceptions are specific and business-named.

### Infrastructure

- EF configurations end with `Configuration`.
- DbContext is `{Product}DbContext` or `{BoundedContext}DbContext`.
- Provider clients are `I{Provider}Client`.
- Provider adapters are `{Provider}Service` or `{Provider}Adapter`.
- Options classes are `{Provider}Options`.
- Background jobs end with `Job`.
- Message consumers end with `Consumer`.
- Message publishers end with `Publisher`.

## 9. Performance And Database Rules

### EF Core Query Rules

- Use `AsNoTracking` for all read-only queries.
- Use projection-first queries for DTO/list responses.
- Do not materialize before filtering, sorting, and pagination.
- Do not leak `IQueryable` outside Persistence.
- Avoid `Include` on list endpoints. Project instead.
- Use `Include` only when returning or mutating aggregate graphs that require navigation data.
- Use `AsSplitQuery` when multiple collection includes would cause cartesian explosion.
- Avoid N+1 queries. Never run database queries in loops unless batched intentionally.
- Use compiled queries for hot, stable query paths.
- Use `AnyAsync` for existence checks, not `CountAsync > 0`.
- Use `ExecuteUpdateAsync`, `ExecuteDeleteAsync`, or bulk libraries for large set-based operations.
- Pass `CancellationToken` to every EF async call.

### Database Rules

- Every list endpoint must paginate.
- Use cursor pagination for high-volume append-only or infinite-scroll data.
- Add indexes for FKs, lookup fields, status filters, time-based sorting, unique business keys, and idempotency keys.
- Enforce uniqueness in the database, not only in application code.
- Keep transactions short.
- Do not perform slow external calls inside database transactions.
- Use optimistic concurrency for contested records.
- Prefer batching over row-by-row writes.
- Review generated SQL for complex queries.

### Cache Rules

- Use cache-aside by default: read cache, load source on miss, set cache, invalidate after writes.
- Use distributed cache for horizontally scaled services.
- In-memory cache is allowed only for per-node ephemeral data or single-node services.
- Cache keys must be namespaced and versioned: `{service}:{env}:{entity}:{id}:v{schema}`.
- Set TTLs based on volatility.
- Add TTL jitter to avoid synchronized expiration.
- Prevent stampedes with single-flight locks, distributed locks, or stale-while-revalidate.
- Never cache sensitive data unless explicitly approved, encrypted, and short-lived.

### API And Runtime Rules

- Use async all the way. No sync-over-async.
- Configure timeouts for external calls.
- Use DI-managed `HttpClient`/Refit clients for pooling.
- Compress responses when payload size justifies it.
- Avoid returning oversized payloads.
- Be aware of allocations on hot paths: avoid unnecessary LINQ/materialization, string concatenation in loops, repeated reflection, and large intermediate collections.

### Background And Scale Rules

- APIs must be stateless.
- Move slow work to background jobs or queues.
- Background jobs and consumers must be idempotent.
- Use retry with jitter for transient failures.
- Use circuit breakers for unstable providers.
- Use outbox for reliable event publishing.
- Prefer eventual consistency for cross-service workflows.

## 10. AI Code Generation Instructions

### Before Generating Code

- Inspect nearby files first.
- Identify the layer and feature folder before adding files.
- Follow existing naming, response, exception, mapping, validation, and DI patterns.
- Preserve dependency direction.
- Do not invent a new architecture style.

### While Generating Code

- Create command/query, handler, validator, and result types in Application.
- Keep API code thin and delegate to mediator/application use case.
- Apply entity conventions automatically:
  - inherit base entity
  - default `int` PK
  - add `PublicId` for UI/API-facing entities
  - protect setters
  - enforce invariants in factory/entity methods
  - add audit, soft delete, and concurrency interfaces when appropriate
- Apply EF configuration conventions automatically:
  - one config per entity
  - configure keys, required fields, relationships, conversions, indexes, query filters, concurrency, audit
  - store enums as strings unless project standard differs
- Apply mapping conventions automatically:
  - API request to command/query
  - projection to DTO for reads
  - explicit mapping for non-trivial transformations
- Apply performance conventions automatically:
  - `AsNoTracking`
  - projection-first
  - pagination
  - minimal `Include`
  - no `IQueryable` leaks
  - cancellation tokens
- Add tests proportional to risk.

### Never Generate

- Business logic in controllers.
- DbContext access in Application.
- Provider SDK usage in Domain/Application handlers.
- Public mutable domain state.
- Generic `Exception`.
- Magic config strings.
- Unpaginated list endpoints.
- Unstructured logs.
- Hidden service locator usage.
- New frameworks without explicit instruction.

## 11. Code Review Checklist

### Architecture

- File is in the correct layer.
- Dependency direction is preserved.
- API is thin.
- Application orchestrates but does not implement persistence/provider details.
- Domain owns business invariants.
- Persistence owns EF Core.
- Infrastructure owns external systems.

### Coding Standards

- Classes are sealed unless inheritance is intentional.
- Constructor injection only.
- Dependencies are readonly.
- Public mutable state is avoided.
- Nullability is explicit.
- Guard clauses are used.
- Async I/O methods end with `Async`.
- Cancellation tokens are propagated.
- Names reveal intent.

### Domain And Persistence

- Entity uses correct base type and interfaces.
- `PublicId` exists where UI/API identity is needed.
- Invariants are enforced in domain methods/factories.
- EF relationships are configured explicitly.
- Enums are persisted/serialized consistently.
- Soft delete, audit, and concurrency are configured where required.

### Query Performance

- Read queries use `AsNoTracking`.
- Queries project to DTO before materialization.
- Pagination is required.
- `Include` is minimal.
- No N+1 risk.
- No `IQueryable` leaks.
- Indexes support filters/sorts/lookups.
- Transactions are short.

### Error Handling And Logging

- No generic exceptions.
- Known failures map to stable error codes.
- Global middleware handles API errors.
- Logs are structured.
- Sensitive data is not logged.
- Unknown errors return safe messages.

### Maintainability

- No unjustified abstractions.
- No duplicate patterns.
- No dead code.
- No feature logic in Shared/Common.
- Mapping contains no business rules.
- Config uses options and startup validation.

## 12. Security Rules

- Authenticate every non-public endpoint.
- Authorize by policy, role, scope, permission, or resource ownership.
- Validate all external input.
- Treat webhooks/callbacks as hostile until signature, timestamp, nonce, and idempotency are verified.
- Store secrets outside source control.
- Never log secrets or full sensitive payloads.
- Use HTTPS-only external provider URLs.
- Use secure password, token, and key handling.
- Use short-lived access tokens and refresh-token rotation where applicable.
- Apply rate limiting to public, auth, search, and expensive endpoints.
- Protect against over-posting by using request DTOs, not entities.
- Avoid exposing internal IDs when public IDs are required.
- Return safe authorization errors; do not reveal resource existence unnecessarily.

## 13. Testing Rules

### Test Project Structure

```text
tests/
  Product.UnitTests/
  Product.IntegrationTests/
  Product.FunctionalTests/
  Product.ArchitectureTests/
```

### Unit Tests

- Test domain invariants and entity behavior without infrastructure.
- Test handlers with mocked contracts.
- Test validators for edge cases.
- Use builders for complex domain objects.
- Do not mock value objects or simple entities.

### Integration Tests

- Use a real database provider or test containers when EF translation matters.
- Test repositories, migrations, concurrency, query filters, and database constraints.
- Do not rely on test order.
- Reset database state between tests.

### Functional/API Tests

- Test API status codes, response envelopes, auth behavior, validation errors, and serialization.
- Avoid testing implementation details through API tests.

### Architecture Tests

- Enforce dependency boundaries.
- Ensure Domain does not reference infrastructure/framework packages.
- Ensure Application does not reference Persistence/Infrastructure implementations.
- Ensure controllers do not inject repositories or DbContext.

### Naming

- Test classes end with `Tests`.
- Test methods use one consistent style:
  - `Method_ShouldExpectedBehavior_WhenCondition`
  - or `GivenCondition_WhenAction_ThenExpectedResult`

## 14. Practical Refactoring Recommendations

- Split large repository interfaces into read/write contracts.
- Rename tracked query methods with `ForUpdate`, `Tracked`, or `WithTracking`.
- Add architecture tests for dependency rules.
- Standardize enum storage and API serialization as strings.
- Add central options validation at startup.
- Add cache key and TTL conventions to each cache provider.
- Add outbox for reliable integration events.
- Add idempotency for payments, webhooks, callbacks, and user-triggered financial operations.
- Replace heavy list `Include` queries with projection-first queries.
- Add indexes for fields exposed by filters and sorts.
- Standardize response and error envelopes with trace/correlation id.
- Introduce transaction, logging, validation, and idempotency pipeline behaviors where useful.

