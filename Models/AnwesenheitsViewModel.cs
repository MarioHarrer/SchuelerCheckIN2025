using Microsoft.AspNetCore.Mvc.Rendering;
using SchuelerCheckIN2025.Migrations;

namespace SchuelerCheckIN2025.Models
{
    public class AnwesenheitsViewModel
    {
        public string SelectedClass { get; set; }  // Welche Klasse im Dropdown gewählt wurde
        public List<SelectListItem> ClassList { get; set; }  // Für das Dropdown
        public List<Schuelerdaten> Students { get; set; }  // Liste der abwesenden Schüler
    }
}
