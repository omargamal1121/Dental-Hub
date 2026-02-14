using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalHub.Application.Interfaces
{
    public class ImageUploadDto
    {
        public string Url {  get; set; }=string.Empty;
        public string PublicId { get; set; } = string.Empty;

    }
}
