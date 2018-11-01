using Inside.Domain.Models;
using Inside.WebApi.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Inside.WebApi.Test
{
    public class AuthControllerTest
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public AuthControllerTest() {

            _server = new TestServer(new WebHostBuilder().UseConfiguration(Program.ConfigurationBuilder()).UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        [Fact]
        public async Task RegisterTest()
        {
            //TODO: Add this user to the database and change asserts
            var registerModel = new RegisterModel
            {
                Email = "john.doe@foo.bar",
                Password = "123456#User",
                Username = "john.doe@foo.bar"

            };
            var content = JsonConvert.SerializeObject(registerModel);
            var stringContent = new StringContent(content, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/Auth/Register", stringContent);

            // Assert
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.False(String.IsNullOrEmpty(responseString));

        }

        [Fact]
        public async Task LoginTest()
        {
            //TODO: Add this user to the database and change asserts
            var loginModel = new LoginViewModel
            {
                UserName = "john.doe@foo.bar",
                Password = "123456#User"

            };
            var content = JsonConvert.SerializeObject(loginModel);
            var stringContent = new StringContent(content, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/Auth/Login", stringContent);

            // Assert
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Equal("INVALID_LOGIN_ATTEMPT", responseString);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            //var responseObject = JsonConvert.DeserializeObject<string>(responseString);

        }

    }
}
