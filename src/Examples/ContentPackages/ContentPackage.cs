using System;
using System.Collections.Immutable;
using System.Xml.Linq;
using FunctionalStuff.Option;

namespace ContentPackages
{
    public class ContentPackage
    {
        public ContentPackage(string filelistPath, ImmutableDictionary<string, FileType> files)
        {
            FilelistPath = filelistPath;
            Files        = files;
        }

        public ContentPackage(string filelistPath)
        {
            static Option<FileType> ValueSelector(XElement xe) =>
                !Enum.TryParse(xe.Name.LocalName, true, out FileType ft)
                    ? Option<FileType>.None()
                    : Option<FileType>.Some(ft);

            static Option<string> KeySelector(XElement xe) =>
                Option<XAttribute>.FromNullable(xe.Attribute("file"))
                                  .Map(a => a.Value)
                                  .Filter(string.IsNullOrWhiteSpace);

            FilelistPath = filelistPath;
            Files = Option<XElement>.FromNullable(XDocument.Load(filelistPath).Root)
                                    .Map(root => root.Elements())
                                    .Bind(elts => elts.TryToImmutableDictionary(KeySelector,
                                              ValueSelector))
                                    .UnwrapOr(new ArgumentException("filelist could not be parsed",
                                                                    nameof(filelistPath)));
        }

        public string FilelistPath { get; }
        public ImmutableDictionary<string, FileType> Files { get; protected set; }
    }
}
