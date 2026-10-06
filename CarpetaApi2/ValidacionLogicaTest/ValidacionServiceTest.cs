using NUnit.Framework;
using ValidacionLogica.Enums;
using ValidacionLogica.Servicios;

namespace ValidacionLogicaTest;

[TestFixture]
public class ValidacionServiceTest : ArchivosTestBase
{
    //Casos que tienen que funcionar

    [Test]
    public void Validar_EntradaVendidaDelEventoDentroDeHorario_RetornaValida()
    {
        // Arrange
        GuardarEventos(CrearEvento(1));
        GuardarEntradas(CrearEntrada("ABC123", eventoId: 1));

        // Act
        var resultado = Servicio.Validar("ABC123", 1, DentroDeVentana);

        // Assert
        Assert.That(resultado.Valida, Is.True);
    }

    [Test]
    public void Validar_EntradaVendidaDelEventoDentroDeHorario_MotivoEsIngresoConfirmado()
    {
        // Arrange
        GuardarEventos(CrearEvento(1));
        GuardarEntradas(CrearEntrada("ABC123", eventoId: 1));

        // Act
        var resultado = Servicio.Validar("ABC123", 1, DentroDeVentana);

        // Assert
        Assert.That(resultado.Motivo, Is.EqualTo("Ingreso confirmado."));
    }

    [Test]
    public void Validar_EntradaVendidaDelEventoDentroDeHorario_RetornaElCodigo()
    {
        // Arrange
        GuardarEventos(CrearEvento(1));
        GuardarEntradas(CrearEntrada("ABC123", eventoId: 1));

        // Act
        var resultado = Servicio.Validar("ABC123", 1, DentroDeVentana);

        // Assert
        Assert.That(resultado.Codigo, Is.EqualTo("ABC123"));
    }

    [Test]
    public void Validar_EntradaVendidaDelEventoDentroDeHorario_RetornaLaFechaDeValidacion()
    {
        // Arrange
        GuardarEventos(CrearEvento(1));
        GuardarEntradas(CrearEntrada("ABC123", eventoId: 1));

        // Act
        var resultado = Servicio.Validar("ABC123", 1, DentroDeVentana);

        // Assert
        Assert.That(resultado.FechaValidacion, Is.EqualTo(DentroDeVentana));
    }

    [Test]
    public void Validar_IngresoExitoso_GuardaLaEntradaComoUsadaEnElArchivo()
    {
        // Arrange
        GuardarEventos(CrearEvento(1));
        GuardarEntradas(CrearEntrada("ABC123", eventoId: 1));

        // Act
        Servicio.Validar("ABC123", 1, DentroDeVentana);

        // Assert
        Assert.That(LeerEntradaDelArchivo("ABC123").Estado, Is.EqualTo(EstadoEntrada.Usada));
    }

    [Test]
    public void Validar_IngresoExitoso_GuardaLaFechaDeValidacionEnElArchivo()
    {
        // Arrange
        GuardarEventos(CrearEvento(1));
        GuardarEntradas(CrearEntrada("ABC123", eventoId: 1));

        // Act
        Servicio.Validar("ABC123", 1, DentroDeVentana);

        // Assert
        Assert.That(LeerEntradaDelArchivo("ABC123").FechaValidacion, Is.EqualTo(DentroDeVentana));
    }

    [Test]
    public void Validar_CodigoEnMinusculas_RetornaValida()
    {
        // Arrange
        GuardarEventos(CrearEvento(1));
        GuardarEntradas(CrearEntrada("ABC123", eventoId: 1));

        // Act
        var resultado = Servicio.Validar("abc123", 1, DentroDeVentana);

        // Assert
        Assert.That(resultado.Valida, Is.True);
    }

    [Test]
    public void Validar_EntradaEscritaPorLaOtraApiDespuesDeIniciarElServicio_RetornaValida()
    {
        // Arrange: el servicio ya existe (lo crea el SetUp) y recién ahora "se vende" la entrada
        GuardarEventos(CrearEvento(1));
        GuardarEntradas(CrearEntrada("NEW999", eventoId: 1));

        // Act
        var resultado = Servicio.Validar("NEW999", 1, DentroDeVentana);

        // Assert
        Assert.That(resultado.Valida, Is.True);
    }

    [Test]
    public void Validar_PrimeraEntradaDeUnaCompraDeTres_LasOtrasDosSiguenVendidas()
    {
        // Arrange: una compra de 3 entradas (mismo compraId)
        GuardarEventos(CrearEvento(1));
        GuardarEntradas(
            CrearEntrada("AAA111", compraId: 10),
            CrearEntrada("BBB222", compraId: 10),
            CrearEntrada("CCC333", compraId: 10));

        // Act
        Servicio.Validar("AAA111", 1, DentroDeVentana);

        // Assert
        Assert.That(LeerEntradaDelArchivo("BBB222").Estado, Is.EqualTo(EstadoEntrada.Vendida));
    }

