using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;

namespace FlightNode.Common.UnitTests
{
    public abstract class BaseTester
    {
        protected IFixture Fixture = new Fixture().Customize(new AutoMoqCustomization());
    }
}
