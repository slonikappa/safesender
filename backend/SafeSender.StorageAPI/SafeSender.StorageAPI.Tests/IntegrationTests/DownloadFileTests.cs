using System.Diagnostics;
using System.Text;
using System.Text.Json.Nodes;
using Flurl.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using NUnit.Framework;
using SafeSender.StorageAPI.Models;
using SafeSender.StorageAPI.Models.ApiModels;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SafeSender.StorageAPI.Tests.IntegrationTests;

[TestFixture]
public class DownloadFileTests
{
    [Test]
    public async Task DownloadFile_CorrectTokenProvided_ReturnsFile()
    {
        var client = SystemUnderTest.GetClient();
        var uploadedFileMock = Encoding.UTF8.GetBytes("mock test string 123");
        var requestModel = new UploadFileRequestModel
        {
            FileBytes = uploadedFileMock,
            FileName = "text.txt",
            PasswordHash = Guid.NewGuid().ToString("N"),
        };
        
        Debug.WriteLine(JsonSerializer.Serialize(requestModel));
        using var response = await client.Request(ApiConstants.UploadEndpointUrl).PostJsonAsync(requestModel);
        
        response.Headers.TryGetFirst("Location", out var location);
        
        var downloadResponse = await client.Request(location).GetAsync();
        var downloadFileModel = await downloadResponse.GetJsonAsync<DownloadFileResponseModel>();

        Assert.AreEqual(uploadedFileMock, downloadFileModel.FileBytes);
        Assert.AreEqual(downloadResponse.StatusCode, StatusCodes.Status200OK);
    }
}