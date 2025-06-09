/* 
 * Note: This solution was made in dotNET version 9 but it will 
 * only include features from the LTS version. 8.-
 */

/*
 * You can use the swagger Identity Endpoints:
 * 1.- Register a new user. (/register)
 * 2.- Login with the your new user data (/login)
 *   2.a. You can choose between cookie or bearer token
 * 3.- From the login (/login) respose you will get a AccessToken and RefreshToken.
 * 4.- You can use /refresh endpoint with you RefreshToken to get a new AccessToken with new ExpirationDate.
*/

using DotNet8_Identity.Database;
using DotNet8_Identity.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Authentication & Authorization
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme; // Or Bearer
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
}).AddCookie(IdentityConstants.ApplicationScheme)
  .AddBearerToken(IdentityConstants.BearerScheme);

// Identity
builder.Services.AddIdentityCore<User>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddApiEndpoints();

// Db setup
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Migrate database
    app.ApplyMigrations();

    // Swagger
    app.UseSwagger();
    app.UseSwaggerUI();
}

// You can test using this protected endpoint..!
app.MapGet("users/me", async (ClaimsPrincipal claims, ApplicationDbContext context) =>
{
    string userId = claims.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

    var user = await context.Users.FindAsync(userId);

    return user;
    
}).RequireAuthorization();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapIdentityApi<User>();

app.Run();
