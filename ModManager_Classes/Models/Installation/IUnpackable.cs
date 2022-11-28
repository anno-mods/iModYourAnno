namespace Imya.Models.Installation
{
    public interface IUnpackable
    {
        String SourceFilepath { get; }
        String UnpackTargetPath { get; }
    }
}
