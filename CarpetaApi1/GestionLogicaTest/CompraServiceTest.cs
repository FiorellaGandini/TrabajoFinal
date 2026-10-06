using NUnit.Framework;

namespace GestionLogicaTest;

[TestFixture]
public class CompraServiceTest : ArchivosTestBase
{
    [Test]
    public void Registrar_SolicitanteNoComprador_LanzaUnauthorizedAccessException()
    {
        // Arrange
        var evento = CrearEventoConModalidad();
        var modalidadId = evento.Modalidades[0].Id;

        // Act & Assert
        Assert.Throws<UnauthorizedAccessException>(() =>
            Compras.Registrar(DniOrganizador, evento.Id, modalidadId, 1, Ahora));
    }

    [Test]
    public void Registrar_EventoInexistente_LanzaArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => Compras.Registrar(DniComprador, 999, 1, 1, Ahora));
    }

    [Test]
    public void Registrar_EventoCancelado_LanzaInvalidOperationException()
    {
        // Arrange
        var evento = CrearEventoConModalidad();
        var modalidadId = evento.Modalidades[0].Id;
        Eventos.Cancelar(DniOrganizador, evento.Id);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            Compras.Registrar(DniComprador, evento.Id, modalidadId, 1, Ahora));
    }

    [Test]
    public void Registrar_ModalidadInexistente_LanzaArgumentException()
    {
        // Arrange
        var evento = CrearEventoConModalidad();

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            Compras.Registrar(DniComprador, evento.Id, 999, 1, Ahora));
    }

    [Test]
    public void Registrar_SinCupoSuficiente_LanzaInvalidOperationException()
    {
        // Arrange
        var evento = CrearEventoConModalidad(cupo: 2);
        var modalidadId = evento.Modalidades[0].Id;

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            Compras.Registrar(DniComprador, evento.Id, modalidadId, 3, Ahora));
    }

    [Test]
    public void Registrar_CasoValido_GeneraUnCodigoDeEntradaPorCadaUnidadComprada()
    {
        // Arrange
        var evento = CrearEventoConModalidad();
        var modalidadId = evento.Modalidades[0].Id;

        // Act
        var compra = Compras.Registrar(DniComprador, evento.Id, modalidadId, 3, Ahora);

        // Assert
        Assert.That(compra.CodigosEntradas, Has.Count.EqualTo(3));
    }

    [Test]
    public void Registrar_CasoValido_DescuentaElCupoDeLaModalidad()
    {
        // Arrange
        var evento = CrearEventoConModalidad(cupo: 10);
        var modalidadId = evento.Modalidades[0].Id;

        // Act
        Compras.Registrar(DniComprador, evento.Id, modalidadId, 4, Ahora);

        // Assert
        var eventoActualizado = Eventos.ObtenerPorId(evento.Id, Ahora);
        Assert.That(eventoActualizado!.Modalidades[0].CupoDisponible, Is.EqualTo(6));
    }

    // --- Condición de promoción (15% de descuento a partir de 5 entradas) ---

    [Test]
    public void Registrar_CantidadMenorACinco_NoAplicaDescuento()
    {
        // Arrange
        var evento = CrearEventoConModalidad(cupo: 10, precio: 100m);
        var modalidadId = evento.Modalidades[0].Id;

        // Act
        var compra = Compras.Registrar(DniComprador, evento.Id, modalidadId, 4, Ahora);

        // Assert
        Assert.That(compra.PrecioTotal, Is.EqualTo(400m));
    }

    [Test]
    public void Registrar_CincoOMasUnidades_AplicaQuinceCientoDeDescuento()
    {
        // Arrange
        var evento = CrearEventoConModalidad(cupo: 10, precio: 100m);
        var modalidadId = evento.Modalidades[0].Id;

        // Act
        var compra = Compras.Registrar(DniComprador, evento.Id, modalidadId, 5, Ahora);

        // Assert
        Assert.That(compra.PrecioTotal, Is.EqualTo(425m));
    }

    [Test]
    public void Registrar_CincoOMasUnidades_LaRecaudacionDelEventoReflejaElDescuento()
    {
        // Arrange
        var evento = CrearEventoConModalidad(cupo: 10, precio: 100m);
        var modalidadId = evento.Modalidades[0].Id;

        // Act
        Compras.Registrar(DniComprador, evento.Id, modalidadId, 5, Ahora);

        // Assert
        var reporte = Reportes.ObtenerRecaudacion(DniOrganizador);
        Assert.That(reporte[0].TotalRecaudado, Is.EqualTo(425m));
    }

    // --- Consultas ---

    [Test]
    public void ObtenerPorId_IdInexistente_DevuelveNull()
    {
        // Arrange & Act
        var compra = Compras.ObtenerPorId(999);

        // Assert
        Assert.That(compra, Is.Null);
    }

    [Test]
    public void ObtenerEntradasDeCompra_CompraValida_DevuelveUnaEntradaPorCadaUnidadComprada()
    {
        // Arrange
        var evento = CrearEventoConModalidad();
        var modalidadId = evento.Modalidades[0].Id;
        var compra = Compras.Registrar(DniComprador, evento.Id, modalidadId, 2, Ahora);

        // Act
        var entradas = Compras.ObtenerEntradasDeCompra(compra.Id);

        // Assert
        Assert.That(entradas, Has.Count.EqualTo(2));
    }
}