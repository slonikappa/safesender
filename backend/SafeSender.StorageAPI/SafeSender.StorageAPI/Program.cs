using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using AnonFilesApi.Implementations;
using AnonFilesApi.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SafeSender.StorageAPI.Database;
using SafeSender.StorageAPI.Interfaces;
using SafeSender.StorageAPI.Middlewares;
using SafeSender.StorageAPI.Models;
using SafeSender.StorageAPI.Models.ApiModels;
using SafeSender.StorageAPI.Models.Enums;
using SafeSender.StorageAPI.Options;
using SafeSender.StorageAPI.Repositories;
using SafeSender.StorageAPI.Services;

[assembly: InternalsVisibleTo("SafeSender.StorageAPI.Tests")]

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// builder.Services.AddSwaggerGen(options =>
// {
//     var comments = Path.Combine(builder.Environment.ContentRootPath, "SafeSender.StorageAPI.xml");
//     options.IncludeXmlComments(comments);
// });

var storageSettingsSection = builder.Configuration.GetSection(StorageOptions.SettingsPath);

// Options registration
builder.Services.Configure<StorageOptions>(storageSettingsSection);
builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection("Application:Database"));

// Repositories registration
var repositoryType = GetFileStorageRepositoryType(storageSettingsSection);
builder.Services.AddScoped(typeof(IFilesRepository), repositoryType);

builder.Services.AddScoped<IFilesInternalInfosRepository, FileInternalInfosRepository>();

// DB registration
builder.Services.AddSingleton<IMongoDbContext, MongoDbContext>();

// API clients registration
builder.Services.AddScoped<IAnonfilesApiClient, AnonfilesApiClient>();

// Services registration
builder.Services.AddScoped<IFilesService, FilesService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.MapGet("api/download/{token}", async (
        [Required] string token,
        [FromServices] IFilesService filesService) =>
    {
        if(string.IsNullOrEmpty(token))
        {
            return Results.BadRequest();
        }
        
        var downloadFileModel = await filesService.DownloadFile(token);
        
        return Results.Ok(downloadFileModel);
    })
    .WithName("GetFile")
    .Produces<DownloadFileResponseModel>()
    .Produces(StatusCodes.Status400BadRequest);

app.MapPost("api/upload", async (
        [FromBody] UploadFileRequestModel model,
        [FromServices] IFilesService filesService) =>
    {
        if (!model.FileBytes.Any())
        {
            return Results.BadRequest();
        }
        
        var result = await filesService.UploadFile(model);

        return result.status
            ? Results.CreatedAtRoute("GetFile", new { result.token })
            : Results.StatusCode(StatusCodes.Status500InternalServerError);
    })
    .WithName("SaveFile")
    .Produces(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status500InternalServerError);

app.Run();

Type GetFileStorageRepositoryType(IConfigurationSection configurationSection)
{
    Enum.TryParse(configurationSection.GetSection("Type").Value, out StorageType storage);

    return storage switch
        {
            StorageType.External => typeof(ExternalStorageRepository),
            StorageType.Local => typeof(LocalStorageRepository),
            _ => throw new ArgumentOutOfRangeException(nameof(storage), storage,
                "Storage type is not specified in settings"),
        };
}