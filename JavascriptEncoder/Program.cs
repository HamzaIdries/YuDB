using YuDB.Storage.Filters;

internal class Program
{
    private static byte[] INTEGRITY_KEY = new byte[]
        {
            0x60, 0x1c, 0x86, 0x57,
            0xe5, 0xbe, 0xba, 0xf1,
            0xb4, 0xe2, 0xf6, 0x52,
            0x10, 0x8c, 0x4c, 0x76
        };

    private static byte[] IV = new byte[]
    {
            0x60, 0x1c, 0x86, 0x57,
            0xe5, 0xbe, 0xba, 0xf1,
            0xb4, 0xe2, 0xf6, 0x52,
            0x10, 0x8c, 0x4c, 0x76
    };

    private static byte[] JAVASCRIPT_STORAGE_ENGINE_KEY = new byte[]
    {
            0xc7, 0xe3, 0x64, 0xc3,
            0xfb, 0xfd, 0x57, 0x32,
            0x5b, 0xcb, 0xdf, 0xde,
            0x12, 0xa9, 0x48, 0x6c
    };

    static void Main(string[] args)
    {
        var integrityFilter = new FileIntegrityFilter(INTEGRITY_KEY);
        var javascriptEncryptionFilter = new FileEncryptionFilter(JAVASCRIPT_STORAGE_ENGINE_KEY, IV);
        var fileFilters = new FileFiltersCollection(integrityFilter, javascriptEncryptionFilter);

        Directory.CreateDirectory("./out");

        foreach (var filepath in args)
        {
            var file = File.ReadAllBytes(filepath);
            var filteredFile = fileFilters.Do(file);
            var filename = Path.GetFileNameWithoutExtension(filepath);
            var extension = Path.GetExtension(filepath);
            string path = Path.Join("./out", filename + extension);
            File.WriteAllBytes(path, filteredFile);
        }
    }
}