# Automatizované testy API pro měření nečistot

## Popis projektu

Testovací sada ověřuje funkčnost API systému pro správu měření nečistot v barelech (UCO monitoring). Testy pokrývají základní operace i edge-case situace.

---

## Jak spustit testy

1. **Otevři projekt v IDE (např. Visual Studio)**
2. **Ověř, že máš připojený internet**
3. Spusť všechny testy přes Test Explorer (nebo `dotnet test` v terminálu)

---

## Požadavky

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Internetové připojení
- Nainstalované NuGet balíčky:
  - `Flurl.Http`
  - `NUnit`
  - `NUnit3TestAdapter`
  - `Microsoft.NET.Test.Sdk`

>  Všechny jsou definovány v `csproj`, stačí `dotnet restore`.

---

## Environmentální proměnné

Volitelně můžeš testy přesměrovat na jiné API pomocí: API_BASE_URL=https://tvuj-api-url

## Přehled testovaných scénářů

| #   | Název testu                        | Ověřovaná funkcionalita                         
|-----|------------------------------------|--------------------------------------------------
| 1   | `CreateBarrel_ShouldReturnId`      | Vytvoření barelu (POST /barrels)                
| 2   | `GetAllBarrels_ShouldReturnList`   | Získání seznamu barelů (GET /barrels)           
| 3   | `CreateMeasurement_ShouldPass`     | Přidání měření (POST /measurements)             
| 4   | `GetAllMeasurements_ShouldPass`    | Seznam všech měření (GET /measurements)         
| 5   | `GetBarrelById_ShouldPass`         | Detail jednoho barelu (GET /barrels/{id})       
| 6   | `DeleteBarrel_ShouldPass`          | Smazání barelu (DELETE /barrels/{id})           
| 7   | `InvalidMeasurement_ShouldReturn400`| Neplatné měření bez barrelId (400 Bad Request) 
| 8   | `GetBarrelByFakeId_ShouldReturn404`| Dotaz na neexistující ID (404 Not Found)         
| 9   | `DeleteFakeBarrel_ShouldReturn404` | Smazání neexistujícího barelu (404 Not Found)   

## Známé problémy (backend / API)

| #  | Test                                  | Popis chyby                                                    | Podezření                         
|----|---------------------------------------|----------------------------------------------------------------|-----------------------------------
| 1  | `CreateMeasurement_ShouldPass`        | Vrací 400 Bad Request i při validním `barrelId`                | Nesoulad mezi dokumentací a API  
| 2  | `GetBarrelById_ShouldPass`            | Vrací 500 Internal Server Error při validním ID                | Serverová chyba                  
| 3  | `DeleteFakeBarrel_ShouldReturn404`    | Vrací 500 místo očekávaného 404                                | Backend nevrací správný status  
| 4  | `GetAllMeasurements_ShouldPass`       | Vrací False, i když bylo měření úspěšně vytvořeno              | Možné zpoždění nebo cacheování   
