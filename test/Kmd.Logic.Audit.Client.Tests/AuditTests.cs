using Xunit;

namespace Kmd.Logic.Audit.Client.Tests
{
    public class AuditTests
    {
        [Fact]
        public void AuditInterfaceExists()
        {
            Assert.NotNull(typeof(IAudit));
        }
    }
}
