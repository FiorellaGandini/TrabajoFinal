using System.Linq;
using NUnit.Framework;

namespace GestionLogicaTest;

[TestFixture]
public class ReporteServiceTest : ArchivosTestBase
{
    [Test]
    public void ObtenerRecaudacion_SolicitanteNoOrganizador_LanzaUnauthorizedAccessException()
    {
        // Arrange & Act & Assert
        Assert.Throws<UnauthorizedAccessException>(() => Reportes.ObtenerRecaudacion(DniComprador));
    }

    [Test]
    public void ObtenerRecaudacion_EventoSinCompras_InformaRecaudacionCero()
    {
        // Arrange
        CrearEventoConModalidad();

        // Act
        var reporte = Reportes.ObtenerRecaudacion(DniOrganizador);

        // Assert
        Assert.That(reporte[0].TotalRecaudado, Is.EqualTo(0));
    }

    [Test]
    public void ObtenerRecaudacion_ConComprasSinDescuento_SumaElTotalRecaudado()
    {
        // Arrange
        var evento = CrearEventoConModalidad(precio: 100m);
        Compras.Registrar(DniComprador, evento.Id, evento.Modalidades[0].Id, 2, Ahora);

        // Act
        var reporte = Reportes.ObtenerRecaudacion(DniOrganizador);

        // Assert
        var reporteDelEvento = reporte.First(r => r.EventoId == evento.Id);
        Assert.That(reporteDelEvento.TotalRecaudado, Is.EqualTo(200m));
    }

    [Test]
    public void ObtenerRecaudacion_ConCompraConDescuentoPorPromocion_SumaElTotalYaDescontado()
    {
        // Arrange
        var evento = CrearEventoConModalidad(precio: 100m);
        Compras.Registrar(DniComprador, evento.Id, evento.Modalidades[0].Id, 5, Ahora);

        // Act
        var reporte = Reportes.ObtenerRecaudacion(DniOrganizador);

        // Assert
        var reporteDelEvento = reporte.First(r => r.EventoId == evento.Id);
        Assert.That(reporteDelEvento.TotalRecaudado, Is.EqualTo(425m)); // 500 - 15%
    }
}