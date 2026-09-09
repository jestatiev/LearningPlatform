# LearningPlatform API

Ovo je završni repozitorij za .NET 8 Web API aplikaciju namijenjenu upravljanju sustavom učenja (Learning Platform). Projekt je u potpunosti usklađen s traženim zahtjevima zadatka.

## Status i Ključne Značajke

- **Arhitektura:** Implementirana je čista višeslojna struktura (Domain → Application → Infrastructure → API) koja koristi Unit of Work i Repository pattern.
- **Baza podataka:** Korišten je Entity Framework Core (Code First) s 8 ključnih entiteta i definiranim relacijama. Migracije su uspješno provedene.
- **Autentifikacija & Autorizacija:** Implementirana je policy-based autorizacija korištenjem JWT tokena za dvije uloge: `Admin` i `Student`.
- **Middleware:** Kreiran i ispravno pozicioniran globalni `ExceptionHandlingMiddleware` za upravljanje greškama, te prilagođeni `RequestLoggingMiddleware` za bilježenje IP adresa i ruta dolaznih zahtjeva.
- **Unit Testovi (20/20):** Servisni sloj (npr. `CourseService`) je temeljito testiran pomoću okvira xUnit, Moq i FluentAssertions. Svi testovi prolaze uspješno bez grešaka u pristupu bazi, s obzirom na to da su repozitoriji i Unit of Work potpuno mockani.
- **Deployment & Publish:** Projekt sadrži generiranu `publish` mapu i `web.config` datoteku koja dokazuje spremnost za lokalni ili IIS deployment.
- **Klijent:** Uključena je jednostavna frontend aplikacija (HTML/JS) koja demonstrira komunikaciju s API-jem.

## 🚀 Pokretanje i Testiranje

**Pokretanje API-ja lokalno:**
```bash
dotnet run --project src/LearningPlatform.API
