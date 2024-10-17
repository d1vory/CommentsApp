

using CommentsApp.Data;
using CommentsApp.Services;
using CommentsApp.Utils;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddHttpLogging(o => { });
builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddDbContext<BaseApplicationContext, ApplicationContext>(ServiceLifetime.Transient);

builder.Services.AddTransient<CommentService>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.WebRootPath.IsNullOrEmpty())
{
    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"));
}

app.UseCors(a => a.AllowAnyOrigin());
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UsePathBase(new PathString("/api"));
app.UseRouting();
app.MapControllers();
app.UseHttpLogging();

app.Run();