    [Test]
    public void Validar_SegundaEntradaDeLaMismaCompra_RetornaValida()
    {
        // Arrange
        GuardarEventos(CrearEvento(1));
        GuardarEntradas(
            CrearEntrada("AAA111", compraId: 10),
            CrearEntrada("BBB222", compraId: 10));
        Servicio.Validar("AAA111", 1, DentroDeVentana);

        // Act
        var resultado = Servicio.Validar("BBB222", 1, DentroDeVentana.AddMinutes(10));

        // Assert
        Assert.That(resultado.Valida, Is.True);
    }

    [Test]
    public void Validar_VariosEventosEnElArchivo_UsaElEventoIndicado()
    {
        // Arrange: el evento 1 está cancelado pero el 2 está activo
        GuardarEventos(CrearEvento(1, EstadoEvento.Cancelado), CrearEvento(2));
        GuardarEntradas(CrearEntrada("ABC123", eventoId: 2));

        // Act
        var resultado = Servicio.Validar("ABC123", 2, DentroDeVentana);

        // Assert
        Assert.That(resultado.Valida, Is.True);
    }

    //Entrada inexistente

    [Test]
    public void Validar_CodigoInexistente_RetornaNoValida()
    {
        // Arrange
        GuardarEventos(CrearEvento(1));
        GuardarEntradas(CrearEntrada("ABC123", eventoId: 1));

        // Act
        var resultado = Servicio.Validar("ZZZ999", 1, DentroDeVentana);

        // Assert
        Assert.That(resultado.Valida, Is.False);
    }

    [Test]
    public void Validar_CodigoInexistente_MotivoIndicaQueNoExiste()
    {
        // Arrange
        GuardarEventos(CrearEvento(1));
        GuardarEntradas(CrearEntrada("ABC123", eventoId: 1));

        // Act
        var resultado = Servicio.Validar("ZZZ999", 1, DentroDeVentana);

        // Assert
        Assert.That(resultado.Motivo, Is.EqualTo("La entrada no existe."));
    }

    [Test]
    public void Validar_SinArchivosCreados_RetornaNoValidaSinLanzarExcepcion()
    {
        // Arrange: no existe ni entradas.json ni eventos.json

        // Act
        var resultado = Servicio.Validar("ABC123", 1, DentroDeVentana);

        // Assert
        Assert.That(resultado.Valida, Is.False);
    }

    [Test]
    public void Validar_CodigoInexistente_NoModificaLasOtrasEntradas()
    {
        // Arrange
        GuardarEventos(CrearEvento(1));
        GuardarEntradas(CrearEntrada("AAA111"), CrearEntrada("BBB222"));

        // Act
        Servicio.Validar("ZZZ999", 1, DentroDeVentana);

        // Assert
        Assert.That(LeerEntradaDelArchivo("AAA111").Estado, Is.EqualTo(EstadoEntrada.Vendida));
    }

    //Entrada de otro evento

    [Test]
    public void Validar_EntradaDeOtroEvento_RetornaNoValida()
    {
        // Arrange: la entrada es del evento 1 pero se controla la puerta del evento 2
        GuardarEventos(CrearEvento(1), CrearEvento(2));
        GuardarEntradas(CrearEntrada("ABC123", eventoId: 1));

        // Act
        var resultado = Servicio.Validar("ABC123", 2, DentroDeVentana);

        // Assert
        Assert.That(resultado.Valida, Is.False);
    }

    [Test]
    public void Validar_EntradaDeOtroEvento_MotivoIndicaOtroEvento()
    {
        // Arrange
        GuardarEventos(CrearEvento(1), CrearEvento(2));
        GuardarEntradas(CrearEntrada("ABC123", eventoId: 1));

        // Act
        var resultado = Servicio.Validar("ABC123", 2, DentroDeVentana);

        // Assert
        Assert.That(resultado.Motivo, Is.EqualTo("La entrada ABC123 es de otro evento."));
    }

    [Test]
    public void Validar_EntradaDeOtroEvento_NoLaMarcaComoUsada()
    {
        // Arrange
        GuardarEventos(CrearEvento(1), CrearEvento(2));
        GuardarEntradas(CrearEntrada("ABC123", eventoId: 1));

        // Act
        Servicio.Validar("ABC123", 2, DentroDeVentana);

        // Assert
        Assert.That(LeerEntradaDelArchivo("ABC123").Estado, Is.EqualTo(EstadoEntrada.Vendida));
    }

