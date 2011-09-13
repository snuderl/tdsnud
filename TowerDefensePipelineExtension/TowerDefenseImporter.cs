using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace TowerDefensePipelineExtension
{
    class TowerDefenseImporter : ContentImporter<TowerDefenseContent>
    {

    [ContentImporter(".xml", DisplayName = "Tower defense importer", DefaultProcessor = "TowerDefenseProcessor")]
        public override TowerDefenseContent Import(string filename, ContentImporterContext context)
        {
            // load the xml
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);

            // create the content from the xml
            ContentImporterContext content = new ContentImporterContext(doc, context);

            // save the filename and directory for the processor to use
            content.Filename = filename;
            content.Directory = filename.Remove(filename.LastIndexOf('\\'));

            return content;
        }
    }
}
