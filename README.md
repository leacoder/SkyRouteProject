# SkyRoute — Flight Search & Booking

A small but production-shaped slice of a flight aggregator: search across three mocked airline providers,
compare prices, and book. Angular frontend + .NET backend, running locally.

- **Backend:** .NET 10, ASP.NET Core (controllers), xUnit (v3).
- **Frontend:** Angular 21 (standalone components + signals, zoneless).

---

## Prerequisites

- .NET SDK 10
- Node.js 20.19+/22.12+/24+ and npm (the repo was built with Node 24)
- The Angular CLI is run via `npx` — no global install needed.

## Run it locally

Two terminals.

### 1. Backend (API)

```bash
cd backend/SkyRoute.Api
dotnet run
```

- API base: `http://localhost:5077`
- Swagger UI: `http://localhost:5077/swagger`

> **Windows Smart App Control note.** If `dotnet run`/`dotnet test` fails with
> `Could not load file or assembly ... A configured application control policy blocked this file (0x800711C7)`,
> Smart App Control is blocking the freshly built, unsigned assemblies. Build/run in **Release** — its
> binaries get a separate reputation verdict that is allowed:
> `dotnet run -c Release`. (On machines without Smart App Control enforced, the default Debug build runs fine.)

### 2. Frontend

```bash
cd frontend/skyroute-web
npm install
npx ng serve
```

- App: `http://localhost:4200`
- The dev server proxies `/api/*` to the backend (`proxy.conf.json`), so the browser stays same-origin.
  The backend also enables CORS for `http://localhost:4200` as a fallback.

## Run the tests

From the repository root (where `SkyRoute.slnx` lives):

```bash
dotnet test
```

The suite is concentrated where it matters: the pricing rules, the `Price` pipeline, and the rounding
mode (the graded business core), plus two `FlightSearchService` tests — aggregation across providers and
resilience when one provider fails — **20 tests total**. (Use `dotnet test -c Release` if Smart App
Control blocks the Debug build, per the note above.)

---

## API

| Method | Route | Purpose |
|---|---|---|
| `POST` | `/api/flights/search` | Search all providers; returns priced, **unsorted** results. |
| `POST` | `/api/bookings` | Confirm a booking; returns a reference code. |
| `GET` | `/api/bookings/{referenceCode}` | Retrieve a confirmed booking (in-memory store). |
| `GET` | `/api/airports` | Reference data for the dropdowns. |

`POST /api/flights/search`:

```jsonc
// request
{ "originCode": "JFK", "destinationCode": "LHR", "departureDate": "2026-07-15", "passengers": 2, "cabin": "Economy" }
// 200
{
  "isInternational": true,
  "requiredDocumentType": "Passport",
  "currency": "USD",
  "results": [
    { "id": "GA413-JFKLHR-20260715", "provider": "GlobalAir", "flightNumber": "GA413",
      "departureTime": "2026-07-15T08:00:00+00:00", "arrivalTime": "2026-07-15T14:40:00+00:00",
      "durationMinutes": 400, "cabin": "Economy", "perPassenger": 230.00, "total": 460.00, "passengerCount": 2 }
    /* ... */
  ]
}
```

