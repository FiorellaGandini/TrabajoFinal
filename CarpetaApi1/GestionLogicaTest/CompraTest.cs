using NUnit.Framework;
using GestionLogica.Entidades;

namespace GestionLogicaTest;

[TestFixture]
public class CompraTest
{
    [Test]
    public void Constructor_IdInvalido_LanzaArgumentException()
    {
        // Arrange
        int idInvalido = 0;

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Compra(idInvalido, 40123456, 1, 1, 2, 100m, DateTime.Now));
    }

    [Test]
    public void Constructor_DniCompradorInvalido_LanzaArgumentException()
    {
        // Arrange
        int dniInvalido = 0;

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Compra(1, dniInvalido, 1, 1, 2, 100m, DateTime.Now));
    }

    [Test]
    public void Constructor_EventoIdInvalido_LanzaArgumentException()
    {
        // Arrange
        int eventoIdInvalido = 0;

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Compra(1, 40123456, eventoIdInvalido, 1, 2, 100m, DateTime.Now));
    }

    [Test]
    public void Constructor_ModalidadEntradaIdInvalido_LanzaArgumentException()
    {
        // Arrange
        int modalidadIdInvalida = 0;

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Compra(1, 40123456, 1, modalidadIdInvalida, 2, 100m, DateTime.Now));
    }

    [Test]
    public void Constructor_CantidadInvalida_LanzaArgumentException()
    {
        // Arrange
        int cantidadInvalida = 0;

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Compra(1, 40123456, 1, 1, cantidadInvalida, 100m, DateTime.Now));
    }

    [Test]
    public void Constructor_PrecioUnitarioNegativo_LanzaArgumentException()
    {
        // Arrange
        decimal precioInvalido = -1m;

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Compra(1, 40123456, 1, 1, 2, precioInvalido, DateTime.Now));
    }

    //15% de descuento a partir de 5 entradas

    [Test]
    public void TieneDescuento_CantidadMenorACinco_DevuelveFalse()
    {
        // Arrange
        var compra = new Compra(1, 40123456, 1, 1, 4, 100m, DateTime.Now);

        // Act
        var resultado = compra.TieneDescuento();

        // Assert
        Assert.That(resultado, Is.False);
    }

    [Test]
    public void TieneDescuento_CantidadIgualACinco_DevuelveTrue()
    {
        // Arrange
        var compra = new Compra(1, 40123456, 1, 1, 5, 100m, DateTime.Now);

        // Act
        var resultado = compra.TieneDescuento();

        // Assert
        Assert.That(resultado, Is.True);
    }

    [Test]
    public void TieneDescuento_CantidadMayorACinco_DevuelveTrue()
    {
        // Arrange
        var compra = new Compra(1, 40123456, 1, 1, 8, 100m, DateTime.Now);

        // Act
        var resultado = compra.TieneDescuento();

        // Assert
        Assert.That(resultado, Is.True);
    }

    [Test]
    public void Constructor_CantidadMenorACinco_PrecioTotalSinDescuento()
    {
        // Arrange & Act
        var compra = new Compra(1, 40123456, 1, 1, 4, 100m, DateTime.Now);

        // Assert
        Assert.That(compra.PrecioTotal, Is.EqualTo(400m));
    }

    [Test]
    public void Constructor_CantidadIgualACinco_PrecioTotalConQuinceCientoDeDescuento()
    {
        // Arrange & Act
        var compra = new Compra(1, 40123456, 1, 1, 5, 100m, DateTime.Now);

        // Assert
        Assert.That(compra.PrecioTotal, Is.EqualTo(425m)); // 500 - 15%
    }

    [Test]
    public void Constructor_CantidadMayorACinco_PrecioTotalConQuinceCientoDeDescuento()
    {
        // Arrange & Act
        var compra = new Compra(1, 40123456, 1, 1, 8, 100m, DateTime.Now);

        // Assert
        Assert.That(compra.PrecioTotal, Is.EqualTo(680m)); // 800 - 15%
    }

    //Agregar codigo entrada

    [Test]
    public void AgregarCodigoEntrada_CodigoVacio_LanzaArgumentException()
    {
        // Arrange
        var compra = new Compra(1, 40123456, 1, 1, 2, 100m, DateTime.Now);
        string codigoInvalido = "";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => compra.AgregarCodigoEntrada(codigoInvalido));
    }

    [Test]
    public void AgregarCodigoEntrada_CasoValido_AgregaElCodigoALaLista()
    {
        // Arrange
        var compra = new Compra(1, 40123456, 1, 1, 2, 100m, DateTime.Now);

        // Act
        compra.AgregarCodigoEntrada("abc123");

        // Assert
        Assert.That(compra.CodigosEntradas, Has.Count.EqualTo(1));
    }

    [Test]
    public void AgregarCodigoEntrada_SuperaLaCantidadComprada_LanzaInvalidOperationException()
    {
        // Arrange
        var compra = new Compra(1, 40123456, 1, 1, 1, 100m, DateTime.Now);
        compra.AgregarCodigoEntrada("abc123");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => compra.AgregarCodigoEntrada("def456"));
    }
}