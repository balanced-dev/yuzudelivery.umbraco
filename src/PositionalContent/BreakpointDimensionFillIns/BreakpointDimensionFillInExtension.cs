using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hifi.PositionalContent;
using Umbraco.Web.Models;

namespace YuzuDelivery.Umbraco.PositionalContent
{
    public static class BreakpointDimensionFillInExtension
    {

        public static List<vmSub_DataPositionalContentDimension> FillInEmptyBreakpoints(this IEnumerable<vmSub_DataPositionalContentDimension> dimensions, List<BreakpointDimensionFillIns> dimensionFillIns)
        {
            var output = dimensions.ToList();

            foreach (var i in dimensionFillIns)
            {
                if (!output.Any(x => x.BreakpointName.ToString() == i.FillIn))
                {
                    var toUse = output.Where(x => x.BreakpointName.ToString() == i.ToCopy).FirstOrDefault();
                    var index = output.IndexOf(toUse);

                    var backFill = new vmSub_DataPositionalContentDimension
                    {
                        BreakpointName = i.FillIn,
                        Styles = toUse.Styles,
                        ContentOverride = toUse.ContentOverride,
                        IsHidden = toUse.IsHidden
                    };
                    output.Insert(i.Inverse ? index + 1 : index, backFill);
                }
            }

            return output;
        }

    }
}
