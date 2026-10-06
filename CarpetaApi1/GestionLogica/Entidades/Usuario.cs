namespace GestionLogica.Entidades;
using GestionLogica.Enums;
public class Usuario
{
    private int dni;
    private string nombre = string.Empty;
    private string username = string.Empty;
    private RolUsuario rol;

    public int Dni
    {
        get => dni;
        private set
        {
            if (value <= 0)
                throw new ArgumentException("El DNI debe ser un número positivo.");
            dni = value;
        }
    }

    public string Nombre
    {
        get => nombre;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El nombre no puede estar vacío.");
            nombre = value;
        }
    }

    public string Username
    {
        get => username;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El username no puede estar vacío.");
            username = value;
        }
    }

    public RolUsuario Rol
    {
        get => rol;
        private set => rol = value;
    }

    public Usuario(int dni, string nombre, string username, RolUsuario rol)
    {
        Dni = dni;
        Nombre = nombre;
        Username = username;
        Rol = rol;
    }

    public bool EsOrganizador() => Rol == RolUsuario.Organizador;
    public bool EsComprador() => Rol == RolUsuario.Comprador;
}