namespace RestApi.Entidades
{
    public class DetallesPedido
    {
        public int Id { get; set; }
        public int PedidoId { get; set; }
        public int PlatoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }

        public virtual Pedido Pedido { get; set; } = null!;
        public virtual Plato Plato { get; set; } = null!;
    }
}
// Los nombres exactos pueden variar un poco
// según tu versión de EF Core.
