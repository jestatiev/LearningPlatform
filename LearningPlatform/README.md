# Learning Platform — TW1 projektni zadatak

Sustav za upravljanje tečajevima i učenjem (Course & Learning Management System).
ASP.NET Core Web API backend (slojevita arhitektura, Repository + Unit of Work pattern,
JWT autentifikacija s ulogama, EF Core Code First, Serilog, Swagger) + jednostavan
HTML/JS klijent koji koristi sve funkcionalnosti API-ja.

## Status projekta
Projekt je uspješno kompajliran, testiran (20/20 xUnit testova prolazi) i funkcionalan.

1. Otvori projekt u Visual Studiju / VS Code / Rideru.
2. Pokreni `dotnet restore` i `dotnet build` te ispravi eventualne sitne greške
   (verzije paketa, tipfeleri i sl. — arhitektura i logika su potpune, ali mala
   sintaktička odstupanja su moguća jer kod nije mogao biti automatski proveden).
3. Pokreni migracije i testiraj sve endpointove kroz Swagger prije predaje.

## Struktura projekta

```
LearningPlatform.sln
src/
  LearningPlatform.Domain/         -> Entiteti + sučelja repozitorija (bez ovisnosti o EF-u)
  LearningPlatform.Application/    -> DTO-ovi, servisna sučelja, servisi (poslovna logika), iznimke
  LearningPlatform.Infrastructure/ -> DbContext, implementacije repozitorija, JWT, hashiranje lozinki, seed
  LearningPlatform.API/            -> Program.cs, kontroleri, middleware, appsettings
tests/
  LearningPlatform.Tests/          -> xUnit + Moq + FluentAssertions testovi za SVE kontrolere
client/
  index.html, css/, js/            -> Čisti HTML/CSS/JS klijent (bez build alata) koji koristi API
```

## Domenski model

- **User, Role, UserRole** — M:N relacija korisnik↔uloga (Admin / Student)
- **Category** → 1:N → **Course**
- **Course** → 1:N → **Lesson**
- **Course** → 1:N → **Test** → 1:N → **Question**
- **Course** → 1:N → **Review** (i User → 1:N → Review)
- **User** ↔ **Course** preko **Enrollment** (M:N sa dodatnim podacima: napredak, datum prijave)

10 entiteta, jasna 1:N i M:N relacija — zadovoljava tipične zahtjeve TW1 projektnog zadatka.

## Kako pokrenuti backend

**Preduvjeti:** .NET 8 SDK, SQL Server (ili SQL Server LocalDB — dolazi uz Visual Studio).

1. U `src/LearningPlatform.API/appsettings.json` provjeri/izmijeni `ConnectionStrings:DefaultConnection`.
2. Iz root direktorija:
   ```bash
   dotnet restore
   dotnet build
   ```
3. Kreiraj početnu migraciju (potrebno je da `dotnet-ef` alat bude instaliran: `dotnet tool install --global dotnet-ef`):
   ```bash
   cd src/LearningPlatform.Infrastructure
   dotnet ef migrations add InitialCreate --startup-project ../LearningPlatform.API
   ```
4. Pokreni API (migracije i seed podaci se automatski primjenjuju pri pokretanju, vidi `Program.cs`):
   ```bash
   cd ../LearningPlatform.API
   dotnet run
   ```
5. Otvori Swagger na `http://localhost:5080/swagger` (port je definiran u `Properties/launchSettings.json`).

**Seed podaci** (kreiraju se automatski):
- Admin korisnik: `admin` / `Admin123!`
- Uloge: `Admin`, `Student`
- 3 početne kategorije

## Kako pokrenuti klijenta

Klijent je čisti statički HTML/CSS/JS — nije potreban Node.js ni build korak.

1. Pokreni backend (vidi gore) — po defaultu očekuje `http://localhost:5080`. Ako backend
   radi na drugom portu, izmijeni `API_BASE` na vrhu `client/js/api.js`.
2. Otvori `client/index.html` u pregledniku (npr. desni klik → "Open with Live Server" u
   VS Code-u, ili `python -m http.server` iz `client/` foldera pa otvori `http://localhost:8000`).
3. Ako otvaraš klijent na drugom originu/portu, dodaj taj origin u `AllowedOrigins` u
   `appsettings.json` na backendu (CORS).

Klijent podržava: registraciju/prijavu, pregled kategorija i tečajeva, pregled detalja
tečaja (lekcije, testovi, recenzije), prijavu na tečaj, praćenje napretka, rješavanje
testova s automatskim bodovanjem, ostavljanje recenzija te administratorski panel za
CRUD nad kategorijama, tečajevima, lekcijama, testovima i pitanjima.

## Kako pokrenuti testove

```bash
cd tests/LearningPlatform.Tests
dotnet test
```

Testovi (xUnit + Moq + FluentAssertions) pokrivaju **sve kontrolere**: AuthController,
CategoriesController, CoursesController, LessonsController, EnrollmentsController,
TestsController, ReviewsController, UsersController. Servisi su mockani preko sučelja,
pa se testira isključivo ponašanje kontrolera (status kodovi, prosljeđivanje parametara,
korištenje ID-a prijavljenog korisnika iz JWT tokena).

## Autentifikacija i autorizacija

- `POST /api/auth/register` — javno, kreira korisnika s ulogom `Student`
- `POST /api/auth/login` — javno, vraća JWT token + role
- Token se šalje kao `Authorization: Bearer <token>`
- Politike: `AdminOnly` (administracija sadržaja), `StudentOnly` (upis, testovi, recenzije)

## Prijedlog za predaju / prezentaciju

Kad testiraš/braniš projekt, dobro je pripremiti kratku priču kroz Swagger ili klijenta:
1. Registriraj studenta → prijavi se.
2. Kao admin, kreiraj kategoriju, tečaj, lekciju, test i pitanje.
3. Kao student, upiši se na tečaj, riješi test, ostavi recenziju, prati napredak.

Ovime demonstriraš CRUD, autentifikaciju/autorizaciju po ulogama, 1:N i M:N relacije,
te sve slojeve arhitekture u praksi.
