namespace RestApi.Entidades
{
    public class Pedido
   {
    public int Id { get; set; }
    public int MesaId { get; set; }
    public DateTime Fecha { get; set; }
    public string Estado { get; set; } = null!;

    public virtual Mesa Mesa { get; set; } = null!;

    public virtual ICollection<DetallesPedido>
        DetallesPedidos { get; set; }
        = new List<DetallesPedido>();
}
}
