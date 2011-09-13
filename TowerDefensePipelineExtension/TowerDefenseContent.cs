using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace TowerDefensePipelineExtension
{
    class TowerDefenseContent
    {
        public string Filename;
        public string Directory;
        List<SpriteContent> sprites;
        public TowerDefenseContent(XmlDocument doc, ContentProcessorContext content)
        {
            sprites = new List<SpriteContent>();
            XmlNode SpriteNode = doc["TowerDefence/Sprites"];

            foreach (XmlNode Sprite in SpriteNode.SelectNodes("Sprite"))
            {
                sprites.Add(new SpriteContent(Sprite);
            }
        }
    }
}