    [Test]
    public void Validar_EventoInexistenteEnElArchivo_RetornaNoValida()
    {
        // Arrange: la entrada apunta a un evento que no está en eventos.json
        GuardarEventos(CrearEvento(1));
        GuardarEntradas(CrearEntrada("ABC123", eventoId: 99));

        // Act
        var resultado = Servicio.Validar("ABC123", 99, DentroDeVentana);

        // Assert
        Assert.That(resultado.Valida, Is.False);
    }

    [Test]
    public void Validar_EventoInexistenteEnElArchivo_MotivoIndicaElIdDelEvento()
    {
        // Arrange
        GuardarEventos(CrearEvento(1));
        GuardarEntradas(CrearEntrada("ABC123", eventoId: 99));

        // Act
        var resultado = Servicio.Validar("ABC123", 99, DentroDeVentana);

        // Assert
        Assert.That(resultado.Motivo, Is.EqualTo("No existe un evento con id 99."));
    }

    //Entrada ya usada

    [Test]
    public void Validar_EntradaYaUsada_SegundoIntentoRetornaNoValida()
    {
        // Arrange
        GuardarEventos(CrearEvento(1));
        GuardarEntradas(CrearEntrada("ABC123", eventoId: 1));
        Servicio.Validar("ABC123", 1, DentroDeVentana);

        // Act
        var segundoIntento = Servicio.Validar("ABC123", 1, DentroDeVentana.AddMinutes(5));

        // Assert
        Assert.That(segundoIntento.Valida, Is.False);
    }

    [Test]
    public void Validar_EntradaYaUsada_MotivoIndicaQueYaFueUtilizada()
    {
        // Arrange
        GuardarEventos(CrearEvento(1));
        GuardarEntradas(CrearEntrada("ABC123", eventoId: 1));
        Servicio.Validar("ABC123", 1, DentroDeVentana);

        // Act
        var segundoIntento = Servicio.Validar("ABC123", 1, DentroDeVentana.AddMinutes(5));

        // Assert
        Assert.That(segundoIntento.Motivo, Is.EqualTo("La entrada ABC123 ya fue utilizada."));
    }

    [Test]
    public void Validar_EntradaYaUsada_NoPisaLaFechaDelPrimerIngreso()
    {
        // Arrange
        GuardarEventos(CrearEvento(1));
        GuardarEntradas(CrearEntrada("ABC123", eventoId: 1));
        Servicio.Validar("ABC123", 1, DentroDeVentana);

        // Act
        Servicio.Validar("ABC123", 1, DentroDeVentana.AddMinutes(30));

        // Assert
        Assert.That(LeerEntradaDelArchivo("ABC123").FechaValidacion, Is.EqualTo(DentroDeVentana));
    }

    [Test]
    public void Validar_EntradaUsadaYServicioReiniciado_RetornaNoValida()
    {
        // Arrange: simula reiniciar la API; el estado vive en el archivo, no en memoria
        GuardarEventos(CrearEvento(1));
        GuardarEntradas(CrearEntrada("ABC123", eventoId: 1));
        Servicio.Validar("ABC123", 1, DentroDeVentana);
        var servicioReiniciado = new ValidacionService();

        // Act
        var resultado = servicioReiniciado.Validar("ABC123", 1, DentroDeVentana.AddMinutes(1));

        // Assert
        Assert.That(resultado.Valida, Is.False);
    }

    //Entrada cancelada (promocion)

    [Test]
    public void Validar_EntradaCancelada_RetornaNoValida()
    {
        // Arrange
        GuardarEventos(CrearEvento(1));
        GuardarEntradas(CrearEntrada("ABC123", eventoId: 1, estado: EstadoEntrada.Cancelada));

        // Act
        var resultado = Servicio.Validar("ABC123", 1, DentroDeVentana);

        // Assert
        Assert.That(resultado.Valida, Is.False);
    }

    [Test]
    public void Validar_EntradaCancelada_MotivoIndicaQueFueCancelada()
    {
        // Arrange
        GuardarEventos(CrearEvento(1));
        GuardarEntradas(CrearEntrada("ABC123", eventoId: 1, estado: EstadoEntrada.Cancelada));

        // Act
        var resultado = Servicio.Validar("ABC123", 1, DentroDeVentana);

        // Assert
        Assert.That(resultado.Motivo, Is.EqualTo("La entrada ABC123 fue cancelada y no es válida."));
    }

