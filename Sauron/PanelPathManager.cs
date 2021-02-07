using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Container;


namespace Sauron
{
    class PanelPathManager
    {
        Dictionary<string, List<PanelPathContainer>> theContainer = new Dictionary<string, List<PanelPathContainer>>();
        private readonly object ContainerLock = new object();

        public void PanelPathAdd(PanelPathContainer thispanel)
        {
            lock (ContainerLock)
            {
                if (theContainer.ContainsKey(thispanel.PanelId))
                {
                    theContainer[thispanel.PanelId].Add(thispanel);
                }
                else
                {
                    theContainer.Add(thispanel.PanelId, new List<PanelPathContainer> { thispanel });
                }
            }
        }

        public void PanelPathAdd(List<PanelPathContainer> panelList)
        {
            foreach (var panel in panelList)
            {
                PanelPathAdd(panel);
            }
        }

        public List<PanelPathContainer> PanelPathGet(string panelId)
        {
            // TODO: 当同时存在多个相同ID产品时的情况；
            if (theContainer.ContainsKey(panelId))
            {
                return theContainer[panelId];
            }
            else
            {
                return null;
            }
        }

        public void Clear()
        {
            theContainer.Clear();
        }

        public bool Contains(string panelId)
        {
            return theContainer.ContainsKey(panelId);
        }

        public string[] GetKeys()
        {
            return theContainer.Keys.ToArray();
        }
        
        public void AddRange(PanelPathManager newManager)
        {
            foreach (var item in newManager.GetKeys())
            {
                this.PanelPathAdd(newManager.PanelPathGet(item));
            }
            
        }

        //public PanelPathContainer PanelPathGetLatest(string panelId)
        //{
        //    if (theContainer.ContainsKey(panelId))
        //    {
        //        List<PanelPathContainer> returnlist = theContainer[panelId];
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}
    }
}
