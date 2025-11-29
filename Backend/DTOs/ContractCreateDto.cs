public class ContractCreateDto
{
    public int UserID { get; set; }
    public int AddressID { get; set; }
    public int ServiceID { get; set; }
    public int ProviderID { get; set; }
    public string ContractNumber { get; set; } = string.Empty;
    public DateTime ContractStartDate { get; set; }
    public DateTime? ContractEndDate { get; set; }
}
