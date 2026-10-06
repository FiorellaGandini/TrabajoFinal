using System.Linq;
using NUnit.Framework;
using GestionLogica.Datos;
using GestionLogica.Enums;

namespace GestionLogicaTest;

[TestFixture]
public class EntradaServiceTest : ArchivosTestBase
{
    [Test]
    public void ObtenerPorCodigo_CodigoInexistente_DevuelveNull()
    {
        // Arrange & Act
        var entrada = Entradas.ObtenerPorCodigo("ZZZZZZ");

        // Assert
        Assert.That(entrada, Is.Null);
    }

    [Test]
    public void ObtenerPorCodigo_CodigoExistente_DevuelveLaEntradaDeEsaCompra()
    {
        // Arrange
        var evento = CrearEventoConModalidad();
        var compra = Compras.Registrar(DniComprador, evento.Id, evento.Modalidades[0].Id, 1, Ahora);
        var codigo = compra.CodigosEntradas[0];

        // Act
        var entrada = Entradas.ObtenerPorCodigo(codigo);

        // Assert
        Assert.That(entrada!.CompraId, Is.EqualTo(compra.Id));
    }

    [Test]
    public void Cancelar_SolicitanteNoComprador_LanzaUnauthorizedAccessException()
    {
        // Arrange
        var evento = CrearEventoConModalidad();
        var compra = Compras.Registrar(DniComprador, evento.Id, evento.Modalidades[0].Id, 1, Ahora);
        var codigo = compra.CodigosEntradas[0];

        // Act & Assert
        Assert.Throws<UnauthorizedAccessException>(() => Entradas.Cancelar(DniOrganizador, codigo));
    }

    [Test]
    public void Cancelar_CodigoInexistente_LanzaArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => Entradas.Cancelar(DniComprador, "ZZZZZZ"));
    }

    [Test]
    public void Cancelar_EntradaDeOtroComprador_LanzaUnauthorizedAccessException()
    {
        // Arrange
        var evento = CrearEventoConModalidad();
        var compra = Compras.Registrar(DniComprador, evento.Id, evento.Modalidades[0].Id, 1, Ahora);
        var codigo = compra.CodigosEntradas[0];

        // Act & Assert
        Assert.Throws<UnauthorizedAccessException>(() => Entradas.Cancelar(DniOtroComprador, codigo));
    }

    [Test]
    public void Cancelar_CasoValido_CambiaEstadoACancelada()
    {
        // Arrange
        var evento = CrearEventoConModalidad();
        var compra = Compras.Registrar(DniComprador, evento.Id, evento.Modalidades[0].Id, 1, Ahora);
        var codigo = compra.CodigosEntradas[0];

        // Act
        var entrada = Entradas.Cancelar(DniComprador, codigo);

        // Assert
        Assert.That(entrada.Estado, Is.EqualTo(EstadoEntrada.Cancelada));
    }

    [Test]
    public void Cancelar_CasoValido_LiberaCupoDeLaModalidad()
    {
        // Arrange
        var evento = CrearEventoConModalidad(cupo: 10);
        var modalidadId = evento.Modalidades[0].Id;
        var compra = Compras.Registrar(DniComprador, evento.Id, modalidadId, 1, Ahora);
        var codigo = compra.CodigosEntradas[0];

        // Act
        Entradas.Cancelar(DniComprador, codigo);

        // Assert
        var eventoActualizado = Eventos.ObtenerPorId(evento.Id, Ahora);
        Assert.That(eventoActualizado!.Modalidades[0].CupoDisponible, Is.EqualTo(10));
    }

    [Test]
    public void Cancelar_EntradaYaUsada_LanzaInvalidOperationException()
    {
        // Arrange
        var evento = CrearEventoConModalidad();
        var compra = Compras.Registrar(DniComprador, evento.Id, evento.Modalidades[0].Id, 1, Ahora);
        var codigo = compra.CodigosEntradas[0];
        MarcarEntradaComoUsada(codigo, evento.Id);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => Entradas.Cancelar(DniComprador, codigo));
    }

    // simula el ingreso por la puerta para poder probar que EntradaService.Cancelar rechaza una entrada ya usada
    private static void MarcarEntradaComoUsada(string codigo, int eventoId)
    {
        var repositorioEntradas = new EntradaRepositorio(RutasArchivos.Entradas);
        var repositorioEventos = new EventoRepositorio(RutasArchivos.Eventos);

        var entradas = repositorioEntradas.Leer();
        var entrada = entradas.First(e => e.Codigo == codigo);
        var evento = repositorioEventos.Leer().First(e => e.Id == eventoId);

        entrada.MarcarComoUsada(evento, evento.HorarioAperturaIngreso);
        repositorioEntradas.Guardar(entradas);
    }
}