namespace Scrips.Core.Models.Identity;

public class EmiratesIdCardData
{
    public string IdNumber { get; set; }
    public string CardNumber { get; set; }
    public string FullNameArabic { get; set; }
    public string FullNameEnglish { get; set; }
    public string ExpiryDate { get; set; }
    public string Gender { get; set; }
    public string DateOfBirth { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string NationalityEn { get; set; }
    public string NationalityAr { get; set; }
    public string PassportNumber { get; set; }
    public byte[] Photo { get; set; }
    public byte[] Signature { get; set; }
}