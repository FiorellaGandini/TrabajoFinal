using NUnit.Framework;
using ValidacionLogica.Entidades;
using ValidacionLogica.Enums;

namespace ValidacionLogicaTest;

[TestFixture]
public class EntradaTest
{
    private static readonly DateTime Momento = new(2026, 11, 1, 20, 30, 0);

    private static Entrada NuevaEntrada(string codigo = "ABC123", int eventoId = 1, int modalidadId = 1,
        int compraId = 1, EstadoEntrada estado = EstadoEntrada.Vendida) =>
        new Entrada(codigo, eventoId, modalidadId, compraId, estado, null);

    //Constructor: datos validos

    [Test]
    public void Constructor_CodigoValido_GuardaElCodigo()
    {
        // Arrange + Act
        var entrada = NuevaEntrada("AB12CD");

        // Assert
        Assert.That(entrada.Codigo, Is.EqualTo("AB12CD"));
    }

    [Test]
    public void Constructor_CodigoEnMinusculas_NormalizaAMayusculas()
    {
        // Arrange + Act
        var entrada = NuevaEntrada("abc123");

        // Assert
        Assert.That(entrada.Codigo, Is.EqualTo("ABC123"));
    }

    [Test]
    public void Constructor_EventoIdValido_GuardaElEventoId()
    {
        // Arrange + Act
        var entrada = NuevaEntrada(eventoId: 3);

        // Assert
        Assert.That(entrada.EventoId, Is.EqualTo(3));
    }

    [Test]
    public void Constructor_ModalidadIdValido_GuardaLaModalidad()
    {
        // Arrange + Act
        var entrada = NuevaEntrada(modalidadId: 2);

        // Assert
        Assert.That(entrada.ModalidadEntradaId, Is.EqualTo(2));
    }

    [Test]
    public void Constructor_CompraIdValido_GuardaLaCompra()
    {
        // Arrange + Act
        var entrada = NuevaEntrada(compraId: 7);

        // Assert
        Assert.That(entrada.CompraId, Is.EqualTo(7));
    }

    [Test]
    public void Constructor_EntradaVendida_GuardaElEstado()
    {
        // Arrange + Act
        var entrada = NuevaEntrada(estado: EstadoEntrada.Vendida);

        // Assert
        Assert.That(entrada.Estado, Is.EqualTo(EstadoEntrada.Vendida));
    }

    [Test]
    public void Constructor_EntradaNueva_NoTieneFechaDeValidacion()
    {
        // Arrange + Act
        var entrada = NuevaEntrada();

        // Assert
        Assert.That(entrada.FechaValidacion, Is.Null);
    }

    //Constructor: codigo invalido

    [Test]
    public void Constructor_CodigoVacio_LanzaArgumentException()
    {
        Assert.Throws<ArgumentException>(() => NuevaEntrada(""));
    }

    [Test]
    public void Constructor_CodigoSoloEspacios_LanzaArgumentException()
    {
        Assert.Throws<ArgumentException>(() => NuevaEntrada("   "));
    }

    [Test]
    public void Constructor_CodigoDeCincoCaracteres_LanzaArgumentException()
    {
        Assert.Throws<ArgumentException>(() => NuevaEntrada("ABC12"));
    }

    [Test]
    public void Constructor_CodigoDeSieteCaracteres_LanzaArgumentException()
    {
        Assert.Throws<ArgumentException>(() => NuevaEntrada("ABC1234"));
    }

    [Test]
    public void Constructor_CodigoConSimbolo_LanzaArgumentException()
    {
        Assert.Throws<ArgumentException>(() => NuevaEntrada("ABC-12"));
    }

    [Test]
    public void Constructor_CodigoConEspacioAdentro_LanzaArgumentException()
    {
        Assert.Throws<ArgumentException>(() => NuevaEntrada("ABC 12"));
    }

    [Test]
    public void Constructor_CodigoConCaracterNoAscii_LanzaArgumentException()
    {
        Assert.Throws<ArgumentException>(() => NuevaEntrada("ÁBC123"));
    }

    [Test]
    public void Constructor_CodigoVacio_MensajeIndicaQueNoPuedeEstarVacio()
    {
        // Arrange + Act
        var excepcion = Assert.Throws<ArgumentException>(() => NuevaEntrada(""));

        // Assert
        Assert.That(excepcion.Message, Is.EqualTo("El código no puede estar vacío."));
    }

    // ── Constructor: ids inválidos ──

    [Test]
    public void Constructor_EventoIdCero_LanzaArgumentException()
    {
        Assert.Throws<ArgumentException>(() => NuevaEntrada(eventoId: 0));
    }

