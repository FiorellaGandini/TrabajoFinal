using GestionLogica.Datos;
using GestionLogica.Entidades;
using GestionLogica.Enums;

namespace GestionLogica.Servicios;

public class UsuarioService
{
    private readonly UsuarioRepositorio repositorio;

    public UsuarioService() => repositorio = new UsuarioRepositorio(RutasArchivos.Usuarios);

    public List<Usuario> ObtenerTodos() => repositorio.Leer();

    public Usuario? ObtenerPorDni(int dni) =>
        repositorio.Leer().FirstOrDefault(u => u.Dni == dni);

    public void ValidarRol(int dni, RolUsuario rolRequerido)
    {
        var usuario = ObtenerPorDni(dni);
        if (usuario is null)
            throw new ArgumentException($"No existe un usuario con DNI {dni}.");
        if (usuario.Rol != rolRequerido)
            throw new UnauthorizedAccessException(
                $"El usuario {usuario.Nombre} tiene rol {usuario.Rol} y no puede realizar esta acción (requiere {rolRequerido}).");
    }

    public void PrecargarSiNoExisten()
    {
        if (repositorio.Leer().Count > 0) return;

        var usuarios = new List<Usuario>
        {
            new Usuario(30111222, "Lucía Fernández", "lfernandez", RolUsuario.Organizador),
            new Usuario(29888777, "Martín Aguirre", "maguirre", RolUsuario.Organizador),
            new Usuario(40123456, "Sofía Gómez", "sgomez", RolUsuario.Comprador),
            new Usuario(38456789, "Nicolás Pereyra", "npereyra", RolUsuario.Comprador),
            new Usuario(41234567, "Valentina Ríos", "vrios", RolUsuario.Comprador),
            new Usuario(37654321, "Tomás Ibáñez", "tibanez", RolUsuario.Comprador),
            new Usuario(42345678, "Camila Suárez", "csuarez", RolUsuario.Comprador),
            new Usuario(39876543, "Agustín Molina", "amolina", RolUsuario.Comprador),
            new Usuario(43456789, "Julieta Torres", "jtorres", RolUsuario.Comprador),
            new Usuario(36765432, "Bruno Acosta", "bacosta", RolUsuario.Comprador),
        };

        repositorio.Guardar(usuarios);
    }
}