namespace UsersService.DTO
{
    public class UserWithProductsDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public List<ProductDTO> Productos { get; set; }
    }
}
