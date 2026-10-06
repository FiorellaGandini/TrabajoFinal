using NUnit.Framework;
using GestionLogica.Entidades;
using GestionLogica.Enums;

namespace GestionLogicaTest;

[TestFixture]
public class EntradaTest
{
    private static Ubicacion CrearUbicacion()
    {
        return new Ubicacion("Teatro Colón", "Cerrito 628");
    }

    private static Evento CrearEvento(DateTime fechaHora, int id = 1)
    {
        return new Evento(id, "Recital", "Descripción del evento", fechaHora, CrearUbicacion(),
            CategoriaEvento.Concierto, prioridad: false, horasAperturaAntes: 1, duracionHoras: 2);
    }

    [Test]
    public void Constructor_CodigoVacio_LanzaArgumentException()
    {
        // Arrange
        string codigoInvalido = "";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Entrada(codigoInvalido, 1, 1, 1));
    }

    [Test]
    public void Constructor_CodigoConMenosDeSeisCaracteres_LanzaArgumentException()
    {
        // Arrange
        string codigoCorto = "ABC12";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Entrada(codigoCorto, 1, 1, 1));
    }

    [Test]
    public void Constructor_CodigoConMasDeSeisCaracteres_LanzaArgumentException()
    {
        // Arrange
        string codigoLargo = "ABC1234";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Entrada(codigoLargo, 1, 1, 1));
    }

    [Test]
    public void Constructor_CodigoConCaracterNoAlfanumerico_LanzaArgumentException()
    {
        // Arrange
        string codigoConGuion = "ABC-12";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Entrada(codigoConGuion, 1, 1, 1));
    }

    [Test]
    public void Constructor_CodigoValidoEnMinusculas_SeGuardaEnMayusculas()
    {
        // Arrange
        string codigoEnMinusculas = "abc123";

        // Act
        var entrada = new Entrada(codigoEnMinusculas, 1, 1, 1);

        // Assert
        Assert.That(entrada.Codigo, Is.EqualTo("ABC123"));
    }

    [Test]
    public void Constructor_EntradaNueva_ArrancaEnEstadoVendida()
    {
        // Arrange & Act
        var entrada = new Entrada("ABC123", 1, 1, 1);

        // Assert
        Assert.That(entrada.Estado, Is.EqualTo(EstadoEntrada.Vendida));
    }

    [Test]
    public void MarcarComoUsada_EventoDistintoAlDeLaEntrada_LanzaInvalidOperationException()
    {
        // Arrange
        var entrada = new Entrada("ABC123", 1, 1, 1);
        var otroEvento = CrearEvento(DateTime.Now.AddHours(1), id: 2);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => entrada.MarcarComoUsada(otroEvento, DateTime.Now));
    }

    [Test]
    public void MarcarComoUsada_MomentoFueraDeLaVentanaDeIngreso_LanzaInvalidOperationException()
    {
        // Arrange
        var fechaHora = DateTime.Now.AddDays(1);
        var evento = CrearEvento(fechaHora, id: 1);
        var entrada = new Entrada("ABC123", evento.Id, 1, 1);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => entrada.MarcarComoUsada(evento, DateTime.Now));
    }

    [Test]
    public void MarcarComoUsada_EntradaQueYaFueUsada_LanzaInvalidOperationException()
    {
        // Arrange
        var fechaHora = DateTime.Now.AddHours(1);
        var evento = CrearEvento(fechaHora, id: 1);
        var entrada = new Entrada("ABC123", evento.Id, 1, 1);
        entrada.MarcarComoUsada(evento, evento.HorarioAperturaIngreso);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => entrada.MarcarComoUsada(evento, evento.HorarioAperturaIngreso));
    }

    [Test]
    public void MarcarComoUsada_EntradaCancelada_LanzaInvalidOperationException()
    {
        // Arrange
        var fechaHora = DateTime.Now.AddHours(1);
        var evento = CrearEvento(fechaHora, id: 1);
        var entrada = new Entrada("ABC123", evento.Id, 1, 1);
        entrada.Cancelar();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => entrada.MarcarComoUsada(evento, evento.HorarioAperturaIngreso));
    }

    [Test]
    public void MarcarComoUsada_CasoValido_CambiaEstadoAUsada()
    {
        // Arrange
        var fechaHora = DateTime.Now.AddHours(1);
        var evento = CrearEvento(fechaHora, id: 1);
        var entrada = new Entrada("ABC123", evento.Id, 1, 1);
        var momento = evento.HorarioAperturaIngreso;

        // Act
        entrada.MarcarComoUsada(evento, momento);

        // Assert
        Assert.That(entrada.Estado, Is.EqualTo(EstadoEntrada.Usada));
    }

    [Test]
    public void MarcarComoUsada_CasoValido_FijaLaFechaDeValidacion()
    {
        // Arrange
        var fechaHora = DateTime.Now.AddHours(1);
        var evento = CrearEvento(fechaHora, id: 1);
        var entrada = new Entrada("ABC123", evento.Id, 1, 1);
        var momento = evento.HorarioAperturaIngreso;

        // Act
        entrada.MarcarComoUsada(evento, momento);

        // Assert
        Assert.That(entrada.FechaValidacion, Is.EqualTo(momento));
    }

    [Test]
    public void Cancelar_EntradaVendida_CambiaEstadoACancelada()
    {
        // Arrange
        var entrada = new Entrada("ABC123", 1, 1, 1);

        // Act
        entrada.Cancelar();

        // Assert
        Assert.That(entrada.Estado, Is.EqualTo(EstadoEntrada.Cancelada));
    }

    [Test]
    public void Cancelar_EntradaYaUsada_LanzaInvalidOperationException()
    {
        // Arrange
        var fechaHora = DateTime.Now.AddHours(1);
        var evento = CrearEvento(fechaHora, id: 1);
        var entrada = new Entrada("ABC123", evento.Id, 1, 1);
        entrada.MarcarComoUsada(evento, evento.HorarioAperturaIngreso);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => entrada.Cancelar());
    }
}