namespace Scrips.Core.Models.Billing
{
    public class FeeSummaryResponse
    {
        public double Gross { get; init; }
        public double Discount { get; init; }
        public double Net { get; init; }
        public double PatientAmount { get; init; }
        public double SponsorAmount { get; init; }
        public double Payable { get; init; }
    }
}
