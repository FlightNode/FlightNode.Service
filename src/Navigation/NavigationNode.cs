using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlightNode.Service.Navigation
{
    public class NavigationNode
    {
        public List<NavigationNode> Children { get; private set; }

        public NavigationEntry Entry { get; set; }

        public NavigationNode()
        {
            Children = new List<NavigationNode>();
        }

        public NavigationNode(string title, string path)
        {
            Children = new List<NavigationNode>();
            Entry = new NavigationEntry
            {
                Title = title,
                Path = path
            };
        }

        public NavigationNode AddChild(NavigationNode child)
        {
            Children.Add(child);
            return this;
        }

    }

    public class NavigationEntry
    {
        public string Title { get; set; }
        public string Path { get; set; }
    }
}