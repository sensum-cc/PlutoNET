namespace PlutoNET.UnitTests;

public class MainTest
{
    // ReSharper disable once NullableWarningSuppressionIsUsed
    private Lua _lua = null!;
    
    [SetUp]
    public void Setup()
    {
        ResourceManager.ExtractEmbedded(); // Extracts embedded resources to the current directory in this case lua libraries
        _lua = new Lua();
        _lua.OpenLibs();
    }

    [Test]
    public void StateCheck()
    {
        Assert.That(_lua.L, Is.Not.EqualTo(IntPtr.Zero));
    }

    [Test]
    public void DoString()
    {
        Assert.That(_lua.DoString("print('Hello World')"), Is.False);
    }
}