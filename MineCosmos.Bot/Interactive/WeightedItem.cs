using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineCosmos.Bot.Interactive
{
    class WeightedItem
    {
        public string Name { get; set; }
        public double Weight { get; set; }
    }

    class WeightedItemSelector
    {
        private List<WeightedItem> items;
        private double[] cumulativeWeights;
        private double totalWeight;

        public WeightedItemSelector(List<WeightedItem> items)
        {
            this.items = items;
            this.cumulativeWeights = new double[items.Count];
            this.totalWeight = 0;

            double cumulativeWeight = 0;
            for (int i = 0; i < items.Count; i++)
            {
                cumulativeWeight += items[i].Weight;
                cumulativeWeights[i] = cumulativeWeight;
                totalWeight += items[i].Weight;
            }
        }

        public WeightedItem GetRandomWeightedItem()
        {
            double randomValue = new Random().NextDouble() * totalWeight;

            for (int i = 0; i < cumulativeWeights.Length; i++)
            {
                if (randomValue < cumulativeWeights[i])
                {
                    return items[i];
                }
            }

            // 如果出现问题，返回 null
            return null;
        }
    }

    
}
