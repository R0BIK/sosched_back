using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using SoschedBack;
using SoschedBack.Storage;

var builder = WebApplication.CreateBuilder(args);

Env.TraversePath().Load();

builder.AddServices();

var app = builder.Build();
await app.Configure();
app.Run();
