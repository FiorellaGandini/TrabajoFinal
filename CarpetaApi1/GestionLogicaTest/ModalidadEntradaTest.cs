using NUnit.Framework;
using GestionLogica.Entidades;

namespace GestionLogicaTest;

[TestFixture]
public class ModalidadEntradaTest
{
    [Test]
    public void Constructor_IdInvalido_LanzaArgumentException()
    {
        // Arrange
        int idInvalido = 0;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new ModalidadEntrada(idInvalido, 1, "General", 1000, 10));
    }

    [Test]
    public void Constructor_EventoIdInvalido_LanzaArgumentException()
    {
        // Arrange
        int eventoIdInvalido = 0;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new ModalidadEntrada(1, eventoIdInvalido, "General", 1000, 10));
    }

    [Test]
    public void Constructor_NombreVacio_LanzaArgumentException()
    {
        // Arrange
        string nombreInvalido = "";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new ModalidadEntrada(1, 1, nombreInvalido, 1000, 10));
    }

    [Test]
    public void Constructor_PrecioNegativo_LanzaArgumentException()
    {
        // Arrange
        decimal precioInvalido = -1;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new ModalidadEntrada(1, 1, "General", precioInvalido, 10));
    }

    [Test]
    public void Constructor_CupoTotalCero_LanzaArgumentException()
    {
        // Arrange
        int cupoInvalido = 0;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new ModalidadEntrada(1, 1, "General", 1000, cupoInvalido));
    }

    [Test]
    public void CupoDisponible_SinVentasRegistradas_EsIgualAlCupoTotal()
    {
        // Arrange
        var modalidad = new ModalidadEntrada(1, 1, "General", 1000, 10);

        // Act
        var cupoDisponible = modalidad.CupoDisponible;

        // Assert
        Assert.That(cupoDisponible, Is.EqualTo(10));
    }

    [Test]
    public void HayCupoDisponible_CantidadMenorAlDisponible_DevuelveTrue()
    {
        // Arrange
        var modalidad = new ModalidadEntrada(1, 1, "General", 1000, 10);

        // Act
        var resultado = modalidad.HayCupoDisponible(5);

        // Assert
        Assert.That(resultado, Is.True);
    }

    [Test]
    public void HayCupoDisponible_CantidadIgualAlDisponible_DevuelveTrue()
    {
        // Arrange
        var modalidad = new ModalidadEntrada(1, 1, "General", 1000, 10);

        // Act
        var resultado = modalidad.HayCupoDisponible(10);

        // Assert
        Assert.That(resultado, Is.True);
    }

    [Test]
    public void HayCupoDisponible_CantidadMayorAlDisponible_DevuelveFalse()
    {
        // Arrange
        var modalidad = new ModalidadEntrada(1, 1, "General", 1000, 10);

        // Act
        var resultado = modalidad.HayCupoDisponible(11);

        // Assert
        Assert.That(resultado, Is.False);
    }

    [Test]
    public void RegistrarVenta_SinCupoSuficiente_LanzaInvalidOperationException()
    {
        // Arrange
        var modalidad = new ModalidadEntrada(1, 1, "General", 1000, 5);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => modalidad.RegistrarVenta(6));
    }

    [Test]
    public void RegistrarVenta_ConCupoSuficiente_IncrementaCupoVendido()
    {
        // Arrange
        var modalidad = new ModalidadEntrada(1, 1, "General", 1000, 10);

        // Act
        modalidad.RegistrarVenta(3);

        // Assert
        Assert.That(modalidad.CupoVendido, Is.EqualTo(3));
    }

    [Test]
    public void LiberarCupo_CantidadInvalida_LanzaArgumentException()
    {
        // Arrange
        var modalidad = new ModalidadEntrada(1, 1, "General", 1000, 10);
        int cantidadInvalida = 0;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => modalidad.LiberarCupo(cantidadInvalida));
    }

    [Test]
    public void LiberarCupo_CantidadMenorAlCupoVendido_DisminuyeCupoVendido()
    {
        // Arrange
        var modalidad = new ModalidadEntrada(1, 1, "General", 1000, 10);
        modalidad.RegistrarVenta(5);

        // Act
        modalidad.LiberarCupo(2);

        // Assert
        Assert.That(modalidad.CupoVendido, Is.EqualTo(3));
    }

    [Test]
    public void LiberarCupo_CantidadMayorAlCupoVendido_NuncaBajaDeCero()
    {
        // Arrange
        var modalidad = new ModalidadEntrada(1, 1, "General", 1000, 10);
        modalidad.RegistrarVenta(2);

        // Act
        modalidad.LiberarCupo(5);

        // Assert
        Assert.That(modalidad.CupoVendido, Is.EqualTo(0));
    }
}