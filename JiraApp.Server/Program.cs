WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSignalR();

builder.Services.AddScoped<IBoardsService, BoardsService>();
builder.Services.AddScoped<IColumnService, ColumnService>();
builder.Services.AddScoped<ITasksService, TasksService>();

builder.Services.AddScoped<IValidator<CreateBoardDto>, CreateBoardValidator>();
builder.Services.AddScoped<IValidator<EditBoardDto>, EditBoardValidator>();
builder.Services.AddScoped<IValidator<CreateColumnDto>, CreateColumnValidator>();
builder.Services.AddScoped<IValidator<EditColumnDto>, EditColumnValidator>();
builder.Services.AddScoped<IValidator<ReorderColumnDto>, ReorderColumnValidator>();
builder.Services.AddScoped<IValidator<CreateTaskDto>, CreateTaskValidator>();
builder.Services.AddScoped<IValidator<EditTaskDto>, EditTaskValidator>();
builder.Services.AddScoped<IValidator<MoveTaskDto>, MoveTaskValidator>();
builder.Services.AddScoped<IValidator<ReorderTaskDto>, ReorderTaskValidator>();

string connStringName = "MainDbContext";
string connectionString = builder.Configuration.GetConnectionString(connStringName)
    ?? throw new InvalidOperationException($"Missing connection string {connStringName}");

builder.Services.AddDbContext<MainDbContext>(opt =>
{
    opt.UseNpgsql(connectionString);
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

app.MapHub<BoardHub>("/hubs/board");
app.MapHub<ColumnHub>("/hubs/column");
app.MapHub<TaskHub>("/hubs/task");
app.MapControllers();
app.MapFallbackToFile("/index.html");

app.Run();
