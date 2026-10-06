using NUnit.Framework;
using GestionLogica.Entidades;

namespace GestionLogicaTest;

[TestFixture]
public class UbicacionTest
{
    [Test]
    public void Constructor_NombreVacio_LanzaArgumentException()
    {
        // Arrange
        string nombreInvalido = "";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Ubicacion(nombreInvalido, "Dirección 123"));
    }

    [Test]
    public void Constructor_DireccionVacia_LanzaArgumentException()
    {
        // Arrange
        string direccionInvalida = "";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Ubicacion("Teatro", direccionInvalida));
    }

    [Test]
    public void TieneCoordenadas_SinCoordenadasCargadas_DevuelveFalse()
    {
        // Arrange
        var ubicacion = new Ubicacion("Teatro", "Dirección 123");

        // Act
        var resultado = ubicacion.TieneCoordenadas();

        // Assert
        Assert.That(resultado, Is.False);
    }

    [Test]
    public void TieneCoordenadas_ConCoordenadasValidas_DevuelveTrue()
    {
        // Arrange
        var ubicacion = new Ubicacion("Teatro", "Dirección 123", -34.6, -58.4);

        // Act
        var resultado = ubicacion.TieneCoordenadas();

        // Assert
        Assert.That(resultado, Is.True);
    }

    [Test]
    public void FijarCoordenadas_SoloLatitudSinLongitud_LanzaArgumentException()
    {
        // Arrange
        var ubicacion = new Ubicacion("Teatro", "Dirección 123");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => ubicacion.FijarCoordenadas(-34.6, null));
    }

    [Test]
    public void FijarCoordenadas_LatitudMenorAMenosNoventa_LanzaArgumentException()
    {
        // Arrange
        var ubicacion = new Ubicacion("Teatro", "Dirección 123");
        double latitudInvalida = -91;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => ubicacion.FijarCoordenadas(latitudInvalida, 0));
    }

    [Test]
    public void FijarCoordenadas_LatitudMayorANoventa_LanzaArgumentException()
    {
        // Arrange
        var ubicacion = new Ubicacion("Teatro", "Dirección 123");
        double latitudInvalida = 91;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => ubicacion.FijarCoordenadas(latitudInvalida, 0));
    }

    [Test]
    public void FijarCoordenadas_LongitudMenorAMenosCientoOchenta_LanzaArgumentException()
    {
        // Arrange
        var ubicacion = new Ubicacion("Teatro", "Dirección 123");
        double longitudInvalida = -181;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => ubicacion.FijarCoordenadas(0, longitudInvalida));
    }

    [Test]
    public void FijarCoordenadas_LongitudMayorACientoOchenta_LanzaArgumentException()
    {
        // Arrange
        var ubicacion = new Ubicacion("Teatro", "Dirección 123");
        double longitudInvalida = 181;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => ubicacion.FijarCoordenadas(0, longitudInvalida));
    }

    [Test]
    public void FijarCoordenadas_ValoresValidos_ActualizaTieneCoordenadas()
    {
        // Arrange
        var ubicacion = new Ubicacion("Teatro", "Dirección 123");

        // Act
        ubicacion.FijarCoordenadas(-34.6, -58.4);

        // Assert
        Assert.That(ubicacion.TieneCoordenadas(), Is.True);
    }
}