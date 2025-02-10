using System.ComponentModel.DataAnnotations;

namespace BlobStorageApi.Models
{
    public class StoredProcedureRequest
    {
        [Required]
        public string DatabaseName { get; set; }

        [Required]
        public string ContainerName { get; set; }

        [Required]
        public string ProcedureName { get; set; }

        [Required]
        public string PartitionName { get; set; }
    }

}
