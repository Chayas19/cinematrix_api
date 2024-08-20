using System.Text;
using CineMatrix_API;
using CineMatrix_API.Filters;
using CineMatrix_API.Repository;
using CineMatrix_API.Services;
using CineMatrix_API.Validations;
using FluentValidation; // Ensure you have this namespace included if User model is here
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddValidatorsFromAssemblyContaining<UserCreationDTOValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<LoginDTOValidator>();




// Configure database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register services
builder.Services.AddScoped<OtpService>();
builder.Services.AddScoped<Passwordservice>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ISMSService, Smsservice>();
builder.Services.AddScoped<IEmailService, EmailService>();



builder.Services.AddControllers(options =>
{
    // Register the exception filter globally
    options.Filters.Add<ExceptionFilter>();
});


// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Configure JWT settings
var jwtSection = builder.Configuration.GetSection("Jwt");
var issuer = jwtSection["Issuer"];
var audience = jwtSection["Audience"];
var key = jwtSection["Key"];
var accessTokenExpirationMinutes = jwtSection.GetValue<int>("AccessTokenExpirationMinutes");

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("PrimeUserOnly", policy => policy.RequireRole("PrimeUser"));
});


// Register JwtService with the required parameters using a factory pattern
builder.Services.AddScoped<JwtService>(provider =>
{
    return new JwtService(key, issuer, audience, accessTokenExpirationMinutes);
});

// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
    };
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles(); // This line enables serving static files from wwwroot

app.UseHttpsRedirection();

app.UseCors("AllowAll"); // Use CORS policy

app.UseAuthentication(); // Enable authentication
app.UseAuthorization();

app.MapControllers();

app.Run();
