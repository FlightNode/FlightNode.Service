using EmitMapper;
using System.Collections.Generic;

namespace FlightNode.Common.BaseClasses
{
    public abstract class DomainLogic
    {
        protected virtual TOutput Map<TInput, TOutput>(TInput input)
        {
            return ObjectMapperManager.DefaultInstance
                               .GetMapper<TInput, TOutput>()
                               .Map(input);
        }


        protected virtual IEnumerable<TOutput> Map<TInput, TOutput>(IEnumerable<TInput> inputCollection)
        {
            foreach (var input in inputCollection)
            {
                yield return
                ObjectMapperManager.DefaultInstance
                                   .GetMapper<TInput, TOutput>()
                                   .Map(input);
            }
        }
    }
}
