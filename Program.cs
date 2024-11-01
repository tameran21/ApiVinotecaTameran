using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Swashbuckle.AspNetCore.Annotations; 

var builder = WebApplication.CreateBuilder(args);

// Configuración de MongoDB
var mongoClient = new MongoClient("mongodb+srv://tameranweb:tameranweb10302024@cluster0.8bvxr.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0"); 
var database = mongoClient.GetDatabase("vinoteca"); 
var vinosCollection = database.GetCollection<Vino>("vinos"); 

builder.Services.AddSingleton(vinosCollection);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations(); 
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Endpoint para obtener todos los vinos desde MongoDB
app.MapGet("/vinos", async (IMongoCollection<Vino> vinosCollection) =>
{
    var vinos = await vinosCollection.Find(new BsonDocument()).ToListAsync();
    return Results.Ok(vinos);
})
.WithName("GetVinos") 
.WithMetadata(new SwaggerOperationAttribute(summary: "Obtiene todos los vinos", description: "Devuelve una lista de todos los vinos disponibles en la colección de MongoDB"));

// Endpoint para obtener un vino específico por ID desde MongoDB
app.MapGet("/vinos/{id}", async (string id, IMongoCollection<Vino> vinosCollection) =>
{
    var vino = await vinosCollection.Find(v => v._Id == id).FirstOrDefaultAsync();
    return vino is not null ? Results.Ok(vino) : Results.NotFound();
})
.WithName("GetVinoById")
.WithMetadata(new SwaggerOperationAttribute(summary: "Obtiene un vino por ID", description: "Devuelve un vino específico de la colección según el ID proporcionado"));

// Endpoint para agregar un nuevo vino a MongoDB
app.MapPost("/vinos", async (Vino vino, IMongoCollection<Vino> vinosCollection) =>
{
    await vinosCollection.InsertOneAsync(vino);
    return Results.Created($"/vinos/{vino._Id}", vino);
})
.WithName("CreateVino")
.WithMetadata(new SwaggerOperationAttribute(summary: "Crea un nuevo vino", description: "Agrega un nuevo vino a la colección en MongoDB"));

app.Run();

// Modelo para los vinos en MongoDB
public class Vino
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string _Id { get; set; }

    [BsonElement("nombre")]
    public string Nombre { get; set; }

    [BsonElement("tituloDescripcion")]
    public string TituloDescripcion { get; set; }

    [BsonElement("descripcion")]
    public string Descripcion { get; set; }

    [BsonElement("imagen")]
    public string Imagen { get; set; }

    [BsonElement("background")]
    public string Background { get; set; }

    [BsonElement("backgroundFicha")]
    public string BackgroundFicha { get; set; }

    [BsonElement("cata")]
    public string? Cata { get; set; }

    [BsonElement("anejamiento")]
    public string Anejamiento { get; set; }

    [BsonElement("nombreAbreviado")]
    public string NombreAbreviado { get; set; }

    [BsonElement("mililitrosBotella")]
    public string MililitrosBotella { get; set; }

    [BsonElement("porcentajeAlcohol")]
    public string PorcentajeAlcohol { get; set; }
    public int id { get; set; }
}
