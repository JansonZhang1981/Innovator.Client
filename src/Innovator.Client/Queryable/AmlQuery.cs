﻿#if REFLECTION
using System.Linq.Expressions;

namespace Innovator.Client.Queryable
{
  internal class AmlQuery
  {
    public IItem Aml { get; set; }
    public LambdaExpression Projection { get; set; }
  }
}
#endif
