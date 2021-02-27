using System.Collections.Immutable;

namespace FunctionalStuff
{
    public class ModContentPackage : ContentPackage
    {
        public ModContentPackage(string filelistPath) : base(filelistPath)
        {
        }
        
        public ModContentPackage AddFile(string filepath, FileType fileType)
        {
            Files = Files.Add(filepath, fileType);
            return this;
        }

        public ModContentPackage RemoveFile(string filepath)
        {
            Files = Files.Remove(filepath);
            return this;
        }

        public ModContentPackage EditFileType(string filepath, FileType fileType) => RemoveFile(filepath).AddFile(filepath, fileType);
    }
}
