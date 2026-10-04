var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/users/swagger.json", "UserService");
    options.SwaggerEndpoint("/swagger/finance/swagger.json", "FinanceService");
    options.EnablePersistAuthorization();
});

app.MapReverseProxy();

app.Run();
