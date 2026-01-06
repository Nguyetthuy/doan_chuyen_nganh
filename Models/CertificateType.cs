namespace finder_work.Models
{
    public class CertificateType
    {
        public int CertificateTypeId { get; set; }
        public string? CertificateTypeName { get; set; }

        // Navigation properties
        public virtual ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();
    }
}