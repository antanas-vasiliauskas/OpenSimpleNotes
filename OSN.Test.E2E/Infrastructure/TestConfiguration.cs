namespace OSN.Test.E2E.Infrastructure;

public static class TestConfiguration
{
    public static string BaseUrl => "http://localhost:3000";
    public static string ChromeDriverPath => "D:\\webdrivers";
    public static int ImplicitWaitTimeoutSeconds => 5;
    public static int ExplicitWaitTimeoutSeconds => 10;
}