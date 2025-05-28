namespace SchuelerCheckIN2025.Models
{
    public class Schuelerdaten
    {
        public int Id { get; set; }
        public string email { get; set; }
        public string schluessel {  get; set; }
        public string klasse {  get; set; }

        public Boolean anwesend { get; set; }

        public Boolean admin { get; set; }

        public TimeOnly zeit {  get; set; }

    }
}
