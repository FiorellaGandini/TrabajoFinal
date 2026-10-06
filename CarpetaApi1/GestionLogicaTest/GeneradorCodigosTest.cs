using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using GestionLogica.Servicios;

namespace GestionLogicaTest;

[TestFixture]
public class GeneradorCodigosTest
{
    [Test]
    public void GenerarLote_CantidadCero_LanzaArgumentException()
    {
        // Arrange
        var codigosExistentes = new List<string>();
        int cantidadInvalida = 0;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => GeneradorCodigos.GenerarLote(cantidadInvalida, codigosExistentes));
    }

    [Test]
    public void GenerarLote_CantidadValida_DevuelveLaCantidadPedidaDeCodigos()
    {
        // Arrange
        int cantidadPedida = 5;
        var codigosExistentes = new List<string>();

        // Act
        var codigos = GeneradorCodigos.GenerarLote(cantidadPedida, codigosExistentes);

        // Assert
        Assert.That(codigos, Has.Count.EqualTo(cantidadPedida));
    }

    [Test]
    public void GenerarLote_CodigoGenerado_TieneSeisCaracteres()
    {
        // Arrange
        var codigosExistentes = new List<string>();

        // Act
        var codigos = GeneradorCodigos.GenerarLote(1, codigosExistentes);

        // Assert
        Assert.That(codigos[0], Has.Length.EqualTo(6));
    }

    [Test]
    public void GenerarLote_CodigoGenerado_EsAlfanumerico()
    {
        // Arrange
        var codigosExistentes = new List<string>();

        // Act
        var codigos = GeneradorCodigos.GenerarLote(1, codigosExistentes);

        // Assert
        Assert.That(codigos[0].All(char.IsAsciiLetterOrDigit), Is.True);
    }

    [Test]
    public void GenerarLote_CodigoYaExistente_NoLoVuelveAGenerar()
    {
        // Arrange
        var codigosExistentes = new List<string> { "AAAAAA" };

        // Act
        var codigos = GeneradorCodigos.GenerarLote(50, codigosExistentes);

        // Assert
        Assert.That(codigos, Does.Not.Contain("AAAAAA"));
    }

    [Test]
    public void GenerarLote_VariosCodigosEnUnMismoLote_NoGeneraDuplicadosEntreSi()
    {
        // Arrange
        var codigosExistentes = new List<string>();

        // Act
        var codigos = GeneradorCodigos.GenerarLote(500, codigosExistentes);

        // Assert
        Assert.That(codigos.Distinct().Count(), Is.EqualTo(codigos.Count));
    }
}