namespace SotaSavegameLib
{
    public class SotaSavegame
    {
        public string? CharacterName { get; private set; }

        public SotaSavegame(string path)
        {
            var xml = new System.Xml.XmlDocument();
            
            xml.Load(path);

            var databaseNode = xml.DocumentElement?.SelectSingleNode("/database");

            if (databaseNode is not null)
            {
                System.Diagnostics.Debug.Print($"File loaded: {path}");

                foreach (var collection in databaseNode.ChildNodes)
                {
                    if(collection is System.Xml.XmlNode node)
                    {
                        if(node.Name == "collections")
                        {
                            LoadCollections(node);
                        }
                    }
                }
            }
        }


        private void LoadCollections(System.Xml.XmlNode collectionsNode)
        {
            foreach(var collection in collectionsNode.ChildNodes)
            {
                if(collection is System.Xml.XmlNode collectionNode && collectionNode.Name == "collection")
                {
                    LoadCollection(collectionNode);
                }
            }
        }


        private void LoadCollection(System.Xml.XmlNode collectionNode)
        {
            if (collectionNode.Attributes is not null)
            {
                foreach (var attr in collectionNode.Attributes)
                {
                    if (attr is not null && attr is System.Xml.XmlAttribute attribute)
                    {
                        if(attribute.Name.ToLower() == "name")
                        {
                            switch(attribute.InnerText.ToLower())
                            {
                                case "usergold":
                                    LoadCollection_UserGold(collectionNode);
                                    break;
                            }
                        }
                    }
                }
            }
        }


        private void LoadCollection_UserGold(System.Xml.XmlNode node)
        {

        }
    }
}