`POST /api/bookings` sends the route context (so the server can independently re-derive the document type
**and** re-price) and the passenger details; it does **not** send a document type, and its price is
display-only. Errors use RFC-9457 ProblemDetails (`400` for invalid input/route, `422` for a document that
does not match the route's required type).

---

## Architecture decisions

- **Two seams, by design.** `IFlightProvider` fetches flights (base fares only); `IPricingRule` turns a base
  fare into a final per-passenger price. A provider **composes** its rule. `FlightSearchService` receives
  `IEnumerable<IFlightProvider>` by DI, queries them in parallel, and aggregates — it never names a concrete
  provider. **Onboarding a third airline is a new rule + a new provider + one DI line, with no edits to
  existing code** (see `Program.cs`). These two interfaces are the *only* abstractions; `BookingService` and
  `AirportCatalog` are concrete (no speculative single-implementation seams).

- **Fetching is separated from pricing** so each can evolve independently and the pricing rules stay pure and
  trivially testable.

- **Pricing is built so it cannot be done in the wrong order.** Money is `decimal` (never `double`). The only
  way to build a `Price` is `Price.From(baseFare, rule, passengerCount)`, which rounds the **per-passenger**
  price first and multiplies by the passenger count last; there is no constructor that takes a total. The
  `PricePerPassenger` value object splits the price into a *discountable fare* and a (today-empty) list of
  *non-discountable components*, so a future tax/fee can never be discounted — the invariant is structural,
  not a comment. Rounding lives in one place (`Rounding.To2`) and uses HALF-UP
  (`MidpointRounding.AwayFromZero`) — the convention a consumer expects on a displayed fare; it is pinned by
  tests in both midpoint directions and is a one-line change if a reviewer prefers banker's rounding.

  | Provider | Rule | Example (base 200, 2 pax) |
  |---|---|---|
  | GlobalAir | base + 15% fuel surcharge, rounded to 2dp | 230.00 / passenger → **460.00** total |
  | BudgetWings | base − 10% (on base only), **min $29.99 per ticket** | 180.00 / passenger → **360.00** total |
  | ArcticAir | base × 1.20 − $10 loyalty discount, **min $49.99 per ticket** | 230.00 / passenger → **460.00** total |

- **The backend is the source of truth.** It owns the airport→country map, so it decides international vs
  domestic, derives the required document (Passport vs National ID), **re-validates** the document number, and
  **re-prices** the selected flight on booking. The Angular form replicates the document logic for instant UX
  (label + validators swap via `setValidators` + `updateValueAndValidity`), but never supplies a trusted price.

- **Sorting happens on the frontend, in memory.** `sortedResults` is a pure `computed` over the untouched raw
  results signal (price ↑/↓, duration, departure). Changing the sort never re-fetches.

- **POST for search** (not GET): the criteria are a structured, evolving object; a JSON body gives clean model
  binding, validation, and ProblemDetails without query-string encoding. The trade-off (POST is not
  HTTP-cacheable) is irrelevant because results are live-priced and not cacheable anyway.

- **Lightweight layering, not Clean Architecture.** One domain/application project + one API project + one
  test project. The seams that matter are interfaces, not extra project boundaries.

## Project structure

```
backend/
  SkyRoute.Domain/        Money, Airport, SearchCriteria, Flight, Booking, RouteRules,
                          Pricing/ (IPricingRule, Price, PricePerPassenger, Rounding, the three rules),
                          Providers/ (IFlightProvider, the three providers, MockFlights),
                          FlightSearchService, BookingService, AirportCatalog
  SkyRoute.Api/           Controllers, Contracts (DTOs + mapping), DomainExceptionHandler, Program.cs
  SkyRoute.Domain.Tests/  Pricing rule / Price / rounding-mode / route-rule / search-aggregation tests
frontend/skyroute-web/    core/ (api service, models, document rules, booking state),
                          features/search/ (search-page, search-form, results-table),
                          features/booking/ (booking-page)
```

## Trade-offs & known limitations

- **Mock providers, in-memory bookings.** Flights are deterministic mock data; bookings live in a
  `ConcurrentDictionary` (no database). Both are clean additive seams, not rewrites.
- **Single currency.** USD only, though `Money` carries its currency so multi-currency is an extension, not a
  refactor.
- **No authentication, no pagination, one-way only.** Out of scope for this slice.
- **Simplified document formats.** Passport = `[A-Z][0-9]{7}`, National ID = 8 digits — illustrative patterns,
  not real-world document validation.
- **Frontend unit tests are out of scope** for the time budget; the pure pieces (the sort comparator and the
  document rules) are small and would be the first to get specs. The .NET pricing core owns the test budget.
- **Timezones are simplified** in the mock (flight times are emitted in UTC and displayed as such).
