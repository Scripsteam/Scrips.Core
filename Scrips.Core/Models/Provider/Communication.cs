using System;

namespace Scrips.Core.Models.Provider
{
    public class Communication
    {
        public Guid? Id { get; set; }
        public Guid LanguageId { get; set; }
        public CodeModel Code { get; set; }
    }
}
