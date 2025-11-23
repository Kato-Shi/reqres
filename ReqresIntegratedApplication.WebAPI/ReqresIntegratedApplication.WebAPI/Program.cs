using ReqResIntegratedApplication.Integration.ReqresIntegration.Manager;
using ReqResIntegratedApplication.Integration.ReqresIntegration.Services;
using ReqresIntegratedApplication.WebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddHttpClient<IUserManager, UserManager>();
builder.Services.AddHttpClient<ReqResClient>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "http://127.0.0.1:5173",
                "http://localhost:4173",
                "http://127.0.0.1:4173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddScoped<UserServices>();
builder.Services.AddScoped<WarehouseDashboardService>();
builder.Services.AddScoped<EmployeeService>();
builder.Services.AddScoped<ResourceService>();
builder.Services.AddScoped<WarehouseAssignmentService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Frontend");

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
