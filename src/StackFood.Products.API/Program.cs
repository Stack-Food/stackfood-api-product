using Microsoft.EntityFrameworkCore;
using StackFood.Products.Application.Interfaces;
using StackFood.Products.Application.UseCases;
using StackFood.Products.Infrastructure.Persistence;
using StackFood.Products.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() {
        Title = "StackFood Products API",
        Version = "v1",
        Description = "API for managing products and categories"
    });
});

// Database Configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ProductsDbContext>(options =>
    options.UseNpgsql(connectionString));

// Repository Registration
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

// Use Case Registration
builder.Services.AddScoped<GetAllProductsUseCase>();
builder.Services.AddScoped<GetProductByIdUseCase>();
builder.Services.AddScoped<CreateProductUseCase>();
builder.Services.AddScoped<UpdateProductUseCase>();
builder.Services.AddScoped<DeleteProductUseCase>();
builder.Services.AddScoped<GetAllCategoriesUseCase>();

// Health Checks
builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString, name: "database");

// CORS
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

// Apply migrations and seed data
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ProductsDbContext>();

    try
    {
        dbContext.Database.Migrate();
        await SeedData(dbContext);
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();

// Seed Data Method
static async Task SeedData(ProductsDbContext context)
{
    if (await context.Categories.AnyAsync())
        return; // Database already seeded

    var categories = new[]
    {
        new StackFood.Products.Domain.Entities.Category("Lanche", "Hambúrgueres e sanduíches", 1),
        new StackFood.Products.Domain.Entities.Category("Acompanhamento", "Batatas fritas, onion rings, etc.", 2),
        new StackFood.Products.Domain.Entities.Category("Bebida", "Refrigerantes, sucos, água", 3),
        new StackFood.Products.Domain.Entities.Category("Sobremesa", "Sorvetes, tortas, brownies", 4)
    };

    await context.Categories.AddRangeAsync(categories);
    await context.SaveChangesAsync();

    var lancheCategory = categories[0];
    var acompanhamentoCategory = categories[1];
    var bebidaCategory = categories[2];
    var sobremesaCategory = categories[3];

    var products = new[]
    {
        // Lanches
        new StackFood.Products.Domain.Entities.Product(
            lancheCategory.Id, "X-Burger", "Hambúrguer clássico com queijo",
            new StackFood.Products.Domain.ValueObjects.Money(25.90m)),
        new StackFood.Products.Domain.Entities.Product(
            lancheCategory.Id, "X-Bacon", "Hambúrguer com bacon crocante",
            new StackFood.Products.Domain.ValueObjects.Money(28.90m)),
        new StackFood.Products.Domain.Entities.Product(
            lancheCategory.Id, "X-Tudo", "Hambúrguer completo com todos os ingredientes",
            new StackFood.Products.Domain.ValueObjects.Money(32.90m)),

        // Acompanhamentos
        new StackFood.Products.Domain.Entities.Product(
            acompanhamentoCategory.Id, "Batata Frita", "Porção de batatas fritas crocantes",
            new StackFood.Products.Domain.ValueObjects.Money(12.90m)),
        new StackFood.Products.Domain.Entities.Product(
            acompanhamentoCategory.Id, "Onion Rings", "Anéis de cebola empanados",
            new StackFood.Products.Domain.ValueObjects.Money(14.90m)),

        // Bebidas
        new StackFood.Products.Domain.Entities.Product(
            bebidaCategory.Id, "Coca-Cola 350ml", "Refrigerante lata",
            new StackFood.Products.Domain.ValueObjects.Money(6.00m)),
        new StackFood.Products.Domain.Entities.Product(
            bebidaCategory.Id, "Suco Natural 500ml", "Suco de laranja natural",
            new StackFood.Products.Domain.ValueObjects.Money(9.00m)),

        // Sobremesas
        new StackFood.Products.Domain.Entities.Product(
            sobremesaCategory.Id, "Sundae", "Sorvete com cobertura de chocolate",
            new StackFood.Products.Domain.ValueObjects.Money(10.90m)),
        new StackFood.Products.Domain.Entities.Product(
            sobremesaCategory.Id, "Brownie", "Brownie de chocolate com nozes",
            new StackFood.Products.Domain.ValueObjects.Money(12.90m))
    };

    await context.Products.AddRangeAsync(products);
    await context.SaveChangesAsync();
}
