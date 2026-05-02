using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Business.Abstract;
using Business.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using Core.Utilities.Security.JWT;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Kitaplar
builder.Services.AddScoped<IBookService, BookManager>();
builder.Services.AddScoped<IBookDal, EfBookDal>();

// Kullanýcýlar
builder.Services.AddScoped<IUserService, UserManager>();
builder.Services.AddScoped<IUserDal, EfUserDal>();

// Rezervasyonlar
builder.Services.AddScoped<IReservationService, ReservationManager>();
builder.Services.AddScoped<IReservationDal, EfReservationDal>();

// Auth ve Token
builder.Services.AddScoped<IAuthService, AuthManager>();
builder.Services.AddScoped<ITokenHelper, JwtHelper>();

// --- ADMIN PANELI VE ENVANTER BAÐIMLILIKLARI ---
builder.Services.AddScoped<IStaffService, StaffManager>();
builder.Services.AddScoped<IStaffDal, EfStaffDal>();

builder.Services.AddScoped<IStudentService, StudentManager>();
builder.Services.AddScoped<IStudentDal, EfStudentDal>();

builder.Services.AddScoped<ICategoryService, CategoryManager>();
builder.Services.AddScoped<ICategoryDal, EfCategoryDal>();

builder.Services.AddScoped<IBookTransactionService, BookTransactionManager>();
builder.Services.AddScoped<IBookTransactionDal, EfBookTransactionDal>();

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

app.UseCors("AllowAll"); 
app.UseAuthentication(); 
app.UseAuthorization();  

app.MapControllers();

app.Run();