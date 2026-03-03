WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSignalR();

builder.Services.AddScoped<IBoardsService, BoardsService>();
builder.Services.AddScoped<IColumnService, ColumnService>();
builder.Services.AddScoped<ITasksService, TasksService>();

builder.Services.AddDbContext<MainDbContext>(opt =>
{
    string hostName = "localhost";
    int port = 5432;
    string databaseName = "kanbanDb";
    bool pooling = true;
    string user = "postgres";
    string password = "Vladi98*";

    opt.UseNpgsql($"Host={hostName};Port={port};Database={databaseName};Pooling={pooling};User ID={user};Password={password};");
});

WebApplication app = builder.Build();

AsyncServiceScope scope = app.Services.CreateAsyncScope();
await using MainDbContext context = scope.ServiceProvider.GetRequiredService<MainDbContext>();

if (context.Database.IsRelational())
{
    await context.Database.MigrateAsync();
}

app.UseDefaultFiles();
app.MapStaticAssets();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Kanban Board")
               .WithTheme(ScalarTheme.Mars)
               .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("/index.html");

app.Run();
