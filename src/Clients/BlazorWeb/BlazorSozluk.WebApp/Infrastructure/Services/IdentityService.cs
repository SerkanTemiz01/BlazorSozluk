using Blazored.LocalStorage;
using BlazorSozluk.Common.Infrastructure.Exceptions;
using BlazorSozluk.Common.Infrastructure.Results;
using BlazorSozluk.Common.Models.Queries;
using BlazorSozluk.Common.Models.RequestModels;
using BlazorSozluk.WebApp.Infrastructure.Services.Interfaces;
using System.Net.Http.Json;
using System.Text.Json;

namespace BlazorSozluk.WebApp.Infrastructure.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly HttpClient httpClient;
        private readonly ISyncLocalStorageService syncLocalStorageService;

        public IdentityService(HttpClient httpClient, ISyncLocalStorageService syncLocalStorageService)
        {
            this.httpClient = httpClient;
            this.syncLocalStorageService = syncLocalStorageService;
        }

        public bool IsLoggedIn => !string.IsNullOrEmpty(GetUserToken());

        public string GetUserToken()
        {
            string token = syncLocalStorageService.GetItem<string>("token");
            if (string.IsNullOrEmpty(token))
                return "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";
            return token;
        }
        public string GetUserName()
        {
            return syncLocalStorageService.GetItem<string>("userName");
        }
        public Guid GetUserId()
        {
            return syncLocalStorageService.GetItem<Guid>("userId");
        }
        public async Task<bool> Login(LoginUserCommand command)
        {
            string responseStr;
            var httpResponse = await httpClient.PostAsJsonAsync("/api/User/Login", command);
            if (!httpResponse.IsSuccessStatusCode && httpResponse != null)
            {
                if (httpResponse.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    responseStr = await httpResponse.Content.ReadAsStringAsync();
                    var validation = JsonSerializer.Deserialize<ValidationResponseModel>(responseStr);
                    responseStr = validation.FlattenErrors;
                    throw new DatabaseValidationException(responseStr);
                }
                return false;
            }
            responseStr = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<LoginUserViewModel>(responseStr);

            if (!string.IsNullOrEmpty(response.Token)) //login success
            {
                syncLocalStorageService.SetItem("token", response.Token);
                syncLocalStorageService.SetItem("userName", response.UserName);
                syncLocalStorageService.SetItem("userId", response.Id);

                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", response.UserName);
                return true;
            }
            return false;
        }
        public void Logout()
        {
            syncLocalStorageService.RemoveItem("token");
            syncLocalStorageService.RemoveItem("userName");
            syncLocalStorageService.RemoveItem("userId");
            httpClient.DefaultRequestHeaders.Authorization = null;

        }
    }
}
