using NUnit.Framework;
using ApiTestBarrels.Models;
using Flurl.Http;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiTestBarrels
{
    [TestFixture]
    public class BarrelApiTests
    {
        private readonly string BaseUrl = Environment.GetEnvironmentVariable("API_BASE_URL")
                                ?? "https://to-barrel-monitor.azurewebsites.net";
        private string? barrelId;

        // Setup: vytvoření jednoho testovacího barelu pro další testy
        [SetUp]
        public async Task SetUp()
        {
            var newBarrel = new BarrelCreateRequest
            {
                qr = "QR_" + Guid.NewGuid().ToString("N"),
                rfid = "RFID_" + Guid.NewGuid().ToString("N"),
                nfc = "NFC_" + Guid.NewGuid().ToString("N")
            };

            var response = await $"{BaseUrl}/barrels"
                .PostJsonAsync(newBarrel)
                .ReceiveJson<Barrel>();

            barrelId = response.id;
            await Task.Delay(500); // buffer kvůli async operacím backendu
        }
        //Test 1: Ověření že po vytvoření barelu máme jeho ID
        [Test, Order(1)]
        public void CreateBarrel_ShouldReturnId()
        {
            Assert.IsNotNull(barrelId);
        }
        //Test 2: Načtení seznamu všech barelů
        [Test, Order(2)]
        public async Task GetAllBarrels_ShouldReturnList()
        {
            var barrels = await $"{BaseUrl}/barrels".GetJsonAsync<List<Barrel>>();
            Assert.IsNotNull(barrels);
            Assert.IsTrue(barrels.Count > 0);
        }
        //Test 3: Přidání měření k existujícímu barelu
        [Test, Order(3)]
        public async Task CreateMeasurement_ShouldPass()
        {
            Assert.IsNotNull(barrelId);

            var measurement = new MeasurementCreateRequest
            {
                barrelId = barrelId!,
                dirtLevel = 1,
                weight = 5
            };

            var response = await $"{BaseUrl}/measurements"
                .PostJsonAsync(measurement)
                .ReceiveJson<Measurement>();

            Assert.IsNotNull(response.id);
            Assert.AreEqual(barrelId, response.barrelId);
        }
        //Test 4: Načtení seznamu všech měření
        [Test, Order(4)]
        public async Task GetAllMeasurements_ShouldPass()
        {
            var measurements = await $"{BaseUrl}/measurements".GetJsonAsync<List<Measurement>>();
            Assert.IsNotNull(measurements);
            Assert.IsTrue(measurements.Count > 0);
        }
        //Test 5: Získání detailu barelu podle ID
        [Test, Order(5)]
        public async Task GetBarrelById_ShouldPass()
        {
            var barrel = await $"{BaseUrl}/barrels/{barrelId}".GetJsonAsync<Barrel>();
            Assert.IsNotNull(barrel);
            Assert.AreEqual(barrelId, barrel.id);
        }
        //Test 6: Smazání vytvořeného barelu
        [Test, Order(6)]
        public async Task DeleteBarrel_ShouldPass()
        {
            try
            {
                var response = await $"{BaseUrl}/barrels/{barrelId}".DeleteAsync();
                Assert.AreEqual(200, response.StatusCode);
            }
            catch (FlurlHttpException ex)
            {
                var err = await ex.GetResponseStringAsync();
                TestContext.WriteLine("DELETE ERROR: " + err);
                Assert.Fail("Delete failed.");
            }
        }
        //Test 7: Pokus o přidání měření bez barrelId
        [Test, Order(7)]
        public void InvalidMeasurement_ShouldReturn400()
        {
            var invalidPayload = new { dirtLevel = 10, weight = 20 };

            Assert.ThrowsAsync<FlurlHttpException>(async () =>
            {
                var res = await $"{BaseUrl}/measurements".PostJsonAsync(invalidPayload);
            });
        }
        //Test 8: Načtení detailu neexistujícího barelu
        [Test, Order(8)]
        public async Task GetBarrelByFakeId_ShouldReturn404()
        {
            try
            {
                await $"{BaseUrl}/barrels/{Guid.NewGuid()}"
                    .AllowHttpStatus("404")
                    .GetAsync()
                    .ReceiveJson<Barrel>();
                Assert.Fail("Expected 404");
            }
            catch (FlurlHttpException ex)
            {
                Assert.AreEqual(404, ex.Call.Response.StatusCode);
            }
        }
        //Test 9: Pokus o smazání neexistujícího barelu
        [Test, Order(9)]
        public async Task DeleteFakeBarrel_ShouldReturn404()
        {
            var fakeId = Guid.NewGuid().ToString();
            try
            {
                await $"{BaseUrl}/barrels/{fakeId}"
                    .AllowHttpStatus("404")
                    .DeleteAsync();
                Assert.Fail("Expected 404");
            }
            catch (FlurlHttpException ex)
            {
                Assert.AreEqual(404, ex.Call.Response.StatusCode);
            }
        }
    }
}
