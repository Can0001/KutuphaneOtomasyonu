using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Business.Abstract;
using Business.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Kitaplar
builder.Services.AddScoped<Business.Abstract.IBookService, Business.Concrete.BookManager>();
builder.Services.AddScoped<DataAccess.Abstract.IBookDal, DataAccess.Concrete.EntityFramework.EfBookDal>();

// Kullanýcýlar
builder.Services.AddScoped<Business.Abstract.IUserService, Business.Concrete.UserManager>();
builder.Services.AddScoped<DataAccess.Abstract.IUserDal, DataAccess.Concrete.EntityFramework.EfUserDal>();

// Rezervasyonlar
builder.Services.AddScoped<Business.Abstract.IReservationService, Business.Concrete.ReservationManager>();
builder.Services.AddScoped<DataAccess.Abstract.IReservationDal, DataAccess.Concrete.EntityFramework.EfReservationDal>();

// Auth ve Token
builder.Services.AddScoped<Business.Abstract.IAuthService, Business.Concrete.AuthManager>();
builder.Services.AddScoped<Core.Utilities.Security.JWT.ITokenHelper, Core.Utilities.Security.JWT.JwtHelper>();

builder.Services.AddSwaggerGen();

// --- JWT KÝMLÝK DOÐRULAMA AYARLARI ---
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = "library.com",
            ValidAudience = "library.com",
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("KutuphaneOtomasyonu_Icin_Kirilamaz_Gizli_Anahtar_2026!?*"))
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles(); // wwwroot klasörünü eriþime açar

app.UseHttpsRedirection();

// --- GÜVENLÝK DUVARI SIRALAMASI (Burasý Hayati Önem Taþýr) ---
app.UseCors("AllowAll"); // Önce kapýyý açýyoruz
app.UseAuthentication(); // Sonra kimlik soruyoruz
app.UseAuthorization();  // Sonra yetkisine bakýyoruz

app.MapControllers();

app.Run();