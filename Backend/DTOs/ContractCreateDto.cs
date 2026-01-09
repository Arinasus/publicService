using System.ComponentModel.DataAnnotations;

public class ContractCreateDto
{
    public int UserID { get; set; }
    public int AddressID { get; set; }
    public int ProviderID { get; set; }
    public string ContractNumber { get; set; } = string.Empty;
    public DateTime ContractStartDate { get; set; }
    public DateTime? ContractEndDate { get; set; }
    
    // ДОБАВИТЬ валидацию:
    [Required(ErrorMessage = "Хотя бы одна услуга должна быть выбрана")]
    [MinLength(1, ErrorMessage = "Выберите хотя бы одну услугу")]
    public List<int> ServiceIds { get; set; } = new List<int>();
}
