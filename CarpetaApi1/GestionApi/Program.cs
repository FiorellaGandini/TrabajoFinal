using GestionLogica.Servicios;

Environment.SetEnvironmentVariable("TZ", "America/Argentina/Buenos_Aires");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<UsuarioService>();
builder.Services.AddSingleton<EventoService>();
builder.Services.AddSingleton<CompraService>();
builder.Services.AddSingleton<EntradaService>();
builder.Services.AddSingleton<ReporteService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var usuarioService = scope.ServiceProvider.GetRequiredService<UsuarioService>();
    usuarioService.PrecargarSiNoExisten();
}

app.Run();