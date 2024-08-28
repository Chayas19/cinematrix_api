using System.Globalization;
using System.Text;
using CineMatrix_API;
using CineMatrix_API.Filters;
using CineMatrix_API.Repository;
using CineMatrix_API.Services;
using CineMatrix_API.Validations;
using FluentAssertions.Common;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<UserCreationDTOValidator>());
builder.Services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<LoginDTOValidator>());
builder.Services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<PaginationDTOValidator>());
builder.Services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<LoginDTOValidator>());
builder.Services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<ReviewDTOValidation>());
builder.Services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<UserRolesDTOValidation>());
builder.Services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<SubscribeDTOValidation>());




builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<OtpService>();
builder.Services.AddScoped<Passwordservice>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ISMSService, Smsservice>();
builder.Services.AddScoped<IOtpService, OtpService>();


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


builder.Services.AddScoped<JwtService>(provider =>
{
    return new JwtService(key, issuer, audience, accessTokenExpirationMinutes);
});


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
builder.Services.AddSwaggerGen((c =>
{
   
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CineMatrix API", Version = "v1" });
    c.EnableAnnotations();


}));
builder.Services.AddSwaggerExamplesFromAssemblyOf<PersonCreatioExample>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles(); 

app.UseHttpsRedirection();

app.UseCors("AllowAll"); // Use CORS policy

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();

app.Run();
