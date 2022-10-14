using Microsoft.EntityFrameworkCore;
using UCS_Backend.Data;
using UCS_Backend.Interfaces;
using UCS_Backend.Interfaces.IManagers;
using UCS_Backend.Interfaces.IRepositories;
using UCS_Backend.Managers;
using UCS_Backend.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IIndividualRepository, IndividualRepository>();
builder.Services.AddTransient<IScheduleRepository, ScheduleRepository>();
builder.Services.AddTransient<IRoomRepository, RoomRepository>();
builder.Services.AddTransient<IScheduleManager, ScheduleManager>();
builder.Services.AddTransient<IClassRepository, ClassRepository>();

// Add DBContext
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("UCS_DB"));
});

// Enable Cors
builder.Services.AddCors(c =>
{
    c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200"));

app.UseAuthorization();

app.MapControllers();

app.Run();
