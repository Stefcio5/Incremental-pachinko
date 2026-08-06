public class PlatformSaveConfiguration
{
    public bool ShouldCallPlayerPrefsSave
    {
        get
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            return true;
#else
            return false;
#endif
        }
    }

    public ISaveCompressor CreateCompressor()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        return new GZipCompressor();
#else
        return new NoOpCompressor();
#endif
    }

    public ISaveSerializer CreateSerializer()
    {
        return new JsonSaveSerializer();
    }
}
