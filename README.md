# LearningPlatform API

.NET 8 Web API za upravljanje tečajevima i sustavom učenja.

## Pokretanje
- Pokretanje API-ja: `dotnet run --project src/LearningPlatform.API`
- Pokretanje testova: `dotnet test`

## Značajke
- Višeslojna arhitektura (Domain, Application, Infrastructure, API)
- JWT autentifikacija i autorizacija (Admin / Student)
- EF Core Code First migracije
- 20/20 prolaznih xUnit testova za servisni sloj
- Prilagođeni Middleware za exception handling i request logging
