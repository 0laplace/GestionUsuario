namespace GestionUsuario
{
    public class ComboBoxItem
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        public ComboBoxItem(int id, string nombre)
        {
            Id = id;
            Nombre = nombre;
        }

        public override string ToString()
        {
            return Nombre;
        }
    }
}
