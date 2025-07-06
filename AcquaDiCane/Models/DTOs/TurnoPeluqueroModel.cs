public class TurnoPeluqueroModel
{
    public int Id { get; set; }
    public string PetName { get; set; }
    public string PetAvatarUrl { get; set; }
    public DateTime Date { get; set; }
    public string Time { get; set; }
    public string ServiceName { get; set; }

    public string OwnerName { get; set; }
    public string OwnerContact { get; set; }
    public string OwnerEmail { get; set; }

    public string Status { get; set; } // "Pending", "Completed", "Canceled"
}
