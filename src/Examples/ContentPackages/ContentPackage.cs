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
            static Option<FileType> ValueSelector(XElement xe)
            {
                if (!Enum.TryParse(xe.Name.LocalName, true, out FileType ft))
                    return Option<FileType>.None();
                return Option<FileType>.Some(ft);
            }

            static Option<string> KeySelector(XElement xe)
            {
                var attribute =
                    xe.Attribute("file");
                if (attribute is null)
                    return Option<string>.None();
                var value = attribute.Value;
                if (string.IsNullOrWhiteSpace(value))
                    return Option<string>.None();
                return Option<string>.Some(value);
            }

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
