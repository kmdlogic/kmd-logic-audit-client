using System;
using System.Collections.Generic;
using System.Text;

namespace Kmd.Logic.Audit.Client.SampleLargeAuditEvents
{
    public class AuditEvent
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid Name { get; set; } = Guid.NewGuid();

        public Guid Department { get; set; } = Guid.NewGuid();

        public Guid Age { get; set; } = Guid.NewGuid();
    }
}
