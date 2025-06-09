
/* 
 * Note: This solution was made in dotNET version 9 but it will 
 * only include features from the LTS version. 8.-
 * 
 */

using DotNet8_Identity.Database;
using DotNet8_Identity.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddAuthorization();
// methid will be Cookie at the moment.
builder.Services.AddAuthentication().AddCookie(IdentityConstants.ApplicationScheme); 

builder.Services.AddIdentityCore<User>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddApiEndpoints();

// Db setup
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // from the helper.
    app.ApplyMigrations();
}

app.UseHttpsRedirection();

app.MapIdentityApi<User>();

app.Run();
