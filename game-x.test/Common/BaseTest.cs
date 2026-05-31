using Mapster;
using Moq.AutoMock;

namespace Test.Common;

public abstract class BaseTest<T> where T: class
{
    protected readonly AutoMocker Mocker;
    protected readonly T Sut;

    protected BaseTest()
    {
        TypeAdapterConfig.GlobalSettings.Scan(AppDomain.CurrentDomain.GetAssemblies());
        Mocker = new AutoMocker();
        Sut = Mocker.CreateInstance<T>();
    }
}
