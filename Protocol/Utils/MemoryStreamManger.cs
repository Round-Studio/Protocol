using Microsoft.IO;

namespace Protocol.Utils;

public static class MemoryStreamManger
{
    public static RecyclableMemoryStreamManager stream { get; set; } = new();
}