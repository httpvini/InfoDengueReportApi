namespace InfoDengueReportAPI.Services.Utils;
public interface IApiClient
{
    Task<string> GetAsync(string url);
}