    [Test]
    public void Validar_EntradaCancelada_ConservaElEstadoEnElArchivo()
    {
        // Arrange
        GuardarEventos(CrearEvento(1));
        GuardarEntradas(CrearEntrada("ABC123", eventoId: 1, estado: EstadoEntrada.Cancelada));

        // Act
        Servicio.Validar("ABC123", 1, DentroDeVentana);

        // Assert
        Assert.That(LeerEntradaDelArchivo("ABC123").Estado, Is.EqualTo(EstadoEntrada.Cancelada));
    }

    [Test]
    public void Validar_EntradaCanceladaDeUnaCompra_NoAfectaALaEntradaVigenteDeLaMismaCompra()
    {
        // Arrange: de una compra de 2 entradas, una fue cancelada
        GuardarEventos(CrearEvento(1));
        GuardarEntradas(
            CrearEntrada("AAA111", compraId: 10, estado: EstadoEntrada.Cancelada),
            CrearEntrada("BBB222", compraId: 10));

        // Act
        var resultado = Servicio.Validar("BBB222", 1, DentroDeVentana);

        // Assert
        Assert.That(resultado.Valida, Is.True);
    }

    //Evento no activo / fuera de horario 

    [Test]
    public void Validar_EventoCancelado_RetornaNoValida()
    {
        // Arrange
        GuardarEventos(CrearEvento(1, EstadoEvento.Cancelado));
        GuardarEntradas(CrearEntrada("ABC123", eventoId: 1));

        // Act
        var resultado = Servicio.Validar("ABC123", 1, DentroDeVentana);

        // Assert
        Assert.That(resultado.Valida, Is.False);
    }

    [Test]
    public void Validar_EventoFinalizado_RetornaNoValida()
    {
        // Arrange
        GuardarEventos(CrearEvento(1, EstadoEvento.Finalizado));
        GuardarEntradas(CrearEntrada("ABC123", eventoId: 1));

        // Act
        var resultado = Servicio.Validar("ABC123", 1, DentroDeVentana);

        // Assert
        Assert.That(resultado.Valida, Is.False);
    }

    [Test]
    public void Validar_EventoCancelado_NoMarcaLaEntradaComoUsada()
    {
        // Arrange
        GuardarEventos(CrearEvento(1, EstadoEvento.Cancelado));
        GuardarEntradas(CrearEntrada("ABC123", eventoId: 1));

        // Act
        Servicio.Validar("ABC123", 1, DentroDeVentana);

        // Assert
        Assert.That(LeerEntradaDelArchivo("ABC123").Estado, Is.EqualTo(EstadoEntrada.Vendida));
    }

    [Test]
    public void Validar_AntesDeLaApertura_RetornaNoValida()
    {
        // Arrange
        GuardarEventos(CrearEvento(1));
        GuardarEntradas(CrearEntrada("ABC123", eventoId: 1));

        // Act
        var resultado = Servicio.Validar("ABC123", 1, AntesDeApertura);

        // Assert
        Assert.That(resultado.Valida, Is.False);
    }

    [Test]
    public void Validar_AntesDeLaApertura_MotivoIndicaFueraDeHorario()
    {
        // Arrange
        GuardarEventos(CrearEvento(1));
        GuardarEntradas(CrearEntrada("ABC123", eventoId: 1));

        // Act
        var resultado = Servicio.Validar("ABC123", 1, AntesDeApertura);

        // Assert
        Assert.That(resultado.Motivo, Is.EqualTo("La entrada ABC123 está fuera del horario de ingreso."));
    }

    [Test]
    public void Validar_AntesDeLaApertura_LaEntradaSigueVendida()
    {
        // Arrange
        GuardarEventos(CrearEvento(1));
        GuardarEntradas(CrearEntrada("ABC123", eventoId: 1));

        // Act
        Servicio.Validar("ABC123", 1, AntesDeApertura);

        // Assert
        Assert.That(LeerEntradaDelArchivo("ABC123").Estado, Is.EqualTo(EstadoEntrada.Vendida));
    }

    [Test]
    public void Validar_DespuesDelCierre_RetornaNoValida()
    {
        // Arrange
        GuardarEventos(CrearEvento(1));
        GuardarEntradas(CrearEntrada("ABC123", eventoId: 1));

        // Act
        var resultado = Servicio.Validar("ABC123", 1, DespuesDelCierre);

        // Assert
        Assert.That(resultado.Valida, Is.False);
    }

    [Test]
    public void Validar_EntradaRechazadaPorHorario_SePuedeValidarLuegoDentroDeHorario()
    {
        // Arrange
        GuardarEventos(CrearEvento(1));
        GuardarEntradas(CrearEntrada("ABC123", eventoId: 1));
        Servicio.Validar("ABC123", 1, AntesDeApertura);

        // Act
        var resultado = Servicio.Validar("ABC123", 1, DentroDeVentana);

        // Assert
        Assert.That(resultado.Valida, Is.True);
    }
}