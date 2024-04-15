using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS_SG_ETABS_Plugin.src.plot_reaction.graph
{
    internal class ClassPlotGraph
    {
        public class SumReactionPerGroup
        {
            public string Name { get; set; }
            public System.Drawing.Color Color { get; set; }
            public double SumReaction { get; set; }

            public SumReactionPerGroup(string name, System.Drawing.Color color, double sumReaction)
            {
                Name = name;
                Color = color;
                SumReaction = Math.Round(sumReaction);
            }
        }
    }
}
