using NUnit.Framework;
using GestionLogica.Enums;

namespace GestionLogicaTest;

[TestFixture]
public class UsuarioServiceTest : ArchivosTestBase
{
    [Test]
    public void ObtenerPorDni_DniExistente_DevuelveElUsuarioCorrespondiente()
    {
        // Arrange & Act
        var usuario = Usuarios.ObtenerPorDni(DniOrganizador);

        // Assert
        Assert.That(usuario!.Dni, Is.EqualTo(DniOrganizador));
    }

    [Test]
    public void ObtenerPorDni_DniInexistente_DevuelveNull()
    {
        // Arrange & Act
        var usuario = Usuarios.ObtenerPorDni(99999999);

        // Assert
        Assert.That(usuario, Is.Null);
    }

    [Test]
    public void ObtenerTodos_DespuesDeLaPrecarga_DevuelveLosDiezUsuariosPrecargados()
    {
        // Arrange & Act
        var usuarios = Usuarios.ObtenerTodos();

        // Assert
        Assert.That(usuarios, Has.Count.EqualTo(10));
    }

    [Test]
    public void ValidarRol_RolCorrecto_NoLanzaExcepcion()
    {
        // Arrange & Act & Assert
        Assert.DoesNotThrow(() => Usuarios.ValidarRol(DniOrganizador, RolUsuario.Organizador));
    }

    [Test]
    public void ValidarRol_RolIncorrecto_LanzaUnauthorizedAccessException()
    {
        // Arrange & Act & Assert
        Assert.Throws<UnauthorizedAccessException>(() => Usuarios.ValidarRol(DniComprador, RolUsuario.Organizador));
    }

    [Test]
    public void ValidarRol_DniInexistente_LanzaArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => Usuarios.ValidarRol(99999999, RolUsuario.Organizador));
    }
}