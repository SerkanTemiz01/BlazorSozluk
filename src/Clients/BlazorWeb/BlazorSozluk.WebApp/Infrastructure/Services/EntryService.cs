using BlazorSozluk.Common.Models.Page;
using BlazorSozluk.Common.Models.Queries;
using BlazorSozluk.Common.Models.RequestModels;
using BlazorSozluk.WebApp.Infrastructure.Services.Interfaces;
using System.Net.Http.Json;

namespace BlazorSozluk.WebApp.Infrastructure.Services
{
    public class EntryService : IEntryService
    {
        private readonly HttpClient client;
        public EntryService(HttpClient client)
        {
            this.client = client;
        }
        public async Task<List<GetEntriesViewModel>> GetEntries()
        {
            try
            {
                var result = await client.GetFromJsonAsync<List<GetEntriesViewModel>>("/api/entry?todaysEnties=false&count=30");
                return result.ToList();
            }
            catch (Exception)
            {

                throw;
            }
           
            
        }
        public async Task<GetEntryDetailViewModel> GetEntryDetail(Guid entryId)
        {
            return await client.GetFromJsonAsync<GetEntryDetailViewModel>($"/api/Entry/{entryId}");
        }
        public async Task<PagedViewModel<GetEntryDetailViewModel>> GetMainPageEntries(int page, int pageSize)
        {
            return await client.GetFromJsonAsync<PagedViewModel<GetEntryDetailViewModel>>($"/api/Entry/MainPageEntries?page={page}&pageSize={pageSize}");
        }
        public async Task<PagedViewModel<GetEntryDetailViewModel>> GetProfilePageEntries(int page, int pageSize, string userName = null)
        {
            return await client.GetFromJsonAsync<PagedViewModel<GetEntryDetailViewModel>>($"/api/Entry/UserEntries?userName={userName}&page={page}&pageSize={pageSize}");
        }
        public async Task<PagedViewModel<GetEntryCommentViewModel>> GetEntryComments(Guid entryId, int page, int pageSize)
        {
            return await client.GetFromJsonAsync<PagedViewModel<GetEntryCommentViewModel>>($"/api/Entry/Comments/{entryId}?page={page}&pageSize={pageSize}");
        }
        public async Task<Guid> CreateEntry(CreateEntryCommand command)
        {
            var response = await client.PostAsJsonAsync("/api/Entry/CreateEntry", command);
            if (!response.IsSuccessStatusCode)
                return Guid.Empty;

            return await response.Content.ReadFromJsonAsync<Guid>();
        }
        public async Task<Guid> CreateEntryComment(CreateEntryCommentCommand command)
        {
            var response = await client.PostAsJsonAsync("/api/Entry/CreateEntryComment", command);
            if (!response.IsSuccessStatusCode)
                return Guid.Empty;
            return await response.Content.ReadFromJsonAsync<Guid>();
        }
        public async Task<List<SearchEntryViewModel>> SearchBySubject(string searchText)
        {
            return await client.GetFromJsonAsync<List<SearchEntryViewModel>>($"/api/Entry/Search?searchText={searchText}");
        }
    }
}
