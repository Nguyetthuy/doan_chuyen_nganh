namespace finder_work.Models
{
    public class Certificate
    {
        public int CertificateId { get; set; }
        public int ProfileId { get; set; }
        public int CertificateTypeId { get; set; }
        public string? CertificateName { get; set; }
        public string? IssueBy { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? CertificateImg { get; set; }

        // Navigation properties
        public virtual UserProfile Profile { get; set; } = null!;
        public virtual CertificateType CertificateType { get; set; } = null!;
    }
}