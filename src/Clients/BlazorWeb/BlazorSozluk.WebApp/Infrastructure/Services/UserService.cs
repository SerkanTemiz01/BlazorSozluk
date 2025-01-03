using BlazorSozluk.Common.Events.User;
using BlazorSozluk.Common.Infrastructure.Exceptions;
using BlazorSozluk.Common.Infrastructure.Results;
using BlazorSozluk.Common.Models.Queries;
using BlazorSozluk.Common.Models.RequestModels;
using BlazorSozluk.WebApp.Infrastructure.Services.Interfaces;
using System.Net.Http.Json;

namespace BlazorSozluk.WebApp.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient client;

        public UserService(HttpClient client)
        {
            this.client = client;
        }

        public async Task<UserDetailViewModel> GetUserDetail(Guid id)
        {
            return await client.GetFromJsonAsync<UserDetailViewModel>($"/api/User/{id}");
        }
        public async Task<UserDetailViewModel> GetUserDetail(string userName)
        {
            return await client.GetFromJsonAsync<UserDetailViewModel>($"/api/User/username/{userName}");
        }
        public async Task<bool> UpdateUser(UserDetailViewModel user)
        {
            var response = await client.PostAsJsonAsync("/api/User/Update", user);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> ChangeUserPassword(string oldPassword, string newPassword)
        {
            var command = new ChangeUserPasswordCommand(null, oldPassword, newPassword);
            var httpResponse = await client.PostAsJsonAsync("/api/User/ChangePassword", command);

            if (!httpResponse.IsSuccessStatusCode && httpResponse != null)
            {
                if (httpResponse.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var validation = await httpResponse.Content.ReadFromJsonAsync<ValidationResponseModel>();
                    throw new DatabaseValidationException(validation.FlattenErrors);
                }
                return false;
            }
            return httpResponse.IsSuccessStatusCode;
        }
    }
}
