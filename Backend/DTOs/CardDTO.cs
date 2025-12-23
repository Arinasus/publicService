namespace Backend.DTOs
{
    public class CardDTO
    {
        public int CardID { get; set; }
        public int UserID { get; set; }
        public string CardNumber { get; set; } = string.Empty; 
        public string ExpiryDate { get; set; } = string.Empty;
        public string CardHolderName { get; set; } = string.Empty;
    }
}
