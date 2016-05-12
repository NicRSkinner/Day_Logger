using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Day_Logger
{
    public static class ConfigOperations
    {
        private static List<string> GetConfig(string elementName)
        {
            bool withinElement = false;
            List<string> retList = new List<string>();

            using (XmlReader reader = XmlReader.Create("Config/Day Logger.config"))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name == elementName)
                                withinElement = true;
                            break;
                        case XmlNodeType.EndElement:
                            if (reader.Name == elementName)
                                withinElement = false;
                            break;
                        case XmlNodeType.Text:
                            if (withinElement == true)
                                retList.Add(reader.Value);
                            break;
                    }
                }
            }

            return retList;
        }
    }
}