    [Test]
    public void Constructor_EventoIdNegativo_LanzaArgumentException()
    {
        Assert.Throws<ArgumentException>(() => NuevaEntrada(eventoId: -1));
    }

    [Test]
    public void Constructor_ModalidadIdCero_LanzaArgumentException()
    {
        Assert.Throws<ArgumentException>(() => NuevaEntrada(modalidadId: 0));
    }

    [Test]
    public void Constructor_ModalidadIdNegativo_LanzaArgumentException()
    {
        Assert.Throws<ArgumentException>(() => NuevaEntrada(modalidadId: -1));
    }

    [Test]
    public void Constructor_CompraIdCero_LanzaArgumentException()
    {
        Assert.Throws<ArgumentException>(() => NuevaEntrada(compraId: 0));
    }

    [Test]
    public void Constructor_CompraIdNegativo_LanzaArgumentException()
    {
        Assert.Throws<ArgumentException>(() => NuevaEntrada(compraId: -1));
    }

    //MarcarComoUsada: entrada vendida

    [Test]
    public void MarcarComoUsada_EntradaVendida_PasaAEstadoUsada()
    {
        // Arrange
        var entrada = NuevaEntrada();

        // Act
        entrada.MarcarComoUsada(Momento);

        // Assert
        Assert.That(entrada.Estado, Is.EqualTo(EstadoEntrada.Usada));
    }

    [Test]
    public void MarcarComoUsada_EntradaVendida_RegistraElMomentoDeIngreso()
    {
        // Arrange
        var entrada = NuevaEntrada();

        // Act
        entrada.MarcarComoUsada(Momento);

        // Assert
        Assert.That(entrada.FechaValidacion, Is.EqualTo(Momento));
    }

    //MarcarComoUsada: entrada ya usada

    [Test]
    public void MarcarComoUsada_EntradaYaUsada_LanzaInvalidOperationException()
    {
        // Arrange
        var entrada = NuevaEntrada();
        entrada.MarcarComoUsada(Momento);

        // Act + Assert
        Assert.Throws<InvalidOperationException>(() => entrada.MarcarComoUsada(Momento.AddMinutes(5)));
    }

    [Test]
    public void MarcarComoUsada_EntradaYaUsada_MensajeIndicaQueYaFueUtilizada()
    {
        // Arrange
        var entrada = NuevaEntrada("ABC123");
        entrada.MarcarComoUsada(Momento);

        // Act
        var excepcion = Assert.Throws<InvalidOperationException>(() => entrada.MarcarComoUsada(Momento.AddMinutes(5)));

        // Assert
        Assert.That(excepcion.Message, Is.EqualTo("La entrada ABC123 ya fue utilizada."));
    }

    [Test]
    public void MarcarComoUsada_EntradaYaUsada_NoPisaLaFechaDelPrimerIngreso()
    {
        // Arrange
        var entrada = NuevaEntrada();
        entrada.MarcarComoUsada(Momento);

        // Act
        Assert.Throws<InvalidOperationException>(() => entrada.MarcarComoUsada(Momento.AddMinutes(5)));

        // Assert
        Assert.That(entrada.FechaValidacion, Is.EqualTo(Momento));
    }

    //MarcarComoUsada: entrada cancelada

    [Test]
    public void MarcarComoUsada_EntradaCancelada_LanzaInvalidOperationException()
    {
        // Arrange
        var entrada = NuevaEntrada(estado: EstadoEntrada.Cancelada);

        // Act + Assert
        Assert.Throws<InvalidOperationException>(() => entrada.MarcarComoUsada(Momento));
    }

    [Test]
    public void MarcarComoUsada_EntradaCancelada_MensajeIndicaQueFueCancelada()
    {
        // Arrange
        var entrada = NuevaEntrada("ABC123", estado: EstadoEntrada.Cancelada);

        // Act
        var excepcion = Assert.Throws<InvalidOperationException>(() => entrada.MarcarComoUsada(Momento));

        // Assert
        Assert.That(excepcion.Message, Is.EqualTo("La entrada ABC123 fue cancelada y no es válida."));
    }

    [Test]
    public void MarcarComoUsada_EntradaCancelada_ConservaElEstadoCancelada()
    {
        // Arrange
        var entrada = NuevaEntrada(estado: EstadoEntrada.Cancelada);

        // Act
        Assert.Throws<InvalidOperationException>(() => entrada.MarcarComoUsada(Momento));

        // Assert
        Assert.That(entrada.Estado, Is.EqualTo(EstadoEntrada.Cancelada));
    }
}