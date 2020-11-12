using System;
using System.Collections.Generic;
using System.Text;

namespace Kmd.Logic.Audit.Client.SampleLargeAuditEvents
{
    public class AuditEvent
    {
        public Guid Id { get; set; }

        public Guid Name { get; set; }

        public Guid Department { get; set; }

        public Guid Age { get; set; }

        public AuditEvent()
        {
            this.Id = Guid.NewGuid();
            this.Name = Guid.NewGuid();
            this.Department = Guid.NewGuid();
            this.Age = Guid.NewGuid();
        }
    }
}
