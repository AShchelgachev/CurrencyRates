using FinanceService.Api.Extensions;
using FinanceService.Application;
using FinanceService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddSwaggerWithJwt();

var app = builder.Build();

app.UseSwagger();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
