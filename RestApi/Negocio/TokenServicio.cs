namespace RestApi.Negocio;

using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using RestApi.Entidades;

public class TokenServicio(IConfiguration config)
{
    public (string Token, DateTime Expira) Crear(Usuario u)
    {
        var jwt = config.GetSection("Jwt");
        var minutos = jwt.GetValue<int>("MinutosExpiracion");
        if (minutos <= 0) minutos = 480;

        var expira = DateTime.UtcNow.AddMinutes(minutos);
        var claveSecreta = jwt["Key"] 
            ?? throw new InvalidOperationException("Falta configurar 'Jwt:Key' en appsettings.json o variables de entorno.");

        var clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(claveSecreta));

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = jwt["Issuer"],
            Audience = jwt["Audience"],
            Expires = expira,
            Claims = new Dictionary<string, object>
            {
                ["sub"] = u.Id.ToString(),
                ["name"] = u.Nombre,
                ["email"] = u.Correo,
                ["role"] = u.Rol
            },
            SigningCredentials = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256)
        };

        var token = new JsonWebTokenHandler().CreateToken(descriptor);
        return (token, expira);
    }
}
