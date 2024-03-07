namespace PlutoNET.UnitTests;

public class MainTest
{
    private Lua _lua = null!;
    
    [SetUp]
    public void Setup()
    {
        ResourceManager.ExtractEmbedded(); // Extracts embedded resources to the current directory in this case lua libraries
        _lua = new Lua();
    }

    [Test]
    public void StateCheck()
    {
        Assert.That(_lua.State, Is.Not.EqualTo(IntPtr.Zero));
    }
}