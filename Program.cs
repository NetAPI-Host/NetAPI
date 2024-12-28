var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Middleware to enforce authentication
app.Use(async (context, next) =>
{
    if (!context.Request.Headers.ContainsKey("Authorization"))
    {
        context.Response.StatusCode = 401; // Unauthorized
        await context.Response.WriteAsync("Missing Authorization header.");
        return;
    }

    var token = context.Request.Headers["Authorization"].ToString();
    if (token != "Bearer MySecureToken")
    {
        context.Response.StatusCode = 403; // Forbidden
        await context.Response.WriteAsync("Invalid token.");
        return;
    }

    await next();
});

app.MapControllers();

app.Run();
