using NUnit.Framework;
using GestionLogica.Entidades;
using GestionLogica.Enums;

namespace GestionLogicaTest;

[TestFixture]
public class UsuarioTest
{
    [Test]
    public void Constructor_DniInvalido_LanzaArgumentException()
    {
        // Arrange
        int dniInvalido = 0;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Usuario(dniInvalido, "Juan Pérez", "jperez", RolUsuario.Comprador));
    }

    [Test]
    public void Constructor_NombreVacio_LanzaArgumentException()
    {
        // Arrange
        string nombreInvalido = "";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Usuario(123, nombreInvalido, "jperez", RolUsuario.Comprador));
    }

    [Test]
    public void Constructor_UsernameVacio_LanzaArgumentException()
    {
        // Arrange
        string usernameInvalido = "";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Usuario(123, "Juan Pérez", usernameInvalido, RolUsuario.Comprador));
    }

    [Test]
    public void EsOrganizador_UsuarioConRolOrganizador_DevuelveTrue()
    {
        // Arrange
        var usuario = new Usuario(123, "Juan Pérez", "jperez", RolUsuario.Organizador);

        // Act
        var resultado = usuario.EsOrganizador();

        // Assert
        Assert.That(resultado, Is.True);
    }

    [Test]
    public void EsOrganizador_UsuarioConRolComprador_DevuelveFalse()
    {
        // Arrange
        var usuario = new Usuario(123, "Juan Pérez", "jperez", RolUsuario.Comprador);

        // Act
        var resultado = usuario.EsOrganizador();

        // Assert
        Assert.That(resultado, Is.False);
    }

    [Test]
    public void EsComprador_UsuarioConRolComprador_DevuelveTrue()
    {
        // Arrange
        var usuario = new Usuario(123, "Juan Pérez", "jperez", RolUsuario.Comprador);

        // Act
        var resultado = usuario.EsComprador();

        // Assert
        Assert.That(resultado, Is.True);
    }

    [Test]
    public void EsComprador_UsuarioConRolOrganizador_DevuelveFalse()
    {
        // Arrange
        var usuario = new Usuario(123, "Juan Pérez", "jperez", RolUsuario.Organizador);

        // Act
        var resultado = usuario.EsComprador();

        // Assert
        Assert.That(resultado, Is.False);
    }
}