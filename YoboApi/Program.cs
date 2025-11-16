using YoboApi.Data;
using Microsoft.EntityFrameworkCore;
using YoboApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlite(builder.Configuration.GetConnectionString("YoboConnection"))
);

builder.Services.AddIdentityCore<AppUser>(options =>
{
    options.User.RequireUniqueEmail = true;

    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;

})
.AddEntityFrameworkStores<AppDbContext>().AddSignInManager();

var jwt = builder.Configuration.GetSection("JwtSettings");

var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));

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
       ValidIssuer = jwt["Issuer"],
       ValidAudience = jwt["Audience"],
       IssuerSigningKey = key,
       ClockSkew = TimeSpan.Zero
   }; 
});

builder.Services.AddCors(o =>
{
   o.AddDefaultPolicy(p=>
    p.WithOrigins("http://localhost:5173")
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()
   ); 
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
   var scheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
   {
       Name = "Authorization",
       Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
       Scheme = "bearer",
       BearerFormat = "JWT",
       In = Microsoft.OpenApi.Models.ParameterLocation.Header,
       Description = "Bearer {token}"
   };
   c.AddSecurityDefinition("Bearer",scheme);
   c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement { {scheme, Array.Empty<string>() } });
});

builder.Services.AddControllers();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
