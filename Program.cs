// using Microsoft.AspNetCore.Authentication.Cookies;
// using Microsoft.AspNetCore.DataProtection;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.IdentityModel.Tokens;
// using Prometheus;
// using System.Text;
// using WebRestaurant.Data;
// using WebRestaurant.Services;

// var builder = WebApplication.CreateBuilder(args);

// // Đọc cấu hình từ appsettings.json
// builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// // // Cấu hình Kestrel để bind tới 0.0.0.0 và PORT
// // var port = Environment.GetEnvironmentVariable("PORT") ?? "8090"; // Mặc định 8090 nếu không có PORT
// // builder.WebHost.UseKestrel(options =>
// // {
// //     options.ListenAnyIP(int.Parse(port)); // Bind tới 0.0.0.0
// // });

// builder.WebHost.UseIIS();

// // Thêm DbContext
// builder.Services.AddDbContext<AppDbContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// // Cấu hình Data Protection
// builder.Services.AddDataProtection()
//     .PersistKeysToFileSystem(new DirectoryInfo("/app/keys")); // Lưu key vào volume

// // Cấu hình authentication với Cookie, JWT và Google
// var jwtSettings = builder.Configuration.GetSection("Jwt");
// var jwtKey = jwtSettings["Key"];
// if (string.IsNullOrEmpty(jwtKey))
// {
//     throw new InvalidOperationException("Jwt:Key is not configured in appsettings.json.");
// }
// var key = Encoding.ASCII.GetBytes(jwtKey);

// builder.Services.AddAuthentication(options =>
// {
//     options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//     options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
// })
// .AddCookie(options =>
// {
//     options.LoginPath = "/Auth/Login";
// })
// .AddJwtBearer(options =>
// {
//     options.RequireHttpsMetadata = false;
//     options.SaveToken = true;
//     options.TokenValidationParameters = new TokenValidationParameters
//     {
//         ValidateIssuerSigningKey = true,
//         IssuerSigningKey = new SymmetricSecurityKey(key),
//         ValidateIssuer = true,
//         ValidIssuer = jwtSettings["Issuer"],
//         ValidateAudience = true,
//         ValidAudience = jwtSettings["Audience"],
//         ValidateLifetime = true,
//         ClockSkew = TimeSpan.Zero
//     };
// })
// .AddGoogle(googleOptions =>
// {
//     var googleAuth = builder.Configuration.GetSection("Authentication:Google");
//     googleOptions.ClientId = googleAuth["ClientId"];
//     googleOptions.ClientSecret = googleAuth["ClientSecret"];
//     googleOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
// });

// builder.Services.AddControllersWithViews();
// builder.Services.AddScoped<JwtService>();

// builder.Services.AddCors(options =>
// {
//     options.AddPolicy("AllowAll", policy =>
//     {
//         policy.AllowAnyOrigin()
//               .AllowAnyMethod()
//               .AllowAnyHeader();
//     });
// });

// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

// builder.WebHost.UseUrls("http://0.0.0.0:5000");


// // Minio upload 
// builder.Services.AddSingleton<MinioService>();

// var app = builder.Build();

// if (!app.Environment.IsDevelopment())
// {
//     app.UseExceptionHandler("/Home/Error");
//     app.UseHsts();
// }

// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI(c =>
//     {
//         c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebRestaurant API V1");
//     });
// }

// app.UseHttpsRedirection();
// app.UseStaticFiles();
// app.UseRouting();
// app.UseCors("AllowAll");
// app.UseAuthentication();
// app.UseAuthorization();

// // Cấu hình Prometheus
// app.UseHttpMetrics(); // Thêm dòng này để thu thập metrics HTTP
// app.UseRouting();

// app.UseHttpMetrics();
// app.UseEndpoints(endpoints =>
// {
//     endpoints.MapControllers();
//     endpoints.MapMetrics(); // expose endpoint /metrics
// });

// app.MapControllerRoute(
//     name: "default",
//     pattern: "{controller=Home}/{action=Index}/{id?}");

// // Seed dữ liệu nếu chưa có
// using (var scope = app.Services.CreateScope())
// {
//     var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//     DataSeeder.SeedData(dbContext);
// }

// app.Run();

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Prometheus;
using System.Text;
using WebRestaurant.Data;
using WebRestaurant.Services;

var builder = WebApplication.CreateBuilder(args);

// Đọc cấu hình từ appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Cấu hình port chạy từ biến môi trường hoặc mặc định là 5000
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

builder.WebHost.UseUrls("http://0.0.0.0:5000");

// Thêm DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection is not configured.")));

// Cấu hình Data Protection
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/app/keys"));

// Cấu hình JWT & Cookie Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSettings["Key"];
if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("Jwt:Key is not configured in appsettings.json or environment variables.");
}
var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Auth/Login";
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
})
.AddGoogle(googleOptions =>
{
    var googleAuth = builder.Configuration.GetSection("Authentication:Google");
    googleOptions.ClientId = googleAuth["ClientId"] ?? throw new InvalidOperationException("Google ClientId is not configured.");
    googleOptions.ClientSecret = googleAuth["ClientSecret"] ?? throw new InvalidOperationException("Google ClientSecret is not configured.");
    googleOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
});

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<JwtService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Tích hợp MinIO
builder.Services.AddSingleton<MinioService>();

var app = builder.Build();

// Xử lý lỗi nếu không ở môi trường phát triển
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebRestaurant API V1");
    });
}

app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

// Prometheus metrics
app.UseHttpMetrics();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapMetrics();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed dữ liệu ban đầu
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DataSeeder.SeedData(dbContext);
}

app.Run();