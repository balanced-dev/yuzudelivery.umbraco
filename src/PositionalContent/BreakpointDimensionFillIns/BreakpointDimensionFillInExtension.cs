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

        public static IList<BreakpointDimension> FillInEmptyBreakpoints(this IEnumerable<BreakpointDimension> dimensions, List<BreakpointDimensionFillIns> dimensionFillIns)
        {
            var output = dimensions.ToList();

            foreach (var i in dimensionFillIns)
            {
                if (!output.Any(x => x.breakpointName == i.FillIn))
                {
                    var toUse = output.Where(x => x.breakpointName == i.ToCopy).FirstOrDefault();
                    var index = output.IndexOf(toUse);

                    var backFill = new BreakpointDimension
                    {
                        breakpointName = i.FillIn,
                        styles = toUse.styles,
                        content = toUse.content,
                        hidden = toUse.hidden
                    };
                    output.Insert(i.Inverse ? index + 1 : index, backFill);
                }
            }

            return output;
        }

    }
}
