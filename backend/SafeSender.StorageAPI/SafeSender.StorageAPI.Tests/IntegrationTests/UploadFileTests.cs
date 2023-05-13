using System.Text;
using Flurl.Http;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using SafeSender.StorageAPI.Models;

namespace SafeSender.StorageAPI.Tests.IntegrationTests;

[TestFixture]
public class UploadFileTests
{
    private const string UploadEndpointUrl = "api/upload";
    
    [Test]
    public async Task UploadFile_FileUploaded_Returns201StatusCode()
    {
        var client = SystemUnderTest.GetClient();
        var uploadedFileMock = Encoding.UTF8.GetBytes("mock test string 123");

        using var response = await client.Request(UploadEndpointUrl).PostJsonAsync(new UploadFileRequestModel
        {
            FileBytes = uploadedFileMock,
            FileName = "text.txt",
            PasswordHash = Guid.NewGuid().ToString("N"),
        });
        
        Assert.AreEqual(StatusCodes.Status201Created, response.StatusCode);
    }